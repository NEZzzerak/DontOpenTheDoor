using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bebebe : MonoBehaviour
{
	public Animation anim;
	
	void Start()
	{
		anim.GetComponent<Animation>();
	}
	
	void Update()
	{
		anim.Play();
	}
}
