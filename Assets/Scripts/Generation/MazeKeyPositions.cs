using System.Collections.Generic;
using UnityEngine;

namespace Generation
{
    public class MazeKeyPositions
    {
        public readonly Vector3 Player;
        public readonly Vector3 Exit;
        public readonly IList<Vector3> Items;
        public readonly IList<Vector3> Monsters;

        public MazeKeyPositions(Vector3 player, Vector3 exit, IList<Vector3> items, IList<Vector3> monsters)
        {
            Player = player;
            Exit = exit;
            Items = items;
            Monsters = monsters;
        }
    }
}