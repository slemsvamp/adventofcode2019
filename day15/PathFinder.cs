using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace day15
{
    public interface IGrid
    {
        int GetMaximumHeapSize();
        PathFinderNode[] GetNeighbours(Point point, bool allowDiagonals);
        PathFinderNode GetNodeFromWorldPoint(Point point);
    }

    public class PathFinder
    {
        // -- Private --
        private readonly int _orthogonalMovementCost;
        private readonly int _diagonalMovementCost;

        // -- Public --
        public PathFinder(int orthogonalMovementCost = 10, int diagonalMovementCost = 14)
        {
            _orthogonalMovementCost = orthogonalMovementCost;
            _diagonalMovementCost = diagonalMovementCost;
        }

        public Point[] FindPath(Point start, Point goal, IGrid grid, bool allowDiagonalMovement = false)
        {
            PathFinderNode startNode = grid.GetNodeFromWorldPoint(start);
            int maxHeapSize = grid.GetMaximumHeapSize();
            var open = new Heap<PathFinderNode>(maxHeapSize);
            HashSet<int> closed = new HashSet<int>();
            open.Add(startNode);

            while (open.Count > 0)
            {
                PathFinderNode currentNode = open.RemoveFirst();
                closed.Add(currentNode.GetHashCode());

                if (currentNode.Position == goal)
                {
                    List<Point> retrace = new List<Point>();
                    var retraceNode = currentNode;
                    while (retraceNode != startNode)
                    {
                        retrace.Add(retraceNode.Position);
                        retraceNode = retraceNode.Parent;
                    }
                    retrace.Reverse();

                    return retrace.ToArray();
                }

                foreach (PathFinderNode neighbour in grid.GetNeighbours(currentNode.Position, allowDiagonalMovement))
                {
                    if (neighbour.TerrainCost == int.MaxValue || closed.Contains(neighbour.GetHashCode()))
                    {
                        continue;
                    }

                    int newMovementCostToNeighbour = currentNode.TravelCost + GetDistance(currentNode.Position, neighbour.Position);
                    if (newMovementCostToNeighbour < neighbour.TravelCost || !open.Contains(neighbour))
                    {
                        neighbour.SetTravelCost(newMovementCostToNeighbour);
                        neighbour.SetDistanceToGoalCost(GetDistance(neighbour.Position, goal));
                        neighbour.SetParent(currentNode);

                        if (!open.Contains(neighbour))
                        {
                            open.Add(neighbour);
                        }
                        else
                        {
                            open.UpdateItem(neighbour);
                        }
                    }
                }
            }

            return new Point[]
            {
                goal
            };
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

    public class PathFinderNode : IHeapItem<PathFinderNode>
    {
        // -- Private --
        private PathFinderNode _parent;
        private readonly int _terrainCost;
        private int _travelCost;
        private int _distanceToGoalCost;
        private int _heapIndex;
        private Point _position;

        // -- Public --
        public PathFinderNode Parent => _parent;
        public int TerrainCost => _terrainCost;
        public int TravelCost => _travelCost;
        public int DistanceToGoalCost => _distanceToGoalCost;
        public Point Position => _position;

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
        public PathFinderNode(Point position, int terrainCost = 0)
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

        internal void SetParent(PathFinderNode parent)
        {
            _parent = parent;
        }

        public int CompareTo(PathFinderNode node)
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
