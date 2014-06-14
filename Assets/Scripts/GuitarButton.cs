using UnityEngine;
using System.Collections;

public class GuitarButton : MonoBehaviour {
	
	public bool failed = false;
	public bool active = false;
	tk2dSprite sprite;
	public bool correctHit = false;
	// Use this for initialization
	void Start () {
		sprite = GetComponent<tk2dSprite>();
		if (collider == null)
		{
			BoxCollider newCollider = gameObject.AddComponent<BoxCollider>();
			Vector3 colliderExtents = newCollider.extents;
			colliderExtents.z = 0.2f;
			newCollider.extents = colliderExtents;
		}
	}
	
	// Update is called once per frame
	void Update () {
		if (failed){
			sprite.spriteId = sprite.GetSpriteIdByName("buttonFail");		
		}
		else if (!active){
			sprite.spriteId = sprite.GetSpriteIdByName("buttonOff");		
		}
		else if (correctHit){
			sprite.spriteId = sprite.GetSpriteIdByName("buttonHitFine");		
		}
		else if (active){
			sprite.spriteId = sprite.GetSpriteIdByName("buttonOn");	
		}
		
		
	}
}
