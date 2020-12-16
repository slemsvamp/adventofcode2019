using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace day18
{
    public interface IGrid
    {
        int GetMaximumHeapSize();
        PathfinderNode[] GetNeighbours(Point point, HashSet<char> keys);
        PathfinderNode GetNodeFromWorldPoint(Point point, HashSet<char> keys);
    }

    public class Retrace
    {
        public Point[] Path { get; set; }
    }

    public class Pathfinder
    {
        // -- Private --
        private readonly int _orthogonalMovementCost;
        private readonly int _diagonalMovementCost;

        // -- Public --
        public Pathfinder(int orthogonalMovementCost = 10, int diagonalMovementCost = 14)
        {
            _orthogonalMovementCost = orthogonalMovementCost;
            _diagonalMovementCost = diagonalMovementCost;
        }

        public Retrace FindPath(Point start, Point goal, IGrid grid, HashSet<char> keys, bool allowDiagonalMovement = false)
        {
            PathfinderNode startNode = grid.GetNodeFromWorldPoint(start, keys);
            int maxHeapSize = grid.GetMaximumHeapSize();
            var open = new Heap<PathfinderNode>(maxHeapSize);
            HashSet<int> closed = new HashSet<int>();
            open.Add(startNode);

            while (open.Count > 0)
            {
                PathfinderNode currentNode = open.RemoveFirst();
                closed.Add(currentNode.GetHashCode());

                if (currentNode.Position == goal)
                {
                    var retrace = new List<Point>();
                    var retraceNode = currentNode;
                    while (retraceNode != startNode)
                    {
                        retrace.Add(retraceNode.Position);
                        retraceNode = retraceNode.Parent;
                    }
                    retrace.Reverse();

                    return new Retrace
                    {
                        Path = retrace.ToArray()
                    };
                }

                foreach (PathfinderNode neighbour in grid.GetNeighbours(currentNode.Position, keys))
                {
                    if (neighbour.TerrainCost == int.MaxValue || closed.Contains(neighbour.GetHashCode()))
                        continue;

                    int newMovementCostToNeighbour = currentNode.TravelCost + GetDistance(currentNode.Position, neighbour.Position);
                    if (newMovementCostToNeighbour < neighbour.TravelCost || !open.Contains(neighbour))
                    {
                        neighbour.SetTravelCost(newMovementCostToNeighbour);
                        neighbour.SetDistanceToGoalCost(GetDistance(neighbour.Position, goal));
                        neighbour.SetParent(currentNode);

                        if (!open.Contains(neighbour))
                            open.Add(neighbour);
                        else
                            open.UpdateItem(neighbour);
                    }
                }
            }

            return null;
        }

        public int GetDistance(Point nodeA, Point nodeB)
        {
            int dstX = Math.Abs(nodeA.X - nodeB.X);
            int dstY = Math.Abs(nodeA.Y - nodeB.Y);

            if (dstX > dstY)
            {
                return 14 * dstY + 10 * (dstX - dstY);
            }

            return 14 * dstX + 10 * (dstY - dstX);
        }
    }

    public class PathfinderNode : IHeapItem<PathfinderNode>
    {
        // -- Private --
        private PathfinderNode _parent;
        private readonly int _terrainCost;
        private int _travelCost;
        private int _distanceToGoalCost;
        private int _heapIndex;
        private Point _position;
        private char _door;

        // -- Public --
        public PathfinderNode Parent => _parent;
        public int TerrainCost => _terrainCost;
        public int TravelCost => _travelCost;
        public int DistanceToGoalCost => _distanceToGoalCost;
        public Point Position => _position;
        public char Door => _door;

        public int HeapIndex
        {
            get
            {
                return _heapIndex;
            }

            set
            {
                _heapIndex = value;
            }
        }

        public int TotalCost
        {
            get
            {
                return TravelCost + DistanceToGoalCost;
            }
        }

        // -- Constructor --
        public PathfinderNode(Point position, int terrainCost = 0)
        {
            _position = position;
            _terrainCost = terrainCost;
        }

        // -- Methods --
        internal void SetTravelCost(int travelCost)
        {
            _travelCost = travelCost;
        }

        internal void SetDistanceToGoalCost(int distanceToGoalCost)
        {
            _distanceToGoalCost = distanceToGoalCost;
        }

        internal void SetParent(PathfinderNode parent)
        {
            _parent = parent;
        }

        public int CompareTo(PathfinderNode node)
        {
            int compare = TotalCost.CompareTo(node.TotalCost);
            if (compare == 0)
            {
                compare = DistanceToGoalCost.CompareTo(node.DistanceToGoalCost);
            }

            return -compare;
        }

        public override int GetHashCode()
        {
            return _position.X << 16 | (0xffff & _position.Y);
        }
    }
}
