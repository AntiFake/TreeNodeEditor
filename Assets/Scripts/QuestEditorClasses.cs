using UnityEngine;
using UnityEditor;
using System;
using System.Collections.Generic;

namespace QuestManagerEditor
{
    public enum QuestType
    {
        CollectQuest, KillQuest
    }

    [Serializable]
    public class ComponentData : ScriptableObject
    {
        [SerializeField]
        public List<QuestNode> questNodes;

        [SerializeField]
        public List<Link> questLinks;

        [SerializeField]
        public GameObject obj;

        public ComponentData()
        {
            questLinks = new List<Link>();
            questNodes = new List<QuestNode>();
        }

        public void OnEnable()
        {
            hideFlags = HideFlags.HideAndDontSave;
        }
    }

    [Serializable]
    public class Link
    {
        [SerializeField]
        public QuestNode nodeFrom;

        [SerializeField]
        public QuestNode nodeTo;

        public void DrawLink()
        {
            Vector3 startPos = new Vector3(nodeFrom.nodeRect.x + nodeFrom.nodeRect.width, nodeFrom.nodeRect.y + nodeFrom.nodeRect.height / 2, 0);
            Vector3 endPos = new Vector3(nodeTo.nodeRect.x, nodeTo.nodeRect.y + nodeTo.nodeRect.height / 2, 0);
            Vector3 startTan = startPos + Vector3.right * 50;
            Vector3 endTan = endPos + Vector3.left * 50;
            Handles.DrawBezier(startPos, endPos, startTan, endTan, Color.black, null, 1);
        }
    }

    [Serializable]
    public class QuestNode
    {
        [SerializeField]
        public string guid;

        [SerializeField]
        public string title;

        [SerializeField]
        public Rect nodeRect;

        [SerializeField]
        public QuestType questType;

        [SerializeField]
        public GameObject questIssuer;

        [SerializeField]
        public GameObject questAcceptor;

        [SerializeField]
        public string questName = "";

        [SerializeField]
        public string questDescription = "";

        [SerializeField]
        public int experienceAmount = 0;

        // KillQuest
        [SerializeField]
        public int objectCountToKill;
        [SerializeField]
        public GameObject objectToKill;

        // CollectQuest
        [SerializeField]
        public int objectCountToCollect;
        [SerializeField]
        public GameObject objectToCollect;

        public QuestNode()
        {
            guid = Guid.NewGuid().ToString();
            title = "1";
        }

        public void DrawNodeWindow()
        {
            questType = (QuestType) EditorGUILayout.EnumPopup("Тип квеста: ", questType);

            questName = EditorGUILayout.TextField("Название квеста:", questName);
            questDescription = EditorGUILayout.TextField("Описание квеста:", questDescription);
            experienceAmount = EditorGUILayout.IntField("Количество опыта:", experienceAmount);
            questIssuer = (GameObject)EditorGUILayout.ObjectField("Дающий квест: ", questIssuer, typeof(GameObject), true);
            questAcceptor = (GameObject)EditorGUILayout.ObjectField("Принимающий квест: ", questAcceptor, typeof(GameObject), true);

            if (questType == QuestType.KillQuest)
            {
                objectCountToKill = EditorGUILayout.IntField("Число мобов: ", objectCountToKill);
                objectToKill = (GameObject)EditorGUILayout.ObjectField("Тип моба: ", objectToKill, typeof(GameObject), true);
            }

            if (questType == QuestType.CollectQuest)
            {
                objectCountToCollect = EditorGUILayout.IntField("Число объектов: ", objectCountToCollect);
                objectToCollect = (GameObject)EditorGUILayout.ObjectField("Тип объекта: ", objectToCollect, typeof(GameObject), true);
            }
        }
    }
}