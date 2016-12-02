using UnityEngine;
using UnityEngine.EventSystems;

namespace UnityView
{
    // 表格视图，每一行（列）的大小均相等，可支持水平与垂直布局
    public class TableView : AbsAdapterView<IAdapter>
    {
        public Alignment Alignment;
        public TableView()
        {
            BounceEnable = true;
        }

        public override ScrollOrentation ScrollOrentation
        {
            get { return base.ScrollOrentation; }
            set
            {
                if (ScrollOrentation == value) return;
                VisibleItemCount = value == ScrollOrentation.Vertical
                    ? Mathf.CeilToInt(Height / TableItemSize) : Mathf.CeilToInt(Width / TableItemSize);
                foreach (var convertView in Items)
                {
                    switch (value)
                    {
                        case ScrollOrentation.Horizontal:
                            convertView.GetRectTransform().sizeDelta = new Vector2(TableItemSize, Height);
                            break;
                        case ScrollOrentation.Vertical:
                            convertView.GetRectTransform().sizeDelta = new Vector2(Width, TableItemSize);
                            break;
                    }
                }
                switch (value)
                {
                    case ScrollOrentation.Horizontal:
                        ScrollRect.horizontal = true;
                        ScrollRect.vertical = false;
                        break;
                    case ScrollOrentation.Vertical:
                        ScrollRect.horizontal = false;
                        ScrollRect.vertical = true;
                        break;
                    case ScrollOrentation.Free:
                        ScrollRect.horizontal = true;
                        ScrollRect.vertical = true;
                        break;
                }
                CalculateVisibleItemCount();
                ContentTransform.anchoredPosition = Vector2.zero;
                Reload();
                base.ScrollOrentation = value;
            }
        }

        protected float TableItemSize = Mathf.Min(Screen.width, Screen.height) * 0.125f;
        public float ItemSize
        {
            get { return TableItemSize; }
            set
            {
                TableItemSize = value;
                CacheUpdate();
                Reload();
            }
        }

        public override float Width
        {
            get { return base.Width; }
            set
            {
                base.Width = value;
                if (ScrollOrentation == ScrollOrentation.Horizontal) VisibleItemCount = Mathf.CeilToInt(value / TableItemSize);
            }
        }

        public override float Height
        {
            get { return base.Height; }
            set
            {
                base.Height = value;
                if (ScrollOrentation == ScrollOrentation.Vertical) VisibleItemCount = Mathf.CeilToInt(value / TableItemSize);
            }
        }

        public override void CacheUpdate()
        {
            base.CacheUpdate();
            VisibleItemCount = ScrollOrentation == ScrollOrentation.Vertical
                ? Mathf.CeilToInt(Height / TableItemSize) : Mathf.CeilToInt(Width / TableItemSize);
            foreach (var convertView in Items)
            {
                switch (ScrollOrentation)
                {
                    case ScrollOrentation.Horizontal:
                        convertView.GetRectTransform().sizeDelta = new Vector2(TableItemSize, Height);
                        break;
                    case ScrollOrentation.Vertical:
                        convertView.GetRectTransform().sizeDelta = new Vector2(Width, TableItemSize);
                        break;
                }
            }
        }

        public override void ScrollToTop(int index)
        {
            ContentTransform.anchoredPosition = ScrollOrentation == ScrollOrentation.Vertical
                ? new Vector2(0, ItemSize * index)
                : new Vector2(ItemSize * index, 0);
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
                case ScrollOrentation.Vertical:
                    VisibleItemCount = (int)((Height / ItemSize) + 1);
                    break;
                case ScrollOrentation.Horizontal:
                    VisibleItemCount = (int)((Width / ItemSize) + 1);
                    break;
            }
            if (StartIndex + VisibleItemCount > CacheSize) VisibleItemCount = CacheSize - StartIndex;
        }

        protected override Vector2 GetItemAnchorPostion(int index)
        {
            switch (ScrollOrentation)
            {
                case ScrollOrentation.Horizontal:
                    return new Vector2(index * ItemSize, 0);
                case ScrollOrentation.Vertical:
                    return new Vector2(0, -index * ItemSize);
            }
            return Vector2.zero;
        }

        protected override int GetStartIndex()
        {
            return
                GetStartIndex(ScrollOrentation == ScrollOrentation.Vertical
                    ? ContentTransform.anchoredPosition.y
                    : -ContentTransform.anchoredPosition.x);
        }
        protected int GetStartIndex(float position)
        {
            if (position < 0) return 0;
            var startIndex = Mathf.FloorToInt(position / TableItemSize);
            if (startIndex > CacheSize - 1) startIndex = CacheSize - 1;
            return startIndex;
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
                var transform = convertView.GetRectTransform();
                transform.SetParent(ContentTransform);
                transform.pivot = transform.anchorMin = transform.anchorMax = new Vector2(0, 1);
                switch (ScrollOrentation)
                {
                    case ScrollOrentation.Horizontal:
                        transform.sizeDelta = new Vector2(TableItemSize, Height);
                        break;
                    case ScrollOrentation.Vertical:
                        transform.sizeDelta = new Vector2(Width, TableItemSize);
                        break;
                }
                Items.Add(convertView);
                return convertView;
            }
        }

        protected override Vector2 CalculateContentSize()
        {
            switch (ScrollOrentation)
            {
                case ScrollOrentation.Vertical:
                    ContentTransform.sizeDelta = new Vector2(Width, TableItemSize * CacheSize);
                    break;
                case ScrollOrentation.Horizontal:
                    ContentTransform.sizeDelta = new Vector2(TableItemSize * CacheSize, Height);
                    break;
            }
            return ContentTransform.sizeDelta;
        }

        public override void OnItemClick(PointerEventData eventData)
        {
            throw new System.NotImplementedException();
        }
    }
}
