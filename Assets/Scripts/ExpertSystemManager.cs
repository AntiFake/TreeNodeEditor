using UnityEngine;
using System.Linq;
using System.Collections.Generic;

namespace ExpertSystemEditor
{
    public class ExpertSystemManager : MonoBehaviour
    {
        public List<Node> nodes;
        public List<Link> links;
        public List<Rule> rules;

        public int GetStartNodeId()
        {
            var min = nodes.Select(i => i.id).Min();
            return nodes.First(i => i.id == min).id;
        }

        public int GetRandomResultNodeId()
        {
            var nds = nodes.Where(i => i.nodeType == NodeType.Result).ToArray();
            return nds[Random.Range(0, nds.Length - 1)].id;
        }
        
        /// <summary>
        /// Получить решение.
        /// </summary>
        /// <param name="nodeId"></param>
        /// <param name="iteration"></param>
        public void GetSolution(Bot bot, ref List<int> solution, int? nodeId)
        {
            // Решение найдено!
            if (!nodeId.HasValue)
                return;

            int? nextNodeId = null;
            var node = nodes.First(i => i.id == nodeId.Value);
            var lnks = links.Where(i => i.nodeFromId == node.id);

            switch (node.nodeType)
            {
                case NodeType.Exploration:
                    {
                        // Условие выполняется.
                        if (bot.explorationLevelPercent >= node.allowableExploration)
                            nextNodeId = lnks.First(i => i.IsTrue).nodeToId;
                        // Условие не выполняется.
                        else
                            nextNodeId = lnks.First(i => !i.IsTrue).nodeToId;

                        break;
                    }
                case NodeType.Charge:
                    {
                        // Допустимый уровень заряда.
                        if (bot.chargeLevelPercent >= node.allowableCharge)
                            nextNodeId = lnks.First(i => i.IsTrue).nodeToId;
                        else
                            nextNodeId = lnks.First(i => !i.IsTrue).nodeToId;

                        break;
                    }
                case NodeType.Passability:
                    {
                        // Проходимый участок карты.
                        if (bot.settings.passability.Contains(bot.nextPoly.type))
                            nextNodeId = lnks.First(i => i.IsTrue).nodeToId;
                        else
                            nextNodeId = lnks.First(i => !i.IsTrue).nodeToId;

                        break;
                    }
                case NodeType.Slope:
                    {
                        // Проходимый угол.
                        if (bot.settings.maxSlopeAngle <= bot.nextPoly.angle)
                            nextNodeId = lnks.First(i => i.IsTrue).nodeToId;
                        else
                            nextNodeId = lnks.First(i => !i.IsTrue).nodeToId;

                        break;
                    }
                case NodeType.Result:
                    {
                        nextNodeId = null;
                        break;
                    }
            }
            solution.Add(node.id);
            GetSolution(bot, ref solution, nextNodeId);
        }

        /// <summary>
        /// Обратный решатель.
        /// </summary>
        /// <param name="solution">Найденное решение.</param>
        /// <param name="nodeId"></param>
        public void GetReversedSolution(ref List<int> solution, int? nodeId)
        {
            if (!nodeId.HasValue)
                return;

            solution.Add(nodeId.Value);
            var node = nodes.First(i => i.id == nodeId);
            var link = links.FirstOrDefault(i => i.nodeToId == nodeId);

            if (link == null)
                GetReversedSolution(ref solution, null);
            else
                GetReversedSolution(ref solution, link.nodeFromId);
        }

        /// <summary>
        /// Проверка гипотезы.
        /// </summary>
        /// <param name="solution"></param>
        /// <returns></returns>
        public int? CheckSolution(List<int> solution)
        {
            for(int i = 0; i < rules.Count; i++)
            {
                int counter = 0;
                foreach (var j in solution)
                {
                    if (rules[i].args.Contains(j))
                        counter++;
                }

                if (counter == rules[i].args.Count)
                    return i;
            }

            return null;
        }

        /// <summary>
        /// Блок объяснений.
        /// </summary>
        /// <returns></returns>
        public List<DescriptionItem> GetDescriptionTable()
        {
            List<DescriptionItem> table = nodes
                .Select(i => new DescriptionItem()
                {
                    description = i.description,
                    number = i.id
                })
                .OrderBy(i => i.number)
                .ToList();
            return table;
        }

        public string GetSolutionString(List<int> solution)
        {
            string sol = string.Empty;

            for (int i = 0; i < solution.Count; i++)
            {
                if (i != solution.Count - 1)
                    sol += solution[i]+ " -> ";
                else
                    sol += solution[i];
            }

            return sol;
        }
    }
}












































































/// <summary>
/// Автоматическое формарование правил.
/// </summary>
/// <param name="rules"></param>
/// <param name="nodeId"></param>
//public void GetAllRules(ref List<string> expressions, ref string res, int id)
//{
//    var node = nodes.First(i => i.id == id);

//    if (node.nodeType != NodeType.Result)
//    {
//        var lnks = links.Where(i => i.nodeFromId == id);
//        foreach (var l in lnks)
//        {
//            if (res.Contains(node.id.ToString()))
//                res = res.Substring(0, res.IndexOf(node.id.ToString()) - 1);
//            res += node.id + " ";
//            GetAllRules(ref expressions, ref res, l.nodeToId);
//        }
//    }
//    else
//    {
//        expressions.Add(res + node.id);
//    }
//}