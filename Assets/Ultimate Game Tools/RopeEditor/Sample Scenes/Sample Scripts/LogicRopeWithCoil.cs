using UnityEngine;
using System.Collections;

public class LogicRopeWithCoil : MonoBehaviour
{
    public UltimateRope Rope;
    public float RopeExtensionSpeed;

    float m_fRopeExtension;

	void Start()
    {
	    m_fRopeExtension = Rope != null ? Rope.m_fCurrentExtension : 0.0f;
	}

    void OnGUI()
    {
        LogicGlobal.GlobalGUI();
        GUILayout.Label("Rope test (Procedural rope with additional coil)");
        GUILayout.Label("Use the keypad + and - to extend the rope");
    }

	void Update()
    {
        if(Input.GetKey(KeyCode.KeypadPlus))  m_fRopeExtension += Time.deltaTime * RopeExtensionSpeed;
        if(Input.GetKey(KeyCode.KeypadMinus)) m_fRopeExtension -= Time.deltaTime * RopeExtensionSpeed;

        if(Rope != null)
        {
            m_fRopeExtension = Mathf.Clamp(m_fRopeExtension, 0.0f, Rope.ExtensibleLength);
            Rope.ExtendRope(UltimateRope.ERopeExtensionMode.LinearExtensionIncrement, m_fRopeExtension - Rope.m_fCurrentExtension);
        }
	}
}
