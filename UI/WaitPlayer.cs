using System;

namespace SHProject.Ingame.UI
{
    [AutoRegisterEvent]
    public class WaitPlayer : MonoBehaviourBase
    {
        [SetField(typeof(TweenAlpha), "Tweener")]
        private TweenAlpha _tweener;

        protected override void Awake()
        {
            base.Awake();
            gameObject.SetActive(false);
        }

        [EventMethod(EventEnum.JoinRoom)]
        public void OnJoinRoom(object sender, EventArgs args)
        {
            gameObject.SetActive(true);

            _tweener.enabled = true;
            _tweener.ResetToBeginning();
            _tweener.PlayForward();
        }

        [EventMethod(EventEnum.BeginTurn)]
        public void OnFillRoom(object sender, EventArgs args)
        {
            _tweener.enabled = false;
            gameObject.SetActive(false);
        }
    }
}