﻿/*
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;
using Global;


public class Hero : Character, IDamageSource
{
	public string Name = "Scout";
    public Transform RHTargetTransform;
    public Transform lightTransform;
	public float speed = 7f;                    // Speed at which the character moves
    public GameObject DadBodyGameObject;

    [HideInInspector]
    public bool readyToShoot;
    [HideInInspector]
    public bool movementLocked;

    public GameObject GrabPointGameObject;
    public float InteractiveRadius = 3f;

	private float characterSpeed;
	private Transform myTransform;              // this transform
	private Vector3 destinationPosition;        // The destination Point
	private float destinationDistance;          // The distance between myTransform and destinationPosition
    private Vector3 targetPoint;
	private SpriteRenderer selectCircle;
	private float kHeroRotationSpeed = 200f;
    private GameObject _objInHands = null;

    protected float baseDamage = 40.0f;

	protected override void Init()
	{
		base.Init();

        movementLocked = false;
		characterSpeed = 0f;
		myTransform = transform;                                  // sets myTransform to this GameObject.transform
		destinationPosition = myTransform.position;
		selectCircle = GetComponentInChildren<SpriteRenderer> (); // hero select circle sprite
	}

	void Awake()
	{
		Init();
	}

	void Update()
	{	
        if (movementLocked)
        {
            anim.SetFloat ("Speed", 0f);
        }
        else
        {
            destinationDistance = Vector3.Distance(destinationPosition, myTransform.position);
            
            characterSpeed = navAgent.velocity.magnitude;
            
            anim.SetFloat ("Speed", characterSpeed);
            
            MoveCharacter (destinationDistance);
        }
	}

	void MoveCharacter(float destinationDistance)
	{
		if(destinationDistance > .5f && navAgent.enabled)
		{
			navAgent.SetDestination(destinationPosition);

//			myTransform.position = Vector3.MoveTowards(myTransform.position, destinationPosition, speed * Time.deltaTime * characterSpeed);
		}
		else
		{
			navAgent.enabled = false;
		}
	}

    #region IDamageSource

    public float AmountOfDamage()
    {
        return baseDamage;
    }

    #endregion
	
	public void SetDistinationPosition(Ray ray)
	{
        if (movementLocked) return;

        RaycastHit hitInfo = new RaycastHit();
        if (Physics.Raycast(ray.origin, ray.direction, out hitInfo))
        {
            navAgent.enabled = true;
            Vector3 targetPoint = hitInfo.point;
            destinationPosition = hitInfo.point;
        }
	}

	public void SelectHero()
	{
		if (selectCircle != null)
		{
			selectCircle.enabled = true;  // show select circle
		}
	}

	public void UnselectHero()
	{
		if (selectCircle != null)
		{
			selectCircle.enabled = false;  // hide select circle
		}
	}

    public void OrientateHero(Ray ray)
	{
		Plane playerPlane = new Plane(Vector3.up, myTransform.position);
		float hitdist = 0.0f;
		
		if (playerPlane.Raycast(ray, out hitdist))
		{
			Vector3 targetPoint = ray.GetPoint(hitdist);
			Quaternion targetRotation = Quaternion.LookRotation(targetPoint - transform.position);
			myTransform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, kHeroRotationSpeed * Time.smoothDeltaTime);
		}

        RaycastHit hitInfo = new RaycastHit();
        if (Physics.Raycast(ray.origin, ray.direction, out hitInfo))
        {
            targetPoint = hitInfo.point;
            weaponDeploy.transform.LookAt(targetPoint);

			// FIXME: It doesn't work without null checking
			if (lightTransform != null)
            	lightTransform.transform.LookAt(targetPoint);
        }
	}

    void OnAnimatorIK()
    {
        if (anim)
        {

            //if the IK is active, set the position and rotation directly to the goal. 
            if (readyToShoot)
            {
                if (anim)
                {
                    anim.SetLookAtWeight(1);
                    anim.SetLookAtPosition(targetPoint);
                }
                // Set the right hand target position and rotation, if one has been assigned
                if (RHTargetTransform != null)
                {
                    anim.SetIKPositionWeight(AvatarIKGoal.RightHand, 1);
                    anim.SetIKRotationWeight(AvatarIKGoal.RightHand, 1);
                    anim.SetIKPosition(AvatarIKGoal.RightHand, RHTargetTransform.position);
                    anim.SetIKRotation(AvatarIKGoal.RightHand, RHTargetTransform.rotation);
                }

            }

            //if the IK is not active, set the position and rotation of the hand and head back to the original position
            else
            {
                anim.SetIKPositionWeight(AvatarIKGoal.RightHand, 0);
                anim.SetIKRotationWeight(AvatarIKGoal.RightHand, 0);
                anim.SetLookAtWeight(0);
            }
        }
    }

    private void Dead()
    {
        isDead = true;
        Instantiate(DadBodyGameObject, transform.position, transform.rotation);
        gameObject.SetActive(false);
    }

    public override void TakeDamage(DamageType damageType, float damage)
    {
        base.TakeDamage(damageType, damage);

        if (LifeLevel < 0 && !isDead)
        {
            Dead();
        }
    }

    public override void TryToInteract()
    {
        base.TryToInteract();

        if (_objInHands)
        {
            DropObject(_objInHands);
        }

        else
        {
            CheckInteractableObjectsAround();
        }
    }

    private void CheckInteractableObjectsAround()
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, InteractiveRadius);
        List<InteractiveObject> itrObjects = new List<InteractiveObject>();
        int i = 0;
        while (i < hitColliders.Length)
        {
            InteractiveObject itrObj = hitColliders[i].GetComponent<InteractiveObject>();
            if (itrObj)
            {
                itrObj.Distance = Vector3.Distance(itrObj.transform.position, transform.position);
                itrObjects.Add(itrObj);
            }

            i++;
        }

        var sortedItrObjects = itrObjects.OrderBy(go => go.Distance).ToList();
        sortedItrObjects[0].Interact();

        if (sortedItrObjects[0] is ItrBarrel)
        {
            GameObject barrel = sortedItrObjects[0].Grab();

            if (barrel)
            {
                _objInHands = barrel;
                GrabObject(_objInHands);
            }
        }
    }

    private void GrabObject(GameObject obj)
    {
        obj.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;
        obj.transform.SetParent(GrabPointGameObject.transform);
        obj.transform.localPosition = Vector3.zero;
        obj.transform.rotation = Quaternion.Euler(Vector3.zero);
    }

    private void DropObject(GameObject obj)
    {
        obj.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;
        GrabPointGameObject.transform.DetachChildren();
        _objInHands = null;
    }
}
*/