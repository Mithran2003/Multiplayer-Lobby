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
        DebugLogConsole.AddCommand("Joinlobby","Joins the first active lobby ",JoinLobby);
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
            Lobby lobby= await LobbyService.Instance.CreateLobbyAsync(lobbyName,maxPlayers);
            hostLobby = lobby;
            Debug.Log($"Created Lobby! {lobby.Name} {lobby.MaxPlayers} , lobby id:{lobby.Id}");
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

    private async void JoinLobby()
    {
        try
        {
            QueryResponse queryResponse = await LobbyService.Instance.QueryLobbiesAsync();
            await LobbyService.Instance.JoinLobbyByIdAsync(queryResponse.Results[0].Id);
            Debug.Log("Joined the lobby"+queryResponse.Results[0].Id);
        }
        catch(LobbyServiceException e)
        {
            Debug.Log(e);
        }
        
    }
}
