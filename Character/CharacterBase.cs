using UnityEngine;
using SHProject.Network;

namespace SHProject.Ingame
{
    [RequireComponent(typeof(PhotonView))]
    public partial class CharacterBase : CommonLocatePhoton
    {
        public bool IsMine => photonView.isMine;

        private bool isMyTurn = false;

        private void OnMouseDown()
        {
            if (!photonView.isMine)
                return;
            if (!isMyTurn)
                return;

            selectObject.SetActive(true);
            GridManager.Instance.SetCharacterGrid(LocateIdx.x, LocateIdx.z, Sight);
        }

        public void SetLocate(Locate locate)
        {
            this.LocateIdx = locate;

            Vector3 pos = cachedTransform.localPosition;
            pos.y = 0f;
            cachedTransform.localPosition = pos;
        }

        public override void OnPhotonInstantiate(PhotonMessageInfo info)
        {
            Debug.Log(info);
            CharacterManager.Instance.RegisterCharacter(info.sender.ID, this);
        }
    }
}
