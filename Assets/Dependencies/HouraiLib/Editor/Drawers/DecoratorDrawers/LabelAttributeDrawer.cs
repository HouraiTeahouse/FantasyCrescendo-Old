using UnityEditor;

/// <summary>
/// Custom Decorator Drawer for LabelAttribute
/// </summary>
[CustomPropertyDrawer(typeof(LabelAttribute))]
public class LabelAttributeDrawer : DecoratorDrawer {

	public override void OnGUI(Rect position) {
		var label = attribute as LabelAttribute;
		EditorGUI.LabelField(position, label.label, label.text);
	}

}
