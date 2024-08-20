using UnityEngine;
using System.Collections;

public class FirstPersonController : MonoBehaviour
{
	public bool canMove {get; private set;} = true;
	private bool isSprinting => canSprint && Input.GetKey(sprintKey);
	private bool shouldJump => Input.GetKeyDown(jumpKey) && characterController.isGrounded;
	private bool shouldCrouch => Input.GetKeyDown(crouchKey) && !duringCrouchAnimation && characterController.isGrounded;
	
	[Header("Functional Options")]
	[SerializeField] private bool canSprint = true;
	[SerializeField] private bool canJump = true;
	[SerializeField] private bool canCrouch = true;
	
	[Header("Controls")]
	[SerializeField] private KeyCode sprintKey = KeyCode.LeftShift;
	[SerializeField] private KeyCode jumpKey = KeyCode.Space;
	[SerializeField] private KeyCode crouchKey = KeyCode.LeftControl;
	
	[Header("Movement Parameters")]
	[SerializeField] private float moveSpeed = 3.0f;
	[SerializeField] private float sprintSpeed = 6.0f;
	[SerializeField] private float crouchSpeed = 1.5f;
	
	[Header("Look Parameters")]
	[SerializeField, Range(1,10)] private float sensX = 3.0f;
	[SerializeField, Range(1,10)] private float sensY = 3.0f;
	[SerializeField, Range(1,100)] private float upperLookLimit = 80.0f;
	[SerializeField, Range(1,100)] private float loverLookLimit = 80.0f;
	
	[Header("Jumping Parameters")]
	[SerializeField] private float jumpForse = 8.0f;
	[SerializeField] private float gravity = 30.0f;
	
	[Header("Crouch Parameters")]
	[SerializeField] private float crouchHeight = 0.5f;
	[SerializeField] private float standingHeight = 2.0f;
	[SerializeField] private float timeToCrouch = 0.25f;
	[SerializeField] private Vector3 crouchCenter = new Vector3(0, 0.5f, 0);
	[SerializeField] private Vector3 standingCenter = new Vector3(0, 0f, 0);
	private bool isCrouching;
	private bool duringCrouchAnimation;
	
	private Camera playerCamera;
	private CharacterController characterController;
	
	private Vector3 moveDirection;
	private Vector2 currentInput;
	
	private float rotationX = 0.0f;
	
	void Awake()
	{
		playerCamera = GetComponentInChildren<Camera>();
		characterController = GetComponent<CharacterController>();
		Cursor.lockState = CursorLockMode.Locked;
		Cursor.visible = false;
	}
	
	void Update()
	{
		if(canMove)
		{
		HandleMovementInput();
		HandleMouseLook();
		
		if(canJump)
			HandleJump();
		
		if(canCrouch)
			HandleCrouch();
		
		ApplyFinalMovements();
		}
	}
	
	private void HandleMovementInput() 
	{
		currentInput = new Vector2((isCrouching ? crouchSpeed : isSprinting ? sprintSpeed : moveSpeed) * Input.GetAxis("Vertical"),(isCrouching ? crouchSpeed : isSprinting ? sprintSpeed : moveSpeed) * Input.GetAxis("Horizontal"));
		float moveDirectionY = moveDirection.y;
		moveDirection = (transform.TransformDirection(Vector3.forward)*currentInput.x + (transform.TransformDirection(Vector3.right)*currentInput.y
		));
		moveDirection.y = moveDirectionY;
	}
	
	private void HandleMouseLook()
	{
		rotationX -= Input.GetAxis("Mouse Y") *sensY;
		
		rotationX = Mathf.Clamp(rotationX,-upperLookLimit,loverLookLimit);
		playerCamera.transform.localRotation = Quaternion.Euler(rotationX,0,0);
		transform.rotation *= Quaternion.Euler(0,Input.GetAxis("Mouse X")*sensX,0); 
	}
	private void HandleJump()
	{
		if(shouldJump)
			moveDirection.y=jumpForse;
	}
	private void HandleCrouch()
	{
		if(shouldCrouch)
		   StartCoroutine(CrouchStand());
	}
	private void ApplyFinalMovements()
	{
		if(!characterController.isGrounded)
			moveDirection.y -= gravity*Time.deltaTime;
		
		characterController.Move(moveDirection*Time.deltaTime);
	}
	private IEnumerator CrouchStand()
	{
		if(isCrouching && Physics.Raycast(playerCamera.transform.position,Vector3.up,1f))
			 yield break;
			 
		duringCrouchAnimation = true;
		
		float timeElapsed = 0f;
		float targetHeight = isCrouching ? standingHeight : crouchHeight;
		float currentHeight = characterController.height;
		Vector3 targetCenter = isCrouching ? standingCenter : crouchCenter;
		Vector3 currentCenter = characterController.center;
		
		while(timeElapsed < timeToCrouch)
		{
			characterController.height = Mathf.Lerp(currentHeight, targetHeight, timeElapsed / timeToCrouch);
			characterController.center = Vector3.Lerp(currentCenter, targetCenter, timeElapsed / timeToCrouch);
			timeElapsed += Time.deltaTime;
			yield return null;
		}
		
		characterController.height = targetHeight;
		characterController.center = targetCenter;
		
		isCrouching = !isCrouching;
		
		duringCrouchAnimation = false;
	}
}
