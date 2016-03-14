using System;
using UnityEngine;

namespace HouraiTeahouse {
    /// <summary>
    ///     Disables image effects if the current system does not support them
    /// </summary>
    public class ShaderModelCheck : MonoBehaviour {
        [SerializeField] private ShaderModelSet[] _shaderSets;

        /// <summary>
        ///     Unity callback. Called on object instantiation.
        /// </summary>
        private void Awake() {
            var shaderModel = SystemInfo.graphicsShaderLevel;
            foreach (var set in _shaderSets)
                foreach (var behaviour in set.Components)
                    if (behaviour)
                        behaviour.enabled &= shaderModel > set.MinimumShaderModel;
        }

        [Serializable]
        private struct ShaderModelSet {
            public int MinimumShaderModel;
            public Behaviour[] Components;
        }
    }
}