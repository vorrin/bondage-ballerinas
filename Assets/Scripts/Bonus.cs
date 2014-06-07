using UnityEngine;
using System.Collections;

public class Bonus : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}

    void OnTriggerEnter( Collider collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            Destroy(gameObject);
            //Player should be awarded stuff
        }
    }

	// Update is called once per frame
	void Update () {
	    
	}
}
