using UnityEngine;
using System.Collections;

public class PointsScript : MonoBehaviour {
	
	const float introTime = 1, tweenOffTime = 3;
	float timeIntroFinishes = 0, timeTweenOffFinishes = 0;
	
	public Vector3 tweenOffPosition = Vector3.zero;
	
	GameObject camera;
	GameManager gameManager;
	
	// Use this for initialization
	void Start () {
		camera = GameObject.FindGameObjectWithTag("MainCamera");
		gameManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
		
		tweenOffPosition = camera.transform.position;
		
		timeIntroFinishes = Time.time + introTime;
		timeTweenOffFinishes = timeIntroFinishes + tweenOffTime;
	}
	
	// Update is called once per frame
	void Update () {
		if(gameManager.gameState == GameManager.GameState.GamePlay){
		if(Time.time >= timeIntroFinishes){
			tweenOffPosition = camera.transform.position;
			//HOTween.To(this.transform,tweenOffTime,"position",tweenOffPosition);
			
			if(Time.time > timeTweenOffFinishes){
				GameObject.Destroy(this.gameObject);
				gameManager.IncreaseScore(int.Parse(GetComponent<TextMesh>().text));
			}
		}
		}
	}
	
	public static float maxAngle = Mathf.PI/2, minForce = 400, maxForce = 700; 
	public static void SpawnPoints(int amount, Vector3 position,float direction, int numObjectsMultiplier){
		if(numObjectsMultiplier <4)
			numObjectsMultiplier = 4;
		if(numObjectsMultiplier %2 != 0)
			numObjectsMultiplier += 1;
		if(amount < numObjectsMultiplier)
			amount = numObjectsMultiplier;
		
		int remainderPoints = amount % numObjectsMultiplier;
		int pointsPerObject = amount/numObjectsMultiplier;
		
		float angle = 0, force = minForce;
		
		for (int i = 0; i < numObjectsMultiplier; i++) {
			GameObject pointsObject = (GameObject)(Instantiate(Resources.Load("PointsText"), position, Quaternion.Euler(Vector3.zero)));
			pointsObject.GetComponent<TextMesh>().text = pointsPerObject.ToString();
			
			angle = Random.Range(direction,direction+(maxAngle/2));
			force = Random.Range(minForce,maxForce);
			pointsObject.rigidbody.AddForce(new Vector3(force*Mathf.Cos(angle),force*Mathf.Sin(angle),0));
		}
		if(remainderPoints != 0){
			GameObject pointsObject = (GameObject)(Instantiate(Resources.Load("PointsText"), position, Quaternion.Euler(Vector3.zero)));
			pointsObject.GetComponent<TextMesh>().text = remainderPoints.ToString();
			
			angle = Random.Range(direction,direction+(maxAngle/2));
			force = Random.Range(minForce,maxForce);
			pointsObject.rigidbody.AddForce(new Vector3(200*Mathf.Cos(0),200*Mathf.Sin(0),0));
		}
	}
}
