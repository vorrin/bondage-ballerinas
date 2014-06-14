using UnityEngine;
using System.Collections;

public class AudioManager : MonoBehaviour
{
	public float bpm = 138;
	public const float barLength = 1.74f, beatLength = 0.435f;
	float timeMusicStarted = 0;
	bool playingMusic = false, playMusicTriggerRecieved = false, playingClip = false;
	public float timeBarStarted = 0, timeNextBarStarts = 0, timeTillNextBar = 0;
	public float timeBeatStarted = 0, timeNextBeatStarts =0, timeTillNextBeat = 0;
	public float timeTillNextBeatLerp = 0;
	public float timeTillNextBarLerp = 0;
	public float timeClipStarted = 0;
	public float totalBarsInLoop = 10, nextBarInLoop = 0;
	public int currentBeat = 0;
	
	public AudioClip bGM;
	public AudioClip testClip;
	
	GameObject player;
	PlayerScript playerScript;

	AudioSource backgroundMusicSpeaker;
	AudioSource soloSpeaker;
	
	GameManager gameManager;
	
	public float pan = -1f;
	
	public float soloVolume = 1f, bGMVolume = 1f;
	
	// Use this for initialization
	void Awake(){
		backgroundMusicSpeaker = GameObject.Find ("Main Camera").AddComponent<AudioSource> ();
		soloSpeaker = GameObject.Find ("Main Camera").AddComponent<AudioSource> ();
		backgroundMusicSpeaker.clip = bGM;	
		soloSpeaker.clip = testClip;
	}
	void Start ()
	{
		gameManager = GetComponent<GameManager>();
		player = GameObject.FindGameObjectWithTag("Player");
		playerScript = player.GetComponent<PlayerScript>();
	}
	
	// Update is called once per frame
	void Update ()
	{
	
		backgroundMusicSpeaker.pan = pan; //(gameManager.scoreMultiplier/gameManager.scoreMultiplierMax) -1;
			
			
		if(gameManager.gameState == GameManager.GameState.GamePlay){
			
		soloSpeaker.volume = soloVolume;
		backgroundMusicSpeaker.volume = bGMVolume;
			
		if (!playingMusic ) {			
			backgroundMusicSpeaker.Play ();
			backgroundMusicSpeaker.loop = true;
			timeMusicStarted = Time.realtimeSinceStartup;
			timeNextBarStarts = timeMusicStarted + barLength;
			timeNextBeatStarts = timeMusicStarted + beatLength;
			playingMusic = true;
		}
		
		if (Time.realtimeSinceStartup > timeNextBarStarts) {
			
			if (playMusicTriggerRecieved && !playingClip) {
				
				playerScript.animator.SetBool("COMBO2",true);
				gameManager.closeCombatTarget.animator.SetBool("COMBO2",true);
				
				soloSpeaker.PlayOneShot (testClip);	
				timeClipStarted = Time.realtimeSinceStartup;
				playingClip = true;
					
				
			}
			
				
			nextBarInLoop++;
			if(nextBarInLoop == totalBarsInLoop)
					nextBarInLoop = 0;
				
			timeBarStarted = timeNextBarStarts;
			timeNextBarStarts += barLength;
		}
			
		if(Time.realtimeSinceStartup > timeNextBeatStarts){
			timeBeatStarted = timeNextBeatStarts;
			timeNextBeatStarts += beatLength;
			currentBeat++;
		}
		
		if (playMusicTriggerRecieved && playingClip && Time.realtimeSinceStartup > timeClipStarted + testClip.length) {
			playMusicTriggerRecieved = false;
			playingClip = false;
				
			gameManager.closeCombatTarget.alive = false;
		}
			
		timeTillNextBar = timeNextBarStarts - Time.realtimeSinceStartup;
		timeTillNextBarLerp = Mathf.Clamp01(timeTillNextBar/barLength);
		timeTillNextBeat = timeNextBeatStarts - Time.realtimeSinceStartup;
		timeTillNextBeatLerp = Mathf.Clamp01(timeTillNextBeat/beatLength);
		}
	}
	
	public void TriggerSolo ()
	{
		if (!playMusicTriggerRecieved) {
			playMusicTriggerRecieved = true;
			
		}
	}
}

/*
 * Solos are always triggered at the start of the next bar
 * we always know which bar of the BGM loop is coming next
 * we must trigger a solo that fits the upcoming bar
 * the solo must match the difficulty of the current stage of the solo
 */