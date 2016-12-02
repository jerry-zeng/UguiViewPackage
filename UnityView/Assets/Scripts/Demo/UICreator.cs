using System.Collections.Generic;
using Assets.Scripts;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityView;
using UnityView.Model;


public class UICreator : MonoBehaviour
{
    private Dictionary<string, UILayout> Items;
    private UIGroup<UILayout> Group;
    private ListView TypeListView;

    protected string[] PartTitles;

    public static UIRect MainRect = new UIRect(280, 0, 1000, 720);


    void Start()
    {
        var eventSystem = FindObjectOfType<EventSystem>();
        eventSystem.pixelDragThreshold = (int) (Screen.width / 50f);
        Items = new Dictionary<string, UILayout>();

        PartTitles = new [] {
            "Components",
            "TableView",
            "ListView",
            "GridView"
        };

        TypeListView = new ListView();
        {
            TypeListView.Name = "Type List View";
            TypeListView.UIRect = new UIRect(20, 0, 240, 720);
            TypeListView.Adapter = new TypeListAdapter(PartTitles);
            TypeListView.BackgroundColor = new Color(0, 0, 0, 0.75f);
            TypeListView.OnItemSelectedListener = (index) =>
            {
                Debug.Log("Select : " + index);
                Group.Select(Items[PartTitles[index]]);
            };
        }

        Group = new UIGroup<UILayout>((layout) =>
        {
            layout.UIObject.SetActive(true);
        }, 
                                      (layout) =>
        {
            if (layout.UIObject.activeSelf) 
                layout.UIObject.SetActive(false);
        });

        // Components
        UILayout components = new Part1();
        components.Name = "Components";
        Items.Add(PartTitles[0], components);
        Group.Add(components);

        // TableView
        UILayout tableView = new Part2();
        tableView.Name = "Table View";
        Items.Add(PartTitles[1], tableView);
        Group.Add(tableView);
        // ListView
        ListView listView = new Part3();
        listView.Name = "List View";

        TextView textView = new TextView(listView);
        textView.UIFrame = new UIFrame(0, 0, 1000, 150);
        textView.TextComponent.alignment = TextAnchor.MiddleCenter;
        textView.FontSize = 30;
        textView.Text = "List View Header";
        listView.HeaderView = textView;

        listView.Adapter = new Part3Adapter();
        Items.Add(PartTitles[2], listView);
        Group.Add(listView);

        // GridView
        GridView gridView = new Part4();
        gridView.Name = "Grid View";
        gridView.ItemSize = new Vector2(150, 150);
        gridView.Adapter = new Part4Adapter();
        Items.Add(PartTitles[3], gridView);
        Group.Add(gridView);
    }
}
