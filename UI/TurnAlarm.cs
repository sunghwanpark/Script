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
        
        [EventMethod(EventEnum.BeginTurn)]
        public void OnBeginTurn(object sender, EventArgs args)
        {
            TValueEventArgs<bool> eventArgs = args as TValueEventArgs<bool>;

            StartCoroutine(TurnAlarmProcess(eventArgs.arg));
        }

        private IEnumerator TurnAlarmProcess(bool isMyTurn)
        {
            _myTurn.cachedGameObject.SetActive(isMyTurn);
            _enemyTurn.cachedGameObject.SetActive(!isMyTurn);

            yield return new WaitForSeconds(2.0f);

            _myTurn.cachedGameObject.SetActive(false);
            _enemyTurn.cachedGameObject.SetActive(false);

            EventHandlerManager.Invoke(EventEnum.StartTurn, this, null);
        }
    }
}