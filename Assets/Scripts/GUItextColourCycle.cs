using UnityEngine;
using System.Collections;

public class GUItextColourCycle : MonoBehaviour {
		

	byte red = 200;
	int maxred = 250;
	byte minred = 180;
	
	// Use this for initialization
	void Start () {
		// new Color(100,128, 166, 255); 
	}
	
	// Update is called once per frame
	void Update () {

		if(red < maxred && red >= minred){
			red+=2;
		} else {
			red = minred;
		}

		transform.guiText.material.color =  new Color32(red, 70, 70, 255);
	}
}
