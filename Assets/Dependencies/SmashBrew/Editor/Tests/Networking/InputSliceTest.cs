using UnityEngine;
using UnityEditor;
using UnityEngine.TestTools;
using NUnit.Framework;
using System.Collections;

namespace HouraiTeahouse.SmashBrew.Characters {

    public class InputSliceTest {

        [Test]
        public void attack_saves_correctly() {
            Assert.IsTrue(new InputSlice { Attack = true }.Attack);
            Assert.IsFalse(new InputSlice { Attack = false }.Attack);
        }

        [Test]
        public void shield_saves_correctly() {
            Assert.IsTrue(new InputSlice { Shield = true }.Shield);
            Assert.IsFalse(new InputSlice { Shield = false }.Shield);
        }

        [Test]
        public void special_saves_correctly() {
            Assert.IsTrue(new InputSlice { Special = true }.Special);
            Assert.IsFalse(new InputSlice { Special = false }.Special);
        }

        [Test]
        public void jump_saves_correctly() {
            Assert.IsTrue(new InputSlice { Jump = true }.Jump);
            Assert.IsFalse(new InputSlice { Jump = false }.Jump);
        }

        [Test]
        public void smash_saves_correctly() {
            Assert.AreEqual(Vector2.up, new InputSlice { Smash = Vector2.up }.Smash);
            Assert.AreEqual(Vector2.down, new InputSlice { Smash = Vector2.down * 0.5f }.Smash);
            Assert.AreEqual(Vector2.right + Vector2.up, new InputSlice { Smash = new Vector2(0.5f, 0.79f) }.Smash);
        }

        [Test]
        public void movement_saves_correctly() {
            Assert.AreEqual(Vector2.up, new InputSlice { Movement = Vector2.up }.Movement);
            Assert.AreEqual(Vector2.down * 62/127f, new InputSlice { Movement = Vector2.down * 62/127f }.Movement);
            Assert.AreEqual(new Vector2(52/127f, 33/127f), new InputSlice { Movement= new Vector2(52/127f, 33/127f) }.Movement);
        }

    }

}
