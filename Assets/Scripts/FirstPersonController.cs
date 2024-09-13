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
	[SerializeField] private bool canUseHeadbob = true;
	[SerializeField] private bool canInteract = true;
	
	[Header("Controls")]
	[SerializeField] private KeyCode sprintKey = KeyCode.LeftShift;
	[SerializeField] private KeyCode jumpKey = KeyCode.Space;
	[SerializeField] private KeyCode crouchKey = KeyCode.LeftControl;
	[SerializeField] private KeyCode interactKey = KeyCode.Mouse1;
	
	
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
	
	[Header("Headbob Parameters")]
	[SerializeField] private float walkBobSpeed = 14.0f;
	[SerializeField] private float walkBobAmount = 0.05f;
	[SerializeField] private float sprintBobSpeed = 18.0f;
	[SerializeField] private float sprintBobAmount = 0.11f;
	[SerializeField] private float crouchBobSpeed = 7.0f;
	[SerializeField] private float crouchBobAmount = 0.025f;
	
	[Header("Interact")]
	[SerializeField] private Vector3 interactionRayPoint = default;
	[SerializeField] private float interactionDistance = default;
	[SerializeField] private LayerMask interactionLayer = default;
	
	private Interact CurrentInteractable;
	private float defaultYpos = 0;
	private float timer;
	
	private bool isCrouching;
	private bool duringCrouchAnimation;
	
	private Camera playerCamera;
	private CharacterController characterController;
	
	private Vector3 moveDirection;
	private Vector2 currentInput;
	
	private float rotationX = 0.0f;
	
	public static FirstPersonController instance;
	
	void Awake()
	{
		instance = this;
		playerCamera = GetComponentInChildren<Camera>();
		characterController = GetComponent<CharacterController>();
		Cursor.lockState = CursorLockMode.Locked;
		Cursor.visible = false;
		defaultYpos = playerCamera.transform.localPosition.y;
	}
	
	void Update()
	{
		if(canMove)
			HandleMovementInput();
			HandleMouseLook();
		
		if(canJump)
			HandleJump();
		
		if(canCrouch)
			HandleCrouch();
			
		if(canUseHeadbob)
			HandleHeadBob();
		
		if(canInteract)
			HandleInteractionCheck();
			HandleInteractionInput();
			
		
		ApplyFinalMovements();
		
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
	private void HandleHeadBob()
	{
		if(!characterController.isGrounded) return;
		
		if(Mathf.Abs(moveDirection.x) > 0.1f || Mathf.Abs(moveDirection.z) > 0.1f)
		{
			timer += Time.deltaTime * (isCrouching ? crouchBobSpeed : isSprinting ? sprintBobSpeed : walkBobSpeed);
			playerCamera.transform.localPosition = new Vector3(
				playerCamera.transform.localPosition.x,defaultYpos + Mathf.Sin(timer) * (isCrouching ? crouchBobAmount : isSprinting ? sprintBobAmount : walkBobAmount),playerCamera.transform.localPosition.z);
		}
	}
	private void HandleInteractionCheck()
	{
		if(Physics.Raycast(playerCamera.ViewportPointToRay(interactionRayPoint),out RaycastHit hit, interactionDistance))
		{
			if(hit.collider.gameObject.layer == 9 && (CurrentInteractable == null || hit.collider.GetInstanceID() != CurrentInteractable.gameObject.GetInstanceID()))
			{
				hit.collider.TryGetComponent(out CurrentInteractable);
				
				if(CurrentInteractable)
					CurrentInteractable.OnFocus();
					
			}
		}
		else if(CurrentInteractable)
		{
			CurrentInteractable.OnLose();
			CurrentInteractable = null;
			
		}
		
		
	}
	
	private void HandleInteractionInput()
	{
		if(Input.GetKeyDown(interactKey) && CurrentInteractable != null && Physics.Raycast(playerCamera.ViewportPointToRay(interactionRayPoint),out RaycastHit hit, interactionDistance, interactionLayer))
		{
			CurrentInteractable.OnInteract();
		}
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
