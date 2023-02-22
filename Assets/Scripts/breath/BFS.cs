using System;
using System.Collections.Generic;

class Node
{
    public int data;
    public List<Node> neighbors;

    public Node(int value) 
    {
        data = value;
        neighbors = new List<Node>();
    }
}

class BreadthFirstSearch {
    public static void BFS(Node start) {
        Queue<Node> queue = new Queue<Node>();
        HashSet<Node> visited = new HashSet<Node>();

        queue.Enqueue(start);
        visited.Add(start);

        while (queue.Count > 0) {
            Node current = queue.Dequeue();
            Console.Write(current.data + " ");

            foreach (Node neighbor in current.neighbors) {
                if (!visited.Contains(neighbor)) {
                    visited.Add(neighbor);
                    queue.Enqueue(neighbor);
                }
            }
        }
    }
}