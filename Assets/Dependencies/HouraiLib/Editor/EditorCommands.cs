using System.Linq;
using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;

namespace HouraiTeahouse {

    public static class EditorCommands {

        [MenuItem("Debug/Clear PlayerPrefs %#p")]
        public static void ClearPlayerPrefs() {
            PlayerPrefs.DeleteAll();
        }

    }

}
