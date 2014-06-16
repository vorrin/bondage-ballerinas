using UnityEngine;
using System.Collections;

public class BearRotation : MonoBehaviour {

	public float speed = 1.5f;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
    public void KickTheBear()
    {
        print("kicking bear ");
        float multi = 5f;
        rigidbody.AddForce(new Vector3(Random.Range(-.7f, .7f), Random.Range(-.2f, .2f), Random.Range(.4f, 1f)) * multi * 3);
        rigidbody.AddTorque(new Vector3(Random.Range(-.7f, .7f), Random.Range(-.6f, .6f), Random.Range(0, -1) ) * multi, ForceMode.Impulse);
    }
	void Update () {
		transform.Rotate(Vector3.up * speed * Time.deltaTime);
        transform.Rotate(Vector3.forward * -2f * Time.deltaTime);
	}
}
