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
        public Dictionary<GameObject, Node> GameObjectsToNodes;
        public Dictionary<Vector2, GameObject> centerToLineRenderer;
        public Dictionary<GameObject, Vector2> lineRendererToCenter;
        private float manipulationPercentage = 0.2f;
        private List<Node> nodes;

        public static GameGraphGenerator Instance;

        private void Start()
        {
            Instance = this;
            graphGenerator = new GraphGenerator();
            graphGenerator.GenerateGraph();
            nodes = new List<Node>(graphGenerator.nodes.Values);

            nodesToGameObjects = new Dictionary<Node, GameObject>();
            GameObjectsToNodes = new Dictionary<GameObject, Node>();

            centerToLineRenderer = new Dictionary<Vector2, GameObject>();
            lineRendererToCenter = new Dictionary<GameObject, Vector2>();
            foreach (var node in nodes)
            {
                GameObject nodeGameObject = CreateGameObject(node);
                nodesToGameObjects.Add(node, nodeGameObject);
                GameObjectsToNodes.Add(nodeGameObject, node);
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
                    Vector3 fromPosition = nodesToGameObjects[node].transform.position + new Vector3(0f, 0f, 0.1f);
                    Vector3 toPosition = nodesToGameObjects[neighbour.Value].transform.position + new Vector3(0f, 0f, 0.1f);
                    Vector3 centerPosition = (fromPosition + toPosition) / 2;
                    
                    GameObject line = Instantiate(linePrefab, transform);
                    line.transform.position=  centerPosition;
                    LineRenderer lineRenderer = line.GetComponent<LineRenderer>();
                    lineRenderer.SetPosition(0,
                        fromPosition);
                    lineRenderer.SetPosition(1,
                        toPosition);
                    centerToLineRenderer.Add(center, line);
                    lineRendererToCenter.Add(line, center);
                }
            }
        }

        private Vector2 CalculateLineCenter(Node node, Node neighbour)
        {
            return (node.Coordinate + neighbour.Coordinate) / 2;
        }

        public GameObject GetLineBetweenNodesIfExists(Node node, Node otherNode)
        {
            Vector2 lineCenter = CalculateLineCenter(node, otherNode);
            if (centerToLineRenderer.ContainsKey(lineCenter))
            {
                return centerToLineRenderer[lineCenter];
            }

            return null;
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