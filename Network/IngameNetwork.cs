using System.Collections;
using UnityEngine;
using SHProject.Ingame;

namespace SHProject.Network
{
    public class IngameNetwork : PunSingleton<IngameNetwork>
    {
        private const int maxPlayerNum = 2;

        private Vector3 previousPos = Vector3.one;
        private Transform myPlayer;

        private void Start()
        {
            PhotonNetwork.OnEventCall += OnEventHandler;
        }

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
            roomOptions.Plugins = new string[] { "JoinExtensionPlugin" };
            PhotonNetwork.CreateRoom(null, roomOptions, TypedLobby.Default);
        }

        public override void OnJoinedRoom()
        {
            EventHandlerManager.Invoke(EventEnum.JoinRoom, this, null);
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
            Debug.Log(newPlayer);

            //if (PhotonNetwork.room.PlayerCount == maxPlayerNum)
            //    EventHandlerManager.Invoke(EventEnum.StartTurn, this, null);
        }

        public override void OnPhotonPlayerDisconnected(PhotonPlayer otherPlayer)
        {
            base.OnPhotonPlayerDisconnected(otherPlayer);
        }

        private GameObject MakeCharacter()
        {
            var obj = PhotonNetwork.Instantiate("Prefabs/player_demo", Vector3.one, Quaternion.identity, 0);
            obj.transform.localRotation = Quaternion.identity;
            return obj;
        }

        private void OnEventHandler(byte eventCode, object content, int senderId)
        {
            switch (eventCode)
            {
                case CustomEventCode.FirstLocate:
                    Hashtable evTable = content as Hashtable;
                    var enum_erator = evTable.GetEnumerator();
                    while(enum_erator.MoveNext())
                    {
                        byte id = (byte)enum_erator.Key;
                        Locate locate = (Locate)enum_erator.Value;

                        var obj = MakeCharacter();
                        obj.transform.localPosition = Map.Instance.GetMapPosition(locate);
                        if(id == PhotonNetwork.player.ID)
                            EventHandlerManager.Invoke(EventEnum.Set_CameraTarget, this, new TValueEventArgs<Transform>(obj.transform));
                    }

                    EventHandlerManager.Invoke(EventEnum.BeginTurn, this, null);
                    break;
            }
        }
    }
}