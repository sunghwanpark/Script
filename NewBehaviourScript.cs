using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using ExitGames.Client.Photon;
using System.Collections.Generic;
using System;

public class NewBehaviourScript : Photon.PunBehaviour
{
    private Vector3 previousPos = Vector3.one;
    private Transform myPlayer;
    private Dictionary<int, PhotonPlayer> players = new Dictionary<int, PhotonPlayer>();

    private void Start()
    {
        PhotonNetwork.ConnectUsingSettings(null);
    }

    private void OnDestroy()
    {
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
        PhotonNetwork.CreateRoom(null);
    }

    public override void OnJoinedRoom()
    {
        var obj = PhotonNetwork.Instantiate("Prefabs/player_demo", Vector3.one, Quaternion.identity, 0);

        myPlayer = obj.transform;
        previousPos = myPlayer.localPosition;
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
        players.Add(newPlayer.ID, newPlayer);
    }

    public override void OnPhotonPlayerDisconnected(PhotonPlayer otherPlayer)
    {
        base.OnPhotonPlayerDisconnected(otherPlayer);

        players.Remove(otherPlayer.ID);
    }
}
