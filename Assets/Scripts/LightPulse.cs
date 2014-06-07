using UnityEngine;
using System.Collections;

public class LightPulse : MonoBehaviour {
		private Light myLight;
		public float maxIntensity = 1f;
		public float minIntensity = 0f;
		public float pulseSpeed = 1f; //here, a value of 0.5f would take 2 seconds and a value of 2f would take half a second
		private float targetIntensity = 1f;
		private float currentIntensity;    
		
		
		void Start(){
			myLight = GetComponent<Light>();
		}  
		void Update(){

		if(myLight.intensity < maxIntensity && myLight.intensity >= minIntensity){
			currentIntensity += Time.deltaTime * pulseSpeed;
		} else {
			currentIntensity = minIntensity;
		}

			myLight.intensity = currentIntensity;
		}
	}