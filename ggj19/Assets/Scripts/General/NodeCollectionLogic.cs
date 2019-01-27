using System.Collections.Generic;
using System.Linq;
using Sirenix.Utilities;
using UnityEngine;

namespace BrutalHack.ggj19.General
{
    public class NodeCollectionLogic
    {
        public ISet<Node> passedNodes = new HashSet<Node>();
        public ISet<Node> deadNodes = new HashSet<Node>();
        public ISet<Vector2> passedConnections = new HashSet<Vector2>();

        public List<Node> TrackAndHandleMove(Node fromNode, Node toNode)
        {
            if (!passedNodes.Contains(fromNode))
            {
                passedNodes.Add(fromNode);
            }

            CollectDeadNodes(fromNode);

            Vector2 connection = CalculateLineCenter(fromNode, toNode);
            if (!passedConnections.Contains(connection))
            {
                passedConnections.Add(connection);
            }

            if (passedNodes.Contains(toNode))
            {
                Path circlePath = CheckForCircle(toNode);
                if (circlePath != null)
                {
                    CollectDeadNodes(circlePath);
                    return circlePath.path;
                }
            }
            else
            {
                passedNodes.Add(toNode);
            }

            return null;
        }

        private void CollectDeadNodes(Path path)
        {
            foreach (var node in path.path)
            {
                CollectDeadNodes(node);
            }
        }

        private void CollectDeadNodes(Node node)
        {
            if (!deadNodes.Contains(node) && IsDeadNode(node))
            {
                deadNodes.Add(node);
            }
        }

        private Vector2 CalculateLineCenter(Node node, Node neighbour)
        {
            return (node.Coordinate + neighbour.Coordinate) / 2;
        }

        private Path CheckForCircle(Node startNode)
        {
            List<Path> oldPaths = GenerateStartPaths(startNode);

            while (!oldPaths.IsNullOrEmpty())
            {
                List<Path> newPaths = new List<Path>();
                foreach (var path in oldPaths.ToArray())
                {
                    List<Path> newGeneratedPaths = TrackNextStep(path);
                    oldPaths.Remove(path);

                    Path circlePath = null;
                    //if on of new generated paths reached startNode => exit you have a path!
                    circlePath = AnyPathBecameACircle(newGeneratedPaths);
                    if (circlePath != null)
                    {
                        return circlePath;
                    }

                    //if on of the new generated paths reached the lastNode of oldPaths => exit you have a path!
                    circlePath = AnyPathEndsIntersect(newGeneratedPaths, oldPaths);
                    if (circlePath != null)
                    {
                        return circlePath;
                    }

                    //if on of the new generated paths reached the lastNode of newPaths => exit you have a path!
                    circlePath = AnyPathEndsIntersect(newGeneratedPaths, newPaths);
                    if (circlePath != null)
                    {
                        return circlePath;
                    }

                    newPaths.AddRange(newGeneratedPaths);
                }

                oldPaths = newPaths;
            }

            return null;
        }

        private Path AnyPathEndsIntersect(List<Path> mainPaths, List<Path> secondaryPaths)
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

        private Path CombinePaths(Path main, Path second)
        {
            for (int i = second.path.Count - 1; i >= 0; i--)
            {
                main.path.Add(second.path[i]);
            }

            return main;
        }

        private Path AnyPathBecameACircle(List<Path> paths)
        {
            foreach (var path in paths)
            {
                if (path.IsACircle())
                {
                    return path;
                }
            }

            return null;
        }

        private List<Path> TrackNextStep(Path path)
        {
            List<Path> newPaths = new List<Path>();
            Node lastNode = path.GetLastNode();
            foreach (var neighbour in lastNode.GetNeighbours())
            {
                if (path.NotInPathWithCircleException(neighbour) && ConnectionToNeighbourPassed(lastNode, neighbour)
                                                                 && !deadNodes.Contains(neighbour))
                {
                    Path newPath = new Path(path.path);
                    newPath.path.Add(neighbour);
                }
            }

            return newPaths;
        }

        private List<Path> GenerateStartPaths(Node node)
        {
            List<Path> paths = new List<Path>();
            foreach (var neighbour in node.GetNeighbours())
            {
                if (ConnectionToNeighbourPassed(node, neighbour)
                    && !deadNodes.Contains(neighbour))
                {
                    Path path = new Path();
                    path.path.Add(node);
                    path.path.Add(neighbour);
                }
            }

            return paths;
        }

        private bool ConnectionToNeighbourPassed(Node node, Node neighbour)
        {
            Vector2 connection = CalculateLineCenter(node, neighbour);
            return passedConnections.Contains(connection);
        }

        private bool IsDeadNode(Node node)
        {
            if (deadNodes.Contains(node))
            {
                return true;
            }

            foreach (var neighbourEntry in node.Neighbours)
            {
                LookupResult result = LeftLookUpComplete(node, neighbourEntry.Value, neighbourEntry.Key);
                if (result.Equals(LookupResult.Bad) || result.Equals(LookupResult.Back))
                {
                    return false;
                }
                //LookupResult.Good => sector dead, check the next sector.
            }

            return true;
        }

        private LookupResult LeftLookUpComplete(Node startNode, Node node, DirectionEnum direction)
        {
            DirectionEnum reverseDirection = direction.Reverse(); 
            DirectionEnum currentDirection = reverseDirection;
            for (int i = 0; i < 4; i++)
            {
                DirectionEnum leftDirection = currentDirection.Left();
                if (reverseDirection.Equals(leftDirection))
                {
                    return LookupResult.Back;
                }

                Node leftNeighbour = null;
                if (node.Neighbours.ContainsKey(leftDirection))
                {
                    leftNeighbour = node.Neighbours[leftDirection];

                    if (!passedConnections.Contains(CalculateLineCenter(node, leftNeighbour)))
                    {
                        return LookupResult.Bad;
                    }

                    if (startNode.Equals(leftNeighbour))
                    {
                        return LookupResult.Good;
                    }

                    LookupResult result = LeftLookUpComplete(startNode, leftNeighbour, leftDirection);
                    if (!result.Equals(LookupResult.Back))
                    {
                        return result;
                    }
                }

                currentDirection = leftDirection;
            }

            Debug.Log("Should not happen :)");
            return LookupResult.Back;
        }
    }

    enum LookupResult
    {
        Good,
        Bad,
        Back
    }

    class Path
    {
        public List<Node> path;

        public Path()
        {
            path = new List<Node>();
        }

        public Path(List<Node> nodes)
        {
            path = new List<Node>(nodes);
        }

        public Node GetLastNode()
        {
            return path.LastOrDefault();
        }

        public Node GetFirstNode()
        {
            return path.FirstOrDefault();
        }

        public bool NotInPathWithCircleException(Node node)
        {
            return NotInPath(node) || (path.Count > 3 && node.Equals(GetFirstNode()));
        }

        public bool NotInPath(Node node)
        {
            return !path.Contains(node);
        }

        public bool IsACircle()
        {
            return GetFirstNode().Equals(GetLastNode());
        }
    }
}