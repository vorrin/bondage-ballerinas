using UnityEngine;
using System.Collections;

public class PlayerControllerData : MonoBehaviour 
{
	public static int ONEPLAYER = 0;
	public static int TWOPLAYERLEFT = 1;
	public static int TWOPLAYERRIGHT = 2;
	
	public int playerId;
	public int controllerId;
	public int controllerLayout;	
	
	public bool isReady = false;
	
	public GUIText infoText;
	
	private GoTween pulseTween;
	private GoTween lowTween;
	private GoTween highTween;
	private Color originalColor;
	public Color playerColor;
	
	void Start () 
	{
		Go.defaultEaseType = GoEaseType.BounceOut;
		this.transform.localPosition += new Vector3(0, 0.1f, 0);
		this.transform.localPositionTo(1, new Vector3(0, -0.1f, 0), true);	
	}		
	
	public void Init(int id, int layout, int pid)
	{
		playerId = pid;
		controllerId = id;
		controllerLayout = layout;
		Debug.Log("Added Controller "+id+ " type "+layout);
	}
	
	public void SetButtonText(string text)
	{
		this.infoText.text = text;
	}
	
	public void PulseButton()
	{
		if (pulseTween != null && pulseTween.state == GoTweenState.Running)
			return;
		Go.defaultEaseType = GoEaseType.CircInOut;
		Go.defaultLoopType = GoLoopType.PingPong;
		pulseTween = Go.to( this.transform, 0.2f, new GoTweenConfig().scale( 0.1f, true ).setIterations(2) );
	}
	
	public void SetButtonLow()
	{
		if (lowTween != null && lowTween.state == GoTweenState.Running)
			return;			
		Go.defaultEaseType = GoEaseType.Linear;		
		lowTween = Go.to (this.transform.Find ("Bar").GetComponent<GUITexture>(), 0.1f, 
			new GoTweenConfig().colorProp("color", originalColor * 0.75f, false));
	}
	
	public void SetButtonHigh()
	{
		if (highTween != null && highTween.state == GoTweenState.Running)
			return;		
		Go.defaultEaseType = GoEaseType.Linear;		
		highTween = Go.to (this.transform.Find ("Bar").GetComponent<GUITexture>(), 0.1f, 
			new GoTweenConfig().colorProp("color", originalColor, false));
	}
	
	public void SetButtonColor()
	{
		int cindex = (controllerId+1) * (controllerLayout == PlayerControllerData.TWOPLAYERRIGHT?2:1);
		
		if (cindex < 0 || cindex >= GameController.playerColorList.Length)
			cindex = GameController.playerColorList.Length - 1;
		
		Color color = GameController.playerColorList[playerId % 4];
		playerColor = color;
		color.a = 0.4f;
		
		this.originalColor = color;
		
		transform.Find ("Bar").GetComponent<GUITexture>().color = color;
	}
	
}