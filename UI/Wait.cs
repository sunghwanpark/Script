using System;

namespace SHProject.Ingame.UI
{
    [AutoRegisterEvent]
    public class Wait : MonoBehaviourBase
    {
        [EventMethod(EventEnum.MyTurn)]
        public void OnMyTurn(object sender, EventArgs args)
        {
            TValueEventArgs<bool> eventArgs = args as TValueEventArgs<bool>;
            this.gameObject.SetActive(!eventArgs.arg);
        }
    }
}