using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Loader : MonoBehaviour {
	// TODO: bæta viðsvo þettaer notað
	public GameObject gameManager;

	// Use this for initialization
	void Awake () {
		if (GameManager.instance == null) {
			Instantiate (gameManager);
		}
	}
	
	// Update is called once per frame
	//void Update () {
	//	
	//s}
}
