using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using HouraiTeahouse.SmashBrew.Characters;
using HouraiTeahouse.SmashBrew.Characters.Statuses;
using NUnit.Framework;
using UnityEngine;

namespace HouraiTeahouse.SmashBrew {

    internal class AbstractDataTest<T> where T : ScriptableObject, IGameData {

        protected delegate object AssetFunc(T data);

        protected delegate IEnumerable AssetManyFunc(T data);

        protected static IEnumerable<T> data;

        protected void LoadData() {
            if (data == null)
                data = Resources.LoadAll<T>(string.Empty).Where(d => d != null && d.IsSelectable && d.IsVisible);
        }

        protected void Check(AssetFunc func) {
            LoadData();
            foreach (T datum in data) {
                object result = func(datum);
                if (result == null)
                    Log.Error("{0} is invalid!".With(datum));
                Assert.NotNull(result);
            }
        }

        protected void CheckMany(AssetManyFunc func) {
            LoadData();
            foreach (T datum in data) {
                foreach (object obj in func(datum)) {
                    if (obj == null)
                        Log.Error("{0} is invalid!".With(data));
                    Assert.NotNull(obj);
                }
            }
        }

    }

    /// <summary> Tests for CharacterData instances </summary>
    internal class CharacterDataTest : AbstractDataTest<CharacterData> {

        [Test]
        public void every_character_prefab_has_disabled_statuses() {
            LoadData();
            foreach (CharacterData character in data) {
                Assert.NotNull(character.Prefab.Load());
                foreach (Status status in character.Prefab.Load().GetComponentsInChildren<Status>())
                    Assert.False(status.enabled);
            }
        }

        [Test]
        public void every_character_has_a_prefab() => Check(d => d.Prefab.Load());


        [Test]
        public void every_character_prefab_has_character_component() =>
            Check(d => d.Prefab.Load().GetComponent<Character>());

        [Test]
        public void every_character_has_equal_pallete_and_portrait_counts() {
            // Checks that the pallete count is the same between MaterialSwap and CharacterData
            LoadData();
            foreach (CharacterData character in data) {
                var swap = character.Prefab.Load().GetComponent<ColorState>();
                Assert.AreEqual(swap.Count, character.PalleteCount);
            }
        }

        [Test]
        public void every_character_has_valid_portraits() {
            // Checks that all of the portraits for each of the character is not null
            LoadData();
            foreach (CharacterData character in data)
                for (var i = 0; i < character.PalleteCount; i++)
                    Assert.NotNull(character.GetPortrait(i).Load());
        }

        [Test]
        public void every_character_has_valid_icons() => Check(d => d.Icon.Load());

        [Test]
        public void every_character_has_valid_home_stage() => Check(d => d.HomeStage.Load());

        [Test]
        public void every_character_has_valid_victory_theme() => Check(d => d.VictoryTheme.Load());

    }

}
