using UnityEngine;
using System;

namespace SHProject.Ingame
{
    [AutoRegisterEvent]
    public class GridManager : Singleton<GridManager>
    {
        [SetField(typeof(Transform), "root")]
        private Transform gridRoot;

        [SerializeField]
        private int defaultGridNum = 10;

        private Grid[,] gridObjects;
        public bool OpenTouchAction { get; set; } = false;

        protected override void Awake()
        {
            base.Awake();
            MakeGrid();
        }

        private void MakeGrid()
        {
            float startAt = defaultGridNum / 2 * Map.CellSize;
            gridObjects = new Grid[defaultGridNum + 1, defaultGridNum + 1];
            for (int i = 0; i <= defaultGridNum; i++)
            {
                for(int j = 0; j <= defaultGridNum; j++)
                {
                    var inst = Extension.Instantiate<Grid>(this, gridRoot);
                    inst.transform.parent = gridRoot.transform;
                    inst.transform.localRotation = Quaternion.identity;

                    Vector3 gridPos = Vector3.zero;
                    gridPos.x = -startAt + (Map.CellSize * j);
                    gridPos.z = startAt - (Map.CellSize * i);
                    inst.transform.localPosition = gridPos;

                    inst.gameObject.SetActive(false);
                    gridObjects[j, i] = inst;
                }
            }
        }
        
        private void HideGrid()
        {
            gridRoot.gameObject.SetActive(false);
        }

        public void SetCharacterGrid(int charX, int charZ, int sight)
        {
            Vector3 gridLocalPos = gridRoot.transform.localPosition;

            gridLocalPos.x = charX * Map.CellSize;
            gridLocalPos.z = charZ * Map.CellSize;
            gridRoot.transform.localPosition = gridLocalPos;
            gridRoot.gameObject.SetActive(true);

            HideSightGrid(sight);
            EventHandlerManager.Invoke(EventEnum.GridCheckVisible, this, null);
        }

        private void HideSightGrid(int sight)
        {
            int divide = defaultGridNum / 2;
            for(int i = divide - sight; i <= divide + sight; i++)
            {
                for(int j = divide - sight; j <= divide + sight; j++)
                {
                    gridObjects[j, i].gameObject.SetActive(true);
                }
            }
        }

        [EventMethod(EventEnum.CharacterMove)]
        public void OnHideGrid(object sender, EventArgs args)
        {
            HideGrid();
        }
    }
}
