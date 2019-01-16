using System;

namespace SHProject.Ingame.UI
{
    [AutoRegisterEvent]
    public class WaitPlayer : MonoBehaviourBase
    {
        [SetField(typeof(TweenAlpha), "Tweener")]
        private TweenAlpha _tweener;

        [EventMethod(EventEnum.JoinRoom, EventRegisterCall.OnEnable, EventRegisterCall.OnDisable)]
        public void OnJoinRoom(object sender, EventArgs args)
        {
            _tweener.gameObject.SetActive(true);

            _tweener.enabled = true;
            _tweener.ResetToBeginning();
            _tweener.PlayForward();
        }

        [EventMethod(EventEnum.BeginTurn, EventRegisterCall.OnEnable, EventRegisterCall.OnDisable)]
        public void OnFillRoom(object sender, EventArgs args)
        {
            _tweener.enabled = false;
            _tweener.gameObject.SetActive(false);
        }
    }
}