using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace HouraiTeahouse.Editor {

    /// <summary>
    /// Custom editor for all ExtendableObjectEditor.
    /// </summary>
    [InitializeOnLoad]
    [CustomEditor(typeof(ExtendableObject), isFallback = true)]
    public class ExtendableObjectEditor : BaseEditor<ExtendableObject> {

        public class ExtensionType {

            public Type Type;
            public ExtensionAttribute Attribute;

        }

        static readonly Dictionary<Type, ExtensionType[]> Matches;
        ObjectSelector<ExtensionType> Selector;

        static ExtendableObjectEditor() {
            //TODO(james7132): This puts a heavy strain on domain reloads. Consider moving somewhere else.
            Matches =
                ReflectionUtilty.AllTypes.ConcreteClasses()
                    .IsAssignableFrom(typeof(ScriptableObject))
                    .WithAttribute<ExtensionAttribute>()
                    .Select(k => new ExtensionType {Attribute = k.Value, Type = k.Key})
                    .GroupBy(et => et.Attribute.TargetType)
                    .ToDictionary(g => g.Key, g => g.ToArray());
        }

        IEnumerable<ExtensionType> GetTypes(bool required) {
            Type type = target.GetType();
            foreach (Type interfaceType in type.GetInterfaces()) {
                if (!Matches.ContainsKey(interfaceType))
                    continue;
                foreach (ExtensionType extensionType in Matches[interfaceType])
                    if (required == extensionType.Attribute.Required)
                        yield return extensionType;
            }
            while (type != null) {
                if (Matches.ContainsKey(type)) {
                    foreach (ExtensionType extensionType in Matches[type])
                        if (required == extensionType.Attribute.Required)
                            yield return extensionType;
                }
                type = type.BaseType;
            }
        }

        void OnEnable() {
            Selector = new ObjectSelector<ExtensionType>(t => t.Type.Name) {Selections = GetTypes(false).ToArray()};
        }

        public override void OnInspectorGUI() {
            DrawDefaultInspector();
            DrawExtensionGUI();
        }

        public void DrawExtensionGUI() {
            if (Target == null)
                return;
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Extensions", EditorStyles.boldLabel);
            using (new EditorGUILayout.HorizontalScope()) {
                ExtensionType selection = Selector.Draw(GUIContent.none);
                if (GUILayout.Button("Add") && selection != null) {
                    Undo.RecordObject(Target, "Add Extension");
                    Target.AddExtension(selection.Type);
                    Repaint();
                }
            }
            foreach (ScriptableObject extension in Target.Extensions.ToArray()) {
                using (new EditorGUILayout.HorizontalScope()) {
                    EditorGUILayout.InspectorTitlebar(true, extension);
                    if (GUILayout.Button(GUIContent.none, "ToggleMixed", GUILayout.Width(15))) {
                        Undo.RecordObject(Target, "Remove Extension");
                        Target.RemoveExtension(extension);
                        Repaint();
                    }
                }
                EditorGUILayout.Space();
                if (extension != null)
                    CreateEditor(extension).OnInspectorGUI();
            }
        }

    }

}