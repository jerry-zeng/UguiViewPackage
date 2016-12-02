using UnityEngine;
using UnityEditor;

namespace UnityView
{
    [CustomEditor( typeof(PageView))]
    public sealed class PageViewEditor : Editor {

        public override void OnInspectorGUI(){
            base.OnInspectorGUI();
        }
    }
}