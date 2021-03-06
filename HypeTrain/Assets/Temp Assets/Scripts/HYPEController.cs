﻿// AUTHOR
// Hayden Platt     (platt@ursamajorgames.com)

using UnityEngine;
using System.Collections;

public class HYPEController : MonoBehaviour {

	[HideInInspector] public GameObject player;
	[HideInInspector] public GameObject gunArm;
	[HideInInspector] public Component gunScript;
	[HideInInspector] public ScoreKeeper HYPECounter;
	public GameObject HYPEPlane;

	private Transform trail;
	private string trailName;

	//Timer variables
	private float HYPETimer;
	private bool hTimerOn = false;
	public float HYPEDuration = 7f; 

	public AudioClip HYPEsound;

	//Default HYPE value
	public static string HYPEMode = "orange";

	public static bool lazers = false;
	public static bool airblasts = false;
	public static bool cannon = false;
	private bool planeSpawn = true;

	// Use this for initialization
	void Start () {
		player = GameObject.Find ("Player");
		trail = player.transform.Find("HYPEtrail");
        gunArm = player.transform.Find("GunArm").gameObject;
		gunScript = gunArm.GetComponent<gun>();
		HYPECounter = player.GetComponent<ScoreKeeper>();
		HYPETimer = HYPEDuration;
	}
	
	// Update is called once per frame
	void Update () {
		//If HYPE is full and player is on top of a train, spawn a HYPE Plane
		if (ScoreKeeper.HYPE == 6 && player.transform.position.y > 18.5 && planeSpawn) {
			Instantiate(HYPEPlane, new Vector3 (player.transform.position.x - 50, 28, 0), Quaternion.identity);
			planeSpawn = false;
		}


		//When HYPE is full, pressing the scroll wheel activates HYPE MODE, faster fire and no reloading, HYPE reset
		if (((Input.GetButtonDown ("Interact") && Input.GetButton ("Reload")) || Input.GetButtonDown ("Fire3")) && ScoreKeeper.HYPE == 6) {
			//Debug.Log ("HYPE MODE");

			trailName = HYPEController.HYPEMode + "Trail";
			trail.Find (trailName).GetComponent<trailToggle>().On ();

			if (HYPEMode == "red"){ //Enable rapid fire
				SpriteRenderer[] renderers = gunArm.GetComponentsInChildren<SpriteRenderer>();
				renderers[1].color = Color.red;
				gunArm.GetComponent<gun> ().magSize = 100;
				gunArm.GetComponent<gun> ().inMag = gunArm.GetComponent<gun> ().magSize;
				gunArm.GetComponent<gun> ().adjustCounter(gunArm.GetComponent<gun> ().inMag);
				gunArm.GetComponent<gun> ().interShotDelay = .1f;
				gunArm.GetComponent<gun> ().rTimerOn = false;
				gunArm.GetComponent<gun> ().kickForce = 300f;
			}

			if (HYPEMode == "purple"){ //Enable lazers, disable bullets
				SpriteRenderer[] renderers = gunArm.GetComponentsInChildren<SpriteRenderer>();
				renderers[1].color = new Color(114, 0, 255, 255);
				lazers = true;
			}
			if (HYPEMode == "blue"){ //Enable air blasts, disable bullets
				SpriteRenderer[] renderers = gunArm.GetComponentsInChildren<SpriteRenderer>();
				renderers[1].color = Color.blue;
				airblasts = true;
			}

			if (HYPEMode == "orange"){ //enable cannonball fire, disable bullets
				SpriteRenderer[] renderers = gunArm.GetComponentsInChildren<SpriteRenderer>();
				renderers[1].color = new Color(255, 144, 0, 255);
				gunArm.GetComponent<gun> ().magSize = 100;
				gunArm.GetComponent<gun> ().inMag = gunArm.GetComponent<gun> ().magSize;
				gunArm.GetComponent<gun> ().adjustCounter(gunArm.GetComponent<gun> ().inMag);
				gunArm.GetComponent<gun> ().interShotDelay = .4f;
				gunArm.GetComponent<gun> ().rTimerOn = false;
				cannon = true;
			}
			hTimerOn = true;
			ScoreKeeper.HYPED = true;
			//AudioSource.PlayClipAtPoint(HYPEsound, transform.position);
		}

		//Timer for how long HYPE lasts, resets gun modifications once time runs out
		if (hTimerOn) {
			HYPETimer -= Time.deltaTime;
			if (HYPETimer <= 0) {
				//Debug.Log ("hype over...");

				trailName = HYPEController.HYPEMode + "Trail";
				trail.Find (trailName).GetComponent<trailToggle>().Off ();

				if(HYPEMode == "red"){	//Reset gun values
					gunArm.GetComponent<gun> ().magSize = 4;
					gunArm.GetComponent<gun> ().inMag = gunArm.GetComponent<gun> ().magSize;
					gunArm.GetComponent<gun> ().adjustCounter(gunArm.GetComponent<gun> ().inMag);
					gunArm.GetComponent<gun> ().interShotDelay = .25f;
					gunArm.GetComponent<gun> ().kickForce = 600f;
				}

				if (HYPEMode == "purple"){ //Disable lazers and reenable bullets
					lazers = false;
				}

				if (HYPEMode == "blue"){
					airblasts = false;
				}

				if (HYPEMode == "orange") {
					cannon = false;
					gunArm.GetComponent<gun> ().magSize = 4;
					gunArm.GetComponent<gun> ().inMag = gunArm.GetComponent<gun> ().magSize;
					gunArm.GetComponent<gun> ().adjustCounter(gunArm.GetComponent<gun> ().inMag);
					gunArm.GetComponent<gun> ().interShotDelay = .25f;
					gunArm.GetComponent<gun> ().kickForce = 600f;
				}

				//Reset HYPE gauge, Timer, and gun color
				HYPECounter.incrementHype(false); //Reset HYPE, since it was activated.
				ScoreKeeper.HYPED = false;
				hTimerOn = false;
				HYPETimer = HYPEDuration;
				SpriteRenderer[] renderers = gunArm.GetComponentsInChildren<SpriteRenderer>();
				renderers[1].color = Color.white;
			}
		}
	}
}
