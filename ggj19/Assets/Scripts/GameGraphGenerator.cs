using System.Collections.Generic;
using UnityEngine;

namespace BrutalHack.ggj19
{
    public class GameGraphGenerator : MonoBehaviour
    {
        public GameObject nodePrefab;
        public GameObject linePrefab;

        private float manipulationPercentage = 0.2f;
        private GraphGenerator graphGenerator;
        private List<Node> nodes;
        private Dictionary<Node, GameObject> nodesToGameObjects;
        private Dictionary<Vector2Int, GameObject> centerToLineRenderer;

        private void Start()
        {
            graphGenerator = new GraphGenerator();
            graphGenerator.GenerateGraph();
            nodes = new List<Node>(graphGenerator.nodes.Values);

            nodesToGameObjects = new Dictionary<Node, GameObject>();
            centerToLineRenderer = new Dictionary<Vector2Int, GameObject>();
            foreach (var node in nodes)
            {
                nodesToGameObjects.Add(node, CreateGameObject(node));
                CreateLines(node);
            }
        }

        private void CreateLines(Node node)
        {
            Debug.Log("Node count: " + node.Neighbours.Count);
            foreach (var neighbour in node.Neighbours)
            {
                Debug.Log("Neighbour.");
                Vector2Int center = CalculateLineCenter(node, neighbour.Value);
                if (!centerToLineRenderer.ContainsKey(center))
                {
                    Debug.Log("Create Line");
                    GameObject line = Instantiate(linePrefab);
                    LineRenderer lineRenderer = line.GetComponent<LineRenderer>();
                    lineRenderer.SetPosition(0, nodesToGameObjects[node].transform.position);
                    lineRenderer.SetPosition(1, nodesToGameObjects[neighbour.Value].transform.position);
                    centerToLineRenderer.Add(center, line);
                }
            }
        }

        private Vector2Int CalculateLineCenter(Node node, Node neighbour)
        {
            Vector3 position = (ConvertNodePositionToWorldPosition(node) + ConvertNodePositionToWorldPosition(neighbour)) / 2;
            return new Vector2Int(Mathf.FloorToInt(position.x), Mathf.FloorToInt(position.y));
        }

        private Vector3 ConvertNodePositionToWorldPosition(Node node)
        {
            return new Vector3(node.Coordinate.x * graphGenerator.nodeDistance, 
                node.Coordinate.y * graphGenerator.nodeDistance, 0);
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
            return Instantiate(nodePrefab, CreatePosition(node), Quaternion.identity, transform);
        }
    }
}