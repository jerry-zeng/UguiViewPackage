using UnityEngine;
using UnityEngine.UI;

namespace UnityView
{
    public class UICanvas : UILayout
    {
        private static UICanvas _instance;

        public Canvas Canvas;
        public CanvasScaler CanvasScaler;
        public GraphicRaycaster GraphicRaycaster;
        public static UICanvas GetInstance()
        {
            return _instance ??
                   (_instance = new UICanvas());
        }

        private UICanvas() : base(Object.FindObjectOfType<Canvas>().gameObject ?? new GameObject("Canvas"))
        {
            Canvas = UIObject.GetComponent<Canvas>() ?? UIObject.AddComponent<Canvas>();
            CanvasScaler = UIObject.GetComponent<CanvasScaler>() ?? UIObject.AddComponent<CanvasScaler>();
            GraphicRaycaster = UIObject.GetComponent<GraphicRaycaster>() ?? UIObject.AddComponent<GraphicRaycaster>();
        }

        public static void Discard()
        {
            _instance = null;
        }
    }
}
