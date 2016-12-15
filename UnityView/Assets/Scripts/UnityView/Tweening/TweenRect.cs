using UnityEngine;
using UnityEngine.UI;
using System.Collections;


namespace UnityView 
{
    [AddComponentMenu("uTools/Tween/Tween Rect Size")]  
    public class TweenRect : TweenValue<Vector2> 
    {
        public RectTransform target;


        public RectTransform cacheRectTransform
        {
            get {
                if(target == null) 
                    target = GetComponent<RectTransform>();
                return target;
            }
        }

        public override Vector2 value
        {
            get{
                return cacheRectTransform.sizeDelta;
            }
            set{
                cacheRectTransform.sizeDelta = value;
            }
        }

        protected override void OnUpdate (float factor, bool isFinished) 
        {
            value = from + factor * (to - from);
        }


        public static TweenRect Begin(RectTransform go, Vector2 from, Vector2 to, float duration, float delay) 
        {
            TweenRect comp = Begin<TweenRect>(go.gameObject, duration);
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
