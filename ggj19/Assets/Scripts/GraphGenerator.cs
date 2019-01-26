﻿using System;
using System.Collections.Generic;
using UnityEngine;

namespace BrutalHack.ggj19
{
    public class GraphGenerator
    {
        public float worldRadius = 50f;
        public float midRadius = 30f;
        public float innerRadius = 15f;
        public float nodeDistance = 3f;
        public Dictionary<Vector2Int, Node> nodes;
        private List<DirectionEnum> directions;

        public void GenerateGraph()
        {
            GenerateDirections();
            GenerateNodes();
            ConnectNodes();
            Debug.Log("GraphGenerated.");
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
            nodes = new Dictionary<Vector2Int, Node>();
            int radiusSteps = Mathf.FloorToInt(worldRadius / nodeDistance);
            for (int i = -radiusSteps; i <= radiusSteps; i++)
            {
                for (int j = -radiusSteps; j <= radiusSteps; j++)
                {
                    Vector2Int coordinate = new Vector2Int(i, j);
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

        private void CreateAndAddPoint(Vector2Int coordinate, CircleEnum circle)
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
            Vector2Int directionValue;
            switch (direction)
            {
                case DirectionEnum.North:
                    directionValue = new Vector2Int(0, 1);
                    break;
                case DirectionEnum.South:
                    directionValue = new Vector2Int(0, -1);
                    break;
                case DirectionEnum.West:
                    directionValue = new Vector2Int(-1, 0);
                    break;
                case DirectionEnum.East:
                    directionValue = new Vector2Int(1, 0);
                    break;
                default: throw new InvalidOperationException("Unknown ENUM Value: " + direction);
            }

            Vector2Int neighbourCoordinate = node.Coordinate + directionValue;
            if (nodes.ContainsKey(neighbourCoordinate))
            {
                return nodes[neighbourCoordinate];
            }

            return null;
        }
    }
}