﻿using UnityEngine;
using System.Collections;

using Global;

public class Character : MonoBehaviour
{
	public GameObject weaponDeploy;	
    public bool autoEquip = false;
    public Weapon.WeaponType curWeaponType;
    public bool isDead = false;

	protected NavMeshAgent navAgent;
	protected Animator anim;
	protected GameManager GM;				// Game Manager Class
	protected EnemyController EC;         // EnemyController Class
	protected GameObject curWeapon;
	protected Weapon GUN;
    protected WeaponFactory weaponFact;
    protected float healthLevel;

	protected virtual void Init()
	{
		navAgent = GetComponent<NavMeshAgent>();
		anim = GetComponent<Animator>();
		GM = GameObject.FindGameObjectWithTag(Tags.gameManager).GetComponent<GameManager>();
        EC = GM.getEnemyCtr();
        weaponFact = GM.getWeaponFact();
        //
        if (autoEquip)
        {
            EquipWeapon(curWeaponType);
        }

        healthLevel = 100f;
	}

    public void TakeDemage(float demage)
    {
        healthLevel -= demage;
    }

	public void AttackCharacter(GameObject hero)
	{
		ShootFromCurGun();
	}

	public void AttackCharacter()
	{
		ShootFromCurGun();
	}

	protected void ShootFromCurGun()
	{
		if (!GUN)
			return;
		
		GUN.Shoot();
	}

    protected virtual void MeleeAttack()
    {

    }

	public void EquipWeapon(Weapon.WeaponType wType)
	{
		if (curWeapon && curWeaponType == wType)
		{
			return;
		}

		curWeaponType = wType;
        
        if (curWeapon)
        {
            Destroy(curWeapon);
        }

		curWeapon = null;
		GUN = null;

		if (curWeaponType != null)
		{
            curWeapon = weaponFact.getWeapon(curWeaponType, weaponDeploy.transform);
            curWeapon.transform.parent = weaponDeploy.transform;
			GUN = curWeapon.GetComponent<Weapon>();
		}
	}
}