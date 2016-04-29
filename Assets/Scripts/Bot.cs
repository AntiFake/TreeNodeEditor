using UnityEngine;
using System.Collections.Generic;
using ExpertSystemEditor;
using System;
using System.Linq;

public class Bot : MonoBehaviour {
    [Serializable]
    public class Poly
    {
        [SerializeField]
        public PolyType type;
        [SerializeField]
        public float angle;
    }

    [Serializable]
    public class BotSettings
    {
        [SerializeField]
        public float maxSlopeAngle;
        [SerializeField]
        public PolyType[] passability;
    }

    // Настройки бота.
    public BotSettings settings;
    // Участок карты для исследования.
    public Poly nextPoly;
    // Процент исследования карты.
    public float explorationLevelPercent;
    // Процент 
    public float chargeLevelPercent;

    // Компонент системы принятия решения.
    private ExpertSystemManager expertSystemManager;

    private string solution;
    private string reversedSolution;
    private List<DescriptionItem> table;
    private int? h;
    private int? rh;


	private void Awake()
    {
        expertSystemManager = GetComponent<ExpertSystemManager>();
        int startNodeId = expertSystemManager.GetStartNodeId();

        List<int> sol = new List<int>();
        expertSystemManager.GetSolution(this, ref sol, startNodeId);
        h = expertSystemManager.CheckSolution(sol);
        solution = expertSystemManager.GetSolutionString(sol);

        List<int> rSol = new List<int>();
        expertSystemManager.GetReversedSolution(ref rSol, expertSystemManager.GetRandomResultNodeId());
        rh = expertSystemManager.CheckSolution(rSol);
        reversedSolution = expertSystemManager.GetSolutionString(rSol);

        // Таблица объяснений.
        table = expertSystemManager.GetDescriptionTable();
	}

    private void OnGUI()
    {
        GUIStyle style = new GUIStyle();
        style.normal.textColor = Color.white;

        GUILayout.Label("Правила", style);
        foreach (var i in expertSystemManager.rules)
        {
            string expression = string.Empty;
            for (int j = 0; j < i.args.Count; j++)
            {
                if (j != i.args.Count - 1)
                    expression += i.args[j] + " и ";
                else
                    expression += i.args[j];
            }

            GUILayout.Label(string.Format("{0} -----> {1}", expression, i.result), style);
        }

        GUILayout.Label("\n");

        GUILayout.Label("Таблица объяснений", style);
        foreach (var i in table)
        {
            GUILayout.Label(string.Format("{0} --------------- {1}", i.number, i.description), style);
        }

        GUILayout.Label("\n");

        GUILayout.Label("Решатель");
        GUILayout.Label(solution);
        GUILayout.Label("Правило");
        GUILayout.Label(h.HasValue ? h.Value.ToString() : "Нет соответствия");

        GUILayout.Label("\n");

        GUILayout.Label("Обратный решатель");
        GUILayout.Label(reversedSolution);
        GUILayout.Label("Правило");
        GUILayout.Label(rh.HasValue ? rh.Value.ToString() : "Нет соответствия");
    }
}
