using UnityEngine;
using UnityView;

namespace Assets.Scripts
{
    public class TypeListAdapter : IListAdapter
    {
        public string[] Types;

        public TypeListAdapter(string[] strings)
        {
            Types = strings;
        }
        public int GetCount()
        {
            return Types.Length;
        }

        public IConvertView GetConvertView(int position, IConvertView convertView)
        {
            TypeListCell cell = convertView as TypeListCell;
            if (cell == null)
            {
                cell = new TypeListCell();
                cell.FontSize = 25;
                cell.TextComponent.alignment = TextAnchor.MiddleCenter;
                UILayout.AddUnderLine(cell.GetRectTransform(), Screen.height / 720f, Color.white);
            }
            cell.Text = "Part" + (position + 1) + "." + Types[position];
            return cell;
        }

        public float GetItemSize(int index)
        {
            return Screen.height * 0.1f;
        }
    }

    public class TypeListCell : TextView, IConvertView
    {
        public RectTransform GetRectTransform()
        {
            return RectTransform;
        }
    }
}
