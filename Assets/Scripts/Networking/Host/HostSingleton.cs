using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class HostSingleton : MonoBehaviour
{
    public static HostSingleton instance;

	public HostGameManager gameManager;

	private void Awake()
	{
		if (instance == null)
		{
			instance = this;
			DontDestroyOnLoad(gameObject);
		}
		else
		{
			Debug.LogError("Another instance of HostSingleton already exists!");
			Destroy(gameObject);
		}
	}


	public void CreateHost()
	{
		gameManager = new HostGameManager();
	}
}
