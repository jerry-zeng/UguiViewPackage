using UnityEngine;

namespace UnityView.Model
{
    public struct UIFrame
    {
        private static readonly float UnitWidth = Screen.width / (Screen.width > Screen.height ? 1280f : 720f);
        private static readonly float UnitHeight = Screen.height / (Screen.width > Screen.height ? 720f : 1280f);

        public Vector2 Origin;
        public Vector2 Size;

        public UIFrame(Vector2 origin, Vector2 size)
        {
            Origin = origin;
            Size = size;
        }

        public float Width
        {
            get { return Size.x; }
        }

        public float Height
        {
            get { return Size.y; }
        }
        public UIFrame(float originX, float originY, float width, float height)
        {
            Origin = new Vector2(originX * UnitWidth, originY * UnitHeight);
            Size = new Vector2(width * UnitWidth, height * UnitHeight);
        }
    }
}
