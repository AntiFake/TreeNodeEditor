using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;
using System;

namespace ExpertSystemEditor
{
    public class ExpertSystemEditor : EditorWindow
    {
        public class DeleteNodeAction
        {
            public string nodeGuid;
        }

        private const float windowWidth = 300f;
        private const float windowHeight = 100f;

        public static ExpertSystemEditor questEditor;
        public ExpertSystem data;

        private List<string> nodesToJoin = new List<string>();
        private List<string> nodesToTear = new List<string>();

        private Vector2 mousePos;

        [MenuItem("Tools/ExpertSystem editor")]
        private static void ShowEditor()
        {
            questEditor = GetWindow<ExpertSystemEditor>();
            questEditor.titleContent = new GUIContent("Экспертная система");
        }

        private void OnEnable()
        {
            if (data == null)
                data = CreateInstance<ExpertSystem>();
            hideFlags = HideFlags.HideAndDontSave;
        }

        private void Update()
        {
            Repaint();
        }

        private void OnGUI()
        {
            Event e = Event.current;
            mousePos = e.mousePosition;

            // Создаем 
            if (questEditor == null)
                ShowEditor();

            //Отображение контекстного меню.
            if (e.button == 1 && !e.shift)
            {
                nodesToJoin.Clear();
                nodesToTear.Clear();

                if (e.type == EventType.MouseDown)
                {
                    bool clickedOnWindow = false;
                    string clickedWindowGuid = null;

                    for (int i = 0; i < data.nodes.Count; i++)
                    {
                        if (data.nodes[i].nodeRect.Contains(mousePos))
                        {
                            clickedWindowGuid = data.nodes[i].guid;
                            clickedOnWindow = true;
                            break;
                        }
                    }

                    // Отображение контекстного меню для создания нод (Клик произведен не по ноде).
                    if (!clickedOnWindow)
                    {
                        GenericMenu menuToAddQuest = new GenericMenu();

                        menuToAddQuest.AddItem(new GUIContent("Добавить ноду"), false, ContextNodeAddCallback, null);

                        menuToAddQuest.ShowAsContext();
                        e.Use();
                    }
                    // Отображение контекстного меню управления нодой.
                    else
                    {
                        GenericMenu menuToControlQuest = new GenericMenu();

                        menuToControlQuest.AddItem(new GUIContent("Удалить ноду"), false, ContextNodeControlCallback, new DeleteNodeAction() { nodeGuid = clickedWindowGuid });

                        menuToControlQuest.ShowAsContext();
                        e.Use();
                    }
                }
            }

            // Соединение ребрами нод графа (зеленый - ДА, красный - НЕТ).
            if ((e.button == 0 || e.button == 1) && e.shift)
            {
                nodesToTear.Clear();

                if (e.type == EventType.MouseDown)
                {
                    bool clickedOnWindow = false;
                    string clickedWindowGuid = null;

                    for (int i = 0; i < data.nodes.Count; i++)
                    {
                        if (data.nodes[i].nodeRect.Contains(mousePos))
                        {
                            clickedWindowGuid = data.nodes[i].guid;
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
                                bool exist = data.links.FirstOrDefault(i => i.nodeFromGuid == nodesToJoin[0] && i.nodeToGuid == nodesToJoin[1]) != null ? true : false;
                                bool existReversed = data.links.FirstOrDefault(i => i.nodeFromGuid == nodesToJoin[1] && i.nodeToGuid == nodesToJoin[0]) != null ? true : false;
                                if (!exist && !existReversed)
                                {
                                    data.links.Add(new Link()
                                    {
                                        nodeFromGuid = data.nodes.First(i => i.guid == nodesToJoin[0]).guid,
                                        nodeToGuid = data.nodes.First(i => i.guid == nodesToJoin[1]).guid,
                                        IsTrue = e.button == 0 ? true : false
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

                    for (int i = 0; i < data.nodes.Count; i++)
                    {
                        if (data.nodes[i].nodeRect.Contains(mousePos))
                        {
                            clickedWindowGuid = data.nodes[i].guid;
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
                            Link link = data.links.FirstOrDefault(i => i.nodeFromGuid == nodesToTear[0] && i.nodeToGuid == nodesToTear[1]);
                            Link linkReversed = data.links.FirstOrDefault(i => i.nodeFromGuid == nodesToTear[1] && i.nodeToGuid == nodesToTear[0]);

                            if (link != null)
                                data.links.Remove(link);
                            else if (linkReversed != null)
                                data.links.Remove(linkReversed);

                            nodesToTear.Clear();
                        }
                    }
                    else
                        nodesToTear.Clear();
                }
            }

            DrawEditor();
        }

        #region Контекстное меню
        /// <summary>
        /// Добавление ноды.
        /// </summary>
        /// <param name="obj">Object.</param>
        private void ContextNodeAddCallback(object obj)
        {
            data.nodes.Add(new Node()
            {
                nodeRect = new Rect(mousePos.x, mousePos.y, windowWidth, windowHeight),
                number = data.counter,
            });
            data.counter++;
        }

        /// <summary>
        /// Контекстное меню ноды.
        /// </summary>
        /// <param name="controlActionType">Control action type.</param>
        private void ContextNodeControlCallback(object controlActionType)
        {
            DeleteNodeAction deleteAction = controlActionType as DeleteNodeAction;
            if (deleteAction != null)
            {
                DeleteNodeLinks(deleteAction.nodeGuid);
                data.nodes = data.nodes.Where(i => i.guid != deleteAction.nodeGuid).ToList();
            }
        }

        /// <summary>
        /// Удаление ребер, исходящих/входящих в удаляемую ноду.
        /// </summary>
        /// <param name="nodeGuid"></param>
        private void DeleteNodeLinks(string nodeGuid)
        {
            data.links = data.links.Where(i => i.nodeFromGuid != nodeGuid && i.nodeToGuid != nodeGuid).ToList();
        }

        #endregion

        #region Отображение
        /// <summary>
        /// Отображенение основных компонентов редактора
        /// </summary>
        private void DrawEditor()
        {
            EditorGUILayout.LabelField("Число нод: " + data.nodes.Count.ToString());
            EditorGUILayout.LabelField("Число линков: " + data.nodes.Count.ToString());
            EditorGUILayout.LabelField("Счетчик нод: " + data.counter.ToString());

            // -------------- Отображение дерева. ------------------
            GUI.BeginGroup(new Rect(0, 0, position.width, position.height));

            // -------------- Кнопки управления. -------------------
            // Сохранение данных.
            if (GUI.Button(new Rect(5, 60, 50, 30), "Save"))
            {
                if (Selection.activeGameObject != null)
                {
                    ExpertSystemManager component = Selection.activeGameObject.GetComponent<ExpertSystemManager>();
                    if (component != null)
                    {
                        if (data.links != null && data.links.Any())
                        {
                            component.nodes = data.nodes;
                            component.links = data.links;
                        }
                    }
                }
            }
            // ------------------------------------------------------

            // -------------- Отображение ребер графа. --------------
            foreach (var link in data.links)
            {
                DrawLink(link);
            }
            // ------------------------------------------------------

            // -------------- Отображение окон нод. -----------------
            BeginWindows();
            for (int i = 0; i < data.nodes.Count; i++)
            {
                data.nodes[i].nodeRect = GUI.Window(i, data.nodes[i].nodeRect, DrawNodeWindow, data.nodes[i].number.ToString());
            }
            EndWindows();
            // ------------------------------------------------------

            GUI.EndGroup();
        }

        /// <summary>
        /// Отображение ребра ор. графа.
        /// </summary>
        /// <param name="nodeFrom">Node from.</param>
        /// <param name="nodeTo">Node to.</param>
        private void DrawLink(Link link)
        {
            Rect rectFrom = data.nodes.First(i => i.guid == link.nodeFromGuid).nodeRect;
            Rect rectTo = data.nodes.First(i => i.guid == link.nodeToGuid).nodeRect;

            Vector3 startPos = new Vector3(rectFrom.x + rectFrom.width, rectFrom.y + rectFrom.height / 2, 0);
            Vector3 endPos = new Vector3(rectTo.x, rectTo.y + rectTo.height / 2, 0);
            Vector3 startTan = startPos + Vector3.right * 50;
            Vector3 endTan = endPos + Vector3.left * 50;

            // Тень.
            //Color shadowCol = new Color(0, 0, 0, 0.06f);
            //for (int i = 0; i < 3; i++)
            //{
            //    Handles.DrawBezier(startPos, endPos, startTan, endTan, shadowCol, null, (i + 1) * 5);
            //}

            if (link.IsTrue)
                Handles.DrawBezier(startPos, endPos, startTan, endTan, new Color(0f, 0.5f, 0f, 1f), null, 3);
            else
                Handles.DrawBezier(startPos, endPos, startTan, endTan, new Color(0.8f, 0f, 0f, 1f), null, 3);
        }

        /// <summary>
        /// Отображение окна ноды с указанным id.
        /// </summary>
        /// <param name="id"></param>
        private void DrawNodeWindow(int id)
        {
            data.nodes[id].DrawNodeWindow();
            GUI.DragWindow();
        }
        #endregion
    }
}