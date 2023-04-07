using System.Collections.Generic;
using UnityEngine;
namespace Assets
{
    static class AStarAlgorithm //Inspiré par le pseudocode de Sebastian Lague: https://www.youtube.com/watch?v=-L-WgKMFuhE
    {

        public const float COST_PER_UNIT = 1;

        public struct AStarNodeValue
        {
            public Vector2 position;
            public float TotalCost => startCost + endCost + costOffset;
            public float startCost;
            public float endCost;
            public float costOffset;
            public bool visited;

            public AStarNodeValue(Vector2 position,float costOffset)
            {
                this.position = position;
                this.costOffset = costOffset;
                startCost = 0;
                endCost = 0;
                visited = false;
            }
        }
        
        public static List<Noeud<AStarNodeValue>> GetPath(Noeud<AStarNodeValue> startingNode, Noeud<AStarNodeValue> endNode)
        {
            List<Noeud<AStarNodeValue>> availableNodes = new();
            Noeud<AStarNodeValue> currentNode = new(null,default);
            availableNodes.Add(startingNode);
            while(availableNodes.Count > 0)
            {
                int currentNodeIndex = GetLowestFCost(availableNodes);
                availableNodes[currentNodeIndex].valeur.visited = true;
                currentNode = availableNodes[currentNodeIndex];
                availableNodes.RemoveAt(currentNodeIndex);

                if (currentNode == endNode)
                    break;
                
                for(int i = 0; i < currentNode.noeudsEnfants.Count; ++i)
                {
                    if (currentNode.noeudsEnfants[i].valeur.visited)
                        continue;

                    float nextStartCost = GetNextPossibleStartCost(currentNode, currentNode.noeudsEnfants[i]);

                    //Si le noeud n'a pas encore été utilisé ou s'il existe un meilleur chemin pour atteindre un noeud 
                    if (currentNode.noeudsEnfants[i].Parent == null || nextStartCost < currentNode.noeudsEnfants[i].valeur.startCost)
                    {
                        //Si le noeud n'a pas encore été utilisé, on instancie sa valeur de fin
                        if (currentNode.noeudsEnfants[i].Parent == null)
                        {
                            currentNode.noeudsEnfants[i].valeur.endCost = GetRelativePositionCost(endNode, currentNode.noeudsEnfants[i]);
                        }
                        currentNode.noeudsEnfants[i].valeur.startCost = nextStartCost;
                        currentNode.noeudsEnfants[i].Parent = currentNode;
                    }
                    if (!availableNodes.Exists(n => n == currentNode.noeudsEnfants[i]))
                        availableNodes.Add(currentNode.noeudsEnfants[i]);
                }
            }
            if (currentNode != endNode)
                Debug.Log("ASTAR FAILED");
            List<Noeud<AStarNodeValue>> path = Noeud<AStarNodeValue>.GetParents(currentNode);
            path.Reverse();
            return path;
        }
        public static float GetPathSize(List<Noeud<AStarNodeValue>> path)
        {
            float pathSize = 0;
            for (int i = 0; i < path.Count; ++i)
                pathSize += path[i].valeur.TotalCost;
            return pathSize;
        }

        private static float GetNextPossibleStartCost(Noeud<AStarNodeValue> baseNode, Noeud<AStarNodeValue> nextNode) =>
            baseNode.valeur.startCost + GetRelativePositionCost(baseNode, nextNode);//+ nextNode.valeur.costOffset

        private static float GetRelativePositionCost(Noeud<AStarNodeValue> nodeA, Noeud<AStarNodeValue> nodeB)=>
            (nodeA.valeur.position - nodeB.valeur.position).magnitude * COST_PER_UNIT;

        private static int GetLowestFCost(List<Noeud<AStarNodeValue>> nodeList)
        {
            int lowestFCostIndex = 0;
            for (int i = 1; i < nodeList.Count; ++i)
            {
                if (nodeList[lowestFCostIndex].valeur.TotalCost > nodeList[i].valeur.TotalCost)
                    lowestFCostIndex = i;
            }
            return lowestFCostIndex;
        }
    }
}
