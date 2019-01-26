using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace BrutalHack.ggj19.Tests
{
    [TestFixture]
    public class TestVector2Comparable
    {
        // A Test behaves as an ordinary method
        [Test]
        public void TestVector2ComparableSimplePasses()
        {
            Dictionary<Vector2, int> dict = new Dictionary<Vector2, int>();
            dict.Add(new Vector2(0, 0), 0);
            dict.Add(new Vector2(1f, 0), 1);
            dict.Add(new Vector2(0, 1f), 2);
            dict.Add(new Vector2(1f, 1f), 3);

            Assert.AreEqual(0, dict[new Vector2(0, 0)]);
            Assert.AreEqual(1, dict[new Vector2(1f, 0)]);
            Assert.AreEqual(2, dict[new Vector2(0, 1f)]);
            Assert.AreEqual(3, dict[new Vector2(1f, 1f)]);
        }

        // A UnityTest behaves like a coroutine in Play Mode. In Edit Mode you can use
        // `yield return null;` to skip a frame.
        [UnityTest]
        public IEnumerator TestVector2ComparableWithEnumeratorPasses()
        {
            // Use the Assert class to test conditions.
            // Use yield to skip a frame.
            yield return null;
        }
    }
}
