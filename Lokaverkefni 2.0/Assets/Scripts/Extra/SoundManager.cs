using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour {
	 
	public AudioClip mainMusic;
	public AudioClip[] selectedVoicelines;
	public static SoundManager instance;

	void Start (){
		instance = this;
		//instance.GetComponent<AudioSource>().PlayOneShot (mainMusic);
		instance.GetComponent<AudioSource> ().clip = mainMusic;
		instance.GetComponent<AudioSource> ().Play();
	}



}
