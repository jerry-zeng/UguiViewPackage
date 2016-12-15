using UnityEngine;
using UnityEditor;
using System.Collections;
using UnityEngine.UI;
using UnityView;


[CustomEditor(typeof(BitmapFontScaling))]
public class BitmapFontScalingEditor :  Editor
{
    Text text = null;

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        BitmapFontScaling script = target as BitmapFontScaling;

        if(text == null)
            text = script.GetComponent<Text>();

        if( text == null )
            return;

        if( serializedObject.FindProperty("textSize").intValue != text.fontSize )
            script.OnValidate();

        serializedObject.ApplyModifiedProperties();
    }
}
