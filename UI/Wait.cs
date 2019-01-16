using System;

namespace SHProject.Ingame.UI
{
    [AutoRegisterEvent]
    public class Wait : MonoBehaviourBase
    {
        [SetField(typeof(UISprite), "Wait")]
        private UISprite _sprWait;

        [EventMethod(EventEnum.MyTurn)]
        public void OnMyTurn(object sender, EventArgs args)
        {
            TValueEventArgs<bool> eventArgs = args as TValueEventArgs<bool>;
            _sprWait.gameObject.SetActive(!eventArgs.arg);
        }
    }
}