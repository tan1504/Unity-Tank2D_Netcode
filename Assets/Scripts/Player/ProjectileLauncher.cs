using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class ProjectileLauncher : NetworkBehaviour
{
	[Header("References")]
	[SerializeField] private InputReader inputReader;
	[SerializeField] private Transform firePoint;
	[SerializeField] private GameObject serverProjectilePrefab;
	[SerializeField] private GameObject clientProjectilePrefab;
	[SerializeField] private GameObject muzzleFlash;
	[SerializeField] private Collider2D playerCollider;

	[Header("Settings")]
	[SerializeField] private float projectileSpeed;
	[SerializeField] private float fireRate;
	[SerializeField] private float muzzleFlashDuration;

	private bool shouldFire;
	private float lastTimeFire;
	private float muzzleFlashTimer;

	public override void OnNetworkSpawn()
	{
		if (!IsOwner)
			return;

		inputReader.PrimaryFireEvent += HandlePrimaryFire;
	}

	public override void OnNetworkDespawn()
	{
		if (!IsOwner)
			return;

		inputReader.PrimaryFireEvent -= HandlePrimaryFire;
	}

	private void Update()
	{
		if (muzzleFlashTimer > 0f)
		{
			muzzleFlashTimer -= Time.deltaTime;

			if (muzzleFlashTimer <= 0f)
			{
				muzzleFlash.SetActive(false);
			}
		}

		if(!IsOwner)
			return;

		if (!shouldFire)
			return;

		if (Time.time < (1/ fireRate) + lastTimeFire)
			return;

		PrimaryFireServerRpc(firePoint.position, firePoint.up);
		SpawnDummyProjectile(firePoint.position, firePoint.up);

		lastTimeFire = Time.time;
	}

	private void HandlePrimaryFire(bool shouldFire)
	{
		this.shouldFire = shouldFire;
	}

	[ServerRpc]
	private void PrimaryFireServerRpc(Vector3 spawnPos, Vector3 direction)
	{
		GameObject newProjectile = Instantiate(serverProjectilePrefab, spawnPos, Quaternion.identity);
		newProjectile.transform.up = direction;

		Physics2D.IgnoreCollision(playerCollider, newProjectile.GetComponent<Collider2D>());

		Rigidbody2D rb = newProjectile.GetComponent<Rigidbody2D>();
		rb.velocity = rb.transform.up * projectileSpeed;

		SpawnDummyProjectileClientRpc(spawnPos, direction);
	}

	[ClientRpc]
	private void SpawnDummyProjectileClientRpc(Vector3 spawnPos, Vector3 direction)
	{
		if (IsOwner)
			return;
		
		SpawnDummyProjectile(spawnPos, direction);
	}

	private void SpawnDummyProjectile(Vector3 spawnPos, Vector3 direction)
	{
		muzzleFlash.SetActive(true);
		muzzleFlashTimer = muzzleFlashDuration;

		GameObject newProjectile = Instantiate(clientProjectilePrefab, spawnPos, Quaternion.identity);
		newProjectile.transform.up = direction;

		Physics2D.IgnoreCollision(playerCollider, newProjectile.GetComponent<Collider2D>());

		Rigidbody2D rb = newProjectile.GetComponent<Rigidbody2D>();
		rb.velocity = rb.transform.up * projectileSpeed;
	}
}