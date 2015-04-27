﻿using UnityEngine;
using System.Collections;

using Global;

public class GuardBehavior : IEnemyBehavior
{
	public bool CanIMove()
	{
		return true;
	}

	public bool CanIRotate()
	{
		return false;
	}

	public bool CanShotBeSuccess(Transform transformA, Transform transformB, float maxAngle)
	{
		return true;
	}

    public bool CanMeleeAttackBeSuccess(float distance)
    {
        return distance < 1.2f ? true : false;
    }
}
