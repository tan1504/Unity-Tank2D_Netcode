using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DetroySeflOnContact : MonoBehaviour
{
	private void OnTriggerEnter2D(Collider2D collision)
	{
		if (collision != null) 
			Destroy(gameObject);
	}
}
