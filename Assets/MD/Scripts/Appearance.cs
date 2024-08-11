using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Appearance : MonoBehaviour
{
    public Sprite normal;
    public Sprite hover;
    public Sprite selected;

    public List<AppearanceSubMenu> subMenus = new List<AppearanceSubMenu>();
    public AppearanceSubMenu userName;
    public AppearanceSubMenu face;
    public AppearanceSubMenu faceframe;
    public AppearanceSubMenu wallpaper;
    public AppearanceSubMenu protector;
    public AppearanceSubMenu field;
    public AppearanceSubMenu grave;
    public AppearanceSubMenu stand;
    public AppearanceSubMenu mate;

    UIPanel panel;
    UI2DSprite back;
    UIButton exit;
    UILabel title;

    public bool isShowed;
    public int player;

    void Awake()
    {
        panel = transform.GetChild(0).GetComponent<UIPanel>();
        panel.alpha = 0f;
        back = transform.GetChild(0).GetChild(0).GetChild(0).GetComponent<UI2DSprite>();
        title = UIHelper.getByName<UILabel>(gameObject, "!lable");
        exit = UIHelper.getByName<UIButton>(gameObject, "exit_");
        EventDelegate.Add(exit.onClick, Hide);

        subMenus.Add(userName);
        subMenus.Add(wallpaper);
        subMenus.Add(face);
        subMenus.Add(faceframe);
        subMenus.Add(protector);
        subMenus.Add(field);
        subMenus.Add(grave);
        subMenus.Add(stand);
        subMenus.Add(mate);
    }
    private void Start()
    {
        gameObject.SetActive(false);
    }

    public void Show(int player)
    {
        isShowed = true;

        Program.I().menu.hide();
        Program.I().setting.hide();
        UIHandler.CloseHomeUI();

        if (Program.I().ocgcore.gameField != null && Program.I().ocgcore.gameField.gameObject != null)
            back.alpha = 0.95f;
        else if (Program.I().deckManager.isShowed)
            back.alpha = 0.95f;
        else
            back.alpha = 0f;

        SEHandler.PlayInternalAudio("se_sys/SE_MENU_SLIDE_03");
        DOTween.To(() => panel.alpha, x => panel.alpha = x, 1f, 0.2f);


        this.player = player;

        foreach (var menu in subMenus)
            menu.Preselect(Config.Get(menu.name + Program.I().appearance.player, menu.transform.GetChild(2).name));

        if (player == 0)
        {
            title.text = "�ҷ��������";
            foreach (var menu in subMenus)
                menu.Selectable(true);
            subMenus[0].Show();
            UIHelper.getByName<UI2DSprite>(gameObject, "item0000").alpha = 0f;
        }
        else
        {
            title.text = "�Է��������";

            subMenus[0].Selectable(false);
            subMenus[1].Selectable(false);
            subMenus[2].Selectable(true);
            subMenus[3].Selectable(true);
            subMenus[4].Selectable(true);
            subMenus[5].Selectable(true);
            subMenus[6].Selectable(true);
            subMenus[7].Selectable(true);
            subMenus[8].Selectable(true);
            subMenus[2].Show();
            UIHelper.getByName<UI2DSprite>(gameObject, "item0000").alpha = 1f;
        }
        UIHelper.getByName<UILabel>(subMenus[0].gameObject, "label_input").text = Config.Get("name", "��������");
    }

    public void Hide()
    {
        isShowed = false;

        Program.I().setting.show();
        if (Program.I().ocgcore.gameField != null && Program.I().ocgcore.gameField.gameObject != null)
            UIHandler.OpenHomeUI(false);
        else if (Program.I().deckManager.isShowed)
            UIHandler.OpenHomeUI(false);
        else
        {
            Program.I().menu.show();
            UIHandler.OpenHomeUI();
        }
        DOTween.To(() => panel.alpha, x => panel.alpha = x, 0f, 0.2f);
        SEHandler.PlayInternalAudio("se_sys/SE_MENU_SLIDE_04");

        foreach (var menu in subMenus)
            menu.Save();
        Config.Set("name", UIHelper.getByName<UILabel>(subMenus[0].gameObject, "label_input").text);
        GameTextureManager.RefreshBack();

        Program.I().setting.RefreshAppearance();
        gameObject.SetActive(false);
    }

    public Sprite GetSprite(string menuName, string spriteName)
    {
        Sprite sprite = null;

        if(spriteName == "���")
        {
            List<AppearanceItem> items = new List<AppearanceItem>();
            foreach (var menu in subMenus)
                if (menuName == menu.name)
                    foreach (var item in menu.GetComponentsInChildren<AppearanceItem>(true))
                        items.Add(item);
            int id = Random.Range(0, items.Count);
            if (items[id].labelName.text == "���")
                id++;
            if (items[id].labelName.text == "�Զ���")
                id++;
            if (items[id].labelName.text == "������")
                id++;
            if (items[id].labelName.text == "�볡�ر�����ͬ")
                id++;
            sprite = items[id].uiSprite.sprite2D;
        }
        else
        {
            foreach (var menu in subMenus)
            {
                if (menuName == menu.name)
                {
                    foreach (var item in menu.GetComponentsInChildren<AppearanceItem>())
                    {
                        if (item.labelName.text == spriteName)
                        {
                            sprite = item.uiSprite.sprite2D;
                            break;
                        }
                    }
                    break;
                }
            }
        }
        return sprite;
    }
}
