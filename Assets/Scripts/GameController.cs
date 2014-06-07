using UnityEngine;
using System.Collections;

public class GameController : MonoBehaviour {

	public static GameController instance;

	public float[] sliderValues;
	public float[] sliderActualValues;

	public bool slidersConnected = false;

	public void Awake()
	{
		instance = this;
		sliderActualValues  = new float[6];
		sliderValues  = new float[6];
		for (int i = 0; i < sliderValues.Length; i++)
		{
			sliderValues[i] = -1;
			sliderActualValues[i] = -1;

		}

		sliderValues[1] = 1;
	}


	void FixedUpdate()
	{
		for (int i = 0; i < sliderValues.Length; i++)
			GameController.instance.sliderValues[i] = Mathf.Lerp(GameController.instance.sliderValues[i], GameController.instance.sliderActualValues[i], 0.06f);
		
	}
	
	void Update () {

		if (Input.GetKeyDown(KeyCode.Alpha0)) sliderValues[0] = ((sliderValues[0] + 1.1f) % 2f) - 1;
		Debug.Log("V0: " + sliderValues[0]);
		if (Input.GetKeyDown(KeyCode.Alpha1)) sliderValues[1] = ((sliderValues[1] + 1.1f) % 2f) - 1;
		if (Input.GetKeyDown(KeyCode.Alpha2)) sliderValues[2] = ((sliderValues[2] + 1.1f) % 2f) - 1;
		if (Input.GetKeyDown(KeyCode.Alpha3)) sliderValues[3] = ((sliderValues[3] + 1.1f) % 2f) - 1;
		if (Input.GetKeyDown(KeyCode.Alpha4)) sliderValues[4] = ((sliderValues[4] + 1.1f) % 2f) - 1;
	}
}
