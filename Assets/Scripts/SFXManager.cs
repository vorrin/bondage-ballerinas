using UnityEngine;
using System.Collections;

public class SFXManager : MonoBehaviour {
	
	public AudioClip playerHitClip;
	public AudioClip enemyHitClip;
	
	AudioSource sFXSpeaker;
	
	// Use this for initialization
	void Start () {
		sFXSpeaker = GameObject.Find ("Main Camera").AddComponent<AudioSource> ();
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	public void PlaySFX(AudioClip clip){
		sFXSpeaker.PlayOneShot(clip);
	}
}
