using System;
using UnityEditor;
using UnityEngine;

[Serializable]
public class NestedStruct
{
    [SerializeField]
    private float m_StructFloat;
    public void OnGUI()
    {
        m_StructFloat = EditorGUILayout.FloatField("Struct Float", m_StructFloat);
    }
}

[Serializable]
public class SerializeMe
{
    [SerializeField]
    private string m_Name;
    [SerializeField]
    private int m_Value;
    [SerializeField]
    private NestedStruct m_Struct;

    public SerializeMe()
    {
        m_Struct = new NestedStruct();
        m_Name = "";
    }

    public void OnGUI()
    {
        m_Name = EditorGUILayout.TextField("Name", m_Name);
        m_Value = EditorGUILayout.IntSlider("Value", m_Value, 0, 10);

        m_Struct.OnGUI();
    }
}