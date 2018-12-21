using System;
using UnityEngine;

namespace SHProject.Ingame
{
    [Serializable]
    public struct Locate
    {
        public short x { get; set; }
        public short z { get; set; }

        public Locate(short x, short z)
        {
            this.x = x;
            this.z = z;
        }

        public static object Deserialize(byte[] data)
        {
            var result = new Locate();
            result.x = data[0];
            result.z = data[1];
            return result;
        }

        public static byte[] Serialize(object customType)
        {
            var c = (Locate)customType;
            return new byte[] { (byte)c.x, (byte)c.z };
        }
    }

    public class CommonLocateMono : MonoBehaviourBase
    {
        private Locate locate;
        protected Locate LocateIdx
        {
            get
            {
                return locate;
            }
            set
            {
                Vector3 pos = cachedTransform.localPosition;
                pos.x = (value.x * Map.CellSize) + 1;
                pos.z = (value.z * Map.CellSize) + 1;

                cachedTransform.localPosition = pos;
                locate.x = value.x;
                locate.z = value.z;
            }
        }
    }

    public class CommonLocatePhoton : PhotonBehaviourBase
    {
        private Locate locate;
        protected Locate LocateIdx
        {
            get
            {
                return locate;
            }
            set
            {
                Vector3 pos = cachedTransform.localPosition;
                pos.x = (value.x * Map.CellSize) + 1;
                pos.z = (value.z * Map.CellSize) + 1;

                cachedTransform.localPosition = pos;
                locate.x = value.x;
                locate.z = value.z;
            }
        }
    }
}