using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using IngameDebugConsole;

public class TestLobby : MonoBehaviour
{
    private Lobby hostLobby;
    private float hartbeatTimer;
    private float hartbeatTimerMax=15f;
    private async void Start()
    {
        await UnityServices.InitializeAsync();
        AuthenticationService.Instance.SignedIn+=()=>
        {
            Debug.Log("Signed in "+AuthenticationService.Instance.PlayerId);
        };
        await AuthenticationService.Instance.SignInAnonymouslyAsync();
        DebugLogConsole.AddCommand("Createlobby","Creates a lobby ",Createlobby);
        DebugLogConsole.AddCommand("Listlobby","List all active lobby ",ListLobbie);
        DebugLogConsole.AddCommand<string>("JoinlobbyByCode","Joins the lobby with a Code",JoinLobbyByCode);
        DebugLogConsole.AddCommand("QuickJoin","Quick Joins a public lobby ",QuickJoinLobby);
    }

    private void Update()
    {
        LobbyHartbeatHandler();
    }

    private async void LobbyHartbeatHandler() 
    {
        if(hostLobby!=null)
        {
            hartbeatTimer-=Time.deltaTime;
            if(hartbeatTimer<=0f)
            {
                hartbeatTimer=hartbeatTimerMax;
                await LobbyService.Instance.SendHeartbeatPingAsync(hostLobby.Id);
            }
        }
    }

    private async void Createlobby()
    {
        try
        {
            string lobbyName="MyLobby";
            int maxPlayers = 4;
            CreateLobbyOptions createLobbyOptions = new CreateLobbyOptions
            {
                IsPrivate = false,
            };
            Lobby lobby= await LobbyService.Instance.CreateLobbyAsync(lobbyName,maxPlayers,createLobbyOptions);
            hostLobby = lobby;
            Debug.Log($"Created Lobby! with the name:{lobby.Name}, Maxplayer:{lobby.MaxPlayers},lobby id:{lobby.Id},lobby Code{lobby.LobbyCode}");
        }
        catch(LobbyServiceException e)
        {
            Debug.Log(e);
        }
            
    }

    private async void ListLobbie() 
    {
        try
        {
            QueryResponse queryResponse = await LobbyService.Instance.QueryLobbiesAsync();
            Debug.Log("Lobbies Found: "+queryResponse.Results.Count);
            foreach (Lobby lobby in queryResponse.Results)
            {
                Debug.Log(lobby.Name+" "+lobby.MaxPlayers);
            }
        }
        catch(LobbyServiceException e)
        {
            Debug.Log(e);   
        }
    }

    private async void JoinLobbyByCode(string lobbyCode)
    {
        try
        {
            await LobbyService.Instance.JoinLobbyByCodeAsync(lobbyCode);
            Debug.Log($"Joined Lobby By Lobby Code:{lobbyCode}");
        }
        catch(LobbyServiceException e)
        {
            Debug.Log(e);
        }
        
    }

    private async void QuickJoinLobby()
    {
        try
        {
            await LobbyService.Instance.QuickJoinLobbyAsync();
            Debug.Log("Enterd a lobby ");
        }
        catch(LobbyServiceException e)
        {
            Debug.Log(e);
        }
    }
}
