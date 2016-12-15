using UnityEngine;
using UnityEngine.UI;
using System.Collections;


namespace UnityView 
{
    [AddComponentMenu("uTools/Tween/Tween Color")]
    public class TweenColor : TweenValue<Color> 
    {
        public GameObject target;
        public bool includeChildren = false;


        protected Graphic[] mGraphics;
        protected Graphic[] cachedGraphics
        {
            get
            {
                if (mGraphics == null)
                {
                    if(target == null)
                        target = gameObject;
                    
                    mGraphics = includeChildren ? target.GetComponentsInChildren<Graphic>() : target.GetComponents<Graphic>();
                }
                return mGraphics;
            }
        }


        protected Color mColor = Color.white;
        public override Color value
        {
            get {
                return mColor;
            }
            set {
                mColor = value;

                for( int i = 0; i < cachedGraphics.Length; i++ )
                    cachedGraphics[i].color = value;
            }
        }

        protected override void OnUpdate(float factor, bool isFinished)
        {
            value = Color.Lerp(from, to, factor);
        }


        public static TweenColor Begin(GameObject go, Color from, Color to, float duration = 1f, float delay = 0f)
        {
            TweenColor comp = Begin<TweenColor>(go, duration);
            comp.value = from;
            comp.value = from;
            comp.from = from;
            comp.to = to;
            comp.duration = duration;
            comp.delay = delay;

            if (duration <= 0)
            {
                comp.Sample(1, true);
                comp.enabled = false;
            }
            return comp;
        }

    }
}
