using UnityEngine;

namespace SHProject.Ingame
{
    public class NormalSoldier : CharacterBase
    {
        protected override short Sight
        {
            get
            {
                return 5;
            }
        }

        protected override void Awake()
        {
            base.Awake();
            LocateIdx = new Locate(3, 3);
        }
    }
}