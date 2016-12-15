using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;

namespace UnityView
{
    public class UIEnhanceScrollView : MonoBehaviour, IBeginDragHandler, IEndDragHandler, IDragHandler
    {
        public interface IEnhanceItem
        {
            int CurveOffSetIndex {get; set;}
            int RealIndex {get; set;}
            float CenterOffSet {get; set;}

            void Refresh( float posX, float scale, int depth);
            void SetSelectState(bool isCenter);
        }

        public interface IAdapter
        {
            int GetCount();

            GameObject GetItemPrototype();
            Vector2 GetItemSize();

            void Refresh(int index, GameObject itemToUpdate, float posX, float scale, int depth);
            void SetSelectState(int index, GameObject itemToUpdate, bool isCenter);
        }


        public AnimationCurve scaleCurve;
        public AnimationCurve positionCurve;
        public AnimationCurve depthCurve = new AnimationCurve(new Keyframe(0, 0), new Keyframe(0.5f, 1), new Keyframe(1, 0));

        // Offset width between item
        public Vector2 ItemSize = new Vector2(100f, 100f);
        protected int ItemCount;
        protected float TotalWidth = 0f;

        // Lerp duration
        public bool lerpTweenNow = true;
        public float lerpDuration = 0.2f;

        protected float curDuration = 0f;
        protected int mCenterIndex = 0;

        // if we can change the target item
        protected bool canChangeItem = false;
        protected float moveOffsetPerItem = 0.2f;

        // originHorizontalValue Lerp to horizontalTargetValue
        public float dragFactor = 0.005f;

        protected float originScrollValue = 0.1f;
        protected float curScrollValue = 0.5f;

        // the start center index
        public int startCenterIndex = 0;

        // targets enhance item in scroll view
        public List<UIEnhanceItem> itemList;

        // sort to get right index
        protected List<UIEnhanceItem> sortedItemList;

        // center and preCentered item
        protected UIEnhanceItem curCenterItem;
        protected UIEnhanceItem preCenterItem;


        protected virtual void Awake()
        {
            ItemCount = itemList.Count;
            moveOffsetPerItem = (Mathf.RoundToInt((1f / ItemCount) * 10000f)) * 0.0001f;

            if( ItemCount % 2 == 0 )
                mCenterIndex = ItemCount / 2 - 1;
            else
                mCenterIndex = ItemCount / 2;

            int index = 0;
            for(int i = ItemCount - 1; i >= 0; i--)
            {
                UIEnhanceItem item = itemList[i];

                item.CurveOffSetIndex = i;
                item.CenterOffSet = moveOffsetPerItem * (mCenterIndex - index);
                item.SetSelectState(false);

                index++;
            }

            // set the center item with startCenterIndex
            if(startCenterIndex < 0 || startCenterIndex >= ItemCount)
            {
                startCenterIndex = mCenterIndex;
            }

            TotalWidth = ItemSize.x * ItemCount;

            // sorted items
            sortedItemList = new List<UIEnhanceItem>( itemList );

            curCenterItem = itemList[startCenterIndex];
            curScrollValue = 0.5f - curCenterItem.CenterOffSet;

            LerpTweenToTarget(0f, curScrollValue, true);
            canChangeItem = true;
        }

        protected void LerpTweenToTarget(float fromValue, float toValue, bool instant = true)
        {
            if( instant == true )
            {
                SortEnhanceItem();
                originScrollValue = toValue;
                UpdateEnhanceScrollView(toValue);
                OnTweenOver();
            }
            else
            {
                originScrollValue = fromValue;
                curScrollValue = toValue;
                curDuration = 0f;
            }
            lerpTweenNow = !instant;
        }


        // Update EnhanceItem state with curve fTime value 
        protected virtual void UpdateEnhanceScrollView(float value)
        {
            for (int i = 0; i < itemList.Count; i++)
            {
                UIEnhanceItem item = itemList[i];

                float posX = GetPosXValue(value, item.CenterOffSet);
                float scaleValue = GetScaleValue(value, item.CenterOffSet);
                int depth = (int)(depthCurve.Evaluate(value + item.CenterOffSet) * itemList.Count);

                item.Refresh(posX, scaleValue, depth);
            }
        }

        protected void Update()
        {
            if( lerpTweenNow )
                TweenViewToTarget();
        }

