using Entities;
using Generation;
using UnityEngine;
using UnityEngine.AI;

namespace Process
{
    [RequireComponent(typeof(MazeParent))]
    [RequireComponent(typeof(EntitiesParent))]
    public class LevelParent : MonoBehaviour
    {
        private MazeParent _mazeParent;
        private EntitiesParent _entitiesParent;

        private void Start()
        {
            _mazeParent = GetComponent<MazeParent>();
            _entitiesParent = GetComponent<EntitiesParent>();

            InitializeEnvironment();
            InitializeEntities();
        }

        /// <summary>
        /// Generates a new maze
        /// </summary>
        private void InitializeEnvironment() {
            _mazeParent.SpawnMaze();
        }

        /// <summary>
        /// Places player into the maze
        /// and adjusts the environment with monsters and items. 
        /// </summary>
        private void InitializeEntities() {
            var keyPositions = _mazeParent.ConstructKeyPositions();

            _entitiesParent.SpawnExit(keyPositions);
            _entitiesParent.SpawnPlayer(keyPositions);
            _entitiesParent.SpawnCoins(keyPositions);
            _entitiesParent.SpawnHunter(keyPositions);
            _entitiesParent.SpawnNavField(keyPositions);
        }
    }
}
