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
            MoveDataCollector nodeCollectionLogic = MoveDataCollector.Instance;
            Node from = CreateNode(new Vector2(0, 0));
            Node to = CreateNode(new Vector2(0, 1));

            Assert.AreEqual(2, nodeCollectionLogic.passedNodes.Count);
            Assert.AreEqual(1, nodeCollectionLogic.passedConnections.Count);
        }

        private Node CreateNode(Vector2 coordinate)
        {
            return new Node {Coordinate = coordinate};
        }
    }
}