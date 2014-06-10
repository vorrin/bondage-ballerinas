using UnityEngine;
using System.Collections;

public class Music : MonoBehaviour {

    GameObject[] ships;

	// Use this for initialization
	void Start () {
        ships = GameObject.FindGameObjectsWithTag("Player");    
	}
	
	// Update is called once per frame
	void Update () {
        float totalPercents= 0f;
        foreach (GameObject ship in ships)
        {
            totalPercents += ship.rigidbody.velocity.magnitude / ship.GetComponent<Ship>().maximumVelocity;
        }

        float averagePercent = totalPercents / ships.Length;

        GetComponent<AudioSource>().pitch = .66666f + (averagePercent/3);
	}
}
