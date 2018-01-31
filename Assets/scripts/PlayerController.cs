using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {

	public float moveSpeed;
	public float jumpForce;
	public CharacterController controller;
	public float gravityScale;

	private Vector3 moveDirection;
	
	public float knockBackForce;
	public float knockBackTime;
	private float knockBackCounter;
	bool canDoubleJump = false;

	public Animator anim;

	// Use this for initialization
	void Start () {
	
	controller = GetComponent<CharacterController>();
	//anim = GetComponent<Animator>();
	
	}
	
	// Update is called once per frame
	void Update () {
		
		
		if (knockBackCounter <= 0)
		{
			//pohyb
			moveDirection = new Vector3(Input.GetAxis("Horizontal") * moveSpeed, moveDirection.y, 0f);

			
			//doublejump
			if (Input.GetButtonDown("Jump") && !controller.isGrounded && canDoubleJump == true)
			{
					moveDirection.y = jumpForce;
					canDoubleJump = false;
			}
			
			//jump
			else if (controller.isGrounded)
			{	
				moveDirection.y = 0f;
				if (Input.GetButtonDown("Jump"))
				{
					moveDirection.y = jumpForce;
					canDoubleJump = true;
				}
			}
	
			moveDirection.y = moveDirection.y + (Physics.gravity.y * gravityScale);
			Vector3 animDirection = Vector3.Normalize(moveDirection);
			anim.SetFloat("Move", animDirection.x);
			anim.SetFloat("Jump", animDirection.y);
			Debug.Log(animDirection.x + " ; " + animDirection.y + " ; " + animDirection.z);

	
			controller.Move(moveDirection * Time.deltaTime);
					
			
		} else
			
		{
			knockBackCounter = knockBackCounter - Time.deltaTime;
		}
		
	}
	
	public void Knockback(Vector3 direction)
	{
		knockBackCounter = knockBackTime;
		
		moveDirection = direction * knockBackForce;
		moveDirection.y = knockBackForce;
	}
	
	
}





