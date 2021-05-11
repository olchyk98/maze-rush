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
        [SerializeField] private GameObject hunterPrefab;
        [SerializeField] private GameObject coinPrefab;
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

        public void SpawnCoins(MazeKeyPositions positions)
        {
            const float y = 1f;

            foreach(Vector3 p in positions.Coins)
            {
                var position = new Vector3(p.x, y, p.z);

                Instantiate(coinPrefab, position, Quaternion.identity);
            }
        }

        public void SpawnHunter(MazeKeyPositions positions)
        {
            var hunter = Instantiate(hunterPrefab, positions.Hunter, Quaternion.identity);
            hunter.GetComponent<HunterBrain>().UpdateMazeSizeCache(positions.MazeEnd);
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
