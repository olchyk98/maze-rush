using Entities;
using Generation;
using UnityEngine;

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
        
            InitializeLevel();
        }
    
        /// <summary>
        /// Generates a new maze, places player into it
        /// and adjusts the environment with monsters and items. 
        /// </summary>
        private void InitializeLevel() {
            _mazeParent.SpawnMaze();
            var keyPositions = _mazeParent.ConstructKeyPositions();
        
            _entitiesParent.SpawnExit(keyPositions.Exit);
            _entitiesParent.SpawnPlayer(keyPositions.Player);
            _entitiesParent.SpawnItems(keyPositions.Items);
            _entitiesParent.SpawnMonsters(keyPositions.Monsters);
        }
    }
}
