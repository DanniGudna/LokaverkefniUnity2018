using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {

	private int turn = 0;
	public static GameManager instance = null;
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

	/**
	 *  updates the turn
	 **/
	void nextTurn(){
		turn++;
		print (turn);
	}
}
