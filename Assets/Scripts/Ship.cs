﻿using UnityEngine;
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
    public float maximumVelocity = 100f;
    public GameObject indicator;
    public SpriteRenderer playerCircle;
    public PlayerColor playerColor;

    public enum PlayerColor
    {
        Blue,
        Red
    }
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
    private PlayerControllerData playerControllerData;

	// Use this for initialization
	void Start () {
        playerControllerData = GetComponent<PlayerControllerData>();
        //DEBUG
	}

    public void Thrust(float deltaTime){
        rigidbody.AddForce( transform.up * (engineStrength * deltaTime ) );
        rigidbody.velocity = Vector3.ClampMagnitude(rigidbody.velocity, maximumVelocity);
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
                    float currentDistance = Vector3.Distance(gameObject.transform.position, currentShip.transform.position)  ;
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
                    ropeComponent.RopeNodes[0].fLength = currentDistance * 1f;
                    ropeComponent.RopeNodes[0].goNode = currentShip.transform.FindChild("Hook").gameObject;
                    ropeComponent.Regenerate();

                }
                
            }
            
        }
    }
	
	// Update is called once per frame
	void Update () {

        //if (Input.GetAxis("Aim_z" + playerControllerData.controllerId))
        //{
        //    print("AIMZ!");
        //    EngineOn(Time.deltaTime);
        //}
        //if (Input.GetAxis("Move_x" + playerControllerData.controllerId))
        //{
        //    print("AIMZ!");
        //    EngineOn(Time.deltaTime);
        //}

        if (Input.GetAxis("Aim_z" + playerControllerData.controllerId) > 0f)
        {
            print("should be runnig");
            Thrust(Time.deltaTime * Input.GetAxis("Aim_z" + playerControllerData.controllerId));
        }

        if (Input.GetAxis("Aim_z" + playerControllerData.controllerId) < 0f)
        {
            Thrust(Time.deltaTime * Input.GetAxis("Aim_z" + playerControllerData.controllerId));
        }


        //Jump1_g == green
        //Fire1_g == red
        //Fire1_z == yellow
        //Fire1_s == blue
        if (Input.GetButton("Jump1_g" + playerControllerData.controllerId))
        {
            indicator.SetActive(true);
            indicator.GetComponent<SpriteRenderer>().color = Color.green;
            if (tryingToLatch != ButtonColors.Null)
            {
                return;
            }
            TryLatching(ButtonColors.Green);
        }

        if (Input.GetButton("Fire1_g" + playerControllerData.controllerId))
        {
            indicator.SetActive(true);
            indicator.GetComponent<SpriteRenderer>().color = Color.red;
            if (tryingToLatch != ButtonColors.Null)
            {
                return;
            }
            TryLatching(ButtonColors.Red);
        }

        if (Input.GetButton("Fire1_z" + playerControllerData.controllerId))
        {

            indicator.SetActive(true);
            indicator.GetComponent<SpriteRenderer>().color = Color.yellow;
            if (tryingToLatch != ButtonColors.Null)
            {
                return;
            }
            TryLatching(ButtonColors.Yellow);
        }
        if (Input.GetButton("Fire1_s" + playerControllerData.controllerId))
        {
            indicator.SetActive(true);
            indicator.GetComponent<SpriteRenderer>().color = Color.blue;
            if (tryingToLatch != ButtonColors.Null)
            {
                return;
            }
            TryLatching(ButtonColors.Blue);
        }


        //if (Input.GetButtonUp("Jump1_g" + playerControllerData.controllerId)  || Input.GetButtonUp("Fire1_g" + playerControllerData.controllerId)  || Input.GetButtonUp("Fire1_z" + playerControllerData.controllerId)  || Input.GetButtonUp("Fire1_s" + playerControllerData.controllerId) ){

        //}



        if (Input.GetButtonUp("Jump1_g" + playerControllerData.controllerId))
        {
            indicator.SetActive(false);
            Unlatch(ButtonColors.Green);
        }

        if (Input.GetButtonUp("Fire1_g" + playerControllerData.controllerId))
        {
            indicator.SetActive(false);
            Unlatch(ButtonColors.Red);
        }

        if (Input.GetButtonUp("Fire1_z" + playerControllerData.controllerId))
        {
            indicator.SetActive(false);
            Unlatch(ButtonColors.Yellow);

        }
        if (Input.GetButtonUp("Fire1_s" + playerControllerData.controllerId))
        {
            indicator.SetActive(false);
            Unlatch(ButtonColors.Blue);
        }
	
	
	}

    void Unlatch(ButtonColors unlatchingColor)
    {
        tryingToLatch = ButtonColors.Null;
        List<Ship> shipsToRemove = new List<Ship>();
        foreach (Ship ship in connections.Keys)
        {
            Dictionary<ButtonColors, UltimateRope> connection = connections[ship];
            if (connection.ContainsKey(unlatchingColor))
            {
                Destroy(connection[unlatchingColor].gameObject);
                ship.connections.Remove(this);
            }
           // connections.Remove(ship);
        }
        connections = new Dictionary<Ship, Dictionary<ButtonColors, UltimateRope>>();


    }
}
