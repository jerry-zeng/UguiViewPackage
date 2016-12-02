using UnityEngine.UI;

namespace UnityView
{
    class ScrollView : UIView
    {
        public enum ScrollOrientation
        {
            None,
            Freedom,
            Honrizontal,
            Vertical
        }
        public ScrollRect ScrollRect;
        public UIView ContentView;

        public ScrollView() : base()
        {
            ScrollRect = UIObject.AddComponent<ScrollRect>();
            ContentView = new UIView();
            ScrollRect.content = ContentView.RectTransform;
        }

        public void Append(UILayout layout)
        {
            
        }
    }
}
