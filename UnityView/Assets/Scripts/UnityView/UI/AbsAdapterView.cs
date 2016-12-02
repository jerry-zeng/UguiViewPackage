using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;


namespace UnityView
{
    public delegate void OnItemSelectedListener(int index);


    public abstract class AbsAdapterView<T> : UIView, IPointerClickHandler where T : IAdapter
    {
        // 缓存的元素个数
        protected int CacheSize = 20;
        // 滚动方向
        public virtual ScrollOrentation ScrollOrentation
        {
            get
            {
                if (ScrollRect.horizontal)
                {
                    return ScrollRect.vertical ? ScrollOrentation.Free : ScrollOrentation.Horizontal;
                }
                return ScrollOrentation.Vertical;
            }
            set
            {
                if (ScrollOrentation == value) return;
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
            }
        }
        // 是否开启边界弹性
        public bool BounceEnable
        {
            set
            {
                ScrollRect.movementType = value ? ScrollRect.MovementType.Elastic : ScrollRect.MovementType.Clamped;
            }
            get
            {
                return ScrollRect.movementType == ScrollRect.MovementType.Clamped;
            }
        }

        public Vector2 Spacing = Vector2.zero;

        public Mask Mask;

        protected ScrollRect ScrollRect;
        protected RectTransform ContentTransform;

        public abstract void ScrollToTop(int index);

        // 获取内容区域的大小
        public Vector2 ContentSize
        {
            get{ return ContentTransform.sizeDelta; }
        }
        protected List<IConvertView> Items = new List<IConvertView>();

        protected int LastStartIndex = 0;

        // 当前显示的元素个数
        protected virtual int CurrentShowItemCount
        {
            get
            {
                int showItemCount = CacheSize - StartIndex;
                return VisibleItemCount < CacheSize - StartIndex ? VisibleItemCount : showItemCount;
            }
        }

        protected PointerClickHandler EventHandler;
        public OnItemSelectedListener OnItemSelectedListener;

        protected T ListAdapter;
        public virtual T Adapter
        {
            get
            {
                return ListAdapter;
            }
            set
            {
                ListAdapter = value;
                CacheSize = value.GetCount();
                CalculateVisibleItemCount();
                CalculateContentSize();
                RepositionItem();
            }
        }

        public abstract UILayout HeaderView { get; set; }
        public abstract UILayout FooterView { get; set; }
        protected Vector2 HeaderViewSize = Vector2.zero;
        protected Vector2 FooterViewSize = Vector2.zero;

        protected AbsAdapterView()
        {
            ScrollRect = UIObject.AddComponent<ScrollRect>();
            ScrollRect.onValueChanged.AddListener(OnScroll);
            EventHandler = UIObject.AddComponent<PointerClickHandler>();
            EventHandler.AbsAdapterView = this;
            var contentView = new UILayout { UIObject = { name = "Content View" } };
            AddSubview(contentView);
            ContentTransform = contentView.RectTransform;
            ContentTransform.pivot = ContentTransform.anchorMin = ContentTransform.anchorMax = new Vector2(0, 1);
            ContentTransform.anchoredPosition = Vector2.zero;
            ScrollRect.content = ContentTransform;

            ScrollOrentation = ScrollOrentation.Vertical;

            BackgroundColor = Color.black;
            Mask = UIObject.AddComponent<Mask>();
            Mask.showMaskGraphic = true;
        }

        public virtual void CacheUpdate() { }
        // 当前起始元素的索引
        protected int StartIndex = 0;
        // ScrollView滚动事件
        protected virtual void OnScroll(Vector2 position)
        {
            Scrolling = true;
            StartIndex = GetStartIndex();
            if (StartIndex != LastStartIndex && StartIndex >= 0)
            {
                RepositionItem();
                LastStartIndex = StartIndex;
            }
        }

        public int VisibleItemCount { get; protected set; }
        public virtual void CalculateVisibleItemCount() { }
        protected abstract Vector2 GetItemAnchorPostion(int index);
        protected abstract int GetStartIndex();
        public abstract IConvertView GetItem(int index);

        // 重载表视图的数据
        public virtual void RepositionItem()
        {
            if (Adapter == null) return;

            if (StartIndex < 0) StartIndex = 0;
            for (int i = 0; i < CurrentShowItemCount; ++i)
            {
                var item = GetItem(i);
                item.GetRectTransform().anchoredPosition = GetItemAnchorPostion(StartIndex + i);
                // 更新View
                Adapter.GetConvertView(StartIndex + i, item);
            }
            HideNonuseableItems();
        }

        public virtual void Reload()
        {
            if (Adapter == null) return;
            CalculateContentSize();
            int count = Mathf.Min(VisibleItemCount, Adapter.GetCount());
            if (Items.Count > count)
            {
                for (int i = count; i < Items.Count; i++)
                {
                    if (Items[i].GetRectTransform().gameObject.activeSelf)
                    {
                        Items[i].GetRectTransform().gameObject.SetActive(false);
                    }
                }
            }
            for (int i = 0; i < count; ++i)
            {
                var item = GetItem(i);
                var transform = item.GetRectTransform();
                transform.pivot = transform.anchorMin = transform.anchorMax = new Vector2(0, 1);
                transform.anchoredPosition = GetItemAnchorPostion(i + StartIndex);
                Adapter.GetConvertView(i + StartIndex, item);
            }
            HideNonuseableItems();
        }

        public virtual void HideNonuseableItems()
        {
            for (int i = CurrentShowItemCount; i < Items.Count; i++)
            {
                GameObject convertView = Items[i].GetRectTransform().gameObject;
                if (convertView.activeSelf)
                {
                    convertView.SetActive(false);
                }
            }
        }

        // 计算ContentSize并进行赋值
        protected abstract Vector2 CalculateContentSize();
        public bool Scrolling = false;
        public void OnPointerClick(PointerEventData eventData)
        {
            if (Scrolling)
            {
                Scrolling = false;
                return;
            }
            OnItemClick(eventData);
        }

        public abstract void OnItemClick(PointerEventData eventData);
    }
    public class PointerClickHandler : MonoBehaviour, IPointerClickHandler
    {
        public IPointerClickHandler AbsAdapterView;
        public void OnPointerClick(PointerEventData eventData)
        {
            AbsAdapterView.OnPointerClick(eventData);
        }
    }

    public interface IConvertView
    {
        RectTransform GetRectTransform();
    }

    public class BaseConvertView : ButtonView, IConvertView
    {
        public RectTransform GetRectTransform()
        {
            return RectTransform;
        }
    }

    public interface IAdapter
    {
        int GetCount();
        IConvertView GetConvertView(int position, IConvertView convertView);
    }
    // 列表视图（高度可变）适配器
    public interface IListAdapter : IAdapter
    {
        float GetItemSize(int index);
    }

    public interface IGridAdapter : IAdapter
    {
        Vector2 GetItemSize(int index);
    }

    public interface ISectionListAdapter : IListAdapter
    {
        
    }
    // 高度可变的列表适配器
    public interface IElasticAdapterView
    {
        void OnItemSizeChanged();
        void OnItemSizeChanged(int index);
    }
}
