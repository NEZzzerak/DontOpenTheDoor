using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Interact : MonoBehaviour
{
	public virtual void Awake()
	{
		gameObject.layer = 9;
	}
  public abstract void OnInteract();
  public abstract void OnFocus();
  public abstract void OnLose();
}
