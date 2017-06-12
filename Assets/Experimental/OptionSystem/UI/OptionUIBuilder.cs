using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;

namespace HouraiTeahouse.Options.UI {

    // A quick example of a Slider UI Builder for testing purposes only.
    public class OptionUIBuilder : MonoBehaviour, ISerializationCallbackReceiver {

        static readonly ILog _log = Log.GetLogger<OptionUIBuilder>();

        [Serializable]
        public class ViewMapping {
            public string Type;
            public RectTransform Prefab;
        }

        [SerializeField]
        OptionSystem optionSystem;

        [Header("Layout")]
        [SerializeField, Range(0f, 1f)]
        float _labelSize = 0.35f;

        [SerializeField, Range(0f, 1f)]
        float _indent = 0.05f;

        [SerializeField]
        float _optionHeight = 40;

        [Header("Subcomponents")]
        [SerializeField]
        RectTransform categoryLabelTemplate;
        [SerializeField]
        RectTransform optionLabelTemplate;

        [SerializeField]
        ViewMapping[] _mappings;

        Dictionary<Type, RectTransform> _prefabs;
        static readonly Dictionary<Type, AbstractOptionViewAttribute> _defaultViews;

        void RefreshTypes() {
            var viewType = typeof(AbstractOptionViewAttribute);
            var types = Assembly.GetExecutingAssembly().GetTypes().Where(
                            t => !t.IsAbstract && viewType.IsAssignableFrom(t));
            if (_prefabs == null)
                _prefabs = new Dictionary<Type, RectTransform>();
            foreach(var type in types)
                if (!_prefabs.ContainsKey(type))
                    _prefabs[type] = null;
        }

        void Reset() {
            RefreshTypes();
        }

        void ISerializationCallbackReceiver.OnBeforeSerialize() {
            var list = new List<ViewMapping>();
            RefreshTypes();
            _mappings = _prefabs.Select(kv => new ViewMapping {
                Type = kv.Key.FullName,
                Prefab = kv.Value
            }).ToArray();
        }

        void ISerializationCallbackReceiver.OnAfterDeserialize() {
            if (_mappings == null)
                return;
            _prefabs = new Dictionary<Type, RectTransform>();
            foreach(var mapping in _mappings) {
                var type = Type.GetType(mapping.Type);
                if (type == null)
                    continue;
                _prefabs.Add(type, mapping.Prefab);
            }
        }

        static OptionUIBuilder() {
            _defaultViews = new Dictionary<Type, AbstractOptionViewAttribute> {
                {typeof(int), new IntField()},
                {typeof(float), new IntField()},
                {typeof(bool), new Toggle()},
                {typeof(string), new TextField()}
            };
        }


        void Start() {
            foreach (CategoryInfo category in optionSystem.Categories) {
                var categoryLabel = Instantiate(categoryLabelTemplate);
                categoryLabel.name = category.Name;
                categoryLabel.transform.SetParent(transform, false);
                categoryLabel.GetComponentInChildren<Text>().text = category.Name;
                foreach (OptionInfo option in category.Options) {
                    var drawer = option.PropertyInfo.GetCustomAttributes(true).OfType<AbstractOptionViewAttribute>().FirstOrDefault();
                    var propertyInfo = option.PropertyInfo;
                    if (drawer == null && !_defaultViews.TryGetValue(propertyInfo.PropertyType, out drawer)) {
                        _log.Error("No drawer and no default drawer can be found for {0} ({1})", 
                            propertyInfo, propertyInfo.PropertyType);
                        continue;
                    }
                    var drawerType = drawer.GetType();
                    if (!_prefabs.ContainsKey(drawerType) || _prefabs[drawerType] == null) {
                        _log.Error("No prefab for drawer {0} could be found.", drawerType);
                        continue;
                    }
                    var container = new GameObject(option.Name, typeof(RectTransform));
                    var containerLayout = container.AddComponent<LayoutElement>();
                    containerLayout.preferredHeight = _optionHeight;
                    containerLayout.flexibleHeight = 0f;
                    container.transform.SetParent(categoryLabel.transform, false);
                    var optionLabel = Instantiate(optionLabelTemplate);
                    var control = Instantiate(_prefabs[drawerType]);
                    optionLabel.SetParent(container.transform, false);
                    control.SetParent(container.transform, false);
                    FillRect(optionLabel, _indent, _labelSize);
                    FillRect(control, _indent + _labelSize, 1 - (_indent + _labelSize));
                    optionLabel.GetComponentInChildren<Text>().text = option.Name;
                    if (drawer == null)
                        continue;
                    drawer.BuildUI(option, control.gameObject);
                }
            }
        }

        void FillRect(RectTransform rect, float start, float size) {
            rect.anchorMin = new Vector2(start, 0);
            rect.anchorMax = new Vector2(start + size, 1f);
            rect.offsetMax = Vector2.zero;
            rect.offsetMin = Vector2.zero;
        }
    }

    
}
