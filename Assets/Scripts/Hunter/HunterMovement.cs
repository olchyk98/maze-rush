using Pathfinding;
using UnityEngine;
using Universal;
using Player;

namespace Hunter
{
    internal enum HunterActivity
    {
        INVESTIGATING,
        FOLLOWING,
        ATTACKING,
        WAITING,
    }

    [RequireComponent(typeof(AIPath))] // Brings Pathfinding::Seeker<@T> [!]
    [RequireComponent(typeof(PlayerFinder))]
    public class HunterMovement : MonoBehaviour
    {
        private Seeker _seeker;
        private AIPath _aipath;
        private Transform _transform;

        private GameObject _target;
        private Transform _targetTransform;
        private PlayerController _targetController;

        private Vector3 _cachedMazeSize = Vector3.zero;

        [SerializeField] [Range(.5f, 10f)] private float _minSpeed;
        [SerializeField] [Range(2f, 20f)] private float _maxSpeed;
        [SerializeField] [Range(.1f, 5f)] private float _attackDistance;
        [SerializeField] private LayerMask _eyesIgnore;

        private HunterActivity _activity = HunterActivity.WAITING;

        private void Start()
        {
            _seeker = GetComponent<Seeker>();
            _aipath = GetComponent<AIPath>();
            _transform = GetComponent<Transform>();

            _target = GetComponent<PlayerFinder>().FindPlayer();
            _targetTransform = _target.GetComponent<Transform>();
            _targetController = _target.GetComponent<PlayerController>();

            _targetController.OnMove += HandleTargetMove;
        }

        private void Update()
        {
            ScheduleBehaviour();
        }

        private void ScheduleBehaviour()
        {
            // TODO: Rewrite the scheme using the StateMachine pattern.
            if (CheckIfTargetInViewport())
            {
                if (CheckIfNearTarget())
                {
                    AttackTarget();
                }
                else
                {
                    FollowTarget();
                }
            }
            else
            {
                // TODO: CONTINUE -> if FOLLOWING in the else block -> stop following and start investigating
                if(_activity.Equals(HunterActivity.FOLLOWING))
                {
                    // TODO: Change if condition and here -> stop following and start investigating
                }
                else
                {
                    InvestigateMaze();
                }
            }
        }

        private Vector3 GenerateRandomDestination()
        {
            float maxX = _cachedMazeSize.x;
            float y = _cachedMazeSize.z;
            float maxZ = _cachedMazeSize.y;

            var destination = new Vector3(
                Random.Range(0f, maxX),
                y,
                Random.Range(0f, maxZ)
            );


            print(destination);
            return destination;
        }

        public void UpdateMazeSizeCache(Vector3 newSize)
        {
            _cachedMazeSize = newSize;
        }

        #region CHECKS
        private bool CheckIfTargetInViewport()
        {
            var verticalShifter = Vector3.up * 1f;
            var shiftedSelf = _transform.position + verticalShifter;
            var shiftedTarget = _targetTransform.position + verticalShifter;

            var didHit = Physics.Linecast(shiftedSelf, shiftedTarget, out RaycastHit hitInfo, _eyesIgnore);

            if(!didHit) return false;
            return hitInfo.collider.gameObject.CompareTag("Player");
        }

        private bool CheckIfNearTarget()
        {
            var distanceToPlayer = Vector3.Distance(_transform.position, _targetTransform.position);
            return distanceToPlayer <= _attackDistance;
        }

        private void HandleTargetMove(Vector3 position, bool isShifting = false)
        {
            if (!isShifting) return;
            FollowTarget();
        }

        private bool CheckIfRouting(HunterActivity activity)
        {
            return _activity.Equals(activity)
                && (!_aipath.reachedEndOfPath || _aipath.pathPending);
        }
        #endregion

        #region BEHAVIOUR
        private void GoToPoint(Vector3 position)
        {
            // Set speed
            _aipath.maxSpeed = _minSpeed;

            // Select a random point in the maze and go to it
            _aipath.destination = position;
            _aipath.SearchPath();
        }

        private void InvestigateMaze()
        {
            // Ignore if already investigating
            if (CheckIfRouting(HunterActivity.INVESTIGATING)) return;

            // Set new activity status
            _activity = HunterActivity.INVESTIGATING;

            GoToPoint(GenerateRandomDestination());
        }

        private void AttackTarget()
        {
            // Hunter jumps on the player and kills him.
            _aipath.maxSpeed = _maxSpeed * 2;
            // TODO: Implement Attack Target
            print("ATTACKING BITCH");
        }

        private void FollowTarget()
        {
            if(CheckIfRouting(HunterActivity.FOLLOWING)) return;

            // Set new activity status
            _activity = HunterActivity.FOLLOWING;

            GoToPoint(_targetTransform.position);

        }
        #endregion
    }
}
