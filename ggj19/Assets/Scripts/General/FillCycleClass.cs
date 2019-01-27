using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Analytics;
using UnityEngine.Serialization;

namespace BrutalHack.ggj19.General
{
    public class FillCycleClass : MonoBehaviour
    {
        public GameGraphGenerator graph;
        public GameObject polygonColliderPrefab;

        public void FillCycle(List<Node> cycleNodes)
        {
            var colliderObject = Instantiate(polygonColliderPrefab);
            PolygonCollider2D polygonCollider2D = colliderObject.GetComponent<PolygonCollider2D>();
            Vector3[] cycleCoordinates =
                cycleNodes.Select(node => graph.nodesToGameObjects[node].transform.position).ToArray();
            polygonCollider2D.points = cycleCoordinates.Select(vector3 => new Vector2(vector3.x, vector3.y)).ToArray();
        }
    }
}