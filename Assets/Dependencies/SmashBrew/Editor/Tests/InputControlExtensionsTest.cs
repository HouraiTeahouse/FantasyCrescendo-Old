using HouraiTeahouse.HouraiInput;
using UnityEngine;
using UnityEditor;
using UnityEngine.TestTools;
using NUnit.Framework;
using System.Collections;

namespace HouraiTeahouse.SmashBrew {

    public class InputControlExtensionsTest {

        [Test]
        public void get_controls_returns_valid_values() {
            var device = new InputDevice("test device");
            var action1 = device.AddControl(InputTarget.Action1, "test");
            var action2 = device.AddControl(InputTarget.Action2, "test");
            var action3 = device.AddControl(InputTarget.Action3, "test");
            var action4 = device.AddControl(InputTarget.Action4, "test");

            var controls = device.GetControls(new []{InputTarget.Action1, InputTarget.Action3, InputTarget.Action4});

            Assert.That(controls, Is.EquivalentTo(new [] {action1, action3, action4}));
        }

    }

}
