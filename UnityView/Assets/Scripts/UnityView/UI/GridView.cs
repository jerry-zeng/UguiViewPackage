using UnityEngine;
using UnityEngine.EventSystems;

namespace UnityView
{
    public class GridView : AbsAdapterView<IAdapter>
    {
        public int Row { get; protected set; }
        public int Column { get; protected set; }

        protected Vector2 GridItemSize = new Vector2(100, 100);

        public Vector2 ItemSize
        {
            get { return GridItemSize; }
            set
            {
                GridItemSize = value;
                if (Adapter == null) return;
                CalculateContentSize();
                RepositionItem();
            }
        }

        public override void ScrollToTop(int index)
        {


        }

        public override UILayout HeaderView
        {
            get { throw new System.NotImplementedException(); }
            set { throw new System.NotImplementedException(); }
        }

        public override UILayout FooterView
        {
            get { throw new System.NotImplementedException(); }
            set { throw new System.NotImplementedException(); }
        }

        public override void CalculateVisibleItemCount()
        {
            switch (ScrollOrentation)
            {
                case ScrollOrentation.Horizontal:
                    VisibleItemCount = (Mathf.CeilToInt((Width - Spacing.x) / (ItemSize.x + Spacing.x)) + 1) * Row;
                    break;
                case ScrollOrentation.Vertical:
                    VisibleItemCount = (Mathf.CeilToInt((Height - Spacing.y) / (ItemSize.y + Spacing.y)) + 1) * Column;
                    break;
            }
            if (StartIndex + VisibleItemCount > CacheSize) VisibleItemCount = CacheSize - StartIndex;
        }

        protected override Vector2 GetItemAnchorPostion(int index)
        {
            //Vector2 basePos = Vector2.zero;
            //Vector2 offset = Vector2.zero;

            int offsetIndex = 0;
            switch (ScrollOrentation)
            {
                case ScrollOrentation.Horizontal:
                    offsetIndex = index % Row;
                    return new Vector2(offsetIndex * (ItemSize.x + Spacing.x) + Spacing.x, -(index / Row * (ItemSize.y + Spacing.y)) - Spacing.y);

                case ScrollOrentation.Vertical:
                    offsetIndex = index % Column;
                    return new Vector2(offsetIndex * (ItemSize.x + Spacing.x) + Spacing.x, -(index / Column * (ItemSize.y + Spacing.y)) - Spacing.y);
            }
            return Vector2.zero;
        }

        protected override void OnScroll(Vector2 position)
        {
            Scrolling = true;
            StartIndex = GetStartIndex();
            if (StartIndex != LastStartIndex && StartIndex >= 0)
            {
                CalculateVisibleItemCount();
                RepositionItem();
                LastStartIndex = StartIndex;
            }
        }

        protected override int GetStartIndex()
        {
            float anchor;
            switch (ScrollOrentation)
            {
                case ScrollOrentation.Horizontal:
                    anchor = -ContentTransform.anchoredPosition.x;
                    StartIndex = (int)(anchor / (ItemSize.x + Spacing.x)) * Row;
                    break;
                case ScrollOrentation.Vertical:
                    anchor = ContentTransform.anchoredPosition.y;
                    StartIndex = (int)(anchor / (ItemSize.y + Spacing.y)) * Column;
                    break;
            }
            if (StartIndex < 0) StartIndex = 0;
            CalculateVisibleItemCount();
            return StartIndex;
        }

        public override IConvertView GetItem(int index)
        {
            if (index < Items.Count)
            {
                IConvertView convertView = Items[index];
                if (false == convertView.GetRectTransform().gameObject.activeSelf)
                {
                    convertView.GetRectTransform().gameObject.SetActive(true);
                }
                return Items[index];
            }
            else
            {
                IConvertView convertView = Adapter.GetConvertView(index, null);
                RectTransform transform = convertView.GetRectTransform();
                transform.SetParent(ContentTransform);
                transform.pivot = transform.anchorMin = transform.anchorMax = new Vector2(0, 1);
                transform.sizeDelta = ItemSize;
                Items.Add(convertView);
                return convertView;
            }
        }

        protected override Vector2 CalculateContentSize()
        {
            switch (ScrollOrentation)
            {
                case ScrollOrentation.Horizontal:
                    // 水平滚动时，先确定行，再确定列
                    Row = (int)((Height - Spacing.y) / (ItemSize.y + Spacing.y));
                    Column = (CacheSize + Row - 1) / Row;
                    ContentTransform.sizeDelta = new Vector2(Column * (ItemSize.x + Spacing.x) + Spacing.x, Height);
                    break;
                case ScrollOrentation.Vertical:
                    // 水平滚动时，先确定列，再确定行
                    Column = (int)((Width - Spacing.x) / (ItemSize.x + Spacing.x));
                    Row = (CacheSize + Column - 1) / Column;
                    ContentTransform.sizeDelta = new Vector2(Width, Row * (ItemSize.y + Spacing.y) + Spacing.y);
                    break;
            }
            CalculateVisibleItemCount();
            return ContentSize;
        }

        public override void OnItemClick(PointerEventData eventData)
        {

        }
    }
}
