﻿using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;


namespace UnityView
{
    [AddComponentMenu("uTools/Tween/Shake")]
    public class TweenShake : Tweener
    {
        public enum EShake
        {
            Position,
            Rotation,
            Scale
        }

        private Transform mTarget;
        public Transform target
        {
            get
            {
                if (mTarget == null)
                {
                    mTarget = transform;
                    CacheTargetInfo();
                }
                return mTarget;
            }
        }

        private Vector3 localPosition = Vector3.zero;
        private Vector3 position = Vector3.zero;
        private Vector3 localScale = Vector3.zero;
        private Vector3 localEulerAngles = Vector3.zero;
        private Vector3 eulerAngles = Vector3.zero;

        [SerializeField]
        protected Vector3 limit;
        public Space space = Space.Self;
        public EShake shakeType = EShake.Position;

        private Vector3 mValue;
        public Vector3 value
        {
            get
            {
                return mValue;
            }
            set
            {
                mValue = value;
                Shake();
            }
        }

        private Transform mTransform = null;
        private RectTransform mRectTransform = null;
        private bool mIs3D = true;

        private bool is3D
        {
            get
            {
                if (mTransform == null)
                {
                    //init once
                    mTransform = transform;
                    mRectTransform = target as RectTransform;
                    mIs3D = (mRectTransform != null) ? false : true;
                }
                return mIs3D;
            }
            set
            {
                mIs3D = value;
            }
        }

        private void CacheTargetInfo()
        {
            localPosition = target.localPosition;
            position = target.position;
            localScale = target.localScale;
            localEulerAngles = target.localEulerAngles;
            eulerAngles = target.eulerAngles;
        }

        protected override void Start()
        {
            base.Start();
        }


        protected override void OnUpdate(float factor, bool isFinished)
        {
            factor = 1 - factor;

            float x = limit.x * factor;
            float y = limit.y * factor;
            float z = limit.z * factor;

            mValue.x = Random.Range(x * -1, x);
            mValue.y = Random.Range(y * -1, y);
            mValue.z = Random.Range(z * -1, z);

            value = mValue;
        }

        private Vector3 tempVector3 = Vector3.one;
        protected void Shake()
        {
            if (shakeType == EShake.Position)
            {
                if (space == Space.Self)
                {
                    tempVector3 = value + localPosition;
                    if (is3D)
                        target.localPosition = tempVector3;
                    else
                        mRectTransform.anchoredPosition = tempVector3;
                }
                else
                {
                    tempVector3 = value + position;
                    if (is3D)
                        target.position = tempVector3;
                    else
                        mRectTransform.anchoredPosition3D = tempVector3;
                }
            }
            else if (shakeType == EShake.Scale)
            {
                target.localScale = value + localScale;
            }
            else
            {
                if (space == Space.Self)
                {
                    target.localEulerAngles = value + localEulerAngles;
                }
                else
                {
                    target.eulerAngles = value + eulerAngles;
                }
            }
        }

        public static TweenShake Begin(GameObject go, Vector3 from, float duration = 1f, float delay = 0f)
        {
            TweenShake comp = Tweener.Begin<TweenShake>(go, duration);
            comp.limit = from;
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