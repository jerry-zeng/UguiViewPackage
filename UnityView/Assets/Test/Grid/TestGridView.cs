using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityView;


public class TestGridView : MonoBehaviour, UIGridView.IAdapter
{
    public UIGridView gridView;
    public GameObject prefab;


    void Start()
    {
        gridView.SetAdapter( this );
    }


    public int GetCount()
    {
        return 31;
    }

    public GameObject GetItemPrototype()
    {
        return prefab;
    }

    public Vector2 GetItemSize()
    {
        return prefab.GetComponent<RectTransform>().sizeDelta;
    }

    public void Refresh(int index, GameObject itemToUpdate)
    {
        Text label = itemToUpdate.GetComponentInChildren<Text>();
        label.text = "Item " + index.ToString();
    }
}

