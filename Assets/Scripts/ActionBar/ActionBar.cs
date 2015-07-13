﻿using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class ActionBar: MonoBehaviour
{
	/// <summary>
	/// Skills slots storage.
	/// </summary>
	[HideInInspector]
	public List<GameObject> SkillSlots = new List<GameObject>();

	/// <summary>
	/// Skill slot prefab.
	/// </summary>
	public GameObject skillSlot;

	/// <summary>
	/// Skills slots count on the action bar.
	/// </summary>
	public int skillSlotsCount;

    public void OnSkillAdded(object source, SkillEventArgs e)
    {
        OrganizeInEmptySlot(e.SkillHero);
    }

	public void Start()
	{
		AddEmptySlots();
		SkillSlotController.SlotUpdated += DeselectSlot;
	}

	/// <summary>
	/// Find an empty slot on the action bar and assign s skill to it.
	/// </summary>
    private void OrganizeInEmptySlot(SkillHero skill)
	{
        foreach(GameObject slot in SkillSlots)
		{
			SkillSlotController controller = slot.GetComponent<SkillSlotController>();

			if (!controller.HasSkill())
			{
				controller.SetSkill(skill);
				break;
			}
		}
	}

	/// <summary>
	/// Create and add an empty skill slot to the action bar.
	/// </summary>
	private void AddEmptySlots()
	{
		for (var i = 0; i < skillSlotsCount; i++)
		{
			GameObject slot = Instantiate(skillSlot) as GameObject;
			slot.transform.SetParent(this.gameObject.transform, false);
			RectTransform rectTransform = slot.GetComponent<RectTransform>();

			rectTransform.localPosition = new Vector3(
				rectTransform.localPosition.x + rectTransform.rect.width * i,
				rectTransform.localPosition.y,
				0);
			SkillSlots.Add(slot);
		}
	}

	#region ChangeActiveSlot handler

	/// <summary>
	/// Remove focused state from a non selected slot.
	/// </summary>
	void DeselectSlot(GameObject newSelectedSlot)
	{
		foreach(GameObject slot in SkillSlots)
		{
			if (newSelectedSlot.GetInstanceID() != slot.GetInstanceID())
			{
				slot.GetComponent<SkillSlotController>().Blur();
			}
		}
	}

	#endregion;
}
