using UnityEngine;
using System.Collections;

public class DebuggerScript : MonoBehaviour {
    Ship ship;
    public bool autoThrust = true;
    public Ship.ButtonColors buttonColor;
	// Use this for initialization
	void Start () {
        ship = GetComponent<Ship>();
	}
	
	// Update is called once per frame
	void Update () {
        ship.TryLatching(buttonColor);
        if (autoThrust) ship.Thrust(Time.deltaTime);
	}
}
