using UnityEngine;
using UnityEngine.Events;

namespace UnityView
{
    public class UIDialog : AbsDialog
    {
        public static void ShowDialog(string title)
        {
            
        }

        public static void ShowDialog(string title, string pTitle)
        {

        }

        public static void ShowDialog(string title, string pTitle, UnityAction pAction)
        {

        }
        public static void ShowDialog(string title, string pTitle, UnityAction pAction, string nTitle, UnityAction nAction)
        {

        }

        public TextView TitleView;
        public ButtonView PositiveButton;
        public UnityAction PositiveAction;
        public ButtonView NagetiveButton;
        public UnityAction NagetiveAction;

        public UIDialog(string title, string pTitle, UnityAction pAction, string nTitle, UnityAction nAction)
        {
            //float width = Mathf.Min(Screen.width, Screen.height) / 3f;
            Rect = new Rect();
            TitleView = new TextView()
            {
                Text = title
            };
        }
    }
}
