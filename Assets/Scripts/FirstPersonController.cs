using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class FirstPersonController : MonoBehaviour
{
	public bool canMove {get; private set;} = true;
	[Header("Movement Parameters")]
	[SerializeField] private float moveSpeed=3.0f;
	[SerializeField] private float gravity=30.0f;
	
	[Header("Look Parameters")]
	[SerializeField, Range(1,10)] private float sensX = 3.0f;
	[SerializeField, Range(1,10)] private float sensY = 3.0f;
	[SerializeField, Range(1,100)] private float upperLookLimit = 80.0f;
	[SerializeField, Range(1,100)] private float loverLookLimit = 80.0f;
	
	private Camera playerCamera;
	private CharacterController characterController;
	
	private Vector3 moveDirection;
	private Vector2 currentInput;
	
	private float rotationX = 0.0f;
	
	void Awake()
	{
		playerCamera = GetComponentInChildren<Camera>();
		characterController = GetComponent<CharacterController>();
		Cursor.lockState=CursorLockMode.Locked;
		Cursor.visible = false;
	}
	
	void Update()
	{
		if(canMove)
		{
		HandleMovementInput();
		HandleMouseLook();
		
		ApplyFinalMovements();
		}
	}
	
	private void HandleMovementInput() 
	{
		currentInput = new Vector2(moveSpeed * Input.GetAxis("Vertical"),moveSpeed * Input.GetAxis("Horizontal"));
		float moveDirectionY = moveDirection.y;
		moveDirection = (transform.TransformDirection(Vector3.forward)*currentInput.x + (transform.TransformDirection(Vector3.right)*currentInput.y
		));
		moveDirection.y=moveDirectionY;
	}
	
	private void HandleMouseLook()
	{
		rotationX -= Input.GetAxis("Mouse Y") *sensY;
		
		rotationX = Mathf.Clamp(rotationX,-upperLookLimit,loverLookLimit);
		playerCamera.transform.localRotation = Quaternion.Euler(rotationX,0,0);
		transform.rotation *= Quaternion.Euler(0,Input.GetAxis("Mouse X")*sensX,0); 
	}
	
	private void ApplyFinalMovements()
	{
		if(!characterController.isGrounded)
			moveDirection.y-=gravity*Time.deltaTime;
		
		characterController.Move(moveDirection*Time.deltaTime);
	}
}
