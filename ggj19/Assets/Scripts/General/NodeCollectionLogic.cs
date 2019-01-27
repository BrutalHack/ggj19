using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace BrutalHack.ggj19.General
{
    public class NodeCollectionLogic
    {
        private static NodeCollectionLogic _instance;

        public static NodeCollectionLogic Instance => _instance ?? (_instance = new NodeCollectionLogic());

        public readonly ISet<Node> passedNodes = new HashSet<Node>();

        public void TrackMove(Node fromNode, Node toNode)
        {
            passedNodes.Add(fromNode);
            passedNodes.Add(toNode);
        }
        
        public void CountScore()
        {
            ScoreController.Instance.Score = passedNodes.Count;
        }
    }
}