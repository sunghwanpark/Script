using System.Collections;
using System.Collections.Generic;
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
            PhotonNetwork.Instantiate("Prefabs/player_demo", new Vector3(0f, 500f, 0f), Quaternion.identity, 0);
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
            CharacterManager.Instance.RemoveCharacter(otherPlayer.ID);
        }

        private void OnEventHandler(byte eventCode, object content, int senderId)
        {
            switch (eventCode)
            {
                case CustomEventCode.FirstLocate:

                    Dictionary<byte, object> evTable = content as Dictionary<byte, object>;
                    int firstTurnIdx = (int)evTable[CustomParameterCode.FirstTurn];

                    Dictionary<byte, object> locateInfos = evTable[CustomParameterCode.Locate] as Dictionary<byte, object>;

                    int count = 0;
                    bool isMyTurn = false;
                    var enum_erator = locateInfos.GetEnumerator();
                    while(enum_erator.MoveNext())
                    {
                        byte id = (byte)enum_erator.Current.Key;
                        Locate locate = (Locate)enum_erator.Current.Value;

                        CharacterBase _char = CharacterManager.Instance.GetCharacter(id);
                        if (_char != null)
                        {
                            _char.SetLocate(locate);
                            if (_char.IsMine)
                            {
                                if (firstTurnIdx == count)
                                    isMyTurn = true;
                                EventHandlerManager.Invoke(EventEnum.Set_CameraTarget, this, new TValueEventArgs<Transform>(_char.transform));
                            }
                        }
                        count++;
                    }
                    Debug.LogFormat("My Turn {0}, TurnIdx {1}", isMyTurn, firstTurnIdx);
                    EventHandlerManager.Invoke(EventEnum.BeginTurn, this, new TValueEventArgs<bool>(isMyTurn));
                    break;
            }
        }
    }
}