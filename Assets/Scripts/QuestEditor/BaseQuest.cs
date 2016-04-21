using UnityEngine;
using UnityEditor;
using System;

namespace Quests
{

    [Serializable]
    public class BaseQuest : MonoBehaviour
    {
        public GameObject QuestFrom { get; set; }
        public GameObject QuestTo { get; set; }
    }

    public class BaseQuestWindow
    {
        public SerializedObject obj;
        public Rect windowRect;
        public int id;
        public string title;

        public virtual void DrawWindow()
        {
            EditorGUILayout.PropertyField(obj.FindProperty("QuestFrom"));
            EditorGUILayout.PropertyField(obj.FindProperty("QuestTo"));
        }
    }
}
