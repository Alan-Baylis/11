﻿using UnityEngine;
using System.Collections;

using Global;

public class Weapon : MonoBehaviour 
{
    public GameObject ShotPrefab;
	public enum WeaponType {Revolver, Shotgun};
	public GameObject shotOutPoint;
	public float shotDamage = 10f;
	public int ammoCount = 10;
	public int ammoInStack = 5;
	public float timeBetweenBullets = 1f;
	public float range = 100f;
	public float timerBetweenReload = 5f;
	
	float timer;
	float reloadTimer;
	Ray shootRay;
	RaycastHit shootHit;
	int shootableMask;
	ParticleSystem gunParticles;
	LineRenderer gunLine;
	AudioSource gunAudio;
	Light gunLight;
	float effectsDisplayTime = 0.05f;

    private GameObject _shotGunObj;

	protected WeaponBehavior weaponBehavior;
	protected int unitAmmoCount = 0;

	// Use this for initialization
	void Awake () {
		shootableMask = LayerMask.GetMask (Layers.heroes);
		
		// Set up the references.
		gunParticles = shotOutPoint.GetComponent<ParticleSystem> ();
		gunLine = shotOutPoint.GetComponent <LineRenderer> ();
		gunAudio = shotOutPoint.GetComponent<AudioSource> ();
		gunLight = shotOutPoint.GetComponent<Light> ();

		timer = timeBetweenBullets;
		reloadTimer = timerBetweenReload;
	}

	void Update ()
	{
		// Add the time since Update was last called to the timer.
		timer += Time.deltaTime;
		reloadTimer += Time.deltaTime;
		
		// If the timer has exceeded the proportion of timeBetweenBullets that the effects should be displayed for...
		if(timer >= effectsDisplayTime)
		{
			// ... disable the effects.
			DisableEffects ();
		}
	}
	
	protected void DisableEffects()
	{
		// Disable the line renderer and the light.
		gunLine.enabled = false;
		gunLight.enabled = false;
	}

	protected void PlayEffects()
	{
		// Play the gun shot audioclip.
		gunAudio.Play ();

		// Enable the light.
		gunLight.enabled = true;

		// Stop the particles from playing if they were, then start the particles.
		gunParticles.Stop ();
		gunParticles.Play ();
		
		// Enable the line renderer and set it's first position to be the end of the gun.
		gunLine.enabled = true;
	}

	protected void Reloading()
	{
		if ((ammoCount - ammoInStack) > 0)
		{
			unitAmmoCount = ammoInStack;
			ammoCount -= ammoInStack;
		}
		else
		{
			unitAmmoCount = ammoCount;
			ammoCount = 0;
		}
	}

	public void Shoot()
	{
		if (timer >= timeBetweenBullets && weaponBehavior.CanIShoot()) 
		{
			if (unitAmmoCount > 0)
			{
				unitAmmoCount--;

				timer = 0f;
				reloadTimer = 0;

				// Enable gunLine, gunLight and Play gunAudio.
				PlayEffects();

				gunLine.SetPosition (0, shotOutPoint.transform.position);
				
				// Set the shootRay so that it starts at the end of the gun and points forward from the barrel.
				shootRay.origin = shotOutPoint.transform.position;
				shootRay.direction = shotOutPoint.transform.forward;
				
				// Perform the raycast against gameobjects on the shootable layer and if it hits something...
				if(Physics.Raycast (shootRay, out shootHit, range, shootableMask))
				{
					Hero hero = shootHit.collider.GetComponent <Hero> ();
					gunLine.SetPosition (1, shootHit.point);
                    hero.TakeDemage(35f);
				}
				else
				{
					gunLine.SetPosition (1, shootRay.origin + shootRay.direction * range);

                    _shotGunObj = (GameObject)Instantiate(ShotPrefab, shotOutPoint.transform.position, shotOutPoint.transform.rotation);
                    _shotGunObj.transform.SetParent(shotOutPoint.transform);
                    Invoke("DeleteShotGunObj", 1f);
				}
                
			}
			else if (reloadTimer >= timerBetweenReload)
			{
				Reloading();
			}
		}
	}

    private void DeleteShotGunObj()
    {
        Destroy(_shotGunObj);
    }
}
