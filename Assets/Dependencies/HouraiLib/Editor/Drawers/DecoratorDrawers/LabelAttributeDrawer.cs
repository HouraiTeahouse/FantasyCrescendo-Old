using HouraiTeahouse;
using UnityEditor;
using UnityEngine;

/// <summary>
/// Custom Decorator Drawer for LabelAttribute
/// </summary>
[CustomPropertyDrawer(typeof(LabelAttribute))]
public class LabelAttributeDrawer : DecoratorDrawer {

	public override void OnGUI(Rect position) {
		var label = attribute as LabelAttribute;
	    if (label.Label == null)
	        EditorGUI.LabelField(position, label.Label);
        else
		    EditorGUI.LabelField(position, label.Label, label.Text);
	}

}
