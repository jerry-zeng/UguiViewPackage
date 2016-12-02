using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace UnityView.Tweening {
	[AddComponentMenu("uTools/Tween/Tween Text(uTools)")]	
	
	public class uTweenText : uTweenValue<float> {

		private Text mText;
		public Text cacheText {
			get {
				if (mText == null) {
					mText = GetComponent<Text>();
				}
				return mText;
			}
		}

        /// <summary>
        /// number after the digit point
        /// </summary>
        public int digits;

        public override float value
        {
            get{
                return base.value;
            }
            set{
                base.value = value;
                cacheText.text = (System.Math.Round(value, digits)).ToString();
            }
        }

        protected override void OnUpdate(float factor, bool isFinished)
		{
            value = from + (to - from) * factor;
		}

		public static uTweenText Begin(Text label, float from, float to, float duration, float delay) {
			uTweenText comp = Begin<uTweenText>(label.gameObject, duration);
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
