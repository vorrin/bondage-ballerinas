using UnityEngine;
using System.Collections;

public class ForceGPULoad : MonoBehaviour {
	
	bool hasForcedLoad = false;
		
	public void ForceLoad(Vector3 blockPlacementPos){
		transform.position = new Vector3(blockPlacementPos.x,blockPlacementPos.y +300, blockPlacementPos.z);
		hasForcedLoad = false;
	}
	void Update(){
		if(!hasForcedLoad){
			GetComponent<Camera>().Render();
			hasForcedLoad = true;
		}
	}
}
