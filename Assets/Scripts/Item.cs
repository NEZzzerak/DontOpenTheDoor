using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{
	public GameObject camera;
	public float distance = 15f;
	private Outline outline;
	GameObject currentItem;
	bool canPickUp = false;
   
   
   	void Start()
	{
		outline = GetComponent<Outline>();
		outline.OutlineWidth = 0;
	}

	
	void Update()
	{
		if (Input.GetKeyDown(KeyCode.E)) PickUp();
		if (Input.GetKeyDown(KeyCode.Q)) Drop();
	}

	void PickUp()
	{
		RaycastHit hit;
		if(Physics.Raycast(camera.transform.position, camera.transform.forward, out hit, distance))
		{
			if(hit.transform.tag == "Weapon")
			{
				if (canPickUp) Drop();

				currentItem = hit.transform.gameObject;
				currentItem.GetComponent<Rigidbody>().isKinematic = true;
				currentItem.GetComponent<Collider>().isTrigger = true;
				currentItem.transform.parent = transform;
				currentItem.transform.localPosition = Vector3.zero;
				currentItem.transform.localEulerAngles = new Vector3(10f, 0f, 0f);
				canPickUp = true;
			}
			outline.OutlineWidth = 3.5f;
		}
		else
		{
			 outline.OutlineWidth = 0;
		}

		
	}

	void Drop()
	{
		currentItem.transform.parent = null;
		currentItem.GetComponent<Rigidbody>().isKinematic = false;
		currentItem.GetComponent<Collider>().isTrigger = false;
		canPickUp = false;
		currentItem = null;
	}
}
