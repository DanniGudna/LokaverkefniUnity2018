using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rider : Unit {

	public Rider(){
		speed = 3;
		range = 1;
		damage = 2;
		health = 8;
		currentCooldown = 0;
		cooldown = 2;
	}

}
