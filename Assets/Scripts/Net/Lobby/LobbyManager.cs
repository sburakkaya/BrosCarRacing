using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using UnityEngine;

public class LobbyManager : MonoBehaviour
{
   private Lobby hostLobby,joinedLobby;
   private float heartbeatTimer,lobbyUpdateTimer;
   private string playerName;
   private async void Start()
   {
      await UnityServices.InitializeAsync();

      AuthenticationService.Instance.SignedIn += () =>
      {
         Debug.Log("Signed in " + AuthenticationService.Instance.PlayerId);
      };
      await AuthenticationService.Instance.SignInAnonymouslyAsync();
      
      playerName = "BrosRacing" + UnityEngine.Random.Range(10, 99);
      Debug.Log(playerName);
   }

   private void Update()
   {
      if (Input.GetKeyDown(KeyCode.C)) CreateLobby();
      if (Input.GetKeyDown(KeyCode.L)) ListLobbies();
      if (Input.GetKeyDown(KeyCode.J)) JoinLobby();
      
      HandleLobbyHeartbeat();
      HandleLobbyHeartbeat(); 
   }

   async void HandleLobbyHeartbeat()
   {
      if (hostLobby != null)
      {
         heartbeatTimer -= Time.deltaTime;
         if (heartbeatTimer < 0f)
         {
            float heartbeatTimerMax = 15;
            heartbeatTimer = heartbeatTimerMax;

            await LobbyService.Instance.SendHeartbeatPingAsync(hostLobby.Id);
         }
      }
   }

   async void HandleLobbyPollForUpdates()
   {
      if (joinedLobby != null)
      {
         lobbyUpdateTimer -= Time.deltaTime;
         if (lobbyUpdateTimer < 0f)
         {
            float lobbyUpdateTimerMax = 1.1f;
            lobbyUpdateTimer = lobbyUpdateTimerMax;

            Lobby lobby = await LobbyService.Instance.GetLobbyAsync(joinedLobby.Id);
            joinedLobby = lobby;
         }
      }
   }
   
   private async void CreateLobby()
   {
      try
      {
         string lobbyName = "MyLobby";
         int maxPlayer = 4;

         CreateLobbyOptions createLobbyOptions = new CreateLobbyOptions
         {
            IsPrivate = false,
            Player = GetPlayer(),
            Data = new Dictionary<string, DataObject> {
               {"GameMode",new DataObject(DataObject.VisibilityOptions.Public,"CaptureTheFlag")},
               {"Map",new DataObject(DataObject.VisibilityOptions.Public,"de_dust2")}
            }
         };
         
         Lobby lobby = await LobbyService.Instance.CreateLobbyAsync(lobbyName, maxPlayer,createLobbyOptions);

         hostLobby = lobby;
         joinedLobby = hostLobby;

         Debug.Log("Created lobby! " + lobby.Name + " " + lobby.MaxPlayers + "" + lobby.Id + "" + lobby.LobbyCode);
         
         PrintPlayers(hostLobby);
      }
      catch (LobbyServiceException e)
      {
         Console.WriteLine(e);
         throw;
      }
   }

   private async void ListLobbies()
   {
      try
      {
         /*QueryLobbiesOptions queryLobbiesOptions = new QueryLobbiesOptions
         {
            Count = 25,
            Filters = new List<QueryFilter>
            {
               new QueryFilter(QueryFilter.FieldOptions.AvailableSlots, "0", QueryFilter.OpOptions.GT),
            },
            Order = new List<QueryOrder>
            {
               new QueryOrder(false, QueryOrder.FieldOptions.Created)
            }
         };*/
         
         QueryResponse queryResponse = await Lobbies.Instance.QueryLobbiesAsync();

         Debug.Log("Lobbies found: " + queryResponse.Results.Count);

         foreach (Lobby lobby in queryResponse.Results)
         {
            Debug.Log(lobby.Name + " " + lobby.MaxPlayers + " " + lobby.Data["GameMode"].Value);
         }
      }
      catch (LobbyServiceException e)
      {
         Console.WriteLine(e);
         throw;
      }
   }

