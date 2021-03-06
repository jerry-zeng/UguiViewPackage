﻿using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using System.Collections;


namespace UnityView 
{
    [AddComponentMenu("uTools/Tween/Button Scale")]
    public class ButtonScale : MonoBehaviour, IPointerEnterHandler,IPointerDownHandler,IPointerClickHandler,IPointerUpHandler,IPointerExitHandler 
    {
        public Transform tweenTarget;
        public Vector3 enter = new Vector3(1.1f, 1.1f, 1.1f);
        public Vector3 down = new Vector3(1.05f, 1.05f, 1.05f);
        public float duration = 0.2f;


        protected Vector3 mScale;

        void Start () {
            if (tweenTarget == null) 
                tweenTarget = GetComponent<Transform>();

            mScale = tweenTarget.localScale;
        }

        public void OnPointerEnter (PointerEventData eventData)
        {
            Scale(enter);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            Scale(mScale);
        }

        public void OnPointerDown (PointerEventData eventData)
        {
            Scale(down);
        }

        public void OnPointerUp (PointerEventData eventData)
        {
            Scale(mScale);
        }

        public void OnPointerClick (PointerEventData eventData)
        {
            
        }

        void Scale(Vector3 to)
        {
            TweenScale.Begin(tweenTarget.gameObject, tweenTarget.localScale, to, duration);
        }
    }
}
