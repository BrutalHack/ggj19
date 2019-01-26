using System.Collections.Generic;
using UnityEngine;

namespace BrutalHack.ggj19.General
{
    public class Node
    {
        public Vector2 Coordinate;
        public CircleEnum Circle;
        public Dictionary<DirectionEnum, Node> Neighbours = new Dictionary<DirectionEnum, Node>();

        public List<Node> GetNeighbours()
        {
            List<Node> nodes = new List<Node>();
            foreach (var neighbourPair in Neighbours)
            {
                nodes.Add(neighbourPair.Value);
            }

            return nodes;
        }
    }
}