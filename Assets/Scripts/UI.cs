using UnityEngine;
using System.Collections;

public class UI : MonoBehaviour {
    public TweenAlpha tweener;
    public BearRotation bear;
    public BondageGod god;

	// Use this for initialization
	void Start () {
         god = GameObject.Find("Bondage God").GetComponent<BondageGod>();
	}
	
	// Update is called once per frame
	void Update () {

        if (!god.gameStarted)
        {
            if (Input.anyKeyDown)
            {
                god.gameStarted = true;
                tweener.enabled = true;
                bear.KickTheBear();
                //  bear.GetComponent<TweenAlpha>().enabled = true;
            }
        }
        
	}
}
