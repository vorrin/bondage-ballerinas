using UnityEngine;
using System.Collections;

public class BondageGod : MonoBehaviour {
    public float minBonusSpawnTime = 5f;
    public float maxBonusSpawnTime = 25f;
    public int maxItemOnScreen = 3;
    public GameObject bonusItem;

	// Use this for initialization
	void Start () {
        StartCoroutine( "BonusRoutine");
	}

    void SpawnBonus()
    {
        print("sPAWNGING");
        Vector3 spawnPoint = Camera.main.ScreenToWorldPoint(new Vector3(
            Random.RandomRange(50, Screen.width - 50),
            Random.RandomRange(50, Screen.height - 50),
            10f)
            );
        Instantiate(bonusItem, spawnPoint, transform.rotation);

    }

    IEnumerator BonusRoutine() {
        SpawnBonus();
        yield return new WaitForSeconds(Random.RandomRange(minBonusSpawnTime, maxBonusSpawnTime));
    }
	
	// Update is called once per frame
	void Update () {
	    
	}
}
