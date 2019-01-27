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
                Debug.Log("Try to find circle");
                List<Node> result = TryToFindCirclePath(to, from);
                if (result != null)
                {
                    Debug.Log("RESULT: ");
                    foreach (var node in result)
                    {
                        Debug.Log("=> " + node.Coordinate);
                    }
                }
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

        public List<Node> TryToFindCirclePath(Node first, Node second)
        {
            List<Path> startPaths = GenerateStartPaths(first, second);
            Debug.Log("StartPaths = " + startPaths.Count);
            if (startPaths.Count == 0)
            {
                return null;
            }

            return SpreadOutWithPaths(startPaths, first);
        }

        public List<Node> SpreadOutWithPaths(List<Path> startPaths, Node first)
        {
            int generation = 0;
            List<Path> oldGeneration = startPaths;
            while (oldGeneration != null && oldGeneration.Count > 0)
            {
                generation++;
                Debug.Log("Generation: " + generation + " OldGenerations: " + oldGeneration.Count);
                List<Path> newGeneration = new List<Path>();

                foreach (var path in oldGeneration.ToList())
                {
                    List<Path> pathExtensions = GenerateHigherPaths(path);
                    Debug.Log("Path Extensions: " + pathExtensions.Count);

                    foreach (var pathExtension in pathExtensions)
                    {
                        if (CanReachStartNode(pathExtension, first))
                        {
                            return pathExtension.path;
                        }
                    }

                    newGeneration.AddRange(pathExtensions);
                }

                oldGeneration = newGeneration;
            }

            return null;
        }

        public bool CanReachStartNode(Path path, Node start)
        {
            foreach (var neighbour in path.GetLastNode().GetNeighbours())
            {
                if (start.Equals(neighbour) && ConnectionTouched(start, neighbour))
                {
                    return true;
                }
            }

            return false;
        }
        
        public Path AnyPathEndsIntersect(List<Path> mainPaths, List<Path> secondaryPaths)
        {
            foreach (var mainPath in mainPaths)
            {
                foreach (var secondaryPath in secondaryPaths)
                {
                    if (mainPath.GetLastNode().Equals(secondaryPath.GetLastNode()))
                    {
                        return CombinePaths(mainPath, secondaryPath);
                    }
                }
            }

            return null;
        }

        public Path CombinePaths(Path main, Path second)
        {
            for (int i = second.path.Count - 2; i > 0; i--)
            {
                main.path.Add(second.path[i]);
            }

            return main;
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

        public List<Path> GenerateStartPaths(Node start, Node second)
        {
            List<Path> paths = new List<Path>();
            Path path = new Path();
            path.path.Add(start);
            path.path.Add(second);
            paths.Add(path);

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