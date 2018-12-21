using UnityEngine;
using System;

namespace SHProject.Ingame
{
    public class Map : Singleton<Map>
    {
        public const int CellSize = 2;

        public Vector3 GetMapPosition(Locate loc)
        {
            Vector3 pos = Vector3.zero;
            pos.x = (loc.x * CellSize) + 1;
            pos.z = (loc.z * CellSize) + 1;
            return pos;
        }

        public Locate GetMapIndex(Vector3 pos)
        {
            short x = (short)((pos.x - 1) / 2);
            short z = (short)((pos.z - 1) / 2);
            return new Locate(x, z);
        }
    }
}