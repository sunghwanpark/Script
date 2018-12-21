using System;
using UnityEngine;
using ExitGames.Client.Photon;

internal class ChatPeer : IPhotonPeerListener
{
    public bool IsConnected { get; set; } = false;
    public Action<string> ChatMessage { get; set; } = null;
    
    public void DebugReturn(DebugLevel level, string message)
    {
        Debug.LogFormat("{0} : {1}", level.ToString(), message);
    }

    public void OnEvent(EventData eventData)
    {
        DebugReturn(DebugLevel.INFO, eventData.ToStringFull());
        if (eventData.Code == 1)
        {
            DebugReturn(DebugLevel.INFO, string.Format("Chat Message: {0}", eventData.Parameters[1]));
            ChatMessage?.Invoke(eventData.Parameters[eventData.Code].ToString());
        }
    }

    public void OnOperationResponse(OperationResponse operationResponse)
    {
        DebugReturn(DebugLevel.INFO, operationResponse.ToStringFull());
        
    }

    public void OnStatusChanged(StatusCode statusCode)
    {
        if (statusCode == StatusCode.Connect)
            IsConnected = true;

        switch (statusCode)
        {
            case StatusCode.Connect:
                DebugReturn(DebugLevel.INFO, "Connected");
                IsConnected = true;
                break;
            default:
                DebugReturn(DebugLevel.ERROR, statusCode.ToString());
                IsConnected = false;
                break;
        }
    }
}
