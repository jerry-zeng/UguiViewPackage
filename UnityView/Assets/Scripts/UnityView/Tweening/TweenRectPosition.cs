using UnityEngine;
using UnityEngine.UI;
using System.Collections;


namespace UnityView 
{
    [AddComponentMenu("uTools/Tween/Tween Rect Position")]
    public class TweenRectPosition : TweenValue<Vector3> 
    {
        public RectTransform target;


        public RectTransform cachedRectTransform 
        { 
            get{
                if(target == null) 
                    target = GetComponent<RectTransform>(); 
                return target;
            }
        }

        public override Vector3 value
        {
            get { 
                return cachedRectTransform.anchoredPosition;
            }
            set { 
                cachedRectTransform.anchoredPosition = value;
            }
        }

        protected override void OnUpdate (float factor, bool isFinished)
        {
            value = from + factor * (to - from);
        }

        public static TweenPosition Begin(GameObject go, Vector3 from, Vector3 to, float duration = 1f, float delay = 0f)
        {
            TweenPosition comp = Begin<TweenPosition>(go, duration);
            comp.value = from;
            comp.from = from;
            comp.to = to;
            comp.duration = duration;
            comp.delay = delay;

            if (duration <= 0) {
                comp.Sample(1, true);
                comp.enabled = false;
            }
            return comp;
        }
    }
}
