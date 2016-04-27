using UnityEngine;
using UnityEditor;
using System;
using System.Collections.Generic;

namespace ExpertSystemEditor
{
    public class ExpertSystem : ScriptableObject
    {
        [SerializeField]
        public int counter;

        [SerializeField]
        public List<Node> nodes;

        [SerializeField]
        public List<Link> links;

        public ExpertSystem()
        {
            links = new List<Link>();
            nodes = new List<Node>();
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

        [SerializeField]
        public bool IsTrue;
    }

    // Обозначает "материал" участка карты.
    public enum PolyType
    {
        Ground,
        Liquid,
        Void,
        Obstacle
    }

    // Настройки робота на проходимость.
    public enum BotPassabilitySettings
    {
        // Ground.
        Soil,
        Dust,
        Sand,
        Pebbles,
        // Water.
        Swamp,
        OpenWater,
        // Obstacles.
        Wall
    }

    [Serializable]
    public class Node
    {
        [SerializeField]
        public string guid;

        [SerializeField]
        public int number;

        [SerializeField]
        public Rect nodeRect;

        [SerializeField]
        public string description;

        [SerializeField]
        public PolyType polyGeneralType;

        [SerializeField]
        public BotPassabilitySettings polyMaterialType;

        public Node()
        {
            guid = Guid.NewGuid().ToString();
        }

        public void DrawNodeWindow()
        {
            polyGeneralType = (PolyType) EditorGUILayout.EnumPopup("Тип: ", polyGeneralType);
            polyMaterialType = (BotPassabilitySettings) EditorGUILayout.EnumPopup("Материал: ", polyMaterialType);
            description = EditorGUILayout.TextField("Вопрос:", description);
        }
    }

    //public enum QuestType
    //{
    //    CollectQuest, KillQuest
    //}

    //[Serializable]
    //public class ComponentData : ScriptableObject
    //{
    //    [SerializeField]
    //    public int counter;

    //    [SerializeField]
    //    public List<QuestNode> questNodes;

    //    [SerializeField]
    //    public List<Link> questLinks;

    //    public ComponentData()
    //    {
    //        questLinks = new List<Link>();
    //        questNodes = new List<QuestNode>();
    //        counter = 0;
    //    }

    //    public void OnEnable() { hideFlags = HideFlags.HideAndDontSave; }
    //}

    //[Serializable]
    //public class QuestNode
    //{
    //    [SerializeField]
    //    public string guid;

    //    [SerializeField]
    //    public int number;

    //    [SerializeField]
    //    public Rect nodeRect;

    //    [SerializeField]
    //    public QuestType questType;

    //    [SerializeField]
    //    public GameObject questIssuer;

    //    [SerializeField]
    //    public GameObject questAcceptor;

    //    [SerializeField]
    //    public string questName;

    //    [SerializeField]
    //    public string questDescription;

    //    [SerializeField]
    //    public int experienceAmount;

    //    // KillQuest
    //    [SerializeField]
    //    public int objectCountToKill;
    //    [SerializeField]
    //    public GameObject objectToKill;

    //    // CollectQuest
    //    [SerializeField]
    //    public int objectCountToCollect;
    //    [SerializeField]
    //    public GameObject objectToCollect;

    //    public QuestNode()
    //    {
    //        guid = Guid.NewGuid().ToString();
    //    }

    //    public void DrawNodeWindow()
    //    {
    //        questType = (QuestType)EditorGUILayout.EnumPopup("Тип квеста: ", questType);

    //        questName = EditorGUILayout.TextField("Название квеста:", questName);
    //        questDescription = EditorGUILayout.TextField("Описание квеста:", questDescription);
    //        experienceAmount = EditorGUILayout.IntField("Количество опыта:", experienceAmount);
    //        questIssuer = (GameObject)EditorGUILayout.ObjectField("Дающий квест: ", questIssuer, typeof(GameObject), true);
    //        questAcceptor = (GameObject)EditorGUILayout.ObjectField("Принимающий квест: ", questAcceptor, typeof(GameObject), true);

    //        if (questType == QuestType.KillQuest)
    //        {
    //            objectCountToKill = EditorGUILayout.IntField("Число мобов: ", objectCountToKill);
    //            objectToKill = (GameObject)EditorGUILayout.ObjectField("Тип моба: ", objectToKill, typeof(GameObject), true);
    //        }

    //        if (questType == QuestType.CollectQuest)
    //        {
    //            objectCountToCollect = EditorGUILayout.IntField("Число объектов: ", objectCountToCollect);
    //            objectToCollect = (GameObject)EditorGUILayout.ObjectField("Тип объекта: ", objectToCollect, typeof(GameObject), true);
    //        }
    //    }
    //}
}