﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TextChanger : MonoBehaviour {

	//Text selectedUnitText;
	Text target;
	//public Text selectedUnitText;
	// index for children sem eg vill eru
	// 1 = unit
	//3
	//5
	//7
	//9
	public static TextChanger instance = null;

	// Use this for initialization
	void Awake () {
		if (instance == null) {
			instance = this;
		} else if (instance != this) {
			Destroy (gameObject);
		}

		DontDestroyOnLoad (gameObject);
	}
	
	// Update is called once per frame
	//void Update () {
	//	
	//}

	public void NewSelectedUnit(Unit unit){
		// stillum nafnið á kallinum
		target = instance.gameObject.transform.GetChild(1).GetComponent<Text> ();
		target.text = unit.Type;
		// Stillum lífið á kallinum
		target = instance.gameObject.transform.GetChild(3).GetComponent<Text> ();
		target.text = unit.Health.ToString();
		// Stillum hversu langt það er þar til kallinn má hreyfa sig aftur
		target = instance.gameObject.transform.GetChild(5).GetComponent<Text> ();
		// TODOD : stilla útriekningar aðferð frekar
		target.text = unit.CurrentCooldown.ToString();
		// Stillum skaðann sem kallinn gerir
		target = instance.gameObject.transform.GetChild(7).GetComponent<Text> ();
		target.text = unit.Damage.ToString();
		// Stillum Rangeið hjá kallinum
		target = instance.gameObject.transform.GetChild(9).GetComponent<Text> ();
		target.text = unit.Range.ToString();
		// Stillum hvað cooldownið er eftir hverja hreyfingu
		target = instance.gameObject.transform.GetChild(11).GetComponent<Text> ();
		target.text = unit.Cooldown.ToString ();
		// Stillum hvaða lið kallinn er í
		//target = instance.gameObject.transform.GetChild(13).GetComponent<Text> ();
		//target.text = unit.Team;
	}


	public void UpdateTurnText (int turn){
		target = instance.gameObject.transform.GetChild(14).GetComponent<Text> ();
		target.text = "Turn: " + turn.ToString ();
	}
		
}