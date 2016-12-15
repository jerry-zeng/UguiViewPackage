using UnityEngine;
using UnityView;


public class RegexExample : MonoBehaviour
{
    [SerializeField]
    RegexHypertext _text;

    const string RegexURL = "http(s)?://([\\w-]+\\.)+[\\w-]+(/[\\w- ./?%&=]*)?";
    const string RegexHashTag = "[#＃][Ａ-Ｚａ-ｚA-Za-z一-鿆0-9０-９ぁ-ヶｦ-ﾟー]+";

    void Start()
    {
        _text.SetClickableByRegex(RegexURL, Color.cyan, OnClickUrl );
        _text.SetClickableByRegex(RegexHashTag, Color.green, OnClickHashTag );
    }

    public void OnClickUrl(string url)
    {
        Debug.Log(url);
        Application.OpenURL(url);
    }

    public void OnClickHashTag(string tagStr)
    {
        Debug.Log(tagStr);
    }

}
