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
        FOLLOWING_STEPS,
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

        private Transform _targetTransform;
        private PlayerController _targetController;

        private Vector3 _cachedMazeSize = Vector3.zero;

        [SerializeField] [Range(2f, 20f)] private float _speed;
        [SerializeField] [Range(.1f, 5f)] private float _attackDistance;
        [SerializeField] [Range(1f, 50f)] private float _hearingDistance;
        [SerializeField] [Range(1f, 20f)] private float _attackEffect;
        // Used to specify what objects hunter is able to see. Usually set to: &player and &obstacle[]
        [SerializeField] private LayerMask _visibleLayers;

        private HunterActivity _activity = HunterActivity.WAITING;

        private void Start()
        {
            _seeker = GetComponent<Seeker>();
            _aipath = GetComponent<AIPath>();
            _transform = GetComponent<Transform>();

            var target = GetComponent<PlayerFinder>().FindPlayer();
            _targetTransform = target.GetComponent<Transform>();
            _targetController = target.GetComponent<PlayerController>();

            _targetController.OnStep += HandleTargetStep;
        }

        private void Update()
        {
            ScheduleBehaviour();
        }

        private void ScheduleBehaviour()
        {
            // TODO: Rewrite scheme using the StateMachine pattern.
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
                InvestigateMaze();
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

            var didHit = Physics.Linecast(shiftedSelf, shiftedTarget, out RaycastHit hitInfo, _visibleLayers);

            if (!didHit) return false;
            return hitInfo.collider.gameObject.CompareTag("Player");
        }

        private bool CheckIfNearTarget()
        {
            var distanceToPlayer = Vector3.Distance(_transform.position, _targetTransform.position);
            return distanceToPlayer <= _attackDistance;
        }
        private void HandleTargetStep(Transform targetTransform)

        {
            var distanceToTarget = Vector3.Distance(targetTransform.position, _transform.position);
            if(distanceToTarget > _hearingDistance) return;

            FollowTarget(true);
        }

        /// <summary>
        /// Returns true if moving because of the passed activity.
        /// </summary>
        /// <param name="activity">
        /// Type of the controlling activity
        /// </param>
        private bool CheckIfRouting(HunterActivity activity)
        {
            return (_activity.Equals(activity))
                && (!_aipath.reachedEndOfPath || _aipath.pathPending);
        }
        #endregion

        #region BEHAVIOUR
        private void GoToPoint(Vector3 position)
        {
            // Select a random point in the maze and go to it
            _aipath.destination = position;
            _aipath.SearchPath();
        }

        private void InvestigateMaze()
        {
            // Ignore if already investigating
            // or following steps
            if (
                CheckIfRouting(HunterActivity.INVESTIGATING)
                || CheckIfRouting(HunterActivity.FOLLOWING_STEPS)
           ) return;

            // Set new activity status
            _activity = HunterActivity.INVESTIGATING;

            // Set speed
            _aipath.maxSpeed = _speed / 2;

            GoToPoint(GenerateRandomDestination());
        }

        private void AttackTarget()
        {
            // Hunter jumps on the player and kills him.
            _aipath.maxSpeed = _speed * 2;

            // TODO: Play Animation
            var targetHealth = _targetTransform
                .gameObject
                .GetComponent<PlayerController>();

            _activity = HunterActivity.ATTACKING;
            targetHealth.ApplyHit(_transform, _attackEffect);
        }

        /// <summary>Build a path to the target and follows it.</summary>
        /// <param name="isSuggested">Specifies if hunter is following target based on hearing.</param>
        private void FollowTarget(bool isSuggested = false)
        {
            if (
                CheckIfRouting(HunterActivity.FOLLOWING)
                || (isSuggested && CheckIfRouting(HunterActivity.FOLLOWING_STEPS))
            ) return;

            // Set new activity status
            _activity = (!isSuggested)
                ? HunterActivity.FOLLOWING
                : HunterActivity.FOLLOWING_STEPS;

            // Set speed
            _aipath.maxSpeed = _speed;

            GoToPoint(_targetTransform.position);

        }
        #endregion
    }
}
