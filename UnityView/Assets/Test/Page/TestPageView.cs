using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityView;


public class TestPageView : MonoBehaviour, UIPageView.IAdapter
{
    public UIPageView pageView;
    public GameObject prefab;
    public Font font;


    void Start()
    {
        pageView.SetAdapter(this);
    }

    public int GetCount()
    {
        return 50;
    }

    public GameObject GetItemPrototype()
    {
        return prefab;
    }

    public void Refresh(int index, GameObject itemToUpdate)
    {
        Transform pageContentTransform = itemToUpdate.transform.Find ("PageContent");
        if( !pageContentTransform ) {
            GameObject pageContent = new GameObject();
            pageContent.name = "PageContent";

            RectTransform contentRT = pageContent.AddComponent<RectTransform> ();
            Text contentText = pageContent.AddComponent<Text> ();

            contentRT.SetParent (itemToUpdate.transform);

            Vector3 pos = contentRT.localPosition;
            pos.z = 0;
            contentRT.localPosition = pos;

            contentRT.localScale = Vector3.one;

            contentRT.anchorMin = Vector2.zero;
            contentRT.anchorMax = Vector2.one;

            contentRT.offsetMin = Vector2.zero;
            contentRT.offsetMax = Vector2.zero;

            contentText.font = this.font;
            contentText.fontSize = 30;
            contentText.alignment = TextAnchor.MiddleCenter;
            contentText.color = Color.red;
            contentText.text = "PageIndex: " + index;
        } 
        else {
            pageContentTransform.GetComponent<Text> ().text = "PageIndex: " + index;
        }
    }

    
}
