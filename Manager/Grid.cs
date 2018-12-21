using System;
using UnityEngine;

namespace SHProject.Ingame
{
    [AutoRegisterEvent]
    [AutoPrefab("Prefabs/GridElem")]
    public class Grid : MonoBehaviourBase
    {
        private void OnMouseUp()
        {
            var parent = cachedTransform.parent;
            var mat = parent.transform.worldToLocalMatrix;
            var localPos = mat.inverse.MultiplyVector(cachedTransform.position);
            localPos.x += 1;
            localPos.z += 1;

            var index = Map.Instance.GetMapIndex(localPos);
            EventHandlerManager.Invoke(EventEnum.Send_CharacterMove, this, new TValueEventArgs<Locate>(index));
        }

        [EventMethod(EventEnum.GridCheckVisible)]
        public void OnObservePositionChange(object sender, EventArgs args)
        {
            gameObject.SetActive(cachedTransform.position.x >= 0 && cachedTransform.position.z >= 0);
        }
    }
}