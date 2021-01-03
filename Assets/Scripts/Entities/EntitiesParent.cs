using System.Collections;
using UnityEngine;
using Hunter;
using Pathfinding;
using Generation;

namespace Entities
{
    public class EntitiesParent : MonoBehaviour
    {
        [SerializeField] private GameObject playerPrefab;
        [SerializeField] private GameObject exitPrefab;
        [SerializeField] private GameObject itemPrefab;
        [SerializeField] private GameObject hunterPrefab;
        [SerializeField] private GameObject navFieldPrefab;

        private Transform _transform;

        private void Start()
        {
            _transform = GetComponent<Transform>();
        }

        public void SpawnPlayer(MazeKeyPositions positions)
        {
            Instantiate(playerPrefab, positions.Player, Quaternion.identity);
        }

        public void SpawnExit(MazeKeyPositions positions)
        {
            Instantiate(exitPrefab, positions.Exit, Quaternion.identity);
        }

        public void SpawnItems(MazeKeyPositions positions)
        {
            Instantiate(itemPrefab, positions.Items[0], Quaternion.identity);
        }

        public void SpawnHunter(MazeKeyPositions positions)
        {
            var hunter = Instantiate(hunterPrefab, positions.Hunter, Quaternion.identity);
            hunter.GetComponent<HunterMovement>().UpdateMazeSizeCache(positions.MazeEnd);
        }

        private IEnumerator BakeMeshOnUpdate(GameObject navField)
        {
            yield return 0;

            navField.GetComponent<AstarPath>().Scan();
        }

        public void SpawnNavField(MazeKeyPositions positions)
        {
            var navField = Instantiate(navFieldPrefab);
            StartCoroutine(BakeMeshOnUpdate(navField));
        }
    }
}
