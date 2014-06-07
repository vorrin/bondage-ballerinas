using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Ship : MonoBehaviour {

    public float engineStrength = 10f;
    public KeyCode thrustButton;
    public KeyCode latchButton;
    private bool latched = false;
    public bool tryingToLatch = false;
    public GameObject ropePrefab;

    
    public Dictionary<Ship, UltimateRope> connections = new Dictionary<Ship, UltimateRope>();
	// Use this for initialization
	void Start () {
	
	}

    public void EngineOn(float deltaTime){
        rigidbody.AddForce( transform.up * (engineStrength * deltaTime ) );
    }

    public void TryLatching() //color option to go in here?
    {
        tryingToLatch = true;
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        foreach (GameObject player in players)
        {
            Ship currentShip = player.GetComponent<Ship>();
            if (currentShip.tryingToLatch && currentShip != this)
            {
                if (connections.ContainsKey(currentShip))
                {
                    return;
                }
                else
                {

                    GameObject rope = Instantiate(ropePrefab as Object) as GameObject;
                    UltimateRope ropeComponent = rope.GetComponent<UltimateRope>();
                    ropeComponent.RopeStart = transform.FindChild("Hook").gameObject;
                    ropeComponent.RopeNodes[0].goNode = currentShip.transform.FindChild("Hook").gameObject;
                    ropeComponent.Regenerate();
                }
                
            }
            
        }
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
