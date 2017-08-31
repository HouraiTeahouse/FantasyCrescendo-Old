using UnityEngine;
using UnityEditor;
using UnityEngine.TestTools;
using NUnit.Framework;
using System.Collections;

namespace HouraiTeahouse.SmashBrew.Characters {

    public class Vector2bTest {

        [Test]
        public void setting_x_with_float_quantizes() {
            var vec = new Vector2b {
                X = 3/256f
            };
            Assert.AreEqual(1/127f, vec.X);
        }

        [Test]
        public void setting_y_with_float_quantizes() {
            var vec = new Vector2b {
                Y = -4/256f
            };
            Assert.AreEqual(-2/127f, vec.Y);
        }

        [Test]
        public void setting_y_clamps() {
            Assert.AreEqual(1f, new Vector2b { Y = 2f}.Y);
            Assert.AreEqual(-1f, new Vector2b { Y = -2f}.Y);
            Assert.AreEqual(30/127f, new Vector2b { Y = 30/127f}.Y);
        }

        [Test]
        public void setting_x_clamps() {
            Assert.AreEqual(1f, new Vector2b { X = 2f}.X);
            Assert.AreEqual(-1f, new Vector2b { X = -2f}.X);
            Assert.AreEqual(-30/127f, new Vector2b { X = -30/127f}.X);
        }

        [Test]
        public void float_vector_conversion() {
            var vector = new Vector2 {
                x = 13/127f,
                y = -58/127f
            };
            Vector2b v2b = vector;
            Vector2 convertVec = v2b;
            Debug.Log(v2b.byteX);
            Assert.AreEqual(13/127f, convertVec.x);
            Assert.AreEqual(-58/127f, convertVec.y);
        }

    }

}
