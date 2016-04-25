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
		public int counter;

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
			counter = 0;
        }

		public void OnEnable() { hideFlags = HideFlags.HideAndDontSave; }
    }

    [Serializable]
    public class Link
    {
        [SerializeField]
        public string nodeFromGuid;

		[SerializeField]
		public string nodeToGuid;
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