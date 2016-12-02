using UnityEngine;

namespace UnityView
{
    public abstract class AbsDialog : UILayout
    {
        // 背景按钮
        public ButtonView BackgroundButton;
        // 是否开启区域外点击消失
        private bool _enableOutboundDismiss = false;
        public bool EnableOutboundDismiss
        {
            set
            {
                if (value)
                {
                    BackgroundButton.OnClickListener.RemoveListener(Destory);
                    BackgroundButton.OnClickListener.AddListener(Destory);
                }
                else
                {
                    BackgroundButton.OnClickListener.RemoveAllListeners();
                }
                _enableOutboundDismiss = value;
            }
            get
            {
                return _enableOutboundDismiss;
            }
        }

        public UIView ContentView;
        // 默认的内容大小
        public static Rect DefaultContentRect;
        protected AbsDialog()
        {
            RectFill(RectTransform);
            BackgroundButton = new ButtonView(this) {UIObject = {name = "Background View"}};
            RectFill(BackgroundButton);
            BackgroundButton.BackgroundColor = Color.clear;
            ContentView = new UIView(this);
            ContentView.UIObject.name = "Content View";
            ContentView.Rect = DefaultContentRect;
        }

        public void Show()
        {
            UIObject.SetActive(true);
        }

        public void Dismiss()
        {
            UIObject.SetActive(false);
        }
    }
}
