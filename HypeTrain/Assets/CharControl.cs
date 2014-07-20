﻿using UnityEngine;
using System.Collections;

public enum JumpState
{
	GROUNDED,
	JUMPING,
	FALLING
}


public class CharControl : MonoBehaviour {

	public float maxSpeed = 2f;
	private JumpState Jump = JumpState.GROUNDED;
	//Ground stuff
	public Transform groundCheck;
	float raycastLength = 0.15f;
	public LayerMask whatIsGround;
	public float PlusJumpForce = 300f;
	public float CurrJumpForce = 0f;
	public float MaxJumpForce = 1100f;

	void Start() {

	}

	void Update () {
		Debug.Log(Jump);
		switch (Jump) {

		case JumpState.GROUNDED: 
			if(Input.GetKey(KeyCode.Space)) {
				Jump = JumpState.JUMPING;
			}
			break;

		case JumpState.JUMPING: 
			if(Input.GetKey(KeyCode.Space) && CurrJumpForce < MaxJumpForce) {
				var timeDiff = Time.deltaTime * 100;
				var forceToAdd = PlusJumpForce*timeDiff;
				CurrJumpForce += forceToAdd;
				rigidbody2D.AddForce(new Vector2(0, forceToAdd));
			}
			else {
				Jump = JumpState.FALLING;
				CurrJumpForce = 0;
			}
			break;

		case JumpState.FALLING: 

			if (Physics2D.Raycast (groundCheck.position, -Vector2.up, raycastLength, whatIsGround)) {
				Jump = JumpState.GROUNDED;
			}
			break;
			
		}
		/*if(grounded && Input.GetKey(KeyCode.Space) && !hasJumped) {
			while(Input.GetKey(KeyCode.Space){
				for(int count = 1; count < 12; count++){
					rigidbody2D.AddForce(new Vector2(0, 100));

				}
			}
		hasJumped = true;
		}
		if (!Input.GetKey(KeyCode.Space) && hasJumped) {
			//Debug.Log ("Reset jump");
			hasJumped= false;
		}*/
	}
	
	// Update is called once per frame
	void FixedUpdate () {


		//Horizontal Movement
		float moveH = Input.GetAxis ("Horizontal");
		rigidbody2D.velocity = new Vector2 (moveH * maxSpeed, rigidbody2D.velocity.y);

	}
}
