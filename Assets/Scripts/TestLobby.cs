using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using IngameDebugConsole;

public class TestLobby : MonoBehaviour
{
    private Lobby hostLobby;
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
    }

    private async void Createlobby()
    {
        try
        {
            string lobbyName="MyLobby";
            int maxPlayers = 4;
            Lobby lobby= await LobbyService.Instance.CreateLobbyAsync(lobbyName,maxPlayers);
            hostLobby = lobby;
            Debug.Log($"Created Lobby! {lobby.Name} {lobby.MaxPlayers}");
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
}
