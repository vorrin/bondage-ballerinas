using UnityEngine;
using System.Collections;

public class DebugShip : Ship {
    Ship ship;
    public bool autoThrust = true;
    public Ship.ButtonColors buttonColor;
	// Use this for initialization
    public override void Start()
    {
        ship = GetComponent<Ship>();
        base.Start();
	}

    public override void TryUnlatching()
    {
        if (buttonColor == ButtonColors.Null)
        {
            return;
        }
        else
        {
            base.TryUnlatching();
        }
    }
	
	// Update is called once per frame
	public override void  Update () {
        //indicator.SetActive(true);
        //indicator.GetComponent<SpriteRenderer>().color = Color.yellow;
        //if (tryingToLatch != ButtonColors.Null)
        //{
        //    return;
        //}
        ship.TryLatching(buttonColor);

        base.Update();
      //  ship.TryLatching(buttonColor);
        if (autoThrust) ship.Thrust(Time.deltaTime);
	}
}
