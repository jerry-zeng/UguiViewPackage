using UnityEngine;
using UnityEngine.UI;
using System.Collections;


namespace UnityView 
{
    [AddComponentMenu("uTools/Tween/Tween Text")]   
    public class TweenText : TweenValue<float> 
    {
        public int digits;


        private Text mText;
        public Text cacheText
        {
            get {
                if (mText == null)
                    mText = GetComponent<Text>();
                return mText;
            }
        }

        protected float mValue = 0f;
        public override float value
        {
            get{
                return mValue;
            }
            set{
                mValue = (float)System.Math.Round(value, digits);

                cacheText.text = mValue.ToString();
            }
        }

        protected override void OnUpdate(float factor, bool isFinished)
        {
            value = from + (to - from) * factor;
        }

        public static TweenText Begin(Text label, float from, float to, float duration, float delay)
        {
            TweenText comp = Begin<TweenText>(label.gameObject, duration);
            comp.value = from;
            comp.from = from;
            comp.to = to;
            comp.delay = delay;

            if (duration <=0) {
                comp.Sample(1, true);
                comp.enabled = false;
            }
            return comp;
        }
    }
}
