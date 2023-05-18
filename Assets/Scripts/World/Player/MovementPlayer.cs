using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class MovementPlayer : NetworkBehaviour
{
	[SerializeField]
	private CharacterController controller;

	[SerializeField]
	private float playerSpeed = 5.0f, playerRunSpeed = 8;
	[SerializeField]
	private float jumpHeight = 1.0f;
	[SerializeField]
	private float gravityValue = -9.81f;
	[SerializeField]
	private float flySpeed = 2;

	private Vector3 playerVelocity;

	[Header("Grounded check parameters:")]
	[SerializeField]
	private LayerMask groundMask;
	[SerializeField]
	private float rayDistance = 1;
	[field: SerializeField]
	public bool IsGrounded { get; private set; }



	private void Awake()
	{
		controller = GetComponent<CharacterController>();
	}

	private Vector3 GetMovementDirection(Vector3 movementInput)
	{
		return transform.right * movementInput.x + transform.forward * movementInput.z;
	}

	public void Fly(Vector3 movementInput, bool ascendInput, bool descendInput)
	{
		Vector3 movementDirection = GetMovementDirection(movementInput);

		if (ascendInput)
		{
			movementDirection += Vector3.up * flySpeed;
		}
		else if (descendInput)
		{
			movementDirection -= Vector3.up * flySpeed;
		}
		ControllerMoveServerRpc(movementDirection * playerSpeed * Time.deltaTime);
	}

	public void Walk(Vector3 movementInput, bool runningInput)
	{
		Vector3 movementDirection = GetMovementDirection(movementInput);
		float speed = runningInput ? playerRunSpeed : playerSpeed;
		if (!IsGrounded && playerVelocity.y <= 0f)
			speed = speed * 0.75f;
		ControllerMoveServerRpc(movementDirection * Time.deltaTime * speed);
	}
	
	[ServerRpc]
	public void ControllerMoveServerRpc(Vector3 newPos)
	{
		if (IsOwner == false) return;
		controller.Move(newPos);
	}

	public void HandleGravity(bool isJumping)
	{
		if (controller.isGrounded && playerVelocity.y < 0)
		{
			playerVelocity.y = 0f;
		}
		if (isJumping && IsGrounded)
			AddJumpForce();
		ApplyGravityForce();
		ControllerMoveServerRpc(playerVelocity * Time.deltaTime);
	}

	private void AddJumpForce()
	{
		//playerVelocity.y = Mathf.Sqrt(jumpHeight * -3.0f * gravityValue);
		playerVelocity.y = jumpHeight;
	}

	private void ApplyGravityForce()
	{
		playerVelocity.y += gravityValue * Time.deltaTime;
		playerVelocity.y = Mathf.Clamp(playerVelocity.y, gravityValue, 10);
	}

	private void Update()
	{
		IsGrounded = Physics.Raycast(transform.position, Vector3.down, rayDistance, groundMask);
	}

	private void OnDrawGizmos()
	{
		Gizmos.DrawRay(transform.position, Vector3.down * rayDistance);
	}
}
