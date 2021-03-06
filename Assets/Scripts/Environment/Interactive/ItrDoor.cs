﻿using UnityEngine;
using System.Collections;
using Global;

public class ItrDoor : InteractiveObject
{
    public enum LockPosition
    {
        Auto = 0,
        UnLock,
        Lock,
        Broken
    }

    public float DoorDetectorRadius = 3f;
    public LockPosition LockPos;
    public Animator Anim;
    public int DoorId;

    private float _autoCloseTime = 3f;
    private float _timeToClose;
    private bool _isDoorDetectorProcessing = false;

    public void DoorOpenClose()
    {
        if (GetIsDoorNotBlock() && GetIsDoorNotBroken())
            Anim.SetBool("Open", !Anim.GetBool("Open"));
    }

    public bool GetIsDoorOpen()
    {
        return Anim.GetBool("Open");
    }

    public bool GetIsDoorNotBlock()
    {
        return LockPos != LockPosition.Lock;
    }

    public bool GetIsDoorNotBroken()
    {
        return LockPos != LockPosition.Broken;
    }

    public void DoorOpen()
    {
        if (GetIsDoorNotBlock() && GetIsDoorOpen() != true && GetIsDoorNotBroken())
        {
            Anim.SetBool("Open", true);
            _timeToClose = Time.time + _autoCloseTime;   
        }
    }

    public void DoorClose()
    {
        if (GetIsDoorNotBlock())
            Anim.SetBool("Open", false);
    }

    protected override void OnFixedUpdate()
    {
        base.OnFixedUpdate();

        if (LockPos == LockPosition.Auto)
        {
            if (GetIsDoorOpen())
            {
                if (Time.time > _timeToClose)
                {
                    DoorClose();
                }
            }

            if (!_isDoorDetectorProcessing)
                CheckIsCharacterAround();
        }
    }

    private void CheckIsCharacterAround()
    {
        _isDoorDetectorProcessing = true;
        Collider[] hitColliders = Physics.OverlapSphere(gameObject.transform.position, DoorDetectorRadius);
        int i = 0;
        while (i < hitColliders.Length)
        {
            if (hitColliders[i].GetComponent<Character>())
            {
                _timeToClose = Time.time + _autoCloseTime;
                DoorOpen();
            }
            i++;
        }
        _isDoorDetectorProcessing = false;
    }

    public override void Interact()
    {
        base.Interact();

        if (Distance <= InterfaceWorkDistance)
        {
            if (LockPos == LockPosition.Auto)
            {
                LockPos = LockPosition.UnLock;
            }

            DoorOpenClose();
        }
    }
}
