using System.Collections.Generic;
using UnityEngine;

public class BoxOutline : ModifiedShadow {
    private const int maxHalfSampleCount = 20;

    [SerializeField] [Range(1, maxHalfSampleCount)] private int m_halfSampleCountX = 1;
    [SerializeField] [Range(1, maxHalfSampleCount)] private int m_halfSampleCountY = 1;

    public int halfSampleCountX {
        get { return m_halfSampleCountX; }

        set {
            m_halfSampleCountX = Mathf.Clamp(value, 1, maxHalfSampleCount);
            if (graphic != null)
                graphic.SetVerticesDirty();
        }
    }

    public int halfSampleCountY {
        get { return m_halfSampleCountY; }

        set {
            m_halfSampleCountY = Mathf.Clamp(value, 1, maxHalfSampleCount);
            if (graphic != null)
                graphic.SetVerticesDirty();
        }
    }

    public override void ModifyVertices(List<UIVertex> verts) {
        if (!IsActive())
            return;

        verts.Capacity = verts.Count * (m_halfSampleCountX * 2 + 1) * (m_halfSampleCountY * 2 + 1);
        var original = verts.Count;
        var count = 0;
        var dx = effectDistance.x / m_halfSampleCountX;
        var dy = effectDistance.y / m_halfSampleCountY;
        for (var x = -m_halfSampleCountX; x <= m_halfSampleCountX; x++) {
            for (var y = -m_halfSampleCountY; y <= m_halfSampleCountY; y++) {
                if (!(x == 0 && y == 0)) {
                    var next = count + original;
                    ApplyShadow(verts, effectColor, count, next, dx * x, dy * y);
                    count = next;
                }
            }
        }
    }
}