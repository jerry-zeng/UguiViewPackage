using UnityEngine;
using System.Collections;


namespace UnityView
{
    [AddComponentMenu("uTools/Tween/Bezier")]
    public class Bezier : Tweener
    {
        public Transform target;
        public Vector3[] path;
        public bool isWorld = false;

        private float from = 0f;
        private float to = 1f;


        public Transform cacheTransfrom 
        {
            get { 
                if(target == null)
                    target = transform;
                return target;
            }
        }

        private float mValue;
        public float value
        {
            get{
                return mValue;
            }
            set{
                mValue = value;

                if (isWorld){
                    cacheTransfrom.position = GetBezierPoint(mValue);
                }
                else{
                    cacheTransfrom.localPosition = GetBezierPoint(mValue);
                }
            }
        }


        protected override void OnUpdate(float factor, bool isFinished)
        {
            value = from + factor * (to - from);
        }


        public void OnDrawGizmos()
        {
            if (path == null)
            {
                return;
            }
            for (int i = 0; i < path.Length; i++)
            {
                Gizmos.DrawWireSphere(path[i], 1f);
            }
        }


        public Vector3 GetBezierPoint(float t)
        {
            return GetBezierPoint(t, path[0], path[1], path[2], path[3]);
        }

        // fomula:
        // (1-t)^3 * p0 + 3(1-t)^2 * t* p1 + 3(1-t)* t^2 * p2 + t^3 * p3
        public Vector3 GetBezierPoint(float t, Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3)
        {
            float u = 1 - t;
            float t2 = t * t;
            float u2 = u * u;
            float u3 = u * u2;
            float t3 = t * t2;

            Vector3 p = u3 * p0;

            p += 3 * u2 * t * p1;
            p += 3 * u * t2 * p2;
            p += t3 * p3;

            return p;
        }

    }
}
