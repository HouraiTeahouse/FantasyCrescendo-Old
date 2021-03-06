using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using HouraiTeahouse.SmashBrew.Characters;
using HouraiTeahouse.SmashBrew.Characters.Statuses;
using NUnit.Framework;
using UnityEngine;
using UnityEditor;
using HouraiTeahouse.Editor;

namespace HouraiTeahouse.SmashBrew {

    internal class AbstractDataTest<T> where T : ScriptableObject, IGameData {

        protected delegate object AssetFunc(T data);

        protected delegate IEnumerable AssetManyFunc(T data);

        protected static IEnumerable<T> data;

        public static IEnumerable<object[]> TestData() {
            if (data == null)
                data = Assets.LoadAll<T>().Where(d => d != null && d.IsSelectable && d.IsVisible);
            foreach (var datum in data)
                yield return new object[] {datum};
        }

    }

    /// <summary> 
    /// Tests for CharacterData instances.
    /// </summary>
    /// <remarks>
    /// Note: These test function as validation for on the data available at build time.
    /// If the data is invalid, these tests will fail.
    /// </remarks>
    internal class CharacterDataTest : AbstractDataTest<CharacterData> {

        [Test, TestCaseSource("TestData")]
        public void has_disabled_statuses(CharacterData character) {
            Assert.NotNull(character.Prefab.Load());
            foreach (Status status in character.Prefab.Load().GetComponentsInChildren<Status>())
                Assert.False(status.enabled);
        }

        [Test, TestCaseSource("TestData")]
        public void has_a_prefab(CharacterData character) {
            Assert.NotNull(character.Prefab.Load());
        }

        [Test, TestCaseSource("TestData")]
        public void has_character_component(CharacterData character) {
            Assert.NotNull(character.Prefab.Load().GetComponent<Character>());
        }

        [Test, TestCaseSource("TestData")]
        public void has_equal_pallete_and_portrait_counts(CharacterData character) {
            var swap = character.Prefab.Load().GetComponent<ColorState>();
            Assert.NotNull(swap);
            Assert.AreEqual(swap.Count, character.PalleteCount);
        }

        [Test, TestCaseSource("TestData")]
        public void has_valid_portraits(CharacterData character) {
            for (var i = 0; i < character.PalleteCount; i++) {
                Assert.NotNull(character.GetPortrait(i).Load());
            }
        }

        [Test, TestCaseSource("TestData")]
        public void has_valid_icons(CharacterData character) {
            Assert.NotNull(character.Icon.Load());
        }

        [Test, TestCaseSource("TestData")]
        public void has_valid_home_stage(CharacterData character) {
            Assert.NotNull(character.HomeStage.Load());
        }

        [Test, TestCaseSource("TestData")]
        public void has_valid_victory_theme(CharacterData character) {
            Assert.NotNull(character.VictoryTheme.Load());
        }

    }

}
