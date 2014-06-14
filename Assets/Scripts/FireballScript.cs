using UnityEngine;
using System.Collections.Generic;

public class FireballScript : MonoBehaviour {
	
	GameObject player;
	PlayerScript playerScript;
	GameObject enemy;
	EnemyScript enemyScript;
	
	Vector3 targetPosition;
	float speed = 5f, angle = 0;
	bool targetPlayer = true;
	
	GameManager gameManager;
	
	// Use this for initialization
	void Start () {
		gameManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
		player = GameObject.FindGameObjectWithTag("Player");
		playerScript = player.GetComponent<PlayerScript>();		
		enemy = FindClosestEnemy();
		enemyScript = enemy.GetComponent<EnemyScript>();
	}	
	
	// Update is called once per frame
	void Update () {
		if(gameManager.gameState == GameManager.GameState.GamePlay){
		//set target position
		if(targetPlayer)
			targetPosition = player.transform.position;
		else
			targetPosition = enemy.transform.position;
		
		Move();
		}
	}
	
	void Move(){
		angle = Vector2.Angle(new Vector2(transform.position.x,transform.position.y),new Vector2(targetPosition.x,targetPosition.y));
		//transform.position = new Vector3(
		//this.pos.x = this.pos.x + speed * Math.cos(angle);
		//		this.pos.y = this.pos.y + speed * Math.sin(angle);
	}
	
	
	
	GameObject FindClosestEnemy() {
        GameObject[] gos;
        gos = GameObject.FindGameObjectsWithTag("Enemy");
        GameObject closest = gos[0];
        float distance = Mathf.Infinity;
        Vector3 position = transform.position;
        foreach (GameObject go in gos) {
            Vector3 diff = go.transform.position - position;
            float curDistance = diff.sqrMagnitude;
            if (curDistance < distance) {
                closest = go;
                distance = curDistance;
            }
        }
        return closest;
    }
}
