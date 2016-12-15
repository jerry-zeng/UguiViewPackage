using UnityEngine;
using UnityEngine.UI;
using System.Collections;


namespace UnityView 
{
    [AddComponentMenu("uTools/Tween/Tween Rotation")]
    public class TweenRotation : TweenValue<Vector3> 
    {
        public Transform target;


        public Transform cacheTransfrom 
        {
            get { 
                if(target == null)
                    target = transform;
                return target;
            }
        }

        public override Vector3 value
        {
            get{
                return cacheTransfrom.localEulerAngles;
            }
            set{
                cacheTransfrom.localRotation = Quaternion.Euler( value );
            }
        }

        protected override void OnUpdate (float factor, bool isFinished)
        {
            value = Vector3.Lerp(from, to, factor);
        }


        public static TweenRotation Begin(GameObject go, Vector3 from, Vector3 to, float duration = 1f, float delay = 0f) 
        {
            TweenRotation comp = Begin<TweenRotation>(go, duration);
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