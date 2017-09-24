using NUnit.Framework;
using System;
using System.Collections;
using UnityEngine;
using UnityEditor;
using UnityEngine.TestTools;

namespace HouraiTeahouse {

    public class MediatorContextTest {

        public class TestEvent {
        }

        public class TestObject : ScriptableObject {
        }

        [Test]
        public void null_mediator_throws_error() {
            Assert.Throws<ArgumentNullException>(() => MediatorContextExtensions.CreateContext(null));
        }

        [Test]
        public void null_unity_object_throws_error() {
            Assert.Throws<ArgumentNullException>(() => Mediator.Global.CreateUnityContext(null));
        }

        [Test]
        public void subscribing_does_not_invoke_callback() {
            var context = new Mediator().CreateContext();
            int count = 0;
            context.Subscribe<TestEvent>((args) => count++);
            Assert.AreEqual(0, count);
        }

        [Test]
        public void subscribing_subscribes_to_mediator() {
            var mediator = new Mediator();
            var context = mediator.CreateContext();
            int count = 0;
            context.Subscribe<TestEvent>((args) => count++);
            mediator.Publish(new TestEvent());
            Assert.AreNotEqual(0, count);
        }

        [Test]
        public void disposing_unsubscribes_events() {
            var mediator = new Mediator();
            var context = mediator.CreateContext();
            int count = 0;
            context.Subscribe<TestEvent>((args) => count++);
            context.Dispose();
            mediator.Publish(new TestEvent());
            Assert.AreEqual(0, count);
        }

        [Test]
        public void destroyed_objects_are_not_called() {
            var testObject = ScriptableObject.CreateInstance<TestObject>();
            var mediator = new Mediator();
            var context = mediator.CreateUnityContext(testObject);
            int count = 0;
            context.Subscribe<TestEvent>((args) => count++);
            UnityEngine.Object.DestroyImmediate(testObject);
            mediator.Publish(new TestEvent());
            Assert.AreEqual(0, count);
        }


    }

}