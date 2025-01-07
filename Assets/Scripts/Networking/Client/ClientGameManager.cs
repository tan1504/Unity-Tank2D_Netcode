using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Networking.Transport.Relay;
using Unity.Services.Core;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ClientGameManager
{
    private JoinAllocation allocation;

    private const string MenuSceneName = "Menu";

    public async Task<bool> InitAsync()
    {
        // Authenticate player
        await UnityServices.InitializeAsync();

        AuthState authState = await AuthenticationWrapper.DoAuth();

        if (authState == AuthState.Authenticated)
            return true;

        return false;
    }

	internal void GoToMenu()
	{
        SceneManager.LoadScene(MenuSceneName);
	}

	public async Task StartClientAsync(string joinCode)
	{
        try
        {
            allocation = await Relay.Instance.JoinAllocationAsync(joinCode);
        }
        catch (Exception e)
        {
            Debug.Log(e);
            return;
        }

		UnityTransport transport = NetworkManager.Singleton.GetComponent<UnityTransport>();

		RelayServerData serverData = new RelayServerData(allocation, "udp");    // protocol
		transport.SetRelayServerData(serverData);

		NetworkManager.Singleton.StartClient();
	}
}
