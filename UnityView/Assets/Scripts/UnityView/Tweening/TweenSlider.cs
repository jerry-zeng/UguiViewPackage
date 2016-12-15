using UnityEngine;
using UnityEngine.UI;
using System.Collections;


namespace UnityView
{
    [AddComponentMenu("uTools/Tween/Tween Slider")] 
    public class TweenSlider : TweenValue<float> 
    {

        private Slider mSlider;
        public Slider cacheSlider 
        {
            get {
                if(mSlider == null)
                    mSlider = GetComponent<Slider>();
                return mSlider;
            }
        }


        public override float value
        {
            get{
                return cacheSlider.value;
            }
            set{
                cacheSlider.value = value;
            }
        }

        protected override void OnUpdate (float factor, bool isFinished)
        {
            value = factor;
        }

        public static TweenSlider Begin(Slider slider, float from, float to, float duration, float delay)
        {
            TweenSlider comp = Begin<TweenSlider>(slider.gameObject, duration);
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
