
// fait par Olivier Castonguay
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
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
        public static List<Noeud<T>> GetAlgorithmPath<T>(List<Noeud<T>>Nodes, Noeud<T> startingNode, Noeud<T> endNode)
        {

           var visitedNodes = new List<Noeud<T>>();
             var stackNode = new Stack<Noeud<T>>();
            stackNode.Push(startingNode);
            Dictionary<Noeud<T>, bool> isVisited = new Dictionary<Noeud<T>, bool>();
            for (int i = 0; i < Nodes.Count; i++) 
                isVisited.TryAdd(Nodes[i], false);
            Debug.Log("new map:---------------------------------------------------------------------------------------");

            while (stackNode.Count > 0)
            {
                Debug.Log(stackNode.Count +","+stackNode.Peek().NoeudsEnfants.Count);
                
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
                    if (result) 
                        stackNode.Pop();
                    
                    if (currentNode == endNode)
                    { 
                        break; 
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
           
    }
}
