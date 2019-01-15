using System;
using System.Collections;
using UnityEngine;

namespace SHProject.Ingame.UI
{
    [AutoRegisterEvent]
    public class TurnAlarm : MonoBehaviourBase
    {
        [SetField(typeof(UILabel), "my_turn")]
        private UILabel _myTurn;

        [SetField(typeof(UILabel), "enemy_turn")]
        private UILabel _enemyTurn;

        protected override void Awake()
        {
            base.Awake();
            gameObject.SetActive(false);
        }

        [EventMethod(EventEnum.BeginTurn)]
        public void OnBeginTurn(object sender, EventArgs args)
        {
            TValueEventArgs<bool> eventArgs = args as TValueEventArgs<bool>;

            gameObject.SetActive(true);
            StartCoroutine(TurnAlarmProcess(eventArgs.arg));
        }

        private IEnumerator TurnAlarmProcess(bool isMyTurn)
        {
            _myTurn.cachedGameObject.SetActive(isMyTurn);
            _enemyTurn.cachedGameObject.SetActive(!isMyTurn);

            yield return new WaitForSeconds(2.0f);

            gameObject.SetActive(false);

            EventHandlerManager.Invoke(EventEnum.StartTurn, this, null);
        }
    }
}