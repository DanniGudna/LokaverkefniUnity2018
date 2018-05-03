using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour {
	 
	public AudioSource voiceSource;
	public AudioSource musicSource;
	public AudioClip mainMusic;
	public AudioClip[] selectedVoicelines;
	public static SoundManager instance = null;

	void Awake (){
		if (instance == null) {
			instance = this;
		} else if (instance != this) {
			Destroy (gameObject);
		}
		
		DontDestroyOnLoad (gameObject);
	}

	public void PlayRandomVoiceline (AudioClip[] clips){

		int randomIndex = Random.Range (0, clips.Length);
		voiceSource.clip = clips[randomIndex];
		voiceSource.Play ();
	}

	public void PlaySingleClip (AudioClip clip){
		voiceSource.clip = clip;
		voiceSource.Play ();
	}



}
