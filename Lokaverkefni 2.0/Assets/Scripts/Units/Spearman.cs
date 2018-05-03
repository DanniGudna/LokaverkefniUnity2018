using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spearman : Unit {

	public Spearman(){
		speed = 2; //2
		range = 1;
		damage = 3;
		health = 10;
		currentCooldown = 0;
		cooldown = 3;
		type = "Spearman";

	}
}
