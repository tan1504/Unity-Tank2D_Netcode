using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class ClientSingleton : MonoBehaviour
{
	public static ClientSingleton instance;

	public ClientGameManager gameManager {  get; private set; }

	private void Awake()
	{
		if (instance == null)
		{
			instance = this;
			DontDestroyOnLoad(gameObject);
		}
		else
		{
			Debug.LogError("Another instance of ClientSingleton already exists!");
			Destroy(gameObject);
		}
	}

	public async Task<bool> CreateClient()
	{
		gameManager = new ClientGameManager();

		return await gameManager.InitAsync();
	}
}
