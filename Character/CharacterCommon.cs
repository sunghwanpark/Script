using System;
using System.Collections;
using UnityEngine;

namespace SHProject.Ingame
{
    [AutoRegisterEvent]
    public partial class CharacterBase : CommonLocatePhoton
    {
        [SetField(typeof(GameObject), "selected")]
        protected GameObject selectObject;

        private Vector3 lookAt = Vector3.forward;
        protected virtual short Sight { get; }

        [EventMethod(EventEnum.CharacterMove, EventRegisterCall.Awake, EventRegisterCall.OnDestroy)]
        public void OnChracterMove(object sender, EventArgs args)
        {
            TValueEventArgs<PhotonPlayer, Locate> eventArgs = args as TValueEventArgs<PhotonPlayer, Locate>;
            selectObject.gameObject.SetActive(false);
            if (eventArgs.arg1.ID != photonView.ownerId)
                return;

            Vector3 charPos = Map.Instance.GetMapPosition(LocateIdx);
            Vector3 targetPos = Map.Instance.GetMapPosition(eventArgs.arg2);

            Vector3 targetLookAt = (targetPos - charPos).normalized;
            StartCoroutine(RotateLerp(targetLookAt, targetPos));
        }

        [EventMethod(EventEnum.CharacterStop, EventRegisterCall.Awake, EventRegisterCall.OnDestroy)]
        void OnCharacterTeleport(object sender, EventArgs args)
        {
            TValueEventArgs<PhotonPlayer, Locate> eventArgs = args as TValueEventArgs<PhotonPlayer, Locate>;
            selectObject.gameObject.SetActive(false);
            if (eventArgs.arg1.ID != photonView.ownerId)
                return;

            cachedTransform.localPosition = Map.Instance.GetMapPosition(eventArgs.arg2);
        }

        private void OnMouseDown()
        {
            if (!photonView.isMine)
                return;
            if (!TurnManager.Instance.IsMyTurn)
                return;

            selectObject.SetActive(true);
            GridManager.Instance.SetCharacterGrid(LocateIdx.x, LocateIdx.z, Sight);
        }

        IEnumerator RotateLerp(Vector3 targetLookAt, Vector3 targetPos)
        {
            var tarQuat = Quaternion.LookRotation(targetLookAt);
            while(!cachedTransform.localRotation.AlmostEquals(tarQuat, 0.5f))
            {
                cachedTransform.localRotation
                    = Quaternion.Lerp(cachedTransform.localRotation, tarQuat, Time.fixedDeltaTime * 3.0f);
                yield return null;
            }

            cachedTransform.localRotation.SetLookRotation(targetLookAt);
            StartCoroutine(Move(targetPos));
        }

        IEnumerator Move(Vector3 targetPos)
        {
            Anim_State = CharacterAnimParam.Run;

            while(!cachedTransform.localPosition.AlmostEquals(targetPos, 0.1f))
            {
                cachedTransform.localPosition =
                    Vector3.MoveTowards(cachedTransform.localPosition, targetPos, Time.fixedDeltaTime * 3.0f);
                yield return null;
            }

            Anim_State = CharacterAnimParam.Idle;

            var locate = Map.Instance.GetMapIndex(targetPos);
            LocateIdx = locate;

            if(photonView.isMine)
                EventHandlerManager.Invoke(EventEnum.Send_CharacterStop, this, new TValueEventArgs<Locate>(LocateIdx));
        }

        
    }
}
