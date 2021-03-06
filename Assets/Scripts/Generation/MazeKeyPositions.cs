using System.Collections.Generic;
using UnityEngine;

namespace Generation
{
    public class MazeKeyPositions
    {
        public readonly Vector3 Player;
        public readonly Vector3 Exit;
        public readonly IList<Vector3> Coins;
        public readonly Vector3 Hunter;
        public readonly Vector3 MazeStart;
        public readonly Vector3 MazeEnd;

        public MazeKeyPositions(Vector3 player, Vector3 exit, IList<Vector3> coins, Vector3 hunter, Vector3 mazeStart, Vector3 mazeEnd)
        {
            Player = player;
            Exit = exit;
            Coins = coins;
            Hunter = hunter;
            MazeStart = mazeStart;
            MazeEnd = mazeEnd;
        }
    }
}
