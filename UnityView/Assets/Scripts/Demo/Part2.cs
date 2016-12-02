using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityView;
using UnityView.Model;

namespace Assets.Scripts
{
    public class Part2 : UILayout
    {
        public TableView TableView;

        public ButtonView SwitchButton;
        public Part2()
        {
            UIRect = UICreator.MainRect;

            TableView = new TableView();
            TableView.RectTransform.SetParent(RectTransform);
            RectFill(TableView);
            TableView.ItemSize = Screen.height * 0.125f;
            TableView.Adapter = new Part2Adapter();

            SwitchButton = new ButtonView(this);
            SwitchButton.UIRect = new UIRect(0, 0, 200, 50);
            SwitchButton.Title = "Switch Orentation";
            SwitchButton.FontSize = 25;
            SwitchButton.BackgroundColor = Color.grey;
            SwitchButton.OnClickListener.AddListener(SwitchOrentation);
        }

        public void SwitchOrentation()
        {
            TableView.ScrollOrentation = TableView.ScrollOrentation == ScrollOrentation.Vertical
                ? ScrollOrentation.Horizontal
                : ScrollOrentation.Vertical;
        }
    }

    public class Part2Adapter : IAdapter
    {
        public int GetCount()
        {
            return 30;
        }

        public IConvertView GetConvertView(int position, IConvertView convertView)
        {
            BaseConvertView cell = convertView as BaseConvertView;
            if (cell == null)
            {
                cell = new BaseConvertView();
                cell.FontSize = 25;
            }
            cell.BackgroundColor = position % 2 == 0 ? Color.white : Color.black;
            cell.TitleColor = position % 2 == 0 ? Color.black : Color.white;
            cell.Title = "Item:" + position;
            return cell;
        }
    }
}
