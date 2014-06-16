using UnityEngine;
using System.Collections;

public class BondageGod : MonoBehaviour {
    public float minBonusSpawnTime = 5f;
    public float maxBonusSpawnTime = 25f;
    public int baxBonusesOnScreen = 3;
    public GameObject bonusItem;
    public TextMesh redScore;
    public TextMesh blueScore;
    public Collider2D bonusSpawnRect;
    public bool gameStarted = false;

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


        //Vector3 spawnPoint = Camera.main.ViewportToWorldPoint(new Vector3(
        //    Random.Range(.2f, .8f),
        //    Random.Range(.1f, .9f),
        //    9.0444445f)
        //    );

        Vector3 pointInRectangle = new Vector3(
            Random.Range(bonusSpawnRect.bounds.min.x, bonusSpawnRect.bounds.max.x),
            Random.Range(bonusSpawnRect.bounds.min.y, bonusSpawnRect.bounds.max.y),
            9.04444445f
            );
        Instantiate(bonusItem, pointInRectangle, transform.rotation);

    }

    IEnumerator BonusRoutine() {
        if (GameObject.FindGameObjectsWithTag("Bonus").Length < baxBonusesOnScreen )
        {
            SpawnBonus();
        }
        yield return new WaitForSeconds(Random.RandomRange(minBonusSpawnTime, maxBonusSpawnTime));
        StartCoroutine("BonusRoutine"); 
    }
	
	// Update is called once per frame
	void Update () {
	    
	}
}
