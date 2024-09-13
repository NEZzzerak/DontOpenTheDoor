using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : Interact
{
	
	private bool isOpen=false;
	private bool canBeInteractedWith = true;
	private Animator anim;
	void Start()
	{
		anim = GetComponent<Animator>();
	}
	public override void OnFocus()
	{
	}

	public override void OnInteract()
	{
		if(canBeInteractedWith)
		{
			isOpen = !isOpen;
			
			Vector3 DoorTransformDirection = transform.TransformDirection(Vector3.right);
			Vector3 PlayerTransformDirection = FirstPersonController.instance.transform.position - transform.position;
			float dot = Vector3.Dot(DoorTransformDirection,PlayerTransformDirection);
			
			anim.SetFloat("dot",dot);
			anim.SetBool("IsOpen",isOpen);
		}
	}

	public override void OnLose()
	{
	}
}
