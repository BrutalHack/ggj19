using BrutalHack.ggj19.General;
using NUnit.Framework;
using UnityEngine;

namespace BrutalHack.ggj19.Tests
{
    [TestFixture]
    public class TestIsDead
    {
        [Test]
        public void Test()
        {
            NodeCollectionLogic nodeCollectionLogic = NodeCollectionLogic.Instance;
            Node from = CreateNode(new Vector2(0, 0));
            Node to = CreateNode(new Vector2(0, 1));
            nodeCollectionLogic.TrackAndHandleMove(from, to);

            Assert.AreEqual(2, nodeCollectionLogic.passedNodes.Count);
            Assert.AreEqual(1, nodeCollectionLogic.deadNodes.Count);
        }

        private Node CreateNode(Vector2 coordinate)
        {
            return new Node {Coordinate = coordinate};
        }
    }
}