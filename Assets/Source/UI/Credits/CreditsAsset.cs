using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;

namespace HouraiTeahouse {

    /// <summary>
    /// Custom asset for allowing easy editing of the game's credits.!
    /// Used with <see cref="CreditsUIBuilder"/> to automatically generate the UI on the credits screen.
    /// </summary>
    [CreateAssetMenu(menuName = "Fantasy Crescendo/Credits Asset")]
    public class CreditsAsset : ScriptableObject {

        [Serializable]
        public class Category {
            //TODO(james7132): Make these readonly.
            public string Name;
            public string[] Contributors;

            public override string ToString() {
                if (Contributors != null && Contributors.Length == 1)
                    return Name + ": " + Contributors[0];
                return Name;
            }
        }

        /// <summary>
        /// Gets aread-only list of categories.
        /// </summary>
        public ReadOnlyCollection<Category> Categories 
            => new ReadOnlyCollection<Category>(_categories);

        [SerializeField]
        Category[] _categories;

    }


}
