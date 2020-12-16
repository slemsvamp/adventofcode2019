using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace day18
{
    public class Graph
    {
        public Dictionary<char, Node> Nodes;
        public Dictionary<string, Edge> Edges;

        public Graph(List<RetraceData> retraceData, Dictionary<char, Point> keyPositions, Dictionary<char, Point> doorPositions, Point characterPosition)
        {
            var characterNode = new Node('@', characterPosition);
            var nodes = new Dictionary<char, Node>();
            foreach (var keyValuePair in keyPositions)
                nodes.Add(keyValuePair.Key, new Node(keyValuePair.Key, keyValuePair.Value));
            foreach (var keyValuePair in doorPositions)
                nodes.Add(keyValuePair.Key, new Node(keyValuePair.Key, keyValuePair.Value));
            nodes.Add(characterNode.Name, characterNode);

            var edges = new Dictionary<string, Edge>();
            foreach (var retrace in retraceData)
            {
                var edgeName = Edge.EdgeName(nodes[retrace.From], nodes[retrace.To]);
                if (!edges.ContainsKey(edgeName))
                    edges.Add(edgeName, new Edge(edgeName, retrace));
            }

            Nodes = nodes;
            Edges = edges;
        }

        public static int Cost = int.MaxValue;

        public int DepthFirstSearch(char nodeName, HashSet<char> unvisited, HashSet<char> collectedKeys, int cost = 0, int level = 0)
        {
            if (unvisited.Count == 0)
            {
                if (cost < Cost)
                {
                    Cost = cost;
                    Console.WriteLine($"New Cost: {cost.ToString()}");
                }
                return cost;
            }

            var edges = Edges.Values.Where(edge => ValidEdge(nodeName, edge, unvisited, collectedKeys)).ToList();

            int answer = 0;

            foreach (var edge in edges.OrderBy(e => e.Cost))
            {
                var otherNode = edge.OtherNodeName(nodeName);
                cost += edge.Cost;
                unvisited.Remove(otherNode);
                collectedKeys.Add(otherNode);
                
                answer = DepthFirstSearch(otherNode, new HashSet<char>(unvisited), new HashSet<char>(collectedKeys), cost, level + 1);
            }

            //if (unvisited.Count == 0)
            //{
            //    if (cost < Cost)
            //    {
            //        Cost = cost;
            //        Console.WriteLine($"New Cost: {cost.ToString()}");
            //    }
            //    return true;
            //}

            return answer;

            //var heap = new Heap<Traverser>(650);

            //heap.Add(Traverser.CreateRoot(startNode, keys));

            //Console.WriteLine($"Added {heap.Count} traverser(s).");

            //while (heap.Count > 0)
            //{
            //    var traverser = heap.RemoveFirst();
            //    var edges = Edges.Values.Where(edge => traverser.ValidEdge(edge)).ToList();

            //    if (edges.Count == 0)
            //    {
            //        Console.WriteLine("Traverser has no more valid edges to traverse.");
            //        return;
            //    }

            //    Console.WriteLine($"Traverser {traverser.Id} has {edges.Count} edge(s).");

            //    var edge = edges.OrderBy(edge => edge.Cost).First();
            //    var toNodeName = edge.OtherNode(traverser.Node.Name);

            //    Console.WriteLine($"Traverser {traverser.Id} chose an edge with cost {edge.Cost}.");

            //    traverser.RecursiveTraverse(edge);
            //}
        }

        public bool ValidEdge(char nodeName, Edge edge, HashSet<char> unvisited, HashSet<char> collectedKeys)
            => IsAtEdge(nodeName, edge.Name)
            && IsUnvisited(unvisited, edge.OtherNodeName(nodeName))
            && HasKeyForTheDoors(collectedKeys, edge.Doors)
            && NeedsKey(nodeName, edge.Name, collectedKeys);

        private bool IsAtEdge(char nodeName, string edgeName)
            => edgeName.Contains(nodeName);

        private bool IsUnvisited(HashSet<char> unvisited, char nodeName)
            => unvisited.Contains(nodeName);

        private bool HasKeyForTheDoors(HashSet<char> collectedKeys, char[] doors)
        {
            foreach (var door in doors)
            {
                if (!collectedKeys.Contains(door))
                    return false;
            }
            return true;
        }

        private bool NeedsKey(char nodeName, string edgeKey, HashSet<char> collectedKeys)
        {
            if (edgeKey.IndexOf(nodeName) == 0)
                return !collectedKeys.Contains(edgeKey[1]);
            return !collectedKeys.Contains(edgeKey[0]);
        }

        public class Traverser : IHeapItem<Traverser>
        {
            private static int _identity;
            
            public Traverser Parent;
            public Node Node;
            public List<char> CollectedKeys;
            public int Cost;
            public HashSet<char> Unvisited;
            public HashSet<char> Visited;
            
            private int _id;
            public int Id => _id;

            public Traverser(Node current, List<char> collectedKeys, int cost, HashSet<char> visited, HashSet<char> unvisited, Traverser parent = null)
            {
                Node = current;
                CollectedKeys = collectedKeys;
                Cost = cost;
                Visited = visited;
                Unvisited = unvisited;
                Parent = parent;
                
                _id = ++_identity;
            }

            public void RecursiveTraverse(Edge edge)
            {
                Cost += edge.Cost;
                
                // RecursiveTraverse(default);
            }

            public bool ValidEdge(Edge edge)
                => IsAtEdge(edge.Name)
                && IsUnvisited(edge.OtherNodeName(Node.Name))
                && HasKeyForTheDoors(edge.Doors)
                && NeedsKey(edge.Name);

            private bool IsAtEdge(string edgeName)
                => edgeName.Contains(Node.Name);

            private bool IsUnvisited(char nodeName)
                => Unvisited.Contains(nodeName);

            private bool HasKeyForTheDoors(char[] doors)
            {
                foreach (var door in doors)
                {
                    if (!CollectedKeys.Contains(door))
                        return false;
                }
                return true;
            }

            private bool NeedsKey(string edgeKey)
            {
                if (edgeKey.IndexOf(Node.Name) == 0)
                    return !CollectedKeys.Contains(edgeKey[1]);
                return !CollectedKeys.Contains(edgeKey[0]);
            }

            public static Traverser CreateRoot(Node current, HashSet<char> unvisited)
                => new Traverser(current, new List<char>(), 0, new HashSet<char>(), unvisited);

            public int HeapIndex { get; set; }

            public int CompareTo(Traverser other)
                => Cost - other.Cost;
        }
    }
}
