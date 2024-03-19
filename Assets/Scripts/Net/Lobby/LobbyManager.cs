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
   private Lobby hostLobby;
   private float heartbeatTimer;
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
      if (Input.GetKeyDown(KeyCode.L)) CreateLobby();
      if (Input.GetKeyDown(KeyCode.K)) ListLobbies();
      if (Input.GetKeyDown(KeyCode.J)) JoinLobby();
      
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
   
   private async void CreateLobby()
   {
      try
      {
         string lobbyName = "MyLobby";
         int maxPlayer = 4;

         CreateLobbyOptions createLobbyOptions = new CreateLobbyOptions
         {
            IsPrivate = false,
            Player = GetPlayer()
         };
         
         Lobby lobby = await LobbyService.Instance.CreateLobbyAsync(lobbyName, maxPlayer,createLobbyOptions);

         hostLobby = lobby;

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
               new QueryFilter(QueryFilter.FieldOptions.AvailableSlots, "0", QueryFilter.OpOptions.GT)
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
            Debug.Log(lobby.Name + " " + lobby.MaxPlayers);
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
         Lobby joinedLobby = await Lobbies.Instance.JoinLobbyByCodeAsync(lobbyCode,joinLobbyByCodeOptions);
         
         Debug.Log("Joined lobby with code" + lobbyCode);
         PrintPlayers(joinedLobby);
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

   void PrintPlayers(Lobby lobby)
   {
      Debug.Log("Player in lobby" + lobby.Name);
      foreach (Player player in lobby.Players)
      {
         Debug.Log(player.Id + " " + player.Data["PlayerName"].Value);
      }
   }
}
