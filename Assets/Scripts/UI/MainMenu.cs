using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;

public class MainMenu : MonoBehaviour
{
	[SerializeField] private TMP_InputField joinCodeField;

	public async void StartHost()
	{
		await HostSingleton.instance.gameManager.StartHostAsync();
	}

	public async void StartClient()
	{
		await ClientSingleton.instance.gameManager.StartClientAsync(joinCodeField.text);
	}
}
