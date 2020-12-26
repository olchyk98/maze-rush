using UnityEngine;

namespace Generation
{
    public class MazeParent : MonoBehaviour
    {
        [SerializeField] private GameObject _cellPrefab;
        private readonly int _mazeWidth = 10;
        private readonly int _mazeHeight = 10;
        
        private void Start()
        {
            RenderMaze(GenerateMaze());
        }

        private GeneratorCell[,] GenerateMaze()
        {
           MazeGenerator generator = new MazeGenerator(_mazeWidth, _mazeHeight);
           return generator.GenerateCells();
        }

        private void RenderMaze(GeneratorCell[,] cells)
        {
            for (var y = 0; y < _mazeHeight; ++y)
            {
                for (var x = 0; x < _mazeWidth; ++x)
                {
                    var cellData = cells[y, x];
                    var cell = InstantiateCell(cellData);
                    RemoveCellWalls(cell, cellData);
                }
            }
        }

        private GameObject InstantiateCell(GeneratorCell cellData)
        {
            var cell = Instantiate(_cellPrefab, Vector3.zero, Quaternion.identity);

            Vector3 size = cell.GetComponent<Collider>().bounds.size;

            cell.transform.position = new Vector3(
                size.x * cellData.x,
                0,
                size.z * cellData.y
            );

            return cell;
        }

        private void RemoveCellWalls(GameObject cell, GeneratorCell cellData)
        {
            Cell cellWalls = cell.GetComponent<Cell>();

            if (!cellData.borderedN) Destroy(cellWalls.WallN);
            if (!cellData.borderedE) Destroy(cellWalls.WallE);
            if (!cellData.borderedS) Destroy(cellWalls.WallS);
            if (!cellData.borderedW) Destroy(cellWalls.WallW);
        }
    }
}