using System.Collections.Generic;
using UnityEngine;

namespace BrutalHack.ggj19
{
    public class Node
    {
        public Vector2 Coordinate;
        public CircleEnum Circle;
        public Dictionary<DirectionEnum, Node> Neighbours = new Dictionary<DirectionEnum, Node>();

        // Start is called before the first frame update
        void Start()
        {
        }

        // Update is called once per frame
        void Update()
        {
        }
    }
}