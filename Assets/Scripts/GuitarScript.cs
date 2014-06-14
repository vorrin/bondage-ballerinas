using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GuitarScript : MonoBehaviour {
	
	public Camera UICamera;
	//GuitarButton[] buttons = new GuitarButton[3];
	List<GuitarButton> buttons = new List<GuitarButton>();

	List<Sequence> sequences = new List<Sequence>();
	AudioClip failAudio;
	bool playingSequence = false;
	float sequenceStartTime = 0;
	int currentSequenceNumber = 0, currentStageInSequence = 0;
	AudioSource speaker;
	
	// Use this for initialization
	void Start () {
		speaker = gameObject.AddComponent<AudioSource>()	;
		failAudio = Resources.Load("Audio/incorrect") as AudioClip;
		GenerateButtons();
		
		GenerateSequences();
		//BeginNewSequence();
		//iTween.MoveTo(gameObject,iTween.Hash("oncomplete","BeginNewSequence","time",1.5f,"position",new Vector3(0.01567525f,-0.037f,-9f)));
		

	}
	
	void OnEnable() {
		//DEBUG/XXX
		//add events
		FingerGestures.OnTap += ScreenTapped;	
	}
	
	void RemoveGuitar(){
		//iTween.MoveTo(gameObject,iTween.Hash("complete","BeginNewSequence","time",1.5f,"position",new Vector3(0f,-2.100825f,-9f)));	
	}
	
	// Update is called once per frame
	void Update () {
		if(playingSequence){
			if(Time.time > sequenceStartTime + sequences[currentSequenceNumber].buttonTiming[currentStageInSequence]){
				if(sequences[currentSequenceNumber].pressedInTime[currentStageInSequence]){
					currentStageInSequence++;
					ResetButtons();
					if(currentStageInSequence>=sequences[currentSequenceNumber].pressedInTime.Count-1){
						Debug.Log("Completed sequence");
						ResetButtons();
						playingSequence = false;
						RemoveGuitar();
					}
					else  buttons[sequences[currentSequenceNumber].buttonSequence[currentStageInSequence]].active = true;
				}
				else{
					Debug.Log("Failed sequence");
					FailedSequence();
					RemoveGuitar();
					
					
				}
			}
		}
	}
	
	public void FailedSequence(){
		buttons[sequences[currentSequenceNumber].buttonSequence[currentStageInSequence]].failed = true;
		speaker.Stop();
		speaker.clip = failAudio;
		speaker.Play();
		playingSequence = false;	
	}
	
	public void ScreenTapped(Vector2 fingerPos){
		Debug.Log("ap tap");	
		Debug.Log(fingerPos);
		for (int i = 0; i < buttons.Count; i++) {	
	        Ray ray = UICamera.ScreenPointToRay(Input.mousePosition);
            RaycastHit hitInfo;
			bool colliderHit = buttons[i].collider.Raycast(ray, out hitInfo, 1.0e8f);
			if (colliderHit) {
				Debug.Log("Tapped Button: " + i.ToString() + "active: " + buttons[i].active.ToString());
				//buttons[i].active = !buttons[i].active;
				if(buttons[i].active){
					buttons[i].correctHit = true;
					sequences[currentSequenceNumber].pressedInTime[currentStageInSequence] = true;
					if (currentStageInSequence == 0){
						speaker.Play();	
						sequenceStartTime = Time.time - sequences[currentSequenceNumber].buttonTiming[currentStageInSequence];
						
					}
					if(sequenceStartTime == 0){//just began a new sequence
						BeginNewSequence();
					}
				}
    		}
		}
	}
	
	void SetupNewSequence(){
		for (int i = 0; i < buttons.Count; i++) {
			buttons[i].active = false;
		}
		sequences[currentSequenceNumber].pressedInTime = new List<bool>();
		for (int i = 0; i < sequences[currentSequenceNumber].buttonSequence.Count; i++) {
			sequences[currentSequenceNumber].pressedInTime.Add(false);
		}
		currentStageInSequence = 0;
		buttons[sequences[currentSequenceNumber].buttonSequence[currentStageInSequence]].active = true;
	}
	
	void BeginNewSequence(){
		
		buttons[sequences[currentSequenceNumber].buttonSequence[currentStageInSequence]].active = true;	
		
		Sequence tmpSequence = sequences[currentSequenceNumber];
		tmpSequence.pressedInTime[0] = true;
		speaker.clip = tmpSequence.audio;
		//speaker.Play( (ulong)(44100 * tmpSequence.buttonTiming[0]) );
		//TODO: Start to play audio clip here
		
		sequenceStartTime = Time.time;
		playingSequence = true;
	}
	
	void ResetButtons(){
		foreach (GuitarButton button in buttons) {
			button.active = false;
			button.correctHit = false;
		}
	}
	
	void GenerateSequences(){
		Sequence sequence = new Sequence();
		sequence.buttonSequence = new List<int>();
		sequence.buttonSequence.Add(1);
		sequence.buttonSequence.Add(2);
		sequence.buttonSequence.Add(0);
		sequence.buttonSequence.Add(1);
		
		sequence.buttonTiming = new List<float>();
		
		
		sequence.buttonTiming.Add(1);
		sequence.buttonTiming.Add(1.5f);
		sequence.buttonTiming.Add(2);
		sequence.buttonTiming.Add(2.5f);
		
		sequence.audio = Resources.Load("Audio/Prototype Lick 10") as AudioClip;
		
		sequence.pressedInTime = new List<bool>();
		for (int i = 0; i < 5; i++){
			sequence.pressedInTime.Add(false);
		}
		sequences.Add(sequence);
	}
	
	void GenerateButtons(){
		GuitarButton[] guitarButtons = GetComponentsInChildren<GuitarButton>();
		foreach (GuitarButton button in guitarButtons){
			buttons.Add(button);	
		}
		//add some buttons
//		Button button = new Button();
//		button.rect = new Rect(0,0,50,70);
//		button.active = false;
//		buttons.Add(button);
//		button.rect.y = buttons[buttons.Count-1].rect.yMax;
//		buttons.Add(button);
//		button.rect.y = buttons[buttons.Count-1].rect.yMax;
//		buttons.Add(button);
	}

}
class Sequence{
	public AudioClip audio;
	public List<int> buttonSequence;
	public List<float> buttonTiming;
	public List<bool> pressedInTime;
}

class Button{
	public Rect rect;
	public bool active;
	GameObject gameObject;
}