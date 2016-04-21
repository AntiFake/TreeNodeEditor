using UnityEngine;
using UnityEditor;
using System;

namespace QuestManagerEditor
{
    public abstract class BaseQuestNode : MonoBehaviour
    {
        public Guid guid;

        public Rect nodeRect;
        public GameObject questIssuer;
        public GameObject questAcceptor;
        public string title = "";
        public string questName = "";
        public string questDescription = "";

        public BaseQuestNode(string t)
        {
            guid = Guid.NewGuid();
            title = t;
        }

        public virtual void DrawNodeWindow()
        {
            questName = EditorGUILayout.TextField("Название квеста:", questName);
            questDescription = EditorGUILayout.TextField("Описание квеста:", questDescription);
            questIssuer = (GameObject)EditorGUILayout.ObjectField("Дающий квест: ", questIssuer, typeof(GameObject), true);
            questAcceptor = (GameObject)EditorGUILayout.ObjectField("Принимающий квест: ", questAcceptor, typeof(GameObject), true);
        }
    }

    public class CollectQuestNode : BaseQuestNode
    {
        public int objectCount;
        public GameObject objectToCollect;
        
        public CollectQuestNode() : base("Сбор предметов") { }

        public override void DrawNodeWindow()
        {
            base.DrawNodeWindow();
            objectCount = EditorGUILayout.IntField("Число объектов: ", objectCount);
            objectToCollect = (GameObject)EditorGUILayout.ObjectField("Объект для сбора: ", objectToCollect, typeof(GameObject), true);
        }
    }

    public class KillQuestNode : BaseQuestNode
    {
        public int objectCount;
        public GameObject objectToKill;

        public KillQuestNode() : base("Убийство мобов") { }

        public override void DrawNodeWindow()
        {
            base.DrawNodeWindow();
            objectCount = EditorGUILayout.IntField("Число мобов: ", objectCount);
            objectToKill = (GameObject)EditorGUILayout.ObjectField("Тип моба: ", objectToKill, typeof(GameObject), true);
        }
    }


    public class Link
    {
        public BaseQuestNode nodeFrom;
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
}