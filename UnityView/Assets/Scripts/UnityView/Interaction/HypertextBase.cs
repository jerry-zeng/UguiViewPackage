using HypertextHelper;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;


namespace UnityView
{

    public abstract class HypertextBase : Text, IPointerClickHandler
    {
        protected Canvas _rootCanvas;
        protected Canvas RootCanvas { get { return _rootCanvas ?? (_rootCanvas = GetComponentInParent<Canvas>()); } }

        protected const int CharVertsNum = 6;
        protected readonly List<ClickableEntry> _entries = new List<ClickableEntry>();
        protected static readonly ObjectPool<List<UIVertex>> _verticesPool = new ObjectPool<List<UIVertex>>(null, l => l.Clear());


        public struct ClickableEntry
        {
            public string Word;
            public int StartIndex;
            public Color Color;
            public Action<string> OnClick;
            public List<Rect> Rects;

            public ClickableEntry(string word, int startIndex, Color color, Action<string> onClick)
            {
                Word = word;
                StartIndex = startIndex;
                Color = color;
                OnClick = onClick;
                Rects = new List<Rect>();
            }
        };

        /// <summary>
        /// クリック可能領域に関する情報を登録します
        /// </summary>
        /// <param name="startIndex">対象ワードの開始インデックス</param>
        /// <param name="wordLength">対象ワードの文字数</param>
        /// <param name="color">対象ワードにつける色</param>
        /// <param name="onClick">対象ワードがクリックされたときのコールバック</param>
        protected void RegisterClickable(int startIndex, int wordLength, Color color, Action<string> onClick)
        {
            if (onClick == null)
            {
                return;
            }

            if (startIndex < 0 || wordLength < 0 || startIndex + wordLength > text.Length)
            {
                return;
            }

            _entries.Add(new ClickableEntry(text.Substring(startIndex, wordLength), startIndex, color, onClick));
        }

        /// <summary>
        /// 登録した情報を削除します
        /// </summary>
        public virtual void RemoveClickable()
        {
            _entries.Clear();
        }

        /// <summary>
        /// テキストの変更などでクリックする文字位置の再計算が必要なときに呼び出されます
        /// RegisterClickable を使ってクリック対象文字の情報を登録してください
        /// </summary>
        protected abstract void RegisterClickable();

        protected override void OnPopulateMesh(VertexHelper vertexHelper)
        {
            base.OnPopulateMesh(vertexHelper);

            _entries.Clear();
            RegisterClickable();

            var vertices = _verticesPool.Get();
            vertexHelper.GetUIVertexStream(vertices);

            Modify(ref vertices);

            vertexHelper.Clear();
            vertexHelper.AddUIVertexTriangleStream(vertices);
            _verticesPool.Release(vertices);
        }

        protected void Modify(ref List<UIVertex> vertices)
        {
            var verticesCount = vertices.Count;

            for (int i = 0, len = _entries.Count; i < len; i++)
            {
                var entry = _entries[i];

                for (int textIndex = entry.StartIndex, wordEndIndex = entry.StartIndex + entry.Word.Length; textIndex < wordEndIndex; textIndex++)
                {
                    var vertexStartIndex = textIndex * CharVertsNum;
                    if (vertexStartIndex + CharVertsNum > verticesCount)
                    {
                        break;
                    }

                    var min = Vector2.one * float.MaxValue;
                    var max = Vector2.one * float.MinValue;

                    for (int vertexIndex = 0; vertexIndex < CharVertsNum; vertexIndex++)
                    {
                        var vertex = vertices[vertexStartIndex + vertexIndex];
                        vertex.color = entry.Color;
                        vertices[vertexStartIndex + vertexIndex] = vertex;

                        var pos = vertices[vertexStartIndex + vertexIndex].position;

                        if (pos.y < min.y)
                        {
                            min.y = pos.y;
                        }

                        if (pos.x < min.x)
                        {
                            min.x = pos.x;
                        }

                        if (pos.y > max.y)
                        {
                            max.y = pos.y;
                        }

                        if (pos.x > max.x)
                        {
                            max.x = pos.x;
                        }
                    }

                    entry.Rects.Add(new Rect { min = min, max = max });
                }

                // 同じ行で隣り合った矩形をまとめる
                List<Rect> mergedRects = new List<Rect>();
                foreach (var charRects in SplitRectsByRow(entry.Rects))
                {
                    mergedRects.Add(MergeRects(charRects));
                }

                entry.Rects = mergedRects;
                _entries[i] = entry;
            }
        }

