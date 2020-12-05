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

        public Graph(List<RetraceData> retraceData, Dictionary<char, Point> keyPositions, Point characterPosition)
        {
            var characterNode = new Node('@', characterPosition);
            var nodes = new Dictionary<char, Node>();
            foreach (var keyValuePair in keyPositions)
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

        public void StartTraversal(char nodeName, HashSet<char> keys)
        {
            var startNode = Nodes[nodeName];

            var heap = new Heap<Traverser>(650);
            
            foreach (var edge in Edges.Where(kvp => kvp.Value.Name.Contains(nodeName) && kvp.Value.Doors.Length == 0))
            {
                var traverser = Traverser.CreateRoot(startNode, keys);
                heap.Add(traverser);
            }

            while (heap.Count > 0)
            {
                var traverser = heap.RemoveFirst();
                var edges = Edges.Where(kvp => traverser.ValidEdge(kvp.Value));


                
            }
        }

        public class Traverser : IHeapItem<Traverser>
        {
            public Traverser Parent;
            public Node Node;
            public List<char> CollectedKeys;
            public int Cost;
            public HashSet<char> Unvisited;
            public HashSet<char> Visited;

            public Traverser(Node current, List<char> collectedKeys, int cost, HashSet<char> visited, HashSet<char> unvisited, Traverser parent = null)
            {
                Node = current;
                CollectedKeys = collectedKeys;
                Cost = cost;
                Visited = visited;
                Unvisited = unvisited;
                Parent = parent;
            }

            public bool ValidEdge(Edge edge)
                => IsAtEdge(edge.Name)
                && HasKeyForTheDoors(edge.Doors)
                && NeedsKey(edge.Name);

            private bool IsAtEdge(string edgeName)
                => edgeName.Contains(Node.Name);

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