   async void JoinLobby()
   {
      try
      {
         QueryResponse queryResponse = await Lobbies.Instance.QueryLobbiesAsync();
         
         await Lobbies.Instance.JoinLobbyByIdAsync(queryResponse.Results[0].Id);
      }
      catch (LobbyServiceException e)
      {
         Console.WriteLine(e);
         throw;
      }
   }
   
   async void JoinLobbyWithCode(string lobbyCode)
   {
      try
      {
         JoinLobbyByCodeOptions joinLobbyByCodeOptions = new JoinLobbyByCodeOptions
         {
            Player = GetPlayer()
         };
         Lobby lobby = await Lobbies.Instance.JoinLobbyByCodeAsync(lobbyCode,joinLobbyByCodeOptions);
         joinedLobby = lobby;
         
         Debug.Log("Joined lobby with code" + lobbyCode);
         PrintPlayers(lobby);
      }
      catch (LobbyServiceException e)
      {
         Console.WriteLine(e);
         throw;
      }
   }

   async void QuickJoinLobby()
   {
      try
      {
         await LobbyService.Instance.QuickJoinLobbyAsync();
      }
      catch (LobbyServiceException e)
      {
         Console.WriteLine(e);
         throw;
      }
   }

   private Player GetPlayer()
   {
      return new Player
      {
         Data = new Dictionary<string, PlayerDataObject>
         {
            { "PlayerName", new PlayerDataObject(PlayerDataObject.VisibilityOptions.Member, playerName) }
         }
      };
   }

   void PrintPlayers()
   {
      PrintPlayers(joinedLobby);
   }
   void PrintPlayers(Lobby lobby)
   {
      Debug.Log("Player in lobby " + lobby.Name + " " + lobby.Data["GameMode"].Value + " " + lobby.Data["Map"].Value);
      foreach (Player player in lobby.Players)
      {
         Debug.Log(player.Id + " " + player.Data["PlayerName"].Value);
      }
   }

   private async void UpdateLobbyGameMode(string gameMode)
   {
      try
      {
         hostLobby = await Lobbies.Instance.UpdateLobbyAsync(hostLobby.Id, new UpdateLobbyOptions
         {
            Data = new Dictionary<string, DataObject>
            {
               { "GameMode", new DataObject(DataObject.VisibilityOptions.Public, gameMode) }
            }
         });
         joinedLobby = hostLobby;
         
         PrintPlayers(hostLobby);
      }
      catch (LobbyServiceException e)
      {
         Console.WriteLine(e);
         throw;
      }
   }

   async void UpdatePlayerName(string newPlayerName)
   {
      try
      {
         playerName = newPlayerName;
         await LobbyService.Instance.UpdatePlayerAsync(joinedLobby.Id, AuthenticationService.Instance.PlayerId,
            new UpdatePlayerOptions
            {
               Data = new Dictionary<string, PlayerDataObject>
               {
                  { "PlayerName", new PlayerDataObject(PlayerDataObject.VisibilityOptions.Member, playerName) }
               }
            });
      }
      catch (LobbyServiceException e)
      {
         Console.WriteLine(e);
         throw;
      }
   }

   private async void LeaveLobby()
   {
      try
      {
         await LobbyService.Instance.RemovePlayerAsync(joinedLobby.Id, AuthenticationService.Instance.PlayerId);
      }
      catch (LobbyServiceException e)
      {
         Console.WriteLine(e);
         throw;
      }
   }
   
   private async void KickPlayer()
   {
      try
      {
         await LobbyService.Instance.RemovePlayerAsync(joinedLobby.Id, joinedLobby.Players[1].Id);
      }
      catch (LobbyServiceException e)
      {
         Console.WriteLine(e);
         throw;
      }
   }

   async void MigrateLobbyHost()
   {
      try
      {
         hostLobby = await Lobbies.Instance.UpdateLobbyAsync(hostLobby.Id, new UpdateLobbyOptions
         {
            HostId = joinedLobby.Players[1].Id,
         });
         joinedLobby = hostLobby;
         
         PrintPlayers(hostLobby);
      }
      catch (LobbyServiceException e)
      {
         Console.WriteLine(e);
         throw;
      }
   }

   async void DeleteLobby()
   {
      try
      {
         await LobbyService.Instance.DeleteLobbyAsync(joinedLobby.Id);
      }
      catch (LobbyServiceException e)
      {
         Console.WriteLine(e);
         throw;
      }

   }
}
