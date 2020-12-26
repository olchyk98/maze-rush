using UnityEngine;

namespace Generation
{
    public class MazeGenerator
    {
        private readonly int _mazeWidth;
        private readonly int _mazeHeight;
        private readonly GeneratorCell[,] _grid;

        public MazeGenerator(int width, int height)
        {
            _mazeWidth = width;
            _mazeHeight = height;
            _grid = InstantiateGrid();
        }

        /// <summary>
        /// Fills grid with the cell instances.
        /// </summary>
        /// <returns>
        /// 2D array maze matrix with newly summoned cell instances.
        /// </returns>
        private GeneratorCell[,] InstantiateGrid()
        {
           GeneratorCell[,] grid = new GeneratorCell[_mazeHeight, _mazeWidth];
           
           for (var y = 0; y < _mazeHeight; ++y)
           {
               for (var x = 0; x < _mazeWidth; ++x)
               {
                   grid[y, x] = new GeneratorCell(x, y);
               }
           }
            
           return grid;
        }

        /// <summary>
        /// Uses the predictable Sidewinder Algorithm to generate a new maze matrix.
        /// </summary>
        /// <returns>
        /// 2D array of the maze matrix.
        /// </returns>
        public GeneratorCell[,] GenerateCells() {
            for (var y = 0; y < _mazeHeight; ++y)
            {
                int runStart = 0;
            
                for (var x = 0; x < _mazeHeight; ++x)
                {
                    if (y > 0 && (x + 1 == _mazeWidth || Random.Range(0, 2) > 0))
                    {
                        var targetX = runStart + Random.Range(0, x - runStart + 1);
                        _grid[y, targetX].borderedN = false;
                        _grid[y - 1, targetX].borderedS = false;

                        runStart = x + 1; // @todo Optimize
                    } else if (x + 1 < _mazeWidth)
                    {
                        _grid[y, x].borderedE = false;
                        _grid[y, x + 1].borderedW = false;
                    }
                }
            }
            
            return _grid;
        }
    }
}