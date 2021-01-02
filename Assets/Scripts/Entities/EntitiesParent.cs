using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

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

        private void Start() {
            _transform = GetComponent<Transform>();
        }

        public void SpawnPlayer(Vector3 position)
        {
            Instantiate(playerPrefab, position, Quaternion.identity);
        }

        public void SpawnExit(Vector3 position)
        {
            Instantiate(exitPrefab, position, Quaternion.identity);
        }

        public void SpawnItems(IList<Vector3> positions)
        {
            Instantiate(itemPrefab, positions[0], Quaternion.identity);
        }

        public void SpawnHunter(Vector3 position) {
            Instantiate(hunterPrefab, position, Quaternion.identity);
        } 

        private System.Collections.IEnumerator BakeMeshOnUpdate(GameObject navField) {
            yield return 0;

            navField.GetComponent<AstarPath>().Scan();
        }

        public void SpawnNavField() {
            var navField = Instantiate(navFieldPrefab);
            StartCoroutine(BakeMeshOnUpdate(navField));
        }
    }
}
