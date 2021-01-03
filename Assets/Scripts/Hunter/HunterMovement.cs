using Pathfinding;
using UnityEngine;
using Universal;

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
        private GameObject _target;
        private Transform _transform;

        private Vector3 _cachedMazeSize = Vector3.zero;

        [SerializeField] [Range(.5f, 10f)] private float minSpeed;
        [SerializeField] [Range(2f, 20f)] private float maxSpeed;

        // @todo: Target.OnMakingNoise += SearchTarget();

        private HunterActivity _activity = HunterActivity.WAITING;

        private void Start()
        {
            _seeker = GetComponent<Seeker>();
            _aipath = GetComponent<AIPath>();
            _target = GetComponent<PlayerFinder>().FindPlayer();
            _transform = GetComponent<Transform>();
        }

        private void Update()
        {
            ScheduleBehaviour();
        }

        private void ScheduleBehaviour()
        {
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

            print($"Generated random destination: {destination} from bounds: {_cachedMazeSize}");

            return destination;
        }

        public void UpdateMazeSizeCache(Vector3 newSize)
        {
            _cachedMazeSize = newSize;
        }

        #region CHECKS
        private bool CheckIfTargetInViewport()
        {
            return false;
        }

        private bool CheckIfNearTarget()
        {
            return false;
        }
        #endregion

        #region BEHAVIOUR
        private void InvestigateMaze()
        {
            if (_activity.Equals(HunterActivity.INVESTIGATING) && (!_aipath.reachedEndOfPath || _aipath.pathPending)) return;

            // Set new activity status
            _activity = HunterActivity.INVESTIGATING;

            // Set speed
            _aipath.maxSpeed = minSpeed;

            // Select a random point in the maze and go to it
            _aipath.destination = GenerateRandomDestination();
            _aipath.SearchPath();
        }

        private void AttackTarget()
        {
            // Hunter jumps on the player and kills him.
            _aipath.maxSpeed = maxSpeed / 2;
        }

        private void FollowTarget()
        {
            _aipath.maxSpeed = maxSpeed;
        }
        #endregion
    }
}
