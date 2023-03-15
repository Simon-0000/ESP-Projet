
// fait par Olivier Castonguay
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using Assets;
using JetBrains.Annotations;
using Unity.VisualScripting;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;



namespace Assets
{
    static class DepthFirstSearch
    {
        public static List<Noeud<T>> GetPath<T>(List<Noeud<T>>nodes, Noeud<T> startingNode, Noeud<T> endNode)
        {
            bool showCompletePath = endNode == null ? true : false;
            var visitedNodes = new List<Noeud<T>>();
            var stackNode = new Stack<Noeud<T>>();
            stackNode.Push(startingNode);
            Dictionary<Noeud<T>, bool> isVisited = new Dictionary<Noeud<T>, bool>();
            for (int i = 0; i < nodes.Count; i++) 
                isVisited.TryAdd(nodes[i], false);
            
            while (stackNode.Count > 0)
            {
                var currentNode = stackNode.Peek();
                

                    visitedNodes.Add(currentNode);
                    
                        
                    bool result=true;
                    foreach (var childNode in currentNode.NoeudsEnfants)
                    {
                        isVisited.TryGetValue(childNode, out result);
                        if (!result&&childNode!=currentNode)
                        {
                            isVisited[childNode] = true;
                            stackNode.Push(childNode);
                            break;
                        }
                    }
                    if (currentNode == endNode) 
                    { 
                        break; 
                    }
                    
                    if (result)
                    {
                        stackNode.Pop();
                        if (!showCompletePath)
                            visitedNodes.RemoveAt(visitedNodes.Count - 1);
                    }
            }

            return visitedNodes;
         
           /* 
           visitedNodes.Add(startingNode);
           for (int i = 0; i < visitedNodes[visitedNodes.Count-1].NoeudsEnfants.Count; i++)
           {
               bool result;
               isVisited.TryGetValue(visitedNodes[visitedNodes.Count - 1].NoeudsEnfants[i], out result);
               if (!result)
               {
                   visitedNodes[visitedNodes.Count - 1].
                  
               }
               
           }*/
        }

        static public void ConnectNodesAccordingToPath<T>(List<Noeud<T>> path)
        {
            ClearConnecions(path);
            for(int i = 0 ; i < path.Count - 1; ++i)
                if (path[i].NoeudsEnfants.All(n => n != path[i + 1]))
                    Noeud<T>.FormerLiensRéciproque(path[i], path[i + 1]);
        }
        static private void ClearConnecions<T>(List<Noeud<T>> nodes)
        {
            for (int i = 0; i < nodes.Count; ++i)
                nodes[i].NoeudsEnfants.Clear();
        }
    }
}
