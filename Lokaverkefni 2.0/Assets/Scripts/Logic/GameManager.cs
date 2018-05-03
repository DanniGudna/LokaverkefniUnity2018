using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {

	private int turn = 0;
	//public HexGrid hexGrid;
	//public static GameManager instance = null;


	//breidd borðsins
	int cellCountX = 10;
	// public int width;
	//hæð borðsins
	int cellCountZ = 10;
	//public int height;
	// Use this for initialization
	void Awake () {
		//if (instance == null) {
		//	instance = this;
		//} else if (instance != this) {
		//	Destroy (gameObject);
		//}

		//DontDestroyOnLoad (gameObject);
		//hexGrid = GetComponent<HexGrid> ();
		//hexGrid = GetComponentInChildren<HexGrid>();
		//InitGame ();

	}

	void InitGame(){
		//dwhexGrid.CreateMap (cellCountX, cellCountZ);
	}
	
	// Update is called once per frame
	//void Update () {
	//	
	//}

	/**
	 *  updates the turn
	 **/
	public void nextTurn(){
		turn++;
		print (turn);
	}
}
