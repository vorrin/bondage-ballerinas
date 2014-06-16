using UnityEngine;
using System.Collections;

public class Bonus : MonoBehaviour {
    public BondageGod bondageGod;
	// Use this for initialization
	void Start () {
        bondageGod = GameObject.Find("Bondage God").GetComponent<BondageGod>();
	}

    void OnTriggerEnter( Collider collision)
    {
        if (collision.gameObject.tag == "Player")
        
       {
           if (collision.gameObject.GetComponent<Ship>().playerColor == Ship.PlayerColor.Blue)
           {
               bondageGod.blueScore.text = (int.Parse(bondageGod.blueScore.text) + 1 ).ToString(); 
           }
           else
           {
               bondageGod.redScore.text = (int.Parse(bondageGod.redScore.text) + 1).ToString(); 
           }


            Destroy(gameObject);
            //Player should be awarded stuff
        }
    }

	// Update is called once per frame
	void Update () {
	    
	}
}
