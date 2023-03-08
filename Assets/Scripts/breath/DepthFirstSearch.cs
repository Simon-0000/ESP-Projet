using System.Collections;
using System.Collections.Generic;
using Assets;
using JetBrains.Annotations;
using Unity.VisualScripting;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;




public static class DepthFirstSearch<T>
{
    public static List<Noeud<T>> Path(Noeud<T> startingNode,Noeud<T> endNode)
    {
      
        var visitedNodes = new List<Noeud<T>>();
        var stackNode = new Stack<Noeud<T>>();
        stackNode.Push(startingNode);

        while (stackNode.Count > 0)
        {
            var currentNode = stackNode.Pop();
            if (!visitedNodes.Contains(currentNode))
            {
               
                visitedNodes.Add(currentNode);
                if (currentNode==endNode)
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
