using System;
using System.Linq;
using UnityEngine;

namespace HouraiTeahouse.SmashBrew {
    /// <summary>
    ///     The pallete swap behaviour for changing out the
    /// </summary>
    [RequiredCharacterComponent]
    [DisallowMultipleComponent]
    [RequireComponent(typeof (PlayerController))]
    public class MaterialSwap : MonoBehaviour {
        private int _color;

        [SerializeField] private Swap[] _swaps;

        /// <summary>
        ///     Gets the number of pallete swaps are available
        /// </summary>
        public int PalleteCount {
            get { return _swaps.Max(swap => swap.SetCount); }
        }


        public int Pallete {
            get { return _color; }
            set {
                _color = value;
                foreach (var swap in _swaps)
                    swap.Set(value);
            }
        }

        [Serializable]
        private class Swap {
            [SerializeField, Tooltip("The set of materials to swap to")] private MaterialSet[] MaterialSets;

            [SerializeField, Tooltip("The set of renderers to apply the materials to")] private Renderer[]
                TargetRenderers;

            public int SetCount {
                get { return MaterialSets.Length; }
            }

            public void Set(int palleteSwap) {
                if (palleteSwap < 0 || palleteSwap >= MaterialSets.Length)
                    return;
                MaterialSets[palleteSwap].Set(TargetRenderers);
            }

            [Serializable]
            public class MaterialSet {
                [SerializeField, Resource(typeof (Material))] [Tooltip("The materials to apply to the renderers")] private string[] _materials;

                public void Set(Renderer[] targets) {
                    if (targets == null)
                        return;
                    var loadedMaterials = new Material[_materials.Length];
                    for (var i = 0; i < loadedMaterials.Length; i++)
                        loadedMaterials[i] = Resources.Load<Material>(_materials[i]);
                    foreach (var renderer in targets)
                        if (renderer)
                            renderer.sharedMaterials = loadedMaterials;
                }
            }
        }
    }
}