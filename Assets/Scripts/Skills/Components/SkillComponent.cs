﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public abstract class SkillComponent
{
    public string[] Tags;

    public virtual void Apply(List<GameObject> targers)
    {
    }
}
