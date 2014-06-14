using UnityEngine;
using System.Collections;

public class LookAtTargetObj : MonoBehaviour {
	public Transform target;
	public float duration =0;
	public bool targetIsPlayer = false;
	
	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {
		if (duration > 0){
			duration -= 1*Time.deltaTime;
			targetIsPlayer = false;
		}
		
		
		if (target == null ||duration <=0 ){
		target = GameObject.FindGameObjectWithTag("Player").transform;
			targetIsPlayer = true;
		}
	
		transform.position = target.transform.position;
	}
}

