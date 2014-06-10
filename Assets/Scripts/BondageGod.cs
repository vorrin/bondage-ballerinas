using UnityEngine;
using System.Collections;

public class BondageGod : MonoBehaviour {
    public float minBonusSpawnTime = 5f;
    public float maxBonusSpawnTime = 25f;
    public int maxItemOnScreen = 3;
    public GameObject bonusItem;
    public TextMesh redScore;
    public TextMesh blueScore;


	// Use this for initialization
	void Start () {
        StartCoroutine( "BonusRoutine");
	}

    void SpawnBonus()
    {
        print("sPAWNGING");
        //Vector3 spawnPoint = Camera.main.ScreenToWorldPoint(new Vector3(
        //    Random.RandomRange(Camera.main.scre, Screen.width - 50),
        //    Random.RandomRange(50, Screen.height - 50),
        //    9.044445f)
        //    );

        //Vector3 spawnPoint = Camera.main.ScreenToWorldPoint(new Vector3(
        //    Random.RandomRange(Screen.width * .2f , Screen.width *.8f),
        //    Random.RandomRange(50, Screen.height - 50),
        //    9.044445f)
        //    );

        Vector3 spawnPoint = Camera.main.ViewportToWorldPoint(new Vector3(
            Random.Range(.2f, .8f),
            Random.Range(.1f, .9f),
            9.0444445f)
            );
        Instantiate(bonusItem, new Vector3(spawnPoint.x , spawnPoint.y, 9.044445f), transform.rotation);

    }

    IEnumerator BonusRoutine() {
        SpawnBonus();
        yield return new WaitForSeconds(Random.RandomRange(minBonusSpawnTime, maxBonusSpawnTime));
        StartCoroutine("BonusRoutine"); 
    }
	
	// Update is called once per frame
	void Update () {
	    
	}
}
