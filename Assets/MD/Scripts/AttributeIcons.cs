using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttributeIcons : MonoBehaviour
{
    public Sprite null_;
    public Sprite light_;
    public Sprite dark_;
    public Sprite water_;
    public Sprite fire_;
    public Sprite earth_;
    public Sprite wind_;
    public Sprite divine_;
    public Sprite spell_;
    public Sprite trap_;

    public void ChangeAttribute(string att)
    {
        GetComponent<UI2DSprite>().sprite2D = GetAttribute(att);
    }

    public Sprite GetAttribute(string att)
    {
        switch (att)
        {
            case "��":
                return null_;
            case "��":
                return light_;
            case "��":
                return dark_;
            case "ˮ":
                return water_;
            case "��":
                return fire_;
            case "��":
                return earth_;
            case "��":
                return wind_;
            case "��":
                return divine_;
            case "ħ��":
                return spell_;
            case "����":
                return trap_;
            default:
                return null_;
        }
    }
}
