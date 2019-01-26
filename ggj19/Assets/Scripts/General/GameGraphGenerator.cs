using System.Collections.Generic;
using UnityEngine;

namespace BrutalHack.ggj19.General
{
    public class GameGraphGenerator : MonoBehaviour
    {
        public GameObject nodePrefab;
        public GameObject linePrefab;
        public GraphGenerator graphGenerator;

        public Dictionary<Node, GameObject> nodesToGameObjects;
        private Dictionary<Vector2, GameObject> centerToLineRenderer;
        private float manipulationPercentage = 0.2f;
        private List<Node> nodes;

        private void Start()
        {
            graphGenerator = new GraphGenerator();
            graphGenerator.GenerateGraph();
            nodes = new List<Node>(graphGenerator.nodes.Values);

            nodesToGameObjects = new Dictionary<Node, GameObject>();
            centerToLineRenderer = new Dictionary<Vector2, GameObject>();
            foreach (var node in nodes)
            {
                nodesToGameObjects.Add(node, CreateGameObject(node));
            }

            foreach (var node in nodes)
            {
                CreateLines(node);
            }
        }

        private void CreateLines(Node node)
        {
            foreach (var neighbour in node.Neighbours)
            {
                Vector2 center = CalculateLineCenter(node, neighbour.Value);
                if (!centerToLineRenderer.ContainsKey(center))
                {
                    GameObject line = Instantiate(linePrefab, transform);
                    LineRenderer lineRenderer = line.GetComponent<LineRenderer>();
                    lineRenderer.SetPosition(0, nodesToGameObjects[node].transform.position);
                    lineRenderer.SetPosition(1, nodesToGameObjects[neighbour.Value].transform.position);
                    centerToLineRenderer.Add(center, line);
                }
            }
        }

        private Vector2 CalculateLineCenter(Node node, Node neighbour)
        {
            return (node.Coordinate + neighbour.Coordinate) / 2;
        }

        private Vector3 CreatePosition(Node node)
        {
            float manipulationRange = CalculateManipulationRange();

            return new Vector3(node.Coordinate.x + Random.Range(-manipulationRange, manipulationRange),
                node.Coordinate.y + Random.Range(-manipulationRange, manipulationRange), 0);
        }

        private float CalculateManipulationRange()
        {
            return graphGenerator.nodeDistance * manipulationPercentage;
        }

        private GameObject CreateGameObject(Node node)
        {
            GameObject nodeGameObject = Instantiate(nodePrefab, CreatePosition(node), Quaternion.identity, transform);
            SpriteRenderer spriteRenderer = nodeGameObject.GetComponent<SpriteRenderer>();
            if (node.Circle.Equals(CircleEnum.InnerCircle))
            {
                spriteRenderer.color = Color.yellow;
            } else if (node.Circle.Equals(CircleEnum.MidCircle))
            {
                spriteRenderer.color = Color.cyan;
            }else if (node.Circle.Equals(CircleEnum.OuterCircle))
            {
                spriteRenderer.color = Color.magenta;
            }
            return nodeGameObject;
        }
    }
}