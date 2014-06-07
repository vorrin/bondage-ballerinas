using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Ship : MonoBehaviour {

    public float engineStrength = 10f;
    public KeyCode thrustButton;
    public KeyCode latchButton;

    public GameObject ropePrefab;
    public Dictionary<Ship, Dictionary<ButtonColors, UltimateRope>> connections = new Dictionary<Ship, Dictionary<ButtonColors, UltimateRope>>();
    public ButtonColors debugShipColor;
    public float maximumDistanceForLatching = 20f;
    public enum ButtonColors
    {
        Red,
        Green,
        Blue,
        Yellow,
        Null
    }


    private ButtonColors tryingToLatch = ButtonColors.Null;
    private bool latched = false;


	// Use this for initialization
	void Start () {
	
	}

    public void EngineOn(float deltaTime){
        rigidbody.AddForce( transform.up * (engineStrength * deltaTime ) );
    }

    public void TryLatching(ButtonColors currentColor) //color option to go in here?
    {
        tryingToLatch = currentColor;
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        foreach (GameObject player in players)
        {
            Ship currentShip = player.GetComponent<Ship>();
            if (currentShip.tryingToLatch != ButtonColors.Null && currentShip.tryingToLatch == currentColor && currentShip != this)
            {
                if (connections.ContainsKey(currentShip))
                {
                    //Already bound, nothing done.
                    return;
                }
                else
                {
                    float currentDistance = Vector3.Distance(gameObject.transform.position, currentShip.transform.position);
                    if (currentDistance > maximumDistanceForLatching)
                    {
                        print("too far");
                        continue;
                        //too far, nothing done.
                    }
                    GameObject rope = Instantiate(ropePrefab as Object) as GameObject;
                    UltimateRope ropeComponent = rope.GetComponent<UltimateRope>();

                    //Dictionary<ButtonColors, UltimateRope> tmpDict = new Dictionary<ButtonColors, UltimateRope>() {
                    //    { currentColor,ropeComponent}
                    //};

                    connections.Add(currentShip, new Dictionary<ButtonColors, UltimateRope>() {  { currentColor, ropeComponent} });
                    currentShip.connections.Add(this, new Dictionary<ButtonColors, UltimateRope>() { { currentColor, ropeComponent } });

                    ropeComponent.RopeStart = transform.FindChild("Hook").gameObject;
                    ropeComponent.RopeNodes[0].fLength = currentDistance; 
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
            if (tryingToLatch != ButtonColors.Null  )
            {
                return;
            }
            TryLatching( debugShipColor );
        }
        if (Input.GetKeyUp(latchButton))
        {
            Unlatch(debugShipColor);

        }

	
	}

    void Unlatch(ButtonColors unlatchingColor)
    {
        tryingToLatch = ButtonColors.Null;
        foreach (Ship ship in connections.Keys)
        {
            Dictionary<ButtonColors, UltimateRope> connection = connections[ship];
            if (connection.ContainsKey(unlatchingColor))
            {
                Destroy(connection[unlatchingColor].gameObject);
                ship.connections.Remove(this);
            }
            connections.Remove(ship);
        }


    }
}
