using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

namespace UnityView
{
    [RequireComponent(typeof(RectMask2D))]
    [RequireComponent(typeof(ScrollRect))]
    public class UIGridView : MonoBehaviour 
    {
        public interface IAdapter
        {
            int GetCount();

            GameObject GetItemPrototype();
            Vector2 GetItemSize();

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

        public int Row 
        {
            get; protected set; 
        }
        public int Column
        {
            get; protected set; 
        }
        public int VisibleItemCount
        {
            get; protected set; 
        }


        public ScrollDirection direction = ScrollDirection.Vertical;
        public int colOrRowCount = 1;


        protected List<GameObject> _itemList = new List<GameObject>();

        protected int TotalItemCount;
        protected GameObject ItemPrefab;
        protected Vector2 ItemSize;

        protected int _startIndex = 0;
        protected int _lastStartIndex = 0;

        protected IAdapter _adapter;


        public virtual void SetAdapter(IAdapter adapter)
        {
            _adapter = adapter;

            TotalItemCount = _adapter.GetCount();
            ItemPrefab = _adapter.GetItemPrototype();
            ItemSize = _adapter.GetItemSize();

            CalculateContentSize();
            CalculateVisibleItemCount();

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
            ScrollRect.onValueChanged.AddListener(OnScroll);

            ContentTransform.pivot = ContentTransform.anchorMin = ContentTransform.anchorMax = new Vector2(0, 1);
            ContentTransform.anchoredPosition = Vector2.zero;

            if(ScrollRect.viewport != null)
            {
                #if UNITY_EDITOR
                Debug.LogWarning( "You'd better not use the ScrollRect.viewport for UIGridView, " +
                             "it may cause unknown bugs, even no one happened yet." );
                #endif
            }
        }

        // update scroll rect.
        protected virtual void OnScroll(Vector2 position)
        {
            _startIndex = GetStartIndex();

            if( _startIndex != _lastStartIndex )
            {
                CalculateVisibleItemCount();
                RepositionItems();

                _lastStartIndex = _startIndex;
            }
        }


        protected virtual void CalculateContentSize()
        {
            if( direction == ScrollDirection.Vertical ){
                //Column = Mathf.FloorToInt(ContentSize.x / ItemSize.x);
                Column = Mathf.Clamp(colOrRowCount, 1, Mathf.FloorToInt(ScrollRectSize.x/ItemSize.x) );
                Row = (TotalItemCount + Column - 1) / Column;

                ContentTransform.sizeDelta = new Vector2(Column * ItemSize.x, Row * ItemSize.y);
            }
            else{
                //Row = Mathf.FloorToInt(ContentSize.y / ItemSize.y);
                Row = Mathf.Clamp(colOrRowCount, 1, Mathf.FloorToInt(ScrollRectSize.y/ItemSize.y));
                Column = (TotalItemCount + Row - 1) / Row;

                ContentTransform.sizeDelta = new Vector2(Column * ItemSize.x, Row * ItemSize.y);
            }
        }

        protected virtual void CalculateVisibleItemCount()
        {
            if( direction == ScrollDirection.Vertical ){
                VisibleItemCount = (Mathf.FloorToInt(ScrollRectSize.y / ItemSize.y) + 2) * Column;
            }
            else{
                VisibleItemCount = (Mathf.FloorToInt(ScrollRectSize.x / ItemSize.x) + 2) * Row;
            }

            VisibleItemCount = Mathf.Min( VisibleItemCount, TotalItemCount - _startIndex );
        }

        protected virtual int GetStartIndex()
        {
            if( direction == ScrollDirection.Vertical ){
                float height = Mathf.Abs(ContentTransform.anchoredPosition.y);

                return Mathf.FloorToInt(height / ItemSize.y) * Column;
            }
            else{
                float width = Mathf.Abs(ContentTransform.anchoredPosition.x);

                return Mathf.FloorToInt(width / ItemSize.x) * Row;
            }
        }

        protected virtual Vector2 GetItemAnchorPostion(int index)
        {
            if( direction == ScrollDirection.Vertical ){
                return new Vector2( (index % Column) * ItemSize.x, -(index / Column * ItemSize.y) );
            }
            else{
                return new Vector2( (index / Row) * ItemSize.x, -(index % Row) * ItemSize.y );
            }
        }


        protected virtual GameObject GetItem(int index)
        {
            if( index < _itemList.Count )
            {
                return _itemList[index];
            }
            else
            {
                GameObject newItem = CreateItem();
                newItem.name = "Item " + index.ToString();
                _itemList.Add(newItem);

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

            trans.localRotation = ItemPrefab.transform.localRotation;
            trans.localScale = ItemPrefab.transform.localScale;
            trans.localPosition = Vector3.zero;

            return go;
        }

        protected virtual void DestroyItem(GameObject go)
        {
            if(go == null) return;

            go.transform.parent = null;
            Destroy(go);
        }

        protected virtual void ClearAllItems()
        {
            for( int i = 0; i < _itemList.Count; i++ )
            {
                DestroyItem(_itemList[i]);
            }
            _itemList.Clear();
        }

        public virtual void RepositionItems()
        {
            if( _adapter == null ) 
                return;

            for( int i = 0; i < VisibleItemCount; ++i )
            {
                GameObject item = GetItem(i);
                item.GetComponent<RectTransform>().anchoredPosition = GetItemAnchorPostion(_startIndex + i);

                _adapter.Refresh(_startIndex + i, item);
            }
        }


        public virtual void ScrollTo(int index)
        {
            #if UNITY_EDITOR
            Debug.LogWarning("This function was not implemented yet");
            #endif
        }
    }

}
