using UnityEngine;
using UnityEditor;
using System;

namespace Quests
{
    [Serializable]
    public class CollectQuest : BaseQuest
    {
        public GameObject ItemToCollect { get; set; }
        public int Count { get; set; }
    }

    public class CollectQuestWindow : BaseQuestWindow
    {
        public override void DrawWindow()
        {
            base.DrawWindow();
            EditorGUILayout.PropertyField(obj.FindProperty("ItemToCollect"));
            EditorGUILayout.PropertyField(obj.FindProperty("Count"));
        }
    }
}
