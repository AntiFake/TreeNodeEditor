using UnityEngine;
using UnityEditor;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ExpertSystemEditor
{
    /// <summary>
    /// Класс, описывающий ЭС.
    /// </summary>
    public class ExpertSystem : ScriptableObject
    {
        [SerializeField]
        public int counter;

        [SerializeField]
        public List<Node> nodes;

        [SerializeField]
        public List<Link> links;

        [SerializeField]
        public List<Rule> rules;

        public ExpertSystem()
        {
            nodes = new List<Node>()
            {
                new Node()
                {
                    id = 0,
                    nodeRect = new Rect(100, 100, 300, 150),
                    nodeType = NodeType.Exploration,
                    description = "В зависимости от того сколько процентов карты исследовано будет приниматься дальнейшее решение по передвижению робота.",
                    question = "Сколько % карты исследовано?",
                    allowableExploration = 100
                },
                new Node()
                {
                    id = 2,
                    nodeRect = new Rect(100, 100, 300, 150),
                    nodeType = NodeType.Charge,
                    description = "Робот полностью исследовал карту. В зависимости от уровня заряда его аккумулятора робот примет решение: стоит ли ему вернуться в исходную точку или просто передать данные и ожидать эвакуации.",
                    question = "Сколько заряда осталось?",
                    allowableCharge = 10
                },
                new Node()
                {
                    id = 3,
                    nodeRect = new Rect(100, 100, 300, 150),
                    nodeType = NodeType.Result,
                    description = "Уровень заряда позволяет роботу осуществить передачу данных и постараться вернуться в исходную точку.",
                    result = "Передать данные и вернуться в ИТ."
                },
                new Node()
                {
                    id = 4,
                    nodeRect = new Rect(100, 100, 300, 150),
                    nodeType = NodeType.Result,
                    description = "Уровень заряда позволяет роботу только произвести передачу данных и ожидать эвакуации.",
                    result = "Передать данные и ожидать эвакуации."
                },
                new Node()
                {
                    id = 1,
                    nodeRect = new Rect(100, 100, 300, 150),
                    nodeType = NodeType.Charge,
                    description = "Робот не полностью исследовал карту. В зависимости от уровня заряда его аккумулятора робот примет решение: стоит ли ему продолжить исследование или же вернуться в исходную точку.",
                    question = "Сколько заряда осталось?",
                    allowableCharge = 30
                },
                new Node()
                {
                    id = 5,
                    nodeRect = new Rect(100, 100, 300, 150),
                    nodeType = NodeType.Result,
                    description = "Уровень заряда аккумулятора низкий, поэтому лучше вернуться в исходную точку, предварительно передав известный данные о карте.",
                    result = "Передать данные и вернуться в исходную точку."
                },
                new Node()
                {
                    id = 6,
                    nodeRect = new Rect(100, 100, 300, 150),
                    nodeType = NodeType.Passability,
                    description = "Заряда аккумулятора достаточно, чтобы продолжить сканировать область и перемещаться по неизвестным учаскам карты.",
                    question = "вляется ли пригодным данный участок местности для перемещения?"
                },
                new Node()
                {
                    id = 7,
                    nodeRect = new Rect(100, 100, 300, 150),
                    nodeType = NodeType.Result,
                    description = "Местность не является пригодной для перемещения.",
                    result = "Проложить маршрут в обход заданной области."
                },
                new Node()
                {
                    id = 8,
                    nodeRect = new Rect(100, 100, 300, 150),
                    nodeType = NodeType.Slope,
                    description = "Робот может пройти через данный участок местности.",
                    question = "Допустим ли угол наклона местности?"
                },
                new Node()
                {
                    id = 9,
                    nodeRect = new Rect(100, 100, 300, 150),
                    nodeType = NodeType.Result,
                    description = "Угол наклона местности слишком большой. Робот не может пересечь ее.",
                    result = "Проложить маршрут в обход заданной области."
                },
                new Node()
                {
                    id = 10,
                    nodeRect = new Rect(100, 100, 300, 150),
                    nodeType = NodeType.Result,
                    description = "Допустимый угол наклона местности. Робот может проехать.",
                    result = "Продолжить движение по этой местности."
                }
            };
            links = new List<Link>()
            {
                new Link()
                {
                    IsTrue = false,
                    nodeFromId = 0,
                    nodeToId = 1
                },
                new Link()
                {
                    IsTrue = true,
                    nodeFromId = 0,
                    nodeToId = 2
                },
                new Link()
                {
                    IsTrue = true,
                    nodeFromId = 2,
                    nodeToId = 3
                },
                new Link()
                {
                    IsTrue = false,
                    nodeFromId = 2,
                    nodeToId = 4
                },
                new Link()
                {
                    IsTrue = false,
                    nodeFromId = 1,
                    nodeToId = 5
                },
                new Link()
                {
                    IsTrue = true,
                    nodeFromId = 1,
                    nodeToId = 6
                },
                new Link()
                {
                    IsTrue = false,
                    nodeFromId = 6,
                    nodeToId = 7
                },
                new Link()
                {
                    IsTrue = true,
                    nodeFromId = 6,
                    nodeToId = 8
                },
                new Link()
                {
                    IsTrue = false,
                    nodeFromId = 8,
                    nodeToId = 9
                },
                new Link()
                {
                    IsTrue = true,
                    nodeFromId = 8,
                    nodeToId = 10
                },
            };

            rules = new List<Rule>()
            {
                new Rule()
                {
                    args = new List<int>() { 1, 6},
                    result = "Исследовано меньше 100% карты и достаточно заряда, чтобы продолжить разведку."
                },
                new Rule()
                {
                    args = new List<int>() { 2 },
                    result = "Карты исследована в полном объеме."
                },
                new Rule()
                {
                    args = new List<int>() { 6, 8},
                    result = "Осталось проверить угол склона местности, чтобы продолжить движение."
                }
            };

            if (nodes.Count > 0)
                counter = nodes.Select(i => i.id).Max() + 1;
            else
                counter = 0;
        }

        public void OnEnable() { hideFlags = HideFlags.HideAndDontSave; }
    }

    /// <summary>
    /// Класс, описывающий дугу графа ЭС.
    /// </summary>
    [Serializable]
    public class Link
    {
        [SerializeField]
        public int nodeFromId;

        [SerializeField]
        public int nodeToId;

        [SerializeField]
        public bool IsTrue;
    }

    /// <summary>
    /// Тип участка карты.
    /// </summary>
    public enum PolyType
    {
        Ground,
        Water,
        Obstacle
    }

    /// <summary>
    /// Тип вопроса ноды.
    /// </summary>
    public enum NodeType
    {
        Charge,
        Exploration,
        Slope,
        Passability,
        Result
    }

    /// <summary>
    /// Класс, описывающий ноду графа ЭС.
    /// </summary>
    [Serializable]
    public class Node
    {
        [SerializeField]
        public int id;

        [SerializeField]
        public Rect nodeRect;

        [SerializeField]
        public string question;

        [SerializeField]
        public string description;

        [SerializeField]
        public string result;

        [SerializeField]
        public NodeType nodeType;

        [SerializeField]
        public float allowableCharge;

        [SerializeField]
        public float allowableExploration;

        public Node() { }

        public void DrawNodeWindow()
        {
            nodeType = (NodeType)EditorGUILayout.EnumPopup("Тип вопроса:", nodeType);
            description = EditorGUILayout.TextField("Описание:", description);

            if (nodeType != NodeType.Result)
            {
                question = EditorGUILayout.TextField("Вопрос:", question);
            }
            else
                result = EditorGUILayout.TextField("Результат:", result);

            if (nodeType == NodeType.Charge)
            {
                EditorGUILayout.LabelField("Допустимый заряд:");
                allowableCharge = EditorGUILayout.Slider(allowableCharge, 0f, 100f);
            }
            if (nodeType == NodeType.Exploration)
            {
                EditorGUILayout.LabelField("Уровень исследования:");
                allowableExploration = EditorGUILayout.Slider(allowableExploration, 0f, 100f);
            }
        }
    }

    /// <summary>
    /// Класс, описывающий элемент таблицы пояснения.
    /// </summary>
    public class DescriptionItem
    {
        public int number;
        public string description;
    }

    public class Argument
    {
        public string desciption;
        public int number;
    }

    /// <summary>
    /// Класс, описывающий правило.
    /// </summary>
    [Serializable]
    public class Rule
    {
        [SerializeField]
        public List<int> args;

        [SerializeField]
        public string result;
    }
}