        protected void TweenViewToTarget()
        {
            curDuration += Time.deltaTime;

            if(curDuration > lerpDuration)
                curDuration = lerpDuration;

            float percent = curDuration / lerpDuration;
            float value = Mathf.Lerp(originScrollValue, curScrollValue, percent);

            UpdateEnhanceScrollView(value);

            if(curDuration >= lerpDuration)
            {
                canChangeItem = true;
                lerpTweenNow = false;
                OnTweenOver();
            }
        }

        protected void OnTweenOver()
        {
            if(preCenterItem != null)
                preCenterItem.SetSelectState(false);
            if(curCenterItem != null)
                curCenterItem.SetSelectState(true);
        }

        // Get the evaluate value to set item's scale
        protected float GetScaleValue(float sliderValue, float added)
        {
            return scaleCurve.Evaluate(sliderValue + added);
        }

        // Get the X value set the Item's position
        protected float GetPosXValue(float sliderValue, float added)
        {
            return positionCurve.Evaluate(sliderValue + added) * TotalWidth;;
        }

        protected int GetMoveCurveFactorCount(UIEnhanceItem preCenterItem, UIEnhanceItem newCenterItem)
        {
            SortEnhanceItem();

            int factorCount = Mathf.Abs(newCenterItem.RealIndex) - Mathf.Abs(preCenterItem.RealIndex);
            return Mathf.Abs(factorCount);
        }

        // sort item with X so we can know how much distance we need to move the timeLine(curve time line)
        public static int SortPosition(UIEnhanceItem a, UIEnhanceItem b) 
        { 
            return a.transform.localPosition.x.CompareTo(b.transform.localPosition.x); 
        }

        protected void SortEnhanceItem()
        {
            sortedItemList.Sort(SortPosition);

            for(int i = sortedItemList.Count - 1; i >= 0; i--)
                sortedItemList[i].RealIndex = i;
        }

        public void FocusOnTargetItem(UIEnhanceItem selectItem)
        {
            if(!canChangeItem)
                return;

            if(curCenterItem == selectItem)
                return;

            preCenterItem = curCenterItem;
            curCenterItem = selectItem;

            // calculate the direction of moving
            float centerXValue = positionCurve.Evaluate(0.5f) * TotalWidth;
            bool isRight = selectItem.transform.localPosition.x > centerXValue;

            // calculate the offset * dFactor
            int moveIndexCount = GetMoveCurveFactorCount(preCenterItem, selectItem);
            float deltaValue = (isRight? -1:1) * moveOffsetPerItem * moveIndexCount;

            canChangeItem = false;
            LerpTweenToTarget(curScrollValue, curScrollValue + deltaValue, false);
        }



        public void FocusOnRight()
        {
            if(!canChangeItem)
                return;

            int targetIndex = curCenterItem.CurveOffSetIndex + 1;
            if(targetIndex > itemList.Count - 1)
                targetIndex = 0;

            FocusOnTargetItem(itemList[targetIndex]);
        }

        public void FocusOnLeft()
        {
            if(!canChangeItem)
                return;

            int targetIndex = curCenterItem.CurveOffSetIndex - 1;
            if(targetIndex < 0)
                targetIndex = itemList.Count - 1;

            FocusOnTargetItem(itemList[targetIndex]);
        }



        public void OnBeginDrag(PointerEventData eventData)
        {

        }

        public void OnDrag(PointerEventData eventData)
        {
            Vector2 delta = eventData.delta;

            if( Mathf.Abs(delta.x) > 0f )
            {
                curScrollValue += delta.x * dragFactor;
                LerpTweenToTarget(0f, curScrollValue, true);
            }
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            // find closed item to be centered
            int focusIndex = 0;
            float value = (curScrollValue - (int)curScrollValue);
            float min = float.MaxValue;
            float tmp = 0.5f * (curScrollValue < 0 ? -1 : 1);

            for(int i = 0; i < itemList.Count; i++)
            {
                float dis = Mathf.Abs(Mathf.Abs(value) - Mathf.Abs((tmp - itemList[i].CenterOffSet)));
                if(dis < min)
                {
                    focusIndex = i;
                    min = dis;
                }
            }

            originScrollValue = curScrollValue;
            float target = ((int)curScrollValue + (tmp - itemList[focusIndex].CenterOffSet));

            preCenterItem = curCenterItem;
            curCenterItem = itemList[focusIndex];

            canChangeItem = false;
            LerpTweenToTarget(originScrollValue, target, false);
        }

    }
}