        protected List<List<Rect>> SplitRectsByRow(List<Rect> rects)
        {
            List<List<Rect>> rectsList = new List<List<Rect>>();
            int rowStartIndex = 0;

            for (int i = 1; i < rects.Count; i++)
            {
                if (rects[i].xMin < rects[i - 1].xMin)
                {
                    rectsList.Add(rects.GetRange(rowStartIndex, i - rowStartIndex));
                    rowStartIndex = i;
                }
            }

            if (rowStartIndex < rects.Count)
            {
                rectsList.Add(rects.GetRange(rowStartIndex, rects.Count - rowStartIndex));
            }

            return rectsList;
        }

        protected Rect MergeRects(List<Rect> rects)
        {
            Vector2 min = Vector2.one * float.MaxValue;
            Vector2 max = Vector2.one * float.MinValue;

            for (int i = 0, len = rects.Count; i < len; i++)
            {
                if (rects[i].xMin < min.x)
                {
                    min.x = rects[i].xMin;
                }

                if (rects[i].yMin < min.y)
                {
                    min.y = rects[i].yMin;
                }

                if (rects[i].xMax > max.x)
                {
                    max.x = rects[i].xMax;
                }

                if (rects[i].yMax > max.y)
                {
                    max.y = rects[i].yMax;
                }
            }

            return new Rect { min = min, max = max };
        }

        protected Vector3 ToLocalPosition(Vector3 position, Camera camera)
        {
            if (!RootCanvas)
            {
                return Vector3.zero;
            }

            if (RootCanvas.renderMode == RenderMode.ScreenSpaceOverlay)
            {
                return transform.InverseTransformPoint(position);
            }

            Vector2 localPosition;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(rectTransform, position, camera, out localPosition);

            return localPosition;
        }


        void IPointerClickHandler.OnPointerClick(PointerEventData eventData)
        {
            Vector3 localPosition = ToLocalPosition(eventData.position, eventData.pressEventCamera);

            ClickableEntry entry;

            for(int i = 0; i < _entries.Count; i++)
            {
                entry = _entries[i];

                if (entry.OnClick == null)
                    continue;

                for(int j = 0; j < entry.Rects.Count; j++)
                {
                    if( !entry.Rects[j].Contains(localPosition) )
                        continue;

                    entry.OnClick(entry.Word);
                    break;
                }
            }
        }

    }

}


namespace HypertextHelper
{
    public class ObjectPool<T> where T : new()
    {
        readonly Stack<T> _stack = new Stack<T>();
        readonly Action<T> _onGet;
        readonly Action<T> _onRelease;

        public int CountAll { get; set; }
        public int CountActive { get { return CountAll - CountInactive; } }
        public int CountInactive { get { return _stack.Count; } }

        public ObjectPool(Action<T> onGet, Action<T> onRelease)
        {
            _onGet = onGet;
            _onRelease = onRelease;
        }

        public T Get()
        {
            T element;
            if (_stack.Count == 0)
            {
                element = new T();
                CountAll++;
            }
            else
            {
                element = _stack.Pop();
            }

            if (_onGet != null)
            {
                _onGet(element);
            }

            return element;
        }

        public void Release(T element)
        {
            if (_stack.Count > 0 && ReferenceEquals(_stack.Peek(), element))
            {
                UnityEngine.Debug.LogError("Internal error. Trying to destroy object that is already released to pool.");
            }

            if (_onRelease != null)
            {
                _onRelease(element);
            }

            _stack.Push(element);
        }
    }
}
