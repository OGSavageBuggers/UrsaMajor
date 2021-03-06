﻿using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class gun : MonoBehaviour {
	//Bullet variables
	public float bulletSpeed = 500f;
	public float kickForce = 1000f;
	public int magSize = 3;
	public int inMag;
	public GameObject bull1;
	public GameObject bull2;
	public GameObject bull3;
	public GameObject bull4;
	public Rigidbody2D bullet;
	public Rigidbody2D key;
	//Timing Variables
	private float reloadTimer;
	private float shotTimer;
	[HideInInspector] public bool rTimerOn = false;
	private bool sTimerOn = false;
	public float reloadTime = 2f;
	public float interShotDelay = .5f;

	private GameObject player = null;
	private GameObject shootFrom = null;
    private playerCharacter playerScript;
	public AudioClip gunshot;
	public AudioClip reload;

	//For changing gun sprite back after firing key
	public static bool keyLoaded = false;
	public SpriteRenderer gunSprite;
	public Sprite gunRegular;

	public GameObject airBlast;
	public GameObject shotParticles;
	public GameObject airShotParticles;
	public LayerMask airBlastMask;

	public Rigidbody2D cannonball;


	/*WHAT IS THE GUN POINTING AT SO TUCKER CAN GO GET EM
	private Vector3 pointingDirection; 
	private RaycastHit pointingAt = new RaycastHit();
	private GameObject tucker;
	Vector3 mouseWorldPosition;*/

	[HideInInspector] public ScoreKeeper HYPECounter;

	// Use this for initialization
	void Start () {
		inMag = magSize;
		reloadTimer = reloadTime;
		shotTimer = interShotDelay;
        player = transform.parent.gameObject;
        shootFrom = transform.FindChild("Gun/BarrelTip").gameObject;
		HYPECounter = player.GetComponent<ScoreKeeper>();
		gunSprite = shootFrom.transform.parent.GetComponent<SpriteRenderer>();
        playerScript = transform.parent.GetComponent<playerCharacter>();
	}
	
	// Update is called once per frame
	void Update () {

		//rotation
		Vector3 mousePos = retical.recPos;
		mousePos.z = 5.23f;
		
		Vector3 objectPos = Camera.main.WorldToScreenPoint (transform.position);
		mousePos.x = mousePos.x - objectPos.x;
		mousePos.y = mousePos.y - objectPos.y;
		
		float angle = Mathf.Atan2(mousePos.y, mousePos.x) * Mathf.Rad2Deg;
		transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle));
	
		//Gun image will flip depending on where the mouse is relative to the player
		if (mousePos.x - 15 > player.transform.position.x) transform.localScale = new Vector3(1,1,1);
		else transform.localScale = new Vector3(1,-1,1);

		if (ShootButton() && Firable () && !HYPEController.lazers && !HYPEController.airblasts && !HYPEController.cannon) {
			//shoot bullet
			AudioSource.PlayClipAtPoint(gunshot, Camera.main.transform.position);

			sTimerOn = true;
			inMag -= 1;
			adjustCounter(inMag);
			var pos = retical.recPos;
			pos.z = transform.position.z - Camera.main.transform.position.z;
			pos = Camera.main.ScreenToWorldPoint(pos);

			var q = Quaternion.FromToRotation(Vector3.up, pos - shootFrom.transform.position);

			Rigidbody2D toShoot = bullet;
			//Fire the key instead of a bullet if it's loaded
			if(keyLoaded){
				toShoot = key;
				gunSprite.sprite = gunRegular;
				keyLoaded = false;
			}
			Rigidbody2D go = Instantiate(toShoot, shootFrom.transform.position, q) as Rigidbody2D;
			go.GetComponent<Rigidbody2D>().AddForce(go.transform.up * bulletSpeed);

			GameObject particles = (GameObject)Instantiate(shotParticles, shootFrom.transform.position, shootFrom.transform.rotation);
			particles.GetComponent<ParticleSystem>().Play ();
			Destroy (particles, particles.GetComponent<ParticleSystem>().startLifetime);

			//If out of bullets after shooting, reload
			if(inMag <= 0){
				AudioSource.PlayClipAtPoint(reload, Camera.main.transform.position);
				rTimerOn = true;
			}

			if(!playerScript.IsGrounded()){
				//Debug.Log(new Vector2(go.transform.up.x * -kickForce, go.transform.up.y * -kickForce));
				player.GetComponent<Rigidbody2D>().AddForce (new Vector2(go.transform.up.x * -kickForce, go.transform.up.y * -kickForce ));
			}
		}

		//If the airblasts power is on
		else if (ShootButton() && Firable () && !HYPEController.lazers && HYPEController.airblasts && !HYPEController.cannon) {
			sTimerOn = true;
			//Get reticle position
			var pos = Camera.main.ScreenToWorldPoint(retical.recPos);
			//Get direction between the gun and the reticle
			Vector2 direction = (pos - shootFrom.transform.position);
			//Appropriate rotation for trigger
			var q = Quaternion.FromToRotation(Vector3.up, direction);

			//If key is loaded, fire key.
			if(keyLoaded){
				gunSprite.sprite = gunRegular;
				//Rigidbody2D go = 
				Instantiate(key, shootFrom.transform.position, q); //as Rigidbody2D;
				keyLoaded = false;
			}

			//Otherwise, fire an air blast
			else{
				GameObject toShoot = airBlast;
				//Airblast is shot, direction is passed
				GameObject go = Instantiate(toShoot, shootFrom.transform.position, q) as GameObject;
				go.GetComponent<AirBlast>().direction = direction;
				kickIfAirbourne(100f);
			}

			//Create particles for shot
			GameObject particles = (GameObject)Instantiate(airShotParticles, shootFrom.transform.position, shootFrom.transform.rotation);
			particles.GetComponent<ParticleSystem>().Play ();
			Destroy (particles, particles.GetComponent<ParticleSystem>().startLifetime);

		}
		/////////////////////////////
			
		else if (ShootButton() && Firable () && !HYPEController.lazers && !HYPEController.airblasts && HYPEController.cannon) { //cannonball hype is on
			//shoot bullet
			AudioSource.PlayClipAtPoint(gunshot, Camera.main.transform.position); //needs cannon sound?
			sTimerOn = true;
			inMag -= 1;
			adjustCounter(inMag);
			var pos = retical.recPos;
			pos.z = transform.position.z - Camera.main.transform.position.z;
			pos = Camera.main.ScreenToWorldPoint(pos);
			
			var q = Quaternion.FromToRotation(Vector3.up, pos - shootFrom.transform.position);
			
			Rigidbody2D toShoot = cannonball;
			//Fire the key instead of a bullet if it's loaded
			if(keyLoaded){
				toShoot = key;
				gunSprite.sprite = gunRegular;
				keyLoaded = false;
			}
			Rigidbody2D go = Instantiate(toShoot, shootFrom.transform.position, q) as Rigidbody2D;
			go.GetComponent<Rigidbody2D>().AddForce(go.transform.up * bulletSpeed * 10);
			
			GameObject particles = (GameObject)Instantiate(shotParticles, shootFrom.transform.position, shootFrom.transform.rotation);
			particles.GetComponent<ParticleSystem>().Play ();
			Destroy (particles, particles.GetComponent<ParticleSystem>().startLifetime);
			
			//If out of bullets after shooting, reload
			if(inMag <= 0){
				AudioSource.PlayClipAtPoint(reload, Camera.main.transform.position);
				rTimerOn = true;
			}
			
			if(!playerScript.IsGrounded()){
				//Debug.Log(new Vector2(go.transform.up.x * -kickForce, go.transform.up.y * -kickForce));
				player.GetComponent<Rigidbody2D>().AddForce (new Vector2(go.transform.up.x * -kickForce * 1.5f, go.transform.up.y * -kickForce * 1.5f ));
			}
		}



		/////////////////////////////

		if (Input.GetButtonDown ("Reload") && inMag != magSize && !rTimerOn) {
			AudioSource.PlayClipAtPoint(reload, Camera.main.transform.position);
			//play reload anim
			rTimerOn = true;
		}

		//Timer for how long reload takes
		if (rTimerOn) {
			reloadTimer -= Time.deltaTime;
			if(reloadTimer <= 0) {
				rTimerOn = false;
				inMag = magSize;
				adjustCounter(inMag);
				reloadTimer = reloadTime;
			}
		}

		//Timer for how long before you can shoot again
		if (sTimerOn) {
			shotTimer -= Time.deltaTime;
			if(shotTimer <= 0) {
				sTimerOn = false;
				shotTimer = interShotDelay;
			}
		}

	}

	/*void FixedUpdate() {

		mouseWorldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
		//Cast a ray from the player thru the gun
		//pointingDirection = (retical.recPos - transform.position);
		pointingDirection = (mouseWorldPosition - transform.position);
		//If this returns true it hit something
		Debug.DrawRay (transform.position, pointingDirection, Color.red);
		Physics.Raycast (transform.position, pointingDirection, out pointingAt, 20f);
		//Debug.Log (pointingAt);
		if (Physics.Raycast (transform.position, pointingDirection, out pointingAt)) {
		//if (Physics.Raycast()) {
			Debug.Log ("GUN: I'm pointing at something.");
			if (pointingAt.collider.tag.Equals ("enemy")) {
				Debug.Log ("GUN: It's an enemy.");
				if (tucker = GameObject.Find ("Tucker")) {
					tucker.GetComponent<TuckerController> ().changeTarget (pointingAt.collider.gameObject);
				}
			}
		} else {
			Debug.Log ("false");
		}
	}*/

	public void kickIfAirbourne(float kickForce){
		//Get reticle position
		var pos = Camera.main.ScreenToWorldPoint(retical.recPos);
		//Get direction between reticle position and the gunpoint
		Vector2 direction = (pos - shootFrom.transform.position);

		//If the player is airbourne, send add force in the opposite direction of the shot
		if(!player.GetComponent<CharControl>().isGrounded()){
			player.GetComponent<Rigidbody2D>().AddForce (-direction * kickForce);
		}
	}

	public void adjustCounter(int currBulls){
		if (currBulls == 4) {
			bull1.SetActive (true);
			bull2.SetActive (true);
			bull3.SetActive (true);
			bull4.SetActive (true);
		}
		if (currBulls == 3) {
			bull1.SetActive (true);
			bull2.SetActive (true);
			bull3.SetActive (true);
			bull4.SetActive (false);
		}
		if (currBulls == 2) {
			bull1.SetActive (true);
			bull2.SetActive (true);
			bull3.SetActive (false);
			bull4.SetActive (false);
		}
		if (currBulls == 1) {
			bull1.SetActive (true);
			bull2.SetActive (false);
			bull3.SetActive (false);
			bull4.SetActive (false);
		}
		if (currBulls == 0) {
			bull1.SetActive (false);
			bull2.SetActive (false);
			bull3.SetActive (false);
			bull4.SetActive (false);
		}
		//Bullet counter modifications for during and after HYPEmode
		if (ScoreKeeper.HYPED) {
			bull1.SetActive (true);
			bull2.SetActive (true);
			bull3.SetActive (true);
			bull4.SetActive (true);
			bull1.GetComponent<RawImage>().color = Color.red;
			bull2.GetComponent<RawImage>().color = Color.red;
			bull3.GetComponent<RawImage>().color = Color.red;
			bull4.GetComponent<RawImage>().color = Color.red;
		}
		if (!ScoreKeeper.HYPED) {
			bull1.GetComponent<RawImage>().color = Color.white;
			bull2.GetComponent<RawImage>().color = Color.white;
			bull3.GetComponent<RawImage>().color = Color.white;
			bull4.GetComponent<RawImage>().color = Color.white;
		}
	}

	bool ShootButton() {
		return (Input.GetButton ("Fire1") || Input.GetAxis ("RTrig") > 0.1);
	}

	bool Firable() {
		return (inMag != 0 && !rTimerOn && !sTimerOn && !PlayerHealth.alreadyDying && !Popup.paused);
	}
}
