﻿using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace UnityView.Tweening {
	[AddComponentMenu("uTools/Tween/Tween Color(uTools)")]
	public class uTweenColor : uTweenValue<Color> {
		
		public GameObject target;
		public bool includeChildren = false;

		Graphic[] mGraphics;
        Graphic[] cachedGraphics
        {
            get
            {
                if (mGraphics == null)
                {
                    if (target == null)
                    {
                        target = gameObject;
                    }
                    mGraphics = includeChildren ? target.GetComponentsInChildren<Graphic>() : target.GetComponents<Graphic>();
                }
                return mGraphics;
            }
        }

		Color mColor = Color.white;

		protected override void Start ()
		{
			base.Start ();
		}

		public override Color value {
			get {
				return mColor;
			}
			set {
				SetColor(transform, value);
				mColor = value;
			}
		}

		protected override void OnUpdate (float factor, bool isFinished)
		{
            value = Color.Lerp(from, to, factor);
		}

        public static uTweenColor Begin(GameObject go, Color from, Color to, float duration = 1f, float delay = 0f)
        {
            uTweenColor comp = Begin<uTweenColor>(go, duration);
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

        void SetColor(Transform _transform, Color _color) {
			foreach (var item in cachedGraphics) {
				item.color = _color;
			}			
		}


	}
}
