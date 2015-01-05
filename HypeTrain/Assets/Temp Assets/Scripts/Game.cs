﻿using UnityEngine;
using System.Collections;

[System.Serializable]
public class Game {
	public static Game current;
	public static int lifetimeLoot;
	public static int currLoot;
	public static int skin;
	public static int hype;
	public static int carsCleared;
	public int enemiesKilled;
	public int hypeModesActivated;
	public int shotsFired;
	public int deaths;
	public float accuracy;
	public static bool skin1 = false;
	public static bool skin2 = false;
	public static bool skin3 = false;
	public static bool skin4 = false;
	public static bool hype1 = true;
	public static bool hype2 = false;
	public static bool firstTime = true;

	public Game () { //constructor

	}

	public static void addLoot(int amount) {
		currLoot += amount;
		lifetimeLoot += amount;
		SaveLoad.Save ();
	}

	public static void addCarsCleared(int amount) {
		carsCleared += amount;
		SaveLoad.Save ();
	}

}
