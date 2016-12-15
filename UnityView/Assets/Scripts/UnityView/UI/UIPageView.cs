using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine.UI;


namespace UnityView
{
    [RequireComponent(typeof(RectMask2D))]
    [RequireComponent(typeof(ScrollRect))]
    public class UIPageView : MonoBehaviour, IBeginDragHandler, IEndDragHandler, IDragHandler
    {
        public interface IAdapter
        {
            int GetCount();

            GameObject GetItemPrototype();

            void Refresh(int index, GameObject itemToUpdate);
        }


        protected ScrollRect _scrollRect;
        public ScrollRect ScrollRect
        {
            get{
                if(_scrollRect == null)
                    _scrollRect = GetComponent<ScrollRect>();
                return _scrollRect;
            }
        }

        public Vector2 ScrollRectSize
        {
            get{
                Vector2 scrollSize = ScrollRect.GetComponent<RectTransform>().sizeDelta;
                if(ScrollRect.viewport != null)
                    scrollSize += ScrollRect.viewport.sizeDelta;

                return scrollSize;
            }
        }

        public RectTransform ContentTransform
        {
            get{ return ScrollRect.content; }
        }
        public Vector2 ContentSize
        {
            get{ return ContentTransform.sizeDelta; }
        }

        protected float NormalizedPosition 
        {
            get
            {
                if( direction == ScrollDirection.Horizontal )
                    return ScrollRect.horizontalNormalizedPosition;
                else
                    return ScrollRect.verticalNormalizedPosition;
            }
            set
            {
                if( direction == ScrollDirection.Horizontal )
                    ScrollRect.horizontalNormalizedPosition = value;
                else
                    ScrollRect.verticalNormalizedPosition = value;
            }
        }

        public int VisibleItemCount
        {
            get; protected set; 
        }


        public ScrollDirection direction = ScrollDirection.Horizontal;
        public float scrollDuration = 0.2f;


        public int pageCount 
        {
            get { 
                return TotalPageCount;
            }
        }


        protected Dictionary<GameObject, int> _itemDict = new Dictionary<GameObject, int>();

        protected int TotalPageCount = 0;
        protected GameObject ItemPrefab;


        protected static int PageCacheSize = 2;

        protected int _curPageIndex = 0;
        protected int _prePageIndex = 0;

        protected IAdapter _adapter;


        public void SetAdapter(IAdapter adapter)
        {
            _adapter = adapter;

            TotalPageCount = adapter.GetCount();
            ItemPrefab = adapter.GetItemPrototype();

            _curPageIndex = 0;
            NormalizedPosition = PageIndexToNormalizedPosition (_curPageIndex);

            CalculateContentSize();
            CalculateVisibleItemCount();

            StopScroll();

            ClearAllItems();
            RepositionItems();
        }


        public virtual void ValidateScrollRect()
        {
            ScrollRect.horizontal = (direction == ScrollDirection.Horizontal);
            ScrollRect.vertical = (direction == ScrollDirection.Vertical);
        }

        protected virtual void Awake()
        {
            ValidateScrollRect();

            ContentTransform.pivot = ContentTransform.anchorMin = ContentTransform.anchorMax = new Vector2(0, 1);
            ContentTransform.anchoredPosition = Vector2.zero;
        }


        protected virtual void CalculateContentSize()
        {
            float contentWidth = ScrollRectSize.x;
            float contentHeight = ScrollRectSize.y;

            if( direction == ScrollDirection.Vertical ){
                contentHeight *= pageCount;
            }
            else{
                contentWidth *= pageCount;
            }

            ContentTransform.sizeDelta = new Vector2(contentWidth, contentHeight);
        }

        protected virtual void CalculateVisibleItemCount()
        {
            VisibleItemCount = 1 + PageCacheSize * 2;
        }


        protected float PageIndexToNormalizedPosition(int pageIndex)
        {
            if (pageCount <= 1) return 0;

            return (float)(pageIndex * 1.0 / (pageCount - 1));
        }

        protected int NormalizedPositionToPageIndex(float position)
        {
            if (pageCount <= 1) return 0;

            return Mathf.Clamp( Mathf.RoundToInt(position * (pageCount - 1)), 0, pageCount - 1);
        }


        #region input
        protected bool isDragging = false;
        protected bool isScrolling = false;
        protected float _beginPosition = 0;
        protected float _endPosition = 0;
        protected float _scrollTime = 0;
        protected float _scrollAcceleration = 0;

        protected Coroutine _scrollCoroutine = null;


        public void JumpToPage(int pageIndex)
        {
            if( pageCount < 1 )
                return;

            if( isDragging )
                return;

            StopScroll();

            _curPageIndex = Mathf.Clamp(pageIndex, 0, pageCount - 1);
            NormalizedPosition = PageIndexToNormalizedPosition(pageIndex);

            RepositionItems();
        }

        public void OnBeginDrag(PointerEventData data)
        {
            isDragging = true;
            StopScroll ();
        }

        public void OnDrag(PointerEventData data)
        {
            UpdatePageIndex();
        }

        public void OnEndDrag(PointerEventData data)
        {
            isDragging = false;
            StartScroll( NormalizedPositionToPageIndex(NormalizedPosition) );
        }


