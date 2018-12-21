using System;

namespace SHProject.Ingame.UI
{
    [AutoRegisterEvent]
    public class Wait : MonoBehaviourBase
    {
        [EventMethod(EventEnum.MyTurn)]
        public void OnMyTurn(object sender, EventArgs args)
        {
            this.gameObject.SetActive(false);
        }
    }
}