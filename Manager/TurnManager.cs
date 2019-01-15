using System;
using System.Collections;
using UnityEngine;

namespace SHProject.Ingame
{
    [AutoRegisterEvent]
    public class TurnManager : PunSingleton<TurnManager>, IPunTurnManagerCallbacks, ITurnListener
    {
        private PunTurnManager turnManager = null;

        public Action<float> Event_TurnTimer { get; set; }
        public bool IsMyTurn
        {
            get
            {
                return PhotonNetwork.isMasterClient ? turnManager.Turn % 2 == 1 : turnManager.Turn % 2 == 0;
            }
            private set { }
        }

        protected override void Awake()
        {
            base.Awake();
            turnManager = GetComponent<PunTurnManager>();
            turnManager.TurnDuration = 40;
            turnManager.TurnManagerListener = this;
        }

        // 텔레포트, 다음턴을 위한 연출 시작
        public void OnPlayerFinished(PhotonPlayer player, int turn, object move)
        {
            Debug.Log(move);

            var loc = (Locate)move;
            Debug.Log(string.Format("Player : {0}, TurnIdx : {1}, MoveIdx : {2}, {3}", player.ID, turn, loc.x, loc.z));
            EventHandlerManager.Invoke(EventEnum.CharacterStop, this, new TValueEventArgs<PhotonPlayer, Locate>(player, loc));
            EventHandlerManager.Invoke(EventEnum.BeginTurn, this, new TValueEventArgs<bool>(IsMyTurn));
        }

        public void OnPlayerMove(PhotonPlayer player, int turn, object move)
        {
            var loc = (Locate)move;
            Debug.Log(string.Format("Player : {0}, TurnIdx : {1}, MoveIdx : {2}, {3}", player.ID, turn, loc.x, loc.z));
            EventHandlerManager.Invoke(EventEnum.CharacterMove, this, new TValueEventArgs<PhotonPlayer, Locate>(player, loc));
        }

        public void OnTurnBegins(int turn)
        {
            Debug.LogFormat("{0} OnTurnBegins", turn);
        }

        public void OnTurnCompleted(int turn)
        {
            Debug.LogFormat("{0} OnTurnCompleted", turn);
        }

        public void OnTurnTimeEnds(int turn)
        {
            Debug.LogFormat("{0} OnTurnTimeEnds", turn);
        }

        private void StartTurn()
        {
            if (PhotonNetwork.isMasterClient)
                turnManager.BeginTurn();

            StartCoroutine(TurnTimer());
            EventHandlerManager.Invoke(EventEnum.MyTurn, this, new TValueEventArgs<bool>(IsMyTurn));
        }

        IEnumerator TurnTimer()
        {
            while (turnManager.Turn > 0)
            {
                Event_TurnTimer?.Invoke(turnManager.RemainingSecondsInTurn);
                yield return new WaitForSeconds(1f);
            }
        }

        #region EVENT_METHOD
        [EventMethod(EventEnum.StartTurn)]
        public void StartTurn(object sender, EventArgs args)
        {
            Debug.Log("Start Turn");
            StartTurn();
        }

        [EventMethod(EventEnum.Send_CharacterMove)]
        public void SendMove(object sender, EventArgs args)
        {
            Debug.Log("Send Move");
            TValueEventArgs<Locate> eventArgs = args as TValueEventArgs<Locate>;
            turnManager.SendMove(eventArgs.arg, false);
        }

        [EventMethod(EventEnum.Send_CharacterStop)]
        public void SendStop(object sender, EventArgs args)
        {
            Debug.Log("Send Stop");
            TValueEventArgs<Locate> eventArgs = args as TValueEventArgs<Locate>;
            turnManager.SendMove(eventArgs.arg, true);
        }

        [EventMethod(EventEnum.CharacterMove)]
        public void StartCharacterMove(object sender, EventArgs args)
        {
            if (PhotonNetwork.isMasterClient)
            {
                StopCoroutine(TurnTimer());
                EventHandlerManager.Invoke(EventEnum.MyTurn, this, new TValueEventArgs<bool>(false));
            }
        }
        #endregion   
    }
}
