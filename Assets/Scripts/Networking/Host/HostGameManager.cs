using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Networking.Transport.Relay;
using Unity.Services.Lobbies;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using UnityEngine;
using UnityEngine.SceneManagement;
using Unity.Services.Lobbies.Models;
using Unity.VisualScripting;
using System.Text;

public class HostGameManager
{
	private Allocation allocation;
	private string joinCode;
	private string lobbyID;

	private NetworkServer networkServer;

	private const int MaxConnections = 20;
	private const string GameSceneName = "Game";

	public async Task StartHostAsync()
	{
		try
		{
			allocation = await Relay.Instance.CreateAllocationAsync(MaxConnections);
		}
		catch (Exception e)
		{
			Debug.Log(e);
			return;
		}

		try
		{
			joinCode = await Relay.Instance.GetJoinCodeAsync(allocation.AllocationId);
			Debug.Log(joinCode);	
		}
		catch (Exception e)
		{
			Debug.Log(e);
			return;
		}

		UnityTransport transport = NetworkManager.Singleton.GetComponent<UnityTransport>();

		RelayServerData serverData = new RelayServerData(allocation, "dtls");    // protocol
		transport.SetRelayServerData(serverData);

		try
		{
			CreateLobbyOptions lobbyOptions = new CreateLobbyOptions();
			lobbyOptions.IsPrivate = false;
			lobbyOptions.Data = new Dictionary<string, DataObject>()
			{
				{
					"JoinCode", new DataObject(
						visibility: DataObject.VisibilityOptions.Member,
						value: joinCode
					)
				}
			};

			string playerName = PlayerPrefs.GetString(NameSelector.PlayerNameKey, "Unknown");
			Lobby lobby = await Lobbies.Instance.CreateLobbyAsync(playerName + "'s "+"Lobby", 
				MaxConnections, lobbyOptions);

			lobbyID = lobby.Id;

			HostSingleton.instance.StartCoroutine(HeartbeatLobby(15));
		}
		catch (LobbyServiceException e)
		{
			Debug.Log(e);
			return;
		}

		networkServer = new NetworkServer(NetworkManager.Singleton);

		UserData userData = new UserData
		{
			userName = PlayerPrefs.GetString(NameSelector.PlayerNameKey, "Missing Name")
		};
		string payload = JsonUtility.ToJson(userData);
		byte[] payloadBytes = Encoding.UTF8.GetBytes(payload);

		NetworkManager.Singleton.NetworkConfig.ConnectionData = payloadBytes;

		NetworkManager.Singleton.StartHost();

		NetworkManager.Singleton.SceneManager.LoadScene(GameSceneName, LoadSceneMode.Single);
	}

	private IEnumerator HeartbeatLobby(float waitTimeSeconds)
	{
		WaitForSecondsRealtime delay = new WaitForSecondsRealtime(waitTimeSeconds);	

		while (true)
		{
			Lobbies.Instance.SendHeartbeatPingAsync(lobbyID);

			yield return delay;
		}
	}
}
