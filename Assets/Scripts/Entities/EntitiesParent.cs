using System.Collections.Generic;
using UnityEngine;

namespace Entities
{
    public class EntitiesParent : MonoBehaviour
    {
        [SerializeField] private GameObject playerPrefab;
        [SerializeField] private GameObject exitPrefab;
        [SerializeField] private GameObject itemPrefab;
        [SerializeField] private GameObject monsterPrefab;
        
        public void SpawnPlayer(Vector3 position)
        {
            Instantiate(playerPrefab, position, Quaternion.identity);
            return;
        }

        public void SpawnExit(Vector3 position)
        {
            Instantiate(exitPrefab, position, Quaternion.identity);
            return;
        }

        public void SpawnItems(IList<Vector3> positions)
        {
            Instantiate(itemPrefab, positions[0], Quaternion.identity);
            return;
        }

        public void SpawnMonsters(IList<Vector3> positions)
        {
            Instantiate(monsterPrefab, positions[0], Quaternion.identity);
            return;
        }
    }
}