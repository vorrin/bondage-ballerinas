using UnityEngine;
using System.Collections;

public class Item : MonoBehaviour {
	public AudioClip pickUpSound;
	
	protected virtual void HitPlayer(Player player) {
			
	}
	
	void ProcessCollision(Collider other) {
		Player hitPlayer = other.GetComponent<Player>();
		if (hitPlayer != null) {
			HitPlayer(hitPlayer);
		}
		
//		Agent hitAgent = other.GetComponent<Agent>();
//		if (hitAgent != null) {
//			hitAgent.GetHit();
//			this.firingPlayer.HitEnemy();
//			GameObject.Destroy(this.gameObject);
//		}
	}
	
	void OnTriggerEnter(Collider other) {
		ProcessCollision(other);
    }
	
	void OnTriggerExit(Collider other) {
		ProcessCollision(other);
    }
	
	void OnTriggerStay(Collider other) {
		ProcessCollision(other);
    }
}
