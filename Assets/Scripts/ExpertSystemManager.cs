using UnityEngine;
using System.Linq;
using System.Collections.Generic;

namespace ExpertSystemEditor
{
    public class ExpertSystemManager : MonoBehaviour
    {
        public List<Node> nodes;
        public List<Link> links;

        public string GetStartNodeId()
        {
            var min = nodes.Select(i => i.number).Min();
            return nodes.First(i => i.number == min).id;
        }

        public string GetRandomResultNodeId()
        {
            var nds = nodes.Where(i => i.nodeType == NodeType.Result).ToArray();
            return nds[Random.Range(0, nds.Length - 1)].id;
        }
        
        /// <summary>
        /// Получить решение.
        /// </summary>
        /// <param name="nodeId"></param>
        /// <param name="iteration"></param>
        public void GetSolution(Bot bot, ref List<string> solution, string nodeId, int iteration = 0)
        {
            // Решение найдено!
            if (string.IsNullOrEmpty(nodeId))
                return;

            string nextNodeId = string.Empty;
            var node = nodes.First(i => i.id == nodeId);
            var lnks = links.Where(i => i.nodeFromGuid == node.id);

            switch (node.nodeType)
            {
                case NodeType.Exploration:
                    {
                        // Условие выполняется.
                        if (bot.explorationLevelPercent >= node.allowableExploration)
                            nextNodeId = lnks.First(i => i.IsTrue).nodeToGuid;
                        // Условие не выполняется.
                        else
                            nextNodeId = lnks.First(i => !i.IsTrue).nodeToGuid;

                        solution.Add(node.id);
                        break;
                    }
                case NodeType.Charge:
                    {
                        // Допустимый уровень заряда.
                        if (bot.chargeLevelPercent >= node.allowableCharge)
                            nextNodeId = lnks.First(i => i.IsTrue).nodeToGuid;
                        else
                            nextNodeId = lnks.First(i => !i.IsTrue).nodeToGuid;

                        solution.Add(node.id);
                        break;
                    }
                case NodeType.Passability:
                    {
                        // Проходимый участок карты.
                        if (bot.settings.passability.Contains(bot.nextPoly.type))
                            nextNodeId = lnks.First(i => i.IsTrue).nodeToGuid;
                        else
                            nextNodeId = lnks.First(i => !i.IsTrue).nodeToGuid;

                        solution.Add(node.id);
                        break;
                    }
                case NodeType.Slope:
                    {
                        // Проходимый угол.
                        if (bot.settings.maxSlopeAngle <= bot.nextPoly.angle)
                            nextNodeId = lnks.First(i => i.IsTrue).nodeToGuid;
                        else
                            nextNodeId = lnks.First(i => !i.IsTrue).nodeToGuid;

                        solution.Add(node.description);
                        break;
                    }
                case NodeType.Result:
                    {
                        nextNodeId = string.Empty;
                        solution.Add(node.id);
                        break;
                    }
            }
            iteration++;
            GetSolution(bot, ref solution, nextNodeId, iteration);
        }

        /// <summary>
        /// Обратный решатель.
        /// </summary>
        /// <param name="solution"></param>
        /// <param name="nodeId"></param>
        public void GetReverseSolution(ref List<string> solution, string nodeId)
        {
            if (string.IsNullOrEmpty(nodeId))
                return;

            // ...
        }
    }
}