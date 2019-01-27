using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace BrutalHack.ggj19.General
{
    public class MoveDataCollector
    {
        private static MoveDataCollector _instance;

        public static MoveDataCollector Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new MoveDataCollector();
                }

                return _instance;
            }
        }

        public ISet<Node> passedNodes = new HashSet<Node>();
        public ISet<Vector2> passedConnections = new HashSet<Vector2>();

        public void Main(Node from, Node to)
        {
            if (CollidedWithPassedNodes(from, to))
            {
            }
        }

        public bool CollidedWithPassedNodes(Node from, Node to)
        {
            bool collisionWithPassedNods = passedNodes.Contains(to);
            passedNodes.Add(from);
            passedNodes.Add(to);
            passedConnections.Add(CalculateLineCenter(from, to));

            return collisionWithPassedNods;
        }

        public List<Node> TryToFindCirclePath(Node start)
        {
            List<Path> startPaths = GenerateStartPaths(start);
            Debug.Log("StartPaths = " + startPaths.Count);
            if (startPaths.Count == 0)
            {
                return null;
            }

            return SpreadOutWithPaths(startPaths, start);
        }

        public List<Node> SpreadOutWithPaths(List<Path> startPaths, Node start)
        {
            int generation = 0;
            List<Path> oldGeneration = startPaths;
            while (oldGeneration != null && oldGeneration.Count > 0)
            {
                generation++;
                Debug.Log("Generation: " + generation);
                List<Path> newGeneration = new List<Path>();

                foreach (var path in oldGeneration.ToList())
                {
                }

                oldGeneration = newGeneration;
            }

            return null;
        }

        public List<Path> GenerateHigherPaths(Path path)
        {
            List<Path> newPaths = new List<Path>();
            Node lastNode = path.GetLastNode();
            foreach (var neighbour in lastNode.GetNeighbours())
            {
                if (path.NotInPath(neighbour) && Touched(lastNode, neighbour))
                {
                    Path newPath = new Path(path.path);
                    newPath.path.Add(neighbour);
                    newPaths.Add(newPath);
                }
            }

            return newPaths;
        }

        public List<Path> GenerateStartPaths(Node start)
        {
            List<Path> paths = new List<Path>();
            foreach (var neighbour in start.GetNeighbours())
            {
                if (Touched(start, neighbour))
                {
                    Path path = new Path();
                    path.path.Add(start);
                    path.path.Add(neighbour);
                }
            }

            return paths;
        }

        public bool Touched(Node from, Node to)
        {
            return NodeTouched(to) && ConnectionTouched(from, to);
        }

        public bool ConnectionTouched(Node from, Node to)
        {
            Vector2 lineCenter = CalculateLineCenter(from, to);
            return passedConnections.Contains(lineCenter);
        }

        public bool NodeTouched(Node node)
        {
            return passedNodes.Contains(node);
        }

        public Vector2 CalculateLineCenter(Node node, Node neighbour)
        {
            return (node.Coordinate + neighbour.Coordinate) / 2;
        }
    }
}