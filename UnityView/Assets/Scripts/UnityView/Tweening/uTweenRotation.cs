using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace UnityView.Tweening {
	[AddComponentMenu("uTools/Tween/Tween Rotation(uTools)")]
	public class uTweenRotation : uTweenValue<Vector3> {

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

        public Quaternion QuaternionValue
        {
            get
            {
                return cacheTransfrom.localRotation;
            }
            set
            {
                cacheTransfrom.localRotation = value;
            }
        }

		protected override void OnUpdate (float _factor, bool _isFinished)
		{
            QuaternionValue = Quaternion.Euler(Vector3.Lerp(from, to, _factor));
		}

		public static uTweenRotation Begin(GameObject go, Vector3 from, Vector3 to, float duration = 1f, float delay = 0f) {
			uTweenRotation comp = Begin<uTweenRotation>(go, duration);
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