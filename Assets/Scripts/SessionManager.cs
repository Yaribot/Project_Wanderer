using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Multiplayer;
using UnityEngine;
//using UnityUtils;
//using Cysharp.Threading.task;


public class SessionManager : MonoBehaviour
{
    ISession activeSession;
    ISession ActiveSession
    {
        get => activeSession;
        set
        {
            activeSession = value;
            Debug.Log($"Active session : {activeSession}");
        }
    }

    async void Start()
    {
        try
        {
            await UnityServices.InitializeAsync(); // Initialize Unity Gaming Services SDKs.
            await AuthenticationService.Instance.SignInAnonymouslyAsync(); // Anonymously authenticate the player
            Debug.Log($"Sign in anonymously succeeded ! PlayerID : {AuthenticationService.Instance.PlayerId}");
        }
        catch(Exception e)
        {
            Debug.LogException(e);
        }
    }

    //async Task<Dictionary<string, PlayerProperty>> GetPlayerProperties() { }

    async void StartSessionAshost()
    {
        var option = new SessionOptions
        {
            MaxPlayers = 4,
            IsLocked = false,
            IsPrivate = false,
        };
    }
}
