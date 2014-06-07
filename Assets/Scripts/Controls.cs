using UnityEngine;
using System.Collections;

public class Controls
{
	public static bool PressingSpace ()
	{
		return Input.GetKey (KeyCode.Space);
	}

	public static bool PressingRightShift ()
	{
		return Input.GetKey (KeyCode.RightShift);
	}
    
	public static bool PressingEnter ()
	{
		return Input.GetKey (KeyCode.Return);
	}

	public static float GetLeftHorizontalAxis ()
	{
		float hMovement = 0;

        
		if (Input.GetKey ("d")) {
			hMovement = 1;
		} else if (Input.GetKey ("a")) {
			hMovement = -1;
		}
		return hMovement;

	}

	public static float GetLeftVerticalAxis ()
	{
		float yMovement = 0;

        
		if (Input.GetKey ("w")) {
			yMovement = 1;
		} else if (Input.GetKey ("s")) {
			yMovement = -1;
		}
		return yMovement;

    
	}

}