using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;
using System;

namespace QuestManagerEditor
{
    public class QuestEditor : EditorWindow
    {
        public class DeleteNodeAction
        {
            public string nodeGuid;
        }

        private enum QuestType { CollectEvent, KillEvent }

        private const float windowWidth = 250f;
        private const float windowHeight = 150f;

        public static QuestEditor questEditor;
        public static GameObject obj;

        private List<BaseQuestNode> questNodes = new List<BaseQuestNode>();
        private List<Link> questLinks = new List<Link>();
        private List<string> nodesToJoin = new List<string>();
        private List<string> nodesToTear = new List<string>();

        private Vector2 mousePos;

        [MenuItem("Tools/Quest editor")]
        private static void ShowEditor()
        {
            questEditor = EditorWindow.GetWindow<QuestEditor>();
            questEditor.titleContent = new GUIContent("Редактор квестов");
        }

        private void OnGUI()
        {
            Event e = Event.current;
            mousePos = e.mousePosition;

            // Создаем 
            if (questEditor == null)
                ShowEditor();

            //Отображение контекстного меню.
            if (e.button == 1)
            {
                nodesToJoin.Clear();
                nodesToTear.Clear();

                if (e.type == EventType.MouseDown)
                {
                    bool clickedOnWindow = false;
                    string clickedWindowGuid = null;

                    for (int i = 0; i < questNodes.Count; i++)
                    {
                        if (questNodes[i].nodeRect.Contains(mousePos))
                        {
                            clickedWindowGuid = questNodes[i].guid;
                            clickedOnWindow = true;
                            break;
                        }
                    }

                    // Отображение контекстного меню для создания нод (Клик произведен не по ноде).
                    if (!clickedOnWindow)
                    {
                        GenericMenu menuToAddQuest = new GenericMenu();

                        menuToAddQuest.AddItem(new GUIContent("Сбор предметов"), false, ContextQuestAddCallback, QuestType.CollectEvent);
                        menuToAddQuest.AddSeparator("");
                        menuToAddQuest.AddItem(new GUIContent("Убийство мобов"), false, ContextQuestAddCallback, QuestType.KillEvent);

                        menuToAddQuest.ShowAsContext();
                        e.Use();
                    }
                    // Отображение контекстного меню управления нодой.
                    else
                    {
                        GenericMenu menuToControlQuest = new GenericMenu();

                        menuToControlQuest.AddItem(new GUIContent("Удалить квест"), false, ContextQuestControlCallback, new DeleteNodeAction() { nodeGuid = clickedWindowGuid });

                        menuToControlQuest.ShowAsContext();
                        e.Use();
                    }
                }
            }

            // Соединение ребрами нод графа.
            if (e.button == 0 && e.shift)
            {
                nodesToTear.Clear();

                if (e.type == EventType.MouseDown)
                {
                    bool clickedOnWindow = false;
                    string clickedWindowGuid = null;

                    for (int i = 0; i < questNodes.Count; i++)
                    {
                        if (questNodes[i].nodeRect.Contains(mousePos))
                        {
                            clickedWindowGuid = questNodes[i].guid;
                            clickedOnWindow = true;
                            break;
                        }
                    }

                    if (clickedOnWindow)
                    {
                        if (nodesToJoin.Count < 2)
                            nodesToJoin.Add(clickedWindowGuid);

                        if (nodesToJoin.Count == 2)
                        {
                            // Если не одна и та же нода.
                            if (nodesToJoin[0] != nodesToJoin[1])
                            {
                                bool exist = questLinks.FirstOrDefault(i => i.nodeFrom.guid == nodesToJoin[0] && i.nodeTo.guid == nodesToJoin[1]) != null ? true : false;
                                bool existReversed = questLinks.FirstOrDefault(i => i.nodeFrom.guid == nodesToJoin[1] && i.nodeTo.guid == nodesToJoin[0]) != null ? true : false;
                                if (!exist && !existReversed)
                                {
                                    questLinks.Add(new Link()
                                    {
                                        nodeFrom = questNodes.First(i => i.guid == nodesToJoin[0]),
                                        nodeTo = questNodes.First(i => i.guid == nodesToJoin[1])
                                    });
                                }
                            }
                            nodesToJoin.Clear();
                        }
                    }
                    else
                        nodesToJoin.Clear();
                }
            }

            // Удаление ребра между нодами графа (@copyright "если конечно они у вас есть").
            if (e.button == 0 && e.alt)
            {
                nodesToJoin.Clear();

                if (e.type == EventType.MouseDown)
                {
                    bool clickedOnWindow = false;
                    string clickedWindowGuid = null;

                    for (int i = 0; i < questNodes.Count; i++)
                    {
                        if (questNodes[i].nodeRect.Contains(mousePos))
                        {
                            clickedWindowGuid = questNodes[i].guid;
                            clickedOnWindow = true;
                            break;
                        }
                    }

                    if (clickedOnWindow)
                    {
                        if (nodesToTear.Count < 2)
                            nodesToTear.Add(clickedWindowGuid);

                        if (nodesToTear.Count == 2)
                        {
                            Link link = questLinks.FirstOrDefault(i => i.nodeFrom.guid == nodesToTear[0] && i.nodeTo.guid == nodesToTear[1]);
                            Link linkReversed = questLinks.FirstOrDefault(i => i.nodeFrom.guid == nodesToTear[1] && i.nodeTo.guid == nodesToTear[0]);

                            if (link != null)
                                questLinks.Remove(link);
                            else if (linkReversed != null)
                                questLinks.Remove(linkReversed);

                            nodesToTear.Clear();
                        }
                    }
                    else
                        nodesToTear.Clear();
                }
            }

            // Отображение ребер графа.
            if (questLinks != null)
            {
                foreach (var link in questLinks)
                {
                    link.DrawLink();
                }
            }

            // Отображение окон нод.
            BeginWindows();
            if (questNodes != null)
            {
                for (int i = 0; i < questNodes.Count; i++)
                {
                    questNodes[i].nodeRect = GUI.Window(i, questNodes[i].nodeRect, DrawNodeWindow, questNodes[i].title);
                }
            }
            EndWindows();

            // Сохранение данных в QuestManager-компонент.
            if (Selection.activeGameObject != null)
            {
                obj = Selection.activeGameObject;
                QuestManager component = obj.GetComponent<QuestManager>();

                if (component != null)
                {
                    if (GUI.Button(new Rect(position.width - 120, position.height - 40, 100, 30), new GUIContent("Сохранить")))
                    {
                        if (questLinks != null && questLinks.Any())
                            component.questTree = Save();
                    }
                }
            }

            //if (Selection.activeGameObject != null)
            //{
            //    obj = Selection.activeGameObject;
            //    EditorGUILayout.LabelField("Current selected object: " + obj.name);

            //    QuestManager comp = obj.GetComponent<QuestManager>();
            //    if (comp != null)
            //    {
            //        comp.health = EditorGUILayout.IntField(comp.health);
            //        comp.username = EditorGUILayout.TextField(comp.username);
            //        comp.cam = (GameObject) EditorGUILayout.ObjectField("Camera", comp.cam, typeof(GameObject), true);
            //    }
            //}
        }

