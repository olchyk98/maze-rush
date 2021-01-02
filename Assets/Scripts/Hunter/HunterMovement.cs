using Pathfinding;
using UnityEngine;
using Universal;

namespace Hunter
{
    [RequireComponent(typeof(AIPath))] // Brings Pathfinding::Seeker<@T> [!]
    [RequireComponent(typeof(PlayerFinder))]
    public class HunterMovement : MonoBehaviour
    {
        private Seeker _seeker; 
        private GameObject _target;
        private Transform _transform;

        private void Start() {
            _seeker = GetComponent<Seeker>();
            _target = GetComponent<PlayerFinder>().FindPlayer();
            _transform = GetComponent<Transform>();
        }
    }
}
