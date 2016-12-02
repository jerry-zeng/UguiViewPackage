using UnityEngine;

namespace UnityView.Model
{
    public struct UIRect
    {
        private static readonly float UnitWidth = Screen.width / (Screen.width > Screen.height ? 1280f : 720f);
        private static readonly float UnitHeight = Screen.height / (Screen.width > Screen.height ? 720f : 1280f);

        public Vector2 Origin;
        public Vector2 Size;

        public float Top
        {
            get { return Origin.y; }
        }

        public float Left
        {
            get { return Origin.x; }
        }

        public float Right
        {
            get { return Left + Width; }
        }

        public float Bottom
        {
            get { return Top + Height; }
        }

        public float Width
        {
            get { return Size.x; }
        }

        public float Height
        {
            get { return Size.y; }
        }
        
        public UIRect(float originX, float originY, float width, float height)
        {
            Origin = new Vector2(originX * UnitWidth, originY * UnitHeight);
            Size = new Vector2(width * UnitWidth, height * UnitHeight);
        }
    }
}
