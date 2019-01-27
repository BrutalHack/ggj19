using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BrutalHack.ggj19.General
{
    public class MarkGraphElements : MonoBehaviour
    {
        private List<Node> nodes;
        private List<GameObject> connections;
        private bool coroutineScheduled;

        public void Start()
        {
            nodes = new List<Node>();
            connections = new List<GameObject>();
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
            Destroy(gameObject);
        }
    }
}