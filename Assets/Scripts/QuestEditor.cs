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

        private const float windowWidth = 250f;
        private const float windowHeight = 200f;

        public static QuestEditor questEditor;
		public ComponentData data;

        private List<string> nodesToJoin = new List<string>();
        private List<string> nodesToTear = new List<string>();

        private Vector2 mousePos;

        [MenuItem("Tools/Quest editor")]
        private static void ShowEditor()
        {
            questEditor = GetWindow<QuestEditor>();
            questEditor.titleContent = new GUIContent("Редактор квестов");
        }

        private void OnEnable()
        {
            if (data == null)
                data = CreateInstance<ComponentData>();
			hideFlags = HideFlags.HideAndDontSave;
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

                    for (int i = 0; i < data.questNodes.Count; i++)
                    {
                        if (data.questNodes[i].nodeRect.Contains(mousePos))
                        {
                            clickedWindowGuid = data.questNodes[i].guid;
                            clickedOnWindow = true;
                            break;
                        }
                    }

                    // Отображение контекстного меню для создания нод (Клик произведен не по ноде).
                    if (!clickedOnWindow)
                    {
                        GenericMenu menuToAddQuest = new GenericMenu();

                        menuToAddQuest.AddItem(new GUIContent("Добавить квест"), false, ContextQuestAddCallback, null);

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

                    for (int i = 0; i < data.questNodes.Count; i++)
                    {
                        if (data.questNodes[i].nodeRect.Contains(mousePos))
                        {
                            clickedWindowGuid = data.questNodes[i].guid;
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
                                bool exist = data.questLinks.FirstOrDefault(i => i.nodeFromGuid == nodesToJoin[0] && i.nodeToGuid == nodesToJoin[1]) != null ? true : false;
                                bool existReversed = data.questLinks.FirstOrDefault(i => i.nodeFromGuid == nodesToJoin[1] && i.nodeToGuid == nodesToJoin[0]) != null ? true : false;
                                if (!exist && !existReversed)
                                {
                                    data.questLinks.Add(new Link()
                                    {
										nodeFromGuid = data.questNodes.First(i => i.guid == nodesToJoin[0]).guid,
										nodeToGuid = data.questNodes.First(i => i.guid == nodesToJoin[1]).guid
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

                    for (int i = 0; i < data.questNodes.Count; i++)
                    {
						if (data.questNodes[i].nodeRect.Contains(mousePos))
                        {
                            clickedWindowGuid = data.questNodes[i].guid;
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
                            Link link = data.questLinks.FirstOrDefault(i => i.nodeFromGuid == nodesToTear[0] && i.nodeToGuid == nodesToTear[1]);
                            Link linkReversed = data.questLinks.FirstOrDefault(i => i.nodeFromGuid == nodesToTear[1] && i.nodeToGuid == nodesToTear[0]);

                            if (link != null)
                                data.questLinks.Remove(link);
                            else if (linkReversed != null)
                                data.questLinks.Remove(linkReversed);

                            nodesToTear.Clear();
                        }
                    }
                    else
                        nodesToTear.Clear();
                }
            }
            
			// Отображение дерева.
			GUI.BeginGroup (new Rect(0, 0, position.width, position.height));
            // Отображение ребер графа.
            foreach (var link in data.questLinks)
            {
				DrawLink(data.questNodes.First(i => i.guid == link.nodeFromGuid).nodeRect, data.questNodes.First(i => i.guid == link.nodeToGuid).nodeRect);
            }

            // Отображение окон нод.
            BeginWindows();
            for (int i = 0; i < data.questNodes.Count; i++)
            {
				data.questNodes[i].nodeRect = GUI.Window(i, data.questNodes[i].nodeRect, DrawNodeWindow, data.questNodes[i].title);
            }
            EndWindows();
			GUI.EndGroup ();

            EditorGUILayout.LabelField("Число нод: " + data.questNodes.Count.ToString());
            EditorGUILayout.LabelField("Число линков: " + data.questLinks.Count.ToString());
			EditorGUILayout.LabelField("Счетчик нод: " + data.counter.ToString());

            // Сохранение данных в QuestManager-компонент.
            if (Selection.activeGameObject != null)
            {
                data.obj = Selection.activeGameObject;
                QuestManager component = data.obj.GetComponent<QuestManager>();
                
                if (component != null)
                {
                    if (GUI.Button(new Rect(position.width - 120, position.height - 40, 100, 30), new GUIContent("Сохранить")))
                    {
                        if (data.questLinks != null && data.questLinks.Any())
                        {
							component.data = data;
                            component.nodes = data.questNodes;
                            component.links = data.questLinks;
							//data.obj.GetComponent<QuestManager>().data = data;
                        }
                    }
                }
            }
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
            data.questNodes[id].DrawNodeWindow();
            GUI.DragWindow();
        }

        private void ContextQuestAddCallback(object obj)
        {
            data.questNodes.Add(new QuestNode()
            {
                nodeRect = new Rect(mousePos.x, mousePos.y, windowWidth, windowHeight),
				title = data.counter.ToString()
            });
			data.counter++;
        }

        private void ContextQuestControlCallback(object controlActionType)
        {
            DeleteNodeAction deleteAction = controlActionType as DeleteNodeAction;
            if (deleteAction != null)
            {
                DeleteNodeLinks(deleteAction.nodeGuid);
                data.questNodes = data.questNodes.Where(i => i.guid != deleteAction.nodeGuid).ToList();
            }
        }

        /// <summary>
        /// Удаление ребер, исходящих/входящих в удаляемую ноду.
        /// </summary>
        /// <param name="nodeGuid"></param>
        private void DeleteNodeLinks(string nodeGuid)
        {
            data.questLinks = data.questLinks.Where(i => i.nodeFromGuid != nodeGuid && i.nodeToGuid != nodeGuid).ToList();
        }

		/// <summary>
		/// Отображение ребра ор. графа.
		/// </summary>
		/// <param name="nodeFrom">Node from.</param>
		/// <param name="nodeTo">Node to.</param>
		private void DrawLink(Rect rectFrom, Rect rectTo)
		{
			Vector3 startPos = new Vector3(rectFrom.x + rectFrom.width, rectFrom.y + rectFrom.height / 2, 0);
			Vector3 endPos = new Vector3(rectTo.x, rectTo.y + rectTo.height / 2, 0);
			Vector3 startTan = startPos + Vector3.right * 50;
			Vector3 endTan = endPos + Vector3.left * 50;

			// Тень.
			Color shadowCol = new Color(0, 0, 0, 0.06f);
			for (int i = 0; i < 3; i++) 
			{
				Handles.DrawBezier(startPos, endPos, startTan, endTan, shadowCol, null, (i + 1) * 5);
			}

			Handles.DrawBezier(startPos, endPos, startTan, endTan, Color.black, null, 1);
		}

        //private void OnEnable()
        //{
        //    EditorApplication.playmodeStateChanged += OnPlaymodeStateChanged;
        //}

        //private void OnDisable()
        //{
        //    EditorApplication.playmodeStateChanged -= OnPlaymodeStateChanged;
        //}

        //private void OnPlaymodeStateChanged()
        //{
        //    if (EditorApplication.isPlaying)
        //    {
        //        foreach (var questNodeId in data.questNodeIds)
        //        {
        //            data.questNodes.Add(EditorUtility.InstanceIDToObject(questNodeId) as QuestNode);
        //        }
        //    }
        //}
    }
}