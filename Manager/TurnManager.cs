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

        protected override void Awake()
        {
            base.Awake();
            turnManager = GetComponent<PunTurnManager>();
            turnManager.TurnDuration = 40;
            turnManager.TurnManagerListener = this;
        }

        public void OnPlayerFinished(PhotonPlayer player, int turn, object move)
        {
            Debug.Log(move);
        }

        public void OnPlayerMove(PhotonPlayer player, int turn, object move)
        {
            var loc = (Locate)move;
            Debug.Log(string.Format("Player : {0}, TurnIdx : {1}, MoveIdx : {2}, {3}", player.ID, turn, loc.x, loc.z));
            EventHandlerManager.Invoke(EventEnum.CharacterMove, this, new TValueEventArgs<PhotonPlayer, Locate>(player, loc));
        }

        public void OnTurnBegins(int turn)
        {
            EventHandlerManager.Invoke(EventEnum.BeginTurn, this, new TValueEventArgs<int>(turn));
        }

        public void OnTurnCompleted(int turn)
        {
        }

        public void OnTurnTimeEnds(int turn)
        {
        }

        private void StartTurn()
        {
            if (PhotonNetwork.isMasterClient)
            {
                if (this.turnManager.Turn == 0)
                    turnManager.BeginTurn();
            }

            StartCoroutine(TurnTimer());
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
        [EventMethod(EventEnum.FillGameRoom)]
        public void StartTurn(object sender, EventArgs args)
        {
            Debug.Log("Start Turn");
            if (this.turnManager.Turn == 0)
                StartTurn();
        }

        [EventMethod(EventEnum.Send_CharacterMove)]
        public void SendMove(object sender, EventArgs args)
        {
            Debug.Log("Send Move");
            TValueEventArgs<Locate> eventArgs = args as TValueEventArgs<Locate>;
            turnManager.SendMove(eventArgs.arg, true);
        }

        [EventMethod(EventEnum.Send_CharacterStop)]
        public void SendStop(object sender, EventArgs args)
        {
            Debug.Log("Send Stop");
            turnManager.
        }

        #endregion   
    }
}
