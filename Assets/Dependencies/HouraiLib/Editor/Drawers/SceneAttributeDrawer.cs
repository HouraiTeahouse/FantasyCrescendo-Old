using UnityEngine;
using UnityEditor;
using System;
using System.Collections.Generic;
using System.Linq;

namespace HouraiTeahouse.Editor {

    /// <summary> 
    /// Custom PropertyDrawer for SceneAttribute 
    /// </summary>
    [CustomPropertyDrawer(typeof(SceneAttribute))]
    internal class SceneAttributeDrawer : PropertyDrawer {

        readonly Dictionary<string, Data> _data;

        public SceneAttributeDrawer() { 
            _data = new Dictionary<string, Data>(); 
        }

        class Data {

            SceneAsset _object;
            string _path;
            public readonly GUIContent Content;

            public Data(SerializedProperty property, GUIContent content) {
                _path = property.stringValue;
                string path = string.Format("Assets/{0}.unity", _path);
                if (Assets.IsBundlePath(_path)) {
                    string[] parts = _path.Split(Resource.BundleSeperator);
                    var paths = AssetDatabase.GetAssetPathsFromAssetBundleAndAssetName(parts[0], parts[1]);
                    path = paths.FirstOrDefault() ?? path;
                }
                _object = AssetDatabase.LoadAssetAtPath<SceneAsset>(path);
                Content = new GUIContent(content);
                UpdateContent(content);
            }

            bool IsValid {
                get { return !_path.IsNullOrEmpty(); }
            }

            public void Draw(Rect position, SerializedProperty property, Type type) {
                EditorGUI.BeginChangeCheck();
                SceneAsset obj;
                using (HGUI.Color(IsValid ? GUI.color : Color.red))
                    obj = EditorGUI.ObjectField(position, Content, _object, type, false) as SceneAsset;
                if (!EditorGUI.EndChangeCheck())
                    return;
                Update(obj);
                property.stringValue = _path;
                EditorUtility.SetDirty(property.serializedObject.targetObject);
            }

            public void UpdateContent(GUIContent label) {
                string message;
                if (!_object) {
                    message = "No object specified";
                } else if (!IsValid) {
                    message = "Not in Build Settings or Asset Bundle. Will not be saved.";
                } else if (_path.IndexOf(Resource.BundleSeperator) >= 0) {
                    string[] splits = _path.Split(Resource.BundleSeperator);
                    message = string.Format("Asset Bundle: {1}\nPath:{1}", splits[0], splits[1]);
                } else {
                    message = string.Format("Path: {0}", _path);
                }

                Content.tooltip = label.tooltip.IsNullOrEmpty() ? message : string.Format("{0}\n{1}", label.tooltip, message);
            }

            void Update(SceneAsset obj) {
                _object = obj;
                var scenePath = Assets.GetScenePath(obj);
                var bundleName = Assets.GetBundlePath(obj);
                _path = scenePath ?? bundleName ?? string.Empty;
            }

        }


        /// <summary>
        ///     <see cref="PropertyDrawer.OnGUI" />
        /// </summary>
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
            if (property.propertyType != SerializedPropertyType.String) {
                EditorGUI.PropertyField(position, property, label);
                return;
            }

            string propertyPath = property.propertyPath;
            Data data;
            if (!_data.TryGetValue(propertyPath, out data)) {
                data = new Data(property, label);
                _data[propertyPath] = data;
            }

            using (HGUI.Property(data.Content, position, property)) {
                data.UpdateContent(label);
                data.Draw(position, property, typeof(SceneAsset));
                _data[propertyPath] = data;
            }
        }

    }
}
