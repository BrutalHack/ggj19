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
        public ISet<Vector2> deadConnections = new HashSet<Vector2>();

        public void TrackAndHandleMove(Node fromNode, Node toNode)
        {
            if (!passedNodes.Contains(fromNode))
            {
                passedNodes.Add(fromNode);
            }

            //TODO fix this must check the nodes in diagonal !!!
            if (!deadNodes.Contains(fromNode) && IsDeadNode(fromNode))
            {
                deadNodes.Add(fromNode);
            }

            Vector2 connection = CalculateLineCenter(fromNode, toNode);
            if (!passedConnections.Contains(connection))
            {
                passedConnections.Add(connection);
            }

            if (passedNodes.Contains(toNode))
            {
                //TODO work in progress!!!
                CheckForCircle(toNode);
            }
            else
            {
                passedNodes.Add(toNode);
            }
        }

        private Vector2 CalculateLineCenter(Node node, Node neighbour)
        {
            return (node.Coordinate + neighbour.Coordinate) / 2;
        }

        private Path CheckForCircle(Node startNode)
        {
            List<Path> oldPaths = GenerateStartPaths(startNode);
            
            //TODO while schleife mit einer sehr intelegenten abbruch bedingung! => oldPaths not empty
            while (!oldPaths.IsNullOrEmpty())
            {
                List<Path> newPaths = new List<Path>();
                foreach (var path in oldPaths.ToArray())
                {
                    List<Path> newGeneratedPaths = TrackNextStep(path);
                    oldPaths.Remove(path);

                    //if on of new generated paths reached startNode => exit you have a path!
                    
                    

                    //if on of the new generated paths reached the lastNode of oldPaths => exit you have a path!

                    //if on of the new generated paths reached the lastNode of newPaths => exit you have a path!

                    //TODO maybe just check all new ends with existing old ends.


                    newPaths.AddRange(newGeneratedPaths);
                }
                oldPaths = newPaths;
                //TODO end of the super intelligent while loop
            }

            return null;
        }

        private List<Path> TrackNextStep(Path path)
        {
            List<Path> newPaths = new List<Path>();
            Node lastNode = path.GetLastNode();
            foreach (var neighbour in lastNode.GetNeighbours())
            {
                if (path.NotInPath(neighbour) && ConnectionToNeighbourPassed(lastNode, neighbour)
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
            foreach (var neighbour in node.GetNeighbours())
            {
                if (!passedNodes.Contains(neighbour)
                    || !passedConnections.Contains(CalculateLineCenter(node, neighbour)))
                {
                    return false;
                }
            }

            return true;
        }
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

        public bool NotInPath(Node node)
        {
            return !path.Contains(node);
        }
    }
}