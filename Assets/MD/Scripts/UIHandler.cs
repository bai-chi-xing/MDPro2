using Spine.Unity;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class UIHandler : MonoBehaviour
{
    public string bgName;
    static GameObject homeUI;
    //static GameObject bgFront;

    private void Start()
    {
        homeUI = GameObject.Find("UI/HomeUI");
        LoadBgFront(Config.Get("Wallpaper0", "���"));
    }

    static void LoadDIYBg()
    {
        LoadBgFront("��Ӱ�����÷���");

        Transform transform = GameObject.Find("UI/HomeUI/RootWallpaper/Wallpaper").transform;
        foreach(var t in transform.GetComponentsInChildren<Transform>(true))
            if(t.name == "Front0002_1")
                Destroy(t.gameObject);
        transform = GameObject.Find("UI/Camera_UI").transform;
        GameObject frontLoader = ABLoader.LoadABFolder("spine/u81497285", "Front");
        frontLoader.transform.parent = transform;
        frontLoader.transform.localPosition = new Vector3(1.5f, 0, 0);

        var aniamation = frontLoader.GetComponentInChildren<SkeletonAnimation>();
        aniamation.loop = true;
        aniamation.AnimationName = "idel";

        //ABLoader.ChangeLayer(frontLoader, "UI");
    }

    public static void LoadBgFront(string name)
    {
        if (name == "�Թ��ǵİ�����")
        {
            LoadDIYBg();
            return;
        }


        name = BgMaping(name);
        Transform transform = GameObject.Find("UI/HomeUI/RootWallpaper/Wallpaper").transform;
        foreach(Transform t in transform.GetComponentsInChildren<Transform>(true))
        {
            if (t.name.StartsWith("Front"))
            {
                Destroy(t.gameObject);
            }
        }
        Transform camera = GameObject.Find("UI/Camera_UI").transform;
        if (camera.GetComponentsInChildren<Transform>(true).Length > 1) Destroy(camera.GetChild(0).gameObject);

        GameObject frontLoader = ABLoader.LoadABFolder("wallpaper/front" + name, "front");
        RectTransform front = frontLoader.transform.GetChild(0).GetComponent<RectTransform>();
        front.parent = transform;
        Destroy(frontLoader);
        front.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Right, 0f, front.rect.width);

        foreach (ParticleSystem p in front.GetComponentsInChildren<ParticleSystem>(true))
            p.Play();
    }

    static string BgMaping(string name)
    {
        switch (name)
        {
            case "���":
                {
                    string re = "";
                    int i = Random.Range(1, 21);
                    if (i > 9)
                        re = "00" + i;
                    else
                        re = "000" + i;
                    return re;
                }
            case "�����ǰ���":
                return "0001";
            case "��Ӱ�����÷���":
                return "0002";
            case "����ս�� ����ʩ����":
                return "0003";
            case "����֮�������� ��������":
                return "0004";
            case "������-��":
                return "0005";
            case "�ƽ��䰣��������":
                return "0006";
            case "˫�֮ʥ�����ˡ�������ʿ":
                return "0007";
            case "�������ɽ���":
                return "0008";
            case "�ػ���������":
                return "0009";
            case "Ԫ��-Ӣ�� ���������":
                return "0010";
            case "����������":
                return "0011";
            case "���ձ��0 δ�����ʻ���":
                return "0012";
            case "��ɫ�ۻ������":
                return "0013";
            case "����������":
                return "0014";
            case "��ƽ�":
                return "0015";
            case "��������������ǰ·":
                return "0016";
            case "��ħ��":
                return "0017";
            case "������ѿ�Ĵ�����":
                return "0018";
            case "�������ܷ���":
                return "0019";
            case "ħ�����֮лĻ":
                return "0020";
            case "���41 ��˯ħ�� ������":
                return "0041";
            default:
                return "0001";
        }
    }
    public static void CloseHomeUI()
    {
        if (homeUI == null)
            return;
        homeUI.SetActive(false);
        Transform camera = GameObject.Find("UI/Camera_UI").transform;
        if (camera.GetComponentsInChildren<Transform>(true).Length > 1) Destroy(camera.GetChild(0).gameObject);
    }

    public static void OpenHomeUI(bool now = true)
    {
        if (homeUI == null)
            return;
        if (now)
        {
            homeUI.SetActive(true);
            LoadBgFront(Config.Get("Wallpaper0", "���"));
        }
        else
        {
            if (homeUI.activeSelf)
            {
                LoadBgFront(Config.Get("Wallpaper0", "���"));
            }
        }
    }
}
