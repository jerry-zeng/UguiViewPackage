using UnityEngine;
using UnityEngine.UI;
using System.Collections;


namespace UnityView 
{
    [AddComponentMenu("uTools/Tween/Tween Image")]  
    public class TweenImageFill : Tweener 
    {
        [Range(0,1)]
        public float from;
        [Range(0, 1)]
        public float to;


        private Image mImage;
        public Image cacheImage 
        {
            get {
                if( mImage == null )
                    mImage = GetComponent<Image>();

                if (mImage.type != Image.Type.Filled) {
                    Debug.LogWarning("[uTweenImage] To use tween the image type must be [Image.Type.Filled]");
                    mImage.type = Image.Type.Filled;
                }
                return mImage;
            }
        }

        public virtual float value 
        {
            get { return cacheImage.fillAmount; }
            set { cacheImage.fillAmount = value; }
        }

        protected override void OnUpdate (float factor, bool isFinished) 
        {
            value = from + factor * (to - from);
        }

        public static TweenImageFill Begin(Image go, float from, float to, float duration, float delay) 
        {
            TweenImageFill comp = Begin<TweenImageFill>(go.gameObject, duration);
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