        protected virtual void StopScroll()
        {
            if (_scrollCoroutine != null) {
                StopCoroutine(_scrollCoroutine);
                _scrollCoroutine = null;
            }

            if(isScrolling)
            {
                isScrolling = false;

                _scrollTime = 0;

                _beginPosition = 0;
                _endPosition = 0;

                _scrollAcceleration = 0;
            }
        }

        protected virtual void StartScroll(int pageIndex)
        {
            if( pageCount < 1 )
                return;

            StopScroll();

            pageIndex = Mathf.Clamp(pageIndex, 0, pageCount - 1);

            _beginPosition = NormalizedPosition;
            _endPosition = PageIndexToNormalizedPosition( pageIndex );

            _scrollAcceleration = 2 * (_endPosition - _beginPosition) / scrollDuration / scrollDuration;

            _scrollTime = 0;
            isScrolling = true;

            _scrollCoroutine = StartCoroutine( Scroll() );
        }

        protected IEnumerator Scroll()
        {
            while( !isDragging && isScrolling )
            {
                _scrollTime += Time.deltaTime;

                if( _scrollTime >= scrollDuration ){
                    NormalizedPosition = _endPosition;
                    StopScroll();
                }
                else{
                    NormalizedPosition = _beginPosition + 0.5f * _scrollAcceleration * _scrollTime * _scrollTime;

                    UpdatePageIndex();

                    yield return null;
                }
            }
        }
        #endregion


        protected virtual void UpdatePageIndex() 
        {
            int newPageIndex = NormalizedPositionToPageIndex( NormalizedPosition );

            if( _curPageIndex != newPageIndex ) {
                _curPageIndex = newPageIndex;

                RepositionItems();
            }
        }

        protected virtual Vector2 GetItemAnchorPostion(int index)
        {
            if( direction == ScrollDirection.Vertical ){
                return new Vector2( 0, -index * ScrollRectSize.y );
            }
            else{
                return new Vector2( index * ScrollRectSize.x, 0 );
            }
        }


        protected virtual GameObject GetItem(int itemIndex)
        {
            if( _itemDict.Count >= VisibleItemCount )
            {
                GameObject newItem = null;

                foreach( var kvs in _itemDict )
                {
                    if( !kvs.Key.activeSelf || !kvs.Key.activeInHierarchy ){
                        newItem = kvs.Key;
                        break;
                    }
                }

                if( newItem != null ){
                    newItem.SetActive(true);
                    newItem.name = "Item " + itemIndex.ToString();

                    _itemDict[newItem] = itemIndex;

                    //Debug.Log("Get Cached Item " + itemIndex.ToString());
                }
                else{
                    Debug.LogError("All item is actived, but you are trying to get an inactived item?");
                }
                return newItem;
            }
            else{
                GameObject newItem = CreateItem();
                newItem.name = "Item " + itemIndex.ToString();

                _itemDict.Add(newItem, itemIndex);

                //Debug.Log("Create Item " + itemIndex.ToString());
                return newItem;
            }
        }

        protected virtual GameObject CreateItem()
        {
            GameObject go = Instantiate( ItemPrefab ) as GameObject;

            RectTransform trans = go.GetComponent<RectTransform>();
            if(trans == null)
                trans = go.AddComponent<RectTransform>();

            trans.SetParent(ContentTransform);
            trans.pivot = trans.anchorMin = trans.anchorMax = new Vector2(0, 1);
            trans.sizeDelta = ScrollRectSize;

            trans.localRotation = ItemPrefab.transform.localRotation;
            trans.localScale = ItemPrefab.transform.localScale;
            trans.localPosition = Vector3.zero;

            return go;
        }

        protected virtual void DestroyItem(GameObject go)
        {
            if(go == null) return;

            go.transform.SetParent(null);
            Destroy(go);
        }

        protected virtual void ClearAllItems()
        {
            var it = _itemDict.GetEnumerator();
            while(it.MoveNext())
            {
                DestroyItem( it.Current.Key );
            }
            _itemDict.Clear();
        }


        public virtual void RepositionItems()
        {
            if( _adapter == null ) 
                return;

            int MinIndex = _curPageIndex - PageCacheSize;
            int MaxIndex = _curPageIndex + PageCacheSize;

            // keep current position flags. 
            bool[] keepFlags = new bool[2*PageCacheSize+1];

            // hide the item that is out of index range.
            var it = _itemDict.GetEnumerator();
            while(it.MoveNext())
            {
                GameObject go = it.Current.Key;
                int index = it.Current.Value;

                if( go.activeInHierarchy && go.activeSelf )
                {
                    if( index < MinIndex || index > MaxIndex || index < 0 || index > TotalPageCount-1)
                    {
                        go.SetActive(false);
                    }
                    else{
                        keepFlags[index - MinIndex] = true;
                    }
                }
            }

            // move the hiden item or create an item to new position by the keepFlags.
            for(int i = MinIndex; i <= MaxIndex; i++)
            {
                if( i >= 0 && i < TotalPageCount && 
                _curPageIndex >= 0 && _curPageIndex < TotalPageCount &&
                keepFlags[i - MinIndex] == false)
                {
                    GameObject item = GetItem( i );
                    item.GetComponent<RectTransform>().anchoredPosition = GetItemAnchorPostion(i);

                    _adapter.Refresh(i, item);
                }
            }
        }


        public virtual void ScrollTo(int index)
        {
            if(_adapter == null)
                return;

            StopScroll();
            StartScroll(index);
        }

    }

}
