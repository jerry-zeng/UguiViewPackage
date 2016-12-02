using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;

namespace UnityView.Tweening {
	[AddComponentMenu("uTools/Tween/Tween Scale(uTools)")]
	
	public class uTweenScale : uTweenValue<Vector3> {

        public Transform target;

        Transform mTransfrom;
        public Transform cacheTransfrom {
            get { 
                if (target == null) {
                    mTransfrom = GetComponent<Transform>();
                }
                else {
                    mTransfrom = target;
                }
                return mTransfrom;          
            }
        }

        public override Vector3 value {
            get { return cacheTransfrom.localScale;}
            set { cacheTransfrom.localScale = value;}
		}

		protected override void OnUpdate (float factor, bool isFinished)
		{
			value = from + factor * (to - from);
		}

		public static uTweenScale Begin(GameObject go, Vector3 from, Vector3 to, float duration = 1f, float delay = 0f) {
			uTweenScale comp = Begin<uTweenScale>(go, duration);
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
