using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Interaction : MonoBehaviour
{
	public float _draggableObjectDistance = 3;
	public Camera _mainCamera;
	public float _raycastDistance = 50f;
	DraggableObject _currentlyDraggedObject;
	private bool _canvastipKEY = false;
	
	void Update()
	{
		  RaycastHit h;
  Ray ray = new Ray(_mainCamera.transform.position, _mainCamera.transform.forward);
  Physics.Raycast(ray, out h, _draggableObjectDistance);
  if (_currentlyDraggedObject != null)
  {

	  Vector3 targetPosition = _mainCamera.transform.position + _mainCamera.transform.forward * _draggableObjectDistance;

  }

  RaycastHit hit;
  bool hitDraggableObject = Physics.Raycast(_mainCamera.transform.position, _mainCamera.transform.forward, out hit, _raycastDistance, LayerMask.GetMask("DraggableObject"));

  if (hit.collider?.GetComponent<DraggableObject>() is DraggableObject draggableObject && Input.GetMouseButtonDown(0))
  {
	  if (hitDraggableObject)
	  {
		  if (hit.collider.GetComponent<DraggableObject>().enabled)
		  {
			  draggableObject.StartFollowingObject();
		  }
		  _currentlyDraggedObject = draggableObject;
		  _canvastipKEY = false;
	  }
  }


  if (Input.GetMouseButtonUp(0))
  {
	  if (_currentlyDraggedObject != null)
	  {
		  _currentlyDraggedObject.StopFollowingObject();
		  _currentlyDraggedObject = null;
		  _canvastipKEY = false;
		  //_canvastipKEY.SetActive(false);
	  }
  }
	}
}
