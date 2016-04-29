using UnityEngine;
using System.Collections.Generic;
using ExpertSystemEditor;
using System;

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

	private void Start ()
    {
        expertSystemManager = GetComponent<ExpertSystemManager>();
        string startNodeId = expertSystemManager.GetStartNodeId();
        List<string> solution = new List<string>();
        expertSystemManager.GetSolution(this, ref solution, startNodeId);
	}
	
	private void Update ()
    {
	
	}
}
