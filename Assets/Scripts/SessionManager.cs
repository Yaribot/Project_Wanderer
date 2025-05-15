using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Multiplayer;
using UnityEngine;
//using UnityUtils;
//using Cysharp.Threading.task;


public class SessionManager : Singleton<SessionManager>
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

    const string playerNamePropertyKey = "playerName";
    async void Start()
    {
        try
        {
            await UnityServices.InitializeAsync(); // Initialize Unity Gaming Services SDKs.
            await AuthenticationService.Instance.SignInAnonymouslyAsync(); // Anonymously authenticate the player
            Debug.Log($"Sign in anonymously succeeded ! PlayerID : {AuthenticationService.Instance.PlayerId}");

            // Start a new session as a host
            StartSessionAshost();
        }
        catch(Exception e)
        {
            Debug.LogException(e);
        }
    }

    //private void RegisterSessionEvent()
    //{
    //    ActiveSession.Changed += OnSessionChanged;
    //}
    //private void UnregisterSessionEvent()
    //{
    //    ActiveSession.Changed -= OnSessionChanged;
    //}

    async Task<Dictionary<string, PlayerProperty>> GetPlayerProperties() 
    {
        // Custom game-specific properties that apply an individual player, ie: name, role, skill level, etc...
        var playerName = await AuthenticationService.Instance.GetPlayerNameAsync();
        var playerNameProperty = new PlayerProperty(playerName, VisibilityPropertyOptions.Member);
        return new Dictionary<string, PlayerProperty> { { playerNamePropertyKey, playerNameProperty } };
    }

    async void StartSessionAshost()
    {
        var playerProperties = await GetPlayerProperties();
        var option = new SessionOptions
        {
            MaxPlayers = 4,
            IsLocked = false,
            IsPrivate = false,
            PlayerProperties = playerProperties
        }.WithDistributedAuthorityNetwork();
        //.WithRelayNetwork(); // Or WithDistributedAuthorityNetwork() to use Distributed Authority instead of Relay

        ActiveSession = await MultiplayerService.Instance.CreateSessionAsync(option);
        Debug.Log($"Session {ActiveSession.Id} created ! Join code: {ActiveSession.Code}");
    }

    async Task JoinSessionById(string sessionId)
    {
        ActiveSession = await MultiplayerService.Instance.JoinSessionByIdAsync(sessionId);
        Debug.Log($"Session {ActiveSession.Id} joined !");
    }
    
    async Task JoinSessionByCode(string sessionCode)
    {
        ActiveSession = await MultiplayerService.Instance.JoinSessionByCodeAsync(sessionCode);
        Debug.Log($"Session {ActiveSession.Id} joined !");
    }

    async Task KickPlayer(string plauerId)
    {
        if (!ActiveSession.IsHost) return;
        await ActiveSession.AsHost().RemovePlayerAsync(plauerId);
    }

    // List all the sessions the player could join
    async Task<IList<ISessionInfo>> QuerySessions()
    {
        var sessionQuerryOptions = new QuerySessionsOptions();
        QuerySessionsResults results = await MultiplayerService.Instance.QuerySessionsAsync(sessionQuerryOptions);
        return results.Sessions;
    }

    async Task LeaveSession()
    {
        if(ActiveSession != null)
        {
            try
            {
                //UnregisterSessionEvent();
                await ActiveSession.LeaveAsync();
            }
            catch
            {
                // Ignored as we are exiting the game
            }
            finally
            {
                ActiveSession = null;
            }
        }
    }
}
