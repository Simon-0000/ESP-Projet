
// fait par Olivier Castonguay
using System.Collections;
using System.Collections.Generic;
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
        public static List<Noeud<T>> GetPath<T>(Noeud<T> startingNode, Noeud<T> endNode)
        {

            var visitedNodes = new List<Noeud<T>>();
            var stackNode = new Stack<Noeud<T>>();
            stackNode.Push(startingNode);

            while (stackNode.Count > 0)
            {
                var currentNode = stackNode.Pop();
                if (!visitedNodes.Exists(n=> object.ReferenceEquals(n,currentNode)))
                {

                    visitedNodes.Add(currentNode);
                    if (currentNode == endNode)
                        break;
                    
                    foreach (var childNode in currentNode.NoeudsEnfants)
                    {
                        stackNode.Push(childNode);
                    }
                }
            }

            return visitedNodes;
        }
    }
}
