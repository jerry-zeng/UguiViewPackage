using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;


namespace UnityView
{

    public class UIEnhanceItem : MonoBehaviour, UIEnhanceScrollView.IEnhanceItem, IPointerClickHandler
    {
        public UIEnhanceScrollView scrollView;

        private RawImage _rawImage;
        protected RawImage RawImage
        {
            get{
                if(_rawImage == null)
                    _rawImage = GetComponent<RawImage>();
                return _rawImage;
            }
        }


        // Start index
        private int curveOffSetIndex = 0;
        public int CurveOffSetIndex
        {
            get { return this.curveOffSetIndex; }
            set { this.curveOffSetIndex = value; }
        }

        // Runtime real index(Be calculated in runtime)
        private int curRealIndex = 0;
        public int RealIndex
        {
            get { return this.curRealIndex; }
            set { this.curRealIndex = value; }
        }

        // Curve center offset 
        private float dCurveCenterOffset = 0.0f;
        public float CenterOffSet
        {
            get { return this.dCurveCenterOffset; }
            set { dCurveCenterOffset = value; }
        }


        void Awake()
        {
            if(scrollView == null)
                scrollView = gameObject.GetComponentInParent<UIEnhanceScrollView>();
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if(scrollView != null)
                scrollView.FocusOnTargetItem(this);
        }


        public virtual void Refresh( float posX, float scaleValue, int newDepth)
        {
            Vector3 pos = transform.localPosition;
            pos.x = posX;
            transform.localPosition = pos;

            transform.localScale = new Vector3(scaleValue, scaleValue, transform.localScale.z);

            transform.SetSiblingIndex(newDepth);
        }

        public virtual void SetSelectState(bool isCenter)
        {
            RawImage.color = isCenter ? Color.white : Color.gray;
        }

    }

}
