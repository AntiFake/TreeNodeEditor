using UnityEngine;
using UnityEditor;
using System;

namespace QuestManagerEditor
{
    [Serializable]
    public abstract class BaseQuestNode : ScriptableObject
    {
        public string guid;
        public string title;
        public Rect nodeRect;

        [SerializeField]
        public BaseQuest quest;

        public BaseQuestNode(string t)
        {
            guid = Guid.NewGuid().ToString();
            title = t;
        }

        public virtual void DrawNodeWindow() { }
    }

    [Serializable]
    public class CollectQuestNode : BaseQuestNode
    {
        public CollectQuestNode() : base("Сбор предметов")
        {
        }

        public override void DrawNodeWindow()
        {
            quest.questName = EditorGUILayout.TextField("Название квеста:", quest.questName);
            quest.questDescription = EditorGUILayout.TextField("Описание квеста:", quest.questDescription);
            quest.experienceAmount = EditorGUILayout.IntField("Количество опыта:", quest.experienceAmount);
            quest.questIssuer = (GameObject)EditorGUILayout.ObjectField("Дающий квест: ", quest.questIssuer, typeof(GameObject), true);
            quest.questAcceptor = (GameObject)EditorGUILayout.ObjectField("Принимающий квест: ", quest.questAcceptor, typeof(GameObject), true);
            (quest as CollectQuest).objectCount = EditorGUILayout.IntField("Число объектов: ", (quest as CollectQuest).objectCount);
            (quest as CollectQuest).objectToCollect = (GameObject)EditorGUILayout.ObjectField("Тип объекта: ", (quest as CollectQuest).objectToCollect, typeof(GameObject), true);
        }
    }

    [Serializable]
    public class KillQuestNode : BaseQuestNode
    {
        public KillQuestNode() : base("Убийство мобов")
        {

        }

        public override void DrawNodeWindow()
        {
            quest.questName = EditorGUILayout.TextField("Название квеста:", quest.questName);
            quest.questDescription = EditorGUILayout.TextField("Описание квеста:", quest.questDescription);
            quest.experienceAmount = EditorGUILayout.IntField("Количество опыта:", quest.experienceAmount);
            quest.questIssuer = (GameObject)EditorGUILayout.ObjectField("Дающий квест: ", quest.questIssuer, typeof(GameObject), true);
            quest.questAcceptor = (GameObject)EditorGUILayout.ObjectField("Принимающий квест: ", quest.questAcceptor, typeof(GameObject), true);
            (quest as KillQuest).objectCount = EditorGUILayout.IntField("Число мобов: ", (quest as KillQuest).objectCount);
            (quest as KillQuest).objectToKill = (GameObject)EditorGUILayout.ObjectField("Тип моба: ", (quest as KillQuest).objectToKill, typeof(GameObject), true);
        }
    }

    [Serializable]
    public class Link
    {
        [SerializeField]
        public BaseQuestNode nodeFrom;

        [SerializeField]
        public BaseQuestNode nodeTo;

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
    public class TreeLink
    {
        [SerializeField]
        public BaseQuest questFrom;
        [SerializeField]
        public BaseQuest questTo;
    }

    [Serializable]
    public class BaseQuest
    {
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
    }

    [Serializable]
    public class KillQuest : BaseQuest
    {
        [SerializeField]
        public int objectCount;

        [SerializeField]
        public GameObject objectToKill;
    }

    [Serializable]
    public class CollectQuest : BaseQuest
    {
        [SerializeField]
        public int objectCount;

        [SerializeField]
        public GameObject objectToCollect;
    }
}