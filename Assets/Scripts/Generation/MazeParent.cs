using System.Collections.Generic;
using UnityEngine;

namespace Generation
{
    public class MazeParent : MonoBehaviour
    {
        [SerializeField] private GameObject cellPrefab;
        
        // @todo Matrix Dimensions should be placed in the inspector.
        // WARNING: Minimum value is 5.
        private readonly int _matrixWidth = 10;
        // WARNING: Minimum value is 5.
        private readonly int _matrixHeight = 10;
        
        /// <summary>
        /// High Level method that instantiates a new instance of the mazeMatrixGenerator
        /// and produces a new maze matrix.
        /// </summary>
        /// <returns>
        /// 2D array matrix of the generated maze.
        /// </returns>
        private GeneratorCell[,] GenerateMaze()
        {
           MazeGenerator generator = new MazeGenerator(_matrixWidth, _matrixHeight);
           return generator.GenerateCells();
        }

        /// <summary>
        /// High Level method that instantiates all cells in the matrix to the real world.
        /// </summary>
        /// <param name="cells">
        /// Maze Matrix produced by mazeMatrixGenerator.
        /// </param>
        private void RenderMaze(GeneratorCell[,] cells)
        {
            for (var y = 0; y < _matrixHeight; ++y)
            {
                for (var x = 0; x < _matrixWidth; ++x)
                {
                    var cellData = cells[y, x];
                    var cell = InstantiateCell(cellData);
                    RemoveCellWalls(cell, cellData);
                }
            }
        }

        private GameObject InstantiateVoidCell()
        {
            return Instantiate(cellPrefab, Vector3.zero, Quaternion.identity);
        }

        /// <summary>
        /// High Level method that places cell in the world
        /// and enchants it by adjusting its size and behaviour.  
        /// </summary>
        /// <param name="cellData">
        /// Cell information produced by mazeMatrixGenerator.
        /// </param>
        /// <returns>
        /// Referencing gameObject of the instantiated cell.
        /// </returns>
        private GameObject InstantiateCell(GeneratorCell cellData)
        {
            var cell = InstantiateVoidCell();
            // @todo The method call should be memoized. That way we could avoid object referencing or memory leaks.
            var size = GetCellSize(cell);

            cell.transform.position = new Vector3(
                size.x * cellData.X,
                0,
                size.z * cellData.Y
            );

            return cell;
        }

        /// <summary>
        /// Removes game cell walls based on the cell info object.
        /// </summary>
        /// <param name="cell">
        /// Referenced cell gameObject.
        /// </param>
        /// <param name="cellData">
        /// Information about cell in the matrix that should be produced during
        /// maze generation.
        /// </param>
        private void RemoveCellWalls(GameObject cell, GeneratorCell cellData)
        {
            Cell cellWalls = cell.GetComponent<Cell>();

            if (!cellData.borderedN) Destroy(cellWalls.WallN);
            if (!cellData.borderedE) Destroy(cellWalls.WallE);
            if (!cellData.borderedS) Destroy(cellWalls.WallS);
            if (!cellData.borderedW) Destroy(cellWalls.WallW);
        }

        /// <summary>
        /// Calculates size of the targeted cell.
        /// </summary>
        /// <param name="directCell">
        /// Reference variable to the targeted cell. If not passed,
        /// the method will return size of the default cell prefab.
        /// </param>
        /// <returns>
        /// Size Vector of the targeted cell.
        /// </returns>
        private Vector3 GetCellSize(GameObject directCell = default)
        {
            var targetCell = (directCell == default)
                ? InstantiateVoidCell()
                : directCell;
            
            var size = targetCell.GetComponent<Collider>().bounds.size;

            // Default cell should be deleted after usage, since
            // we don't want to have any extra cells in the world.
            if(directCell == default) Destroy(targetCell);
            
            return size;
        }

        /// <summary>
        /// Returns a real-world position for cell  
        /// </summary>
        /// <param name="matrixPosition">
        /// X and Y position in the matrix map.
        /// </param>
        /// <returns>
        /// Real-world position of the matrix point on the map.
        /// </returns>
        private Vector3 GetCellPositionAt(Vector2 matrixPosition)
        {
            var cellSize = GetCellSize();

            return new Vector3(
                cellSize.x * matrixPosition.x + cellSize.x / 2, 
                .5f,
                cellSize.z * matrixPosition.y
            );
        }

        /// <summary>
        /// Randomizes position/number of entities.
        /// </summary>
        /// <returns>
        /// List of entity positions.
        /// </returns>
        private IList<Vector3> RandomizeMultiplePositions(int count, int skipX = 0, int skipY = 0)
        {
            var positions = new List<Vector3>();

            for (var ma = 0; ma < count; ++ma)
            {
                var matrixPosition = new Vector2(
                    Random.Range(skipX, _matrixWidth), 
                    Random.Range(skipY, _matrixHeight)
                );
                
                positions.Add(GetCellPositionAt(matrixPosition));
            }

            return positions;
        }

        /// <returns>
        /// Returns randomized monster positions.
        /// </returns>
        private IList<Vector3> RandomizeMonsterPositions()
        {
            return RandomizeMultiplePositions(Random.Range(4, 9), 0, 4);
        }

        /// <returns>
        /// Returns randomized item positions.
        /// </returns>
        private IList<Vector3> RandomizeItemPositions()
        {
            return RandomizeMultiplePositions(Random.Range(1, 4), 0, 4);
        }

        /// <summary>
        /// Constructs important an object that contains real-world important maze positions.
        /// </summary>
        /// <returns>
        /// Object of real-world positions. 
        /// </returns>
        public MazeKeyPositions ConstructKeyPositions()
        {
            var player = GetCellPositionAt(Vector2.zero);
            var exit = GetCellPositionAt(new Vector2(_matrixWidth - 1, _matrixHeight - 1));
            IList<Vector3> monsters = RandomizeMonsterPositions();
            IList<Vector3> items = RandomizeItemPositions();
            
            return new MazeKeyPositions(player, exit, items, monsters);
        }
        
        /// <summary>
        /// Generates and Instantiates maze in the world.
        /// </summary>
        public void SpawnMaze()
        {
            RenderMaze(GenerateMaze());
        }
    }
}
