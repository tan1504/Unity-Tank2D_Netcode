using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class PlayerAim : NetworkBehaviour
{
    [SerializeField] private InputReader inputReader;
    [SerializeField] private Transform turretTransform;

	private void LateUpdate()
	{
		if (!IsOwner)
			return;

		Vector2 mousePosition = Camera.main.ScreenToWorldPoint(inputReader.AimPosion);
		Vector2 aimDirection = mousePosition - (Vector2)transform.position;
		float aimAngle = Mathf.Atan2(aimDirection.y, aimDirection.x) * Mathf.Rad2Deg - 90f;

		turretTransform.rotation = Quaternion.Euler(0, 0, aimAngle);
	}
}
