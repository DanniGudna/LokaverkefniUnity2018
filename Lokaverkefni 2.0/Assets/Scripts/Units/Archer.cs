﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Archer : Unit {


	public Archer(){
		speed = 1;
		range = 3;
		damage = 4;
		health = 5;
		currentCooldown = 0;
		cooldown = 4;
		type = "Archer";

	}
	// Use this for initialization
	//void Start () {
	//	test ();
	//}

	//public int getSpeed (){
//		return speed;
//	}
	

}
