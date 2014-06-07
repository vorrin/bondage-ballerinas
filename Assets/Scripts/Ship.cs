using UnityEngine;
using System.Collections;

public class Ship : MonoBehaviour {

    public float engineStrength = 10f;
    public KeyCode thrustButton;
    public KeyCode latchButton;
    private bool latched = false; 
	// Use this for initialization
	void Start () {
	
	}

    public void EngineOn(float deltaTime){
        rigidbody.AddForce( transform.up * (engineStrength * deltaTime ) );
    }

    public void TryLatching()
    {

    }
	
	// Update is called once per frame
	void Update () {
        if (Input.GetKey(thrustButton))
        {
            EngineOn(Time.deltaTime);
        }
        if (Input.GetKey(latchButton))
        {
            TryLatching();
        }

	
	}
}
