using UnityEngine;
using UnityEngine.UI;
using System.Collections;


namespace UnityView 
{
    [AddComponentMenu("uTools/Tween/Tween Alpha")]
    public class TweenAlpha : TweenValue<float> 
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
                    if (target == null)
                        target = gameObject;
                    
                    mGraphics = includeChildren ? target.GetComponentsInChildren<Graphic>() : target.GetComponents<Graphic>();
                }
                return mGraphics;
            }
        }

        protected float mAlpha = 0f;
        public override float value
        {
            get{
                return mAlpha;
            }
            set{
                mAlpha = value;

                for( int i = 0; i < cachedGraphics.Length; i++ )
                {
                    Color color = cachedGraphics[i].color;
                    color.a = value;
                    cachedGraphics[i].color = color;
                }
            }
        }

        protected override void OnUpdate (float factor, bool isFinished)
        {
            value = factor;
        }


        public static TweenAlpha Begin(GameObject go, float from, float to, float duration = 1f, float delay = 0f)
        {
            TweenAlpha comp = Begin<TweenAlpha>(go, duration);
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