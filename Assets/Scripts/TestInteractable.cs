using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestInteractable : Interact
{
	private Outline line;
	void Start()
	{
		line = GetComponent<Outline>();
		line.OutlineWidth = 0;
	}
	public override void OnFocus()
	{
	   line.OutlineWidth = 10f;
	}

	public override void OnInteract()
	{
	   
	}

	public override void OnLose()
	{
	  line.OutlineWidth = 0;
	}

 
}
