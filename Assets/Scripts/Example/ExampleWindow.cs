using UnityEngine;
using UnityEditor;

public class MyWindow : EditorWindow
{
    private SerializeMe m_SerialziedThing;

    [MenuItem("Window/Serialization")]
    static void Init()
    {
        GetWindow<MyWindow>();
    }

    void OnEnable()
    {
        hideFlags = HideFlags.HideAndDontSave;
        if (m_SerialziedThing == null)
            m_SerialziedThing = new SerializeMe();
    }

    void OnGUI()
    {
        GUILayout.Label("Serialized Things", EditorStyles.boldLabel);
        m_SerialziedThing.OnGUI();
    }
}
