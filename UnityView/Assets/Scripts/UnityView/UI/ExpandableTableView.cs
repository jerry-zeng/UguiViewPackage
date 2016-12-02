using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace UnityView
{
    // 可展开的TableView
    public class ExpandableTableView : AbsAdapterView<IListAdapter>
    {
        public override void ScrollToTop(int index)
        {
            throw new NotImplementedException();
        }

        public override UILayout HeaderView
        {
            get { throw new NotImplementedException(); }
            set { throw new NotImplementedException(); }
        }

        public override UILayout FooterView
        {
            get { throw new NotImplementedException(); }
            set { throw new NotImplementedException(); }
        }

        protected override Vector2 GetItemAnchorPostion(int index)
        {
            throw new NotImplementedException();
        }

        protected override int GetStartIndex()
        {
            throw new NotImplementedException();
        }

        public override void HideNonuseableItems()
        {
            throw new NotImplementedException();
        }

        public override IConvertView GetItem(int index)
        {
            throw new NotImplementedException();
        }

        protected override Vector2 CalculateContentSize()
        {
            throw new NotImplementedException();
        }

        public override void OnItemClick(PointerEventData eventData)
        {
            throw new NotImplementedException();
        }
    }
}
