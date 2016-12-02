using System.Collections.Generic;
using UnityEngine;

namespace UnityView.Model
{
    public struct UIBorder
    {
        public Color Color;
        public Vector2 Offset;
        public bool UseGraphicAlpha;

        public UIBorder(Color color, Vector2 offset, bool useGraphicAlpha = false)
        {
            Color = color;
            Offset = offset;
            UseGraphicAlpha = useGraphicAlpha;
        }
    }
}
