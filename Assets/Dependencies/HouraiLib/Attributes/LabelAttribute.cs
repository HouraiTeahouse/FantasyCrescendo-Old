using System;
using UnityEngine;

namespace HouraiTeahouse {

    /// <summary>
    /// A PropertyAttribute that exposes a Layer control on the editor
    /// </summary>
    public class LabelAttribute : PropertyAttribute {

	    public string Label { get; set;}

	    public string Text { get; set; }

	    public LabelAttribute(string text) : this(null, text) {
	    }

	    public LabelAttribute(string label, string text) {
		Label = label;
		Text = text;
	    }

    }

}