        private void Update()
        {
            Repaint();
        }

        /// <summary>
        /// Отображение окна ноды с указанным id.
        /// </summary>
        /// <param name="id"></param>
        private void DrawNodeWindow(int id)
        {
            questNodes[id].DrawNodeWindow();
            GUI.DragWindow();
        }

        private void ContextQuestAddCallback(object questType)
        {
            switch ((QuestType)questType)
            {
                case QuestType.CollectEvent:
                    questNodes.Add(new CollectQuestNode()
                    {
                        nodeRect = new Rect(mousePos.x, mousePos.y, windowWidth, windowHeight)
                    });
                    break;
                case QuestType.KillEvent:
                    questNodes.Add(new KillQuestNode()
                    {
                        nodeRect = new Rect(mousePos.x, mousePos.y, windowWidth, windowHeight)
                    });
                    break;
            }
        }

        private void ContextQuestControlCallback(object controlActionType)
        {
            DeleteNodeAction deleteAction = controlActionType as DeleteNodeAction;
            if (deleteAction != null)
            {
                DeleteNodeLinks(deleteAction.nodeGuid);
                questNodes = questNodes.Where(i => i.guid != deleteAction.nodeGuid).ToList();
            }
        }

        /// <summary>
        /// Удаление ребер, исходящих/входящих в удаляемую ноду.
        /// </summary>
        /// <param name="nodeGuid"></param>
        private void DeleteNodeLinks(string nodeGuid)
        {
            questLinks = questLinks.Where(i => i.nodeFrom.guid != nodeGuid && i.nodeTo.guid != nodeGuid).ToList();
        }

        private List<TreeLink> Save()
        {
            List<TreeLink> treeLinks = new List<TreeLink>();

            foreach (var link in questLinks)
            {
                TreeLink l = new TreeLink()
                {
                    questFrom = GetTreeQuest(link.nodeFrom),
                    questTo = GetTreeQuest(link.nodeTo),
                };
                treeLinks.Add(l);
            }

            return treeLinks;
        }

        private BaseQuest GetTreeQuest(BaseQuestNode node)
        {
            BaseQuest quest = null;

            if (node is CollectQuestNode)
            {
                quest = new CollectQuest()
                {
                    questIssuer = (node as CollectQuestNode).quest.questIssuer,
                    questAcceptor = (node as CollectQuestNode).quest.questAcceptor,
                    questName = (node as CollectQuestNode).quest.questName,
                    questDescription = (node as CollectQuestNode).quest.questDescription,
                    experienceAmount = (node as CollectQuestNode).quest.experienceAmount,
                    objectCount = ((node as CollectQuestNode).quest as CollectQuest).objectCount,
                    objectToCollect = ((node as CollectQuestNode).quest as CollectQuest).objectToCollect
                };
            }

            if (node is KillQuestNode)
            {
                quest = new KillQuest()
                {
                    questIssuer = (node as KillQuestNode).quest.questIssuer,
                    questAcceptor = (node as KillQuestNode).quest.questAcceptor,
                    questName = (node as KillQuestNode).quest.questName,
                    questDescription = (node as KillQuestNode).quest.questDescription,
                    experienceAmount = (node as KillQuestNode).quest.experienceAmount,
                    objectCount = ((node as CollectQuestNode).quest as KillQuest).objectCount,
                    objectToKill = ((node as CollectQuestNode).quest as KillQuest).objectToKill
                };
            }

            return quest;
        }
    }
}