using UnityEngine;
using System.Collections;

public class BasicMove : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		int yMovement = 0;
		int xMovement = 0;

		if (Input.GetKey ("w")) {
			yMovement = 1;

		} else if (Input.GetKey ("s")) {
			yMovement = -1;
		} else if (Input.GetKey ("d")) {
			xMovement = 1;	
		} else if (Input.GetKey ("a")) {
			xMovement = -1;
		}

		this.transform.rigidbody.AddForce(new Vector3(xMovement, yMovement, 0));
	}
}
