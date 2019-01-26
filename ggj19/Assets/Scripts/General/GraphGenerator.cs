using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

namespace BrutalHack.ggj19.General
{
    public class GraphGenerator
    {
        public float worldRadius = 50f;
        public float midRadius = 30f;
        public float innerRadius = 15f;
        public float nodeDistance = 3f;
        public Dictionary<Vector2, Node> nodes;
        private List<DirectionEnum> directions;
        private int radiusSteps;
        private float manipulationPercentage = 0.1f;

        public void GenerateGraph()
        {
            radiusSteps = Mathf.FloorToInt(worldRadius / nodeDistance);
            GenerateDirections();
            GenerateNodes();
            ConnectNodes();
            RemoveRandomNodes();
        }

        private void RemoveRandomNodes()
        {
            int removeCount = Mathf.FloorToInt(nodes.Count * manipulationPercentage);
            for (int i = 0; i < removeCount; i++)
            {
                int removeAtX = Random.Range(-radiusSteps, radiusSteps);
                int removeAtY = Random.Range(-radiusSteps, radiusSteps);
                Vector2 toRemove = new Vector2(removeAtX * nodeDistance, removeAtY * nodeDistance);
                if (toRemove.Equals(Vector2.zero))
                {
                    continue;
                }
                if (nodes.ContainsKey(toRemove) && nodes[toRemove].Neighbours.Count > 1)
                {
                    RemoveNode(nodes[toRemove]);
                }
            }
        }

        private void RemoveNode(Node node)
        {
            foreach (var neighbourPair in node.Neighbours.ToList())
            {
                foreach (var neighbourNeighbourPair in neighbourPair.Value.Neighbours.ToList())
                {
                    if (neighbourNeighbourPair.Value.Equals(node))
                    {
                        neighbourPair.Value.Neighbours.Remove(neighbourNeighbourPair.Key);
                    }
                }
            }

            nodes.Remove(node.Coordinate);
        }

        private void GenerateDirections()
        {
            directions = new List<DirectionEnum>
            {
                DirectionEnum.North, DirectionEnum.South, DirectionEnum.West, DirectionEnum.East
            };
        }

        private void GenerateNodes()
        {
            nodes = new Dictionary<Vector2, Node>();
            for (int i = -radiusSteps; i <= radiusSteps; i++)
            {
                for (int j = -radiusSteps; j <= radiusSteps; j++)
                {
                    Vector2 coordinate = new Vector2(i * nodeDistance, j * nodeDistance);
                    if (PointInRadius(coordinate, innerRadius))
                    {
                        CreateAndAddPoint(coordinate, CircleEnum.InnerCircle);
                    }
                    else if (PointInRadius(coordinate, midRadius))
                    {
                        CreateAndAddPoint(coordinate, CircleEnum.MidCircle);
                    }
                    else if (PointInRadius(coordinate, worldRadius))
                    {
                        CreateAndAddPoint(coordinate, CircleEnum.OuterCircle);
                    }
                }
            }
        }

        private void CreateAndAddPoint(Vector2 coordinate, CircleEnum circle)
        {
            nodes.Add(coordinate, new Node
            {
                Coordinate = coordinate,
                Circle = circle
            });
        }

        private bool PointInRadius(Vector2 point, float radius)
        {
            return Mathf.Sqrt(Mathf.Pow(point.x, 2) + Mathf.Pow(point.y, 2)) <= radius;
        }

        private void ConnectNodes()
        {
            foreach (var nodePair in nodes)
            {
                foreach (var direction in directions)
                {
                    Node node = nodePair.Value;
                    Node neighbor = GetNeighbour(node, direction);
                    if (neighbor != null)
                    {
                        node.Neighbours.Add(direction, neighbor);
                    }
                }
            }
        }

        private Node GetNeighbour(Node node, DirectionEnum direction)
        {
            Vector2 directionValue;
            switch (direction)
            {
                case DirectionEnum.North:
                    directionValue = new Vector2(0, 1 * nodeDistance);
                    break;
                case DirectionEnum.South:
                    directionValue = new Vector2(0, -1 * nodeDistance);
                    break;
                case DirectionEnum.West:
                    directionValue = new Vector2(-1 * nodeDistance, 0);
                    break;
                case DirectionEnum.East:
                    directionValue = new Vector2(1 * nodeDistance, 0);
                    break;
                default: throw new InvalidOperationException("Unknown ENUM Value: " + direction);
            }

            Vector2 neighbourCoordinate = node.Coordinate + directionValue;
            if (nodes.ContainsKey(neighbourCoordinate))
            {
                return nodes[neighbourCoordinate];
            }

            return null;
        }
    }
}