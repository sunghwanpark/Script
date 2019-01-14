using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SHProject.Ingame
{
    public class IngameNetwork : PunSingleton<IngameNetwork>
    {
        private const int maxPlayerNum = 2;

        private Vector3 previousPos = Vector3.one;
        private Transform myPlayer;

        public void Login(string userId)
        {
            PhotonNetwork.AuthValues = new AuthenticationValues();
            PhotonNetwork.AuthValues.UserId = userId;
            PhotonNetwork.ConnectUsingSettings(null);
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            PhotonNetwork.Disconnect();
        }

        public override void OnJoinedLobby()
        {
            Debug.Log("JoinRandom");
            PhotonNetwork.JoinRandomRoom();
        }

        public override void OnConnectedToMaster()
        {
            // when AutoJoinLobby is off, this method gets called when PUN finished the connection (instead of OnJoinedLobby())
            PhotonNetwork.JoinRandomRoom();
        }

        public void OnPhotonRandomJoinFailed()
        {
            RoomOptions roomOptions = new RoomOptions();
            roomOptions.MaxPlayers = 2;
            roomOptions.CleanupCacheOnLeave = true;
            PhotonNetwork.CreateRoom(null, roomOptions, TypedLobby.Default);
        }

        public override void OnJoinedRoom()
        {
            var obj = PhotonNetwork.Instantiate("Prefabs/player_demo", Vector3.one, Quaternion.identity, 0);

            myPlayer = obj.transform;
            previousPos = myPlayer.localPosition;
            EventHandlerManager.Invoke(EventEnum.CharacterJoin, this, new TValueEventArgs<Transform>(obj.transform));
        }

        public override void OnPhotonPlayerActivityChanged(PhotonPlayer otherPlayer)
        {
            base.OnPhotonPlayerActivityChanged(otherPlayer);
        }

        public override void OnOwnershipTransfered(object[] viewAndPlayers)
        {
            base.OnOwnershipTransfered(viewAndPlayers);
            Debug.Log(viewAndPlayers);
        }

        public override void OnPhotonPlayerConnected(PhotonPlayer newPlayer)
        {
            if (PhotonNetwork.room.PlayerCount == maxPlayerNum)
                EventHandlerManager.Invoke(EventEnum.StartTurn, this, null);
        }

        public override void OnPhotonPlayerDisconnected(PhotonPlayer otherPlayer)
        {
            base.OnPhotonPlayerDisconnected(otherPlayer);
        }
    }
}