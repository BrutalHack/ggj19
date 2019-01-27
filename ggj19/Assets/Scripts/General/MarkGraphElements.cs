using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace BrutalHack.ggj19.General
{
    public class MarkGraphElements : MonoBehaviour
    {
        private List<Node> nodes;
        private List<GameObject> connections;
        private bool coroutineScheduled;
        private NodeCollectionLogic nodeCollectionLogic;
        public List<Node> outerNodeCycle;

        public void Start()
        {
            nodes = new List<Node>();
            connections = new List<GameObject>();
            nodeCollectionLogic = NodeCollectionLogic.Instance;
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            GameObject otherGameObject = other.gameObject;
            GameGraphGenerator graph = GameGraphGenerator.Instance;

            if (graph.GameObjectsToNodes.ContainsKey(otherGameObject))
            {
                Node node = graph.GameObjectsToNodes[otherGameObject];
                nodes.Add(node);
                Debug.Log("Hit a Node: " + node.Coordinate);
            }
            else if (graph.lineRendererToCenter.ContainsKey(otherGameObject))
            {
                //This is a connection!
                connections.Add(otherGameObject);
                Debug.Log("Hit a connection at " + graph.lineRendererToCenter[otherGameObject]);
            }

            if (!coroutineScheduled)
            {
                coroutineScheduled = true;
                StartCoroutine(CheckTriggersNextFrame());
            }
        }

        private IEnumerator CheckTriggersNextFrame()
        {
            yield return null; // Skip frame
            //TODO do stuff with the nodes?
            Debug.Log("I hit" + nodes.Count + " Nodes and " + connections.Count + "connections");

            nodes.RemoveAll(outerNodeCycle.Contains);

            GameGraphGenerator graph = GameGraphGenerator.Instance;
            
            foreach (var node in nodes)
            {
                nodeCollectionLogic.deadNodes.Add(node);
                graph.nodesToGameObjects[node].GetComponent<SpriteRenderer>().color = Color.black;
            }

            foreach (var connection in connections)
            {
                nodeCollectionLogic.passedConnections.Add(graph.lineRendererToCenter[connection]);
                connection.GetComponent<LineRenderer>().startColor = Color.black;
                connection.GetComponent<LineRenderer>().endColor = Color.black;
            }
            
            Destroy(gameObject);
        }
    }
}