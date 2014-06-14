using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class CameraControl : MonoBehaviour {
	
	private PlayerScript player;
	GameManager gameManager;
	
	public Vector3 positionOffset = new Vector3(14,9,-35);
	public Vector3 lookAtPlayerOffset = new Vector3(14,8,0);
	float closeCombatZoomMax = 10f, closeCombatZoomCurrent = 0f, closeCombatZoomSpeed = 100f;
	Vector3 angle = new Vector3(14,14,0);
	private Vector3 defaultCameraAngle;
	private Vector3 defaultCameraOffset; 
	Vector3 relativePos;
	public LookAtTargetObj lookAt;
	// Use this for initialization
	void Start () {

		defaultCameraOffset = positionOffset; 
		defaultCameraAngle = angle;
		player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerScript>();
		gameManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
		
		transform.rotation = Quaternion.Euler(angle);
	//
		
		lookAt = GameObject.Find("LookAt").GetComponent<LookAtTargetObj>();
		
	}
	
	// Update is called once per frame
	void Update () {
		if(gameManager.gameState == GameManager.GameState.GamePlay){
		if(gameManager.closeCombat){
			if(closeCombatZoomCurrent < closeCombatZoomMax){
				closeCombatZoomCurrent += closeCombatZoomSpeed*Time.deltaTime;
			}
			if(closeCombatZoomCurrent > closeCombatZoomMax){
				closeCombatZoomCurrent = closeCombatZoomMax;	
			}
		}else{
			if(closeCombatZoomCurrent > 0){
				closeCombatZoomCurrent -= closeCombatZoomSpeed*Time.deltaTime;
			}
			if(closeCombatZoomCurrent < 0){
				closeCombatZoomCurrent = 0;	
			}
		}
		
		if(!gameManager.closeCombat && lookAt.targetIsPlayer)
			relativePos = (lookAt.transform.position + lookAtPlayerOffset) - transform.position;
		else
			relativePos = lookAt.transform.position - transform.position;
        		
		Quaternion rotation = Quaternion.LookRotation(relativePos);
		transform.rotation = Quaternion.Lerp(transform.rotation, rotation, 1f*Time.deltaTime);
			
			Vector3 newPos = new Vector3(player.transform.position.x + positionOffset.x,
			player.transform.position.y + positionOffset.y,
			player.transform.position.z + positionOffset.z + closeCombatZoomCurrent);
			
			newPos = Vector3.Lerp(transform.position,newPos, 1*Time.deltaTime);
		
			transform.position = newPos;

			
		}
	}
	
	public void SetLookAtTarget(GameObject target, float duration){
	/*	Debug.Log ("Looking at "+ target.name + " for "+ duration);
		lookAt.target = target.transform;
		lookAt.duration = duration;*/
	}
}

