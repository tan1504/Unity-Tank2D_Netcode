using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class ApplicationController : MonoBehaviour
{
	[SerializeField] private ClientSingleton clientPrefab;
	[SerializeField] private HostSingleton hostPrefab;

	private async void Start()
	{
		DontDestroyOnLoad(gameObject);

		// Dedicated servers dont have a person playing on them, so they dont need any graphics and any rendering
		await LaunchInMode(SystemInfo.graphicsDeviceType == UnityEngine.Rendering.GraphicsDeviceType.Null);
	}

	private async Task LaunchInMode(bool isDedicatedServer)
	{
		if (isDedicatedServer)
		{
			
		}
		else
		{
			ClientSingleton clientSingleton = Instantiate(clientPrefab);
			bool authenticated = await clientSingleton.CreateClient();
			  
			HostSingleton hostSingleton = Instantiate(hostPrefab);
			hostSingleton.CreateHost();

			// Go to main menu
			if (authenticated)
			{
				clientSingleton.gameManager.GoToMenu();
			}
		}
	}
}
