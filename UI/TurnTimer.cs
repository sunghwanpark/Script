using System;

namespace SHProject.Ingame.UI
{
    [AutoRegisterEvent]
    public class TurnTimer : MonoBehaviourBase
    {
        private UILabel _turnTimer;

        protected override void Awake()
        {
            base.Awake();
            _turnTimer = GetComponent<UILabel>();

            gameObject.SetActive(false);

            TurnManager.Instance.Event_TurnTimer += OnTurnTimer;
        }

        [EventMethod(EventEnum.BeginTurn)]
        public void StartTurn(object sender, EventArgs args)
        {
            gameObject.SetActive(true);
        }

        public void OnTurnTimer(float timer)
        {
            _turnTimer.text = ((int)timer).ToString();
        }
    }
}