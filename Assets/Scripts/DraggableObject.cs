using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class DraggableObject : MonoBehaviour
{
	private Rigidbody _rigidbody;
	[SerializeField] private bool _isKinematicAfterDrop;
	[SerializeField] private Vector3 _targetPosition;
	[SerializeField] private bool _follow;
	[SerializeField] private float _followSpeed = 15f;
	[SerializeField] float _velocityLimit = 8f;
	[SerializeField] private float _stopTransitionTime = 0.5f; 

	private Vector3 moveDirection;
	private float _rotationSpeed = 150f;
	private Coroutine _stopCoroutine; 
	private Outline line;

	private void Start()
	{
		_rigidbody = GetComponent<Rigidbody>();
		line = GetComponent<Outline>();
		line.OutlineWidth = 0;
	}

	public void Update()
	{
		if (!_follow)
			return;

		if (Vector3.Distance(transform.position, _targetPosition) > 3f)
		{
			StopFollowingObject();
		}

		SetTargetPosition();
		moveDirection = _targetPosition - _rigidbody.position;
		_rigidbody.velocity = Vector3.ClampMagnitude(moveDirection * _followSpeed, _velocityLimit);

		if (Cursor.lockState == CursorLockMode.None)
		{
			float mouseX = Input.GetAxis("Mouse X");
			float mouseY = Input.GetAxis("Mouse Y");
			Vector3 cameraForward = Camera.main.transform.forward;
			cameraForward.y = 0f;
			cameraForward.Normalize();

			Vector3 cameraRight = Vector3.Cross(Vector3.up, cameraForward);

			transform.Rotate(cameraRight * mouseY * _rotationSpeed * Time.deltaTime);
			transform.Rotate(Vector3.up * mouseX * _rotationSpeed * Time.deltaTime);
		}
	}

	public void StartFollowingObject()
	{
		_rigidbody.isKinematic = false;
		_rigidbody.freezeRotation = true;
		_follow = true;
		_targetPosition = Camera.main.transform.position + Camera.main.transform.forward * 2;
		line.OutlineWidth = 10;
	}

	public void SetTargetPosition()
	{
		_targetPosition = Camera.main.transform.position + Camera.main.transform.forward * 2;
	}

	public void StopFollowingObject()
	{
		_follow = false;

		if (_isKinematicAfterDrop)
		{
			_stopCoroutine = StartCoroutine(SmoothStop());
		}
		line.OutlineWidth = 0;
		_rigidbody.freezeRotation = false;
	}

	private IEnumerator SmoothStop()
	{
		float elapsedTime = 0f;
		while (elapsedTime < _stopTransitionTime)
		{
			_rigidbody.velocity *= (1 - elapsedTime / _stopTransitionTime);
			elapsedTime += Time.deltaTime;
			yield return null;
		}

		_rigidbody.velocity = Vector3.zero; 
		if (_isKinematicAfterDrop)
			_rigidbody.isKinematic = true; 
	}
}
