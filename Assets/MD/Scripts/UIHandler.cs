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
    static GameObject cameraUI;

    private void Awake()
    {
        homeUI = GameObject.Find("UI/HomeUI");
        cameraUI = GameObject.Find("UI/Camera_UI");
        LoadBgFront(Config.Get("Wallpaper0", "���"));
    }

    static void LoadDIYBg(string name)
    {
        LoadBgFront("��Ӱ�����÷���");

        Transform transform = GameObject.Find("UI/HomeUI/RootWallpaper/Wallpaper").transform;
        foreach(var t in transform.GetComponentsInChildren<Transform>(true))
            if(t.name == "Front0002_1")
                Destroy(t.gameObject);
        transform = GameObject.Find("UI/Camera_UI").transform;

        if (name == "�Թ��ǵİ�����")
        {
            GameObject frontLoader = ABLoader.LoadABFromFolder("spine/u81497285", "Front");
            frontLoader.transform.parent = transform;
            frontLoader.transform.localScale = Vector3.one;
            frontLoader.transform.localPosition = new Vector3(1.5f, 0, 0);
            var aniamation = frontLoader.GetComponentInChildren<SkeletonAnimation>();
            aniamation.loop = true;
            aniamation.AnimationName = "idel";
        }
        else if (name == "I��Pαװ�������")
        {
            GameObject frontLoader = ABLoader.LoadABFromFile("wallpaper/65741786");
            Vector3 size = frontLoader.transform.localScale;
            Vector3 position = frontLoader.transform.localPosition;
            frontLoader.transform.parent = transform;
            frontLoader.transform.localScale = size;
            frontLoader.transform.localPosition = position;
        }
        else if(name == "I��Pαװ������ȣ��컭��")
        {
            GameObject frontLoader = ABLoader.LoadABFromFile("wallpaper/65741787");
            Vector3 size = frontLoader.transform.localScale;
            Vector3 position = frontLoader.transform.localPosition;
            frontLoader.transform.parent = transform;
            frontLoader.transform.localScale = size;
            frontLoader.transform.localPosition = position;
        }
        else if (name == "��ħ������ʿ-���������ʿ")
        {
            GameObject frontLoader = ABLoader.LoadABFromFile("wallpaper/37818794");
            Vector3 size = frontLoader.transform.localScale;
            Vector3 position = frontLoader.transform.localPosition;
            frontLoader.transform.parent = transform;
            frontLoader.transform.localScale = size;
            frontLoader.transform.localPosition = position;
        }
    }

    public static void LoadBgFront(string name)
    {
        if (name == "�Թ��ǵİ�����"
            ||
            name == "��ħ������ʿ-���������ʿ"
            ||
            name == "I��Pαװ�������"
            ||
            name == "I��Pαװ������ȣ��컭��"
            )
        {
            LoadDIYBg(name);
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
        cameraUI.transform.DestroyChildren();

        GameObject frontLoader = ABLoader.LoadABFromFolder("wallpaper/front" + name, "front");
        RectTransform front = frontLoader.transform.GetChild(0).GetComponent<RectTransform>();
        front.parent = transform;
        Destroy(frontLoader);
        front.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Right, 0f, front.rect.width);
        front.localEulerAngles = Vector3.zero;
        front.localScale = Vector3.one;

        GameObject layer_1 = null;
        GameObject layer_2 = null;
        GameObject layer_3 = null;
        GameObject layer_1_1 = null;
        GameObject layer_1_2 = null;
        GameObject layer_1_3 = null;

        foreach(var child in front.GetComponentsInChildren<Transform>())
        {
            if (child.name.EndsWith("_1_1"))
                layer_1_1 = child.gameObject;
            else if (child.name.EndsWith("_1_2"))
                layer_1_2 = child.gameObject;
            else if (child.name.EndsWith("_1_3"))
                layer_1_3 = child.gameObject;
            else if (child.name.EndsWith("_1") && child.name.EndsWith("_1_1") == false)
                layer_1 = child.gameObject;
            else if (child.name.EndsWith("_2") && child.name.EndsWith("_1_2") == false)
                layer_2 = child.gameObject;
            else if (child.name.EndsWith("_3") && child.name.EndsWith("_1_3") == false)
                layer_3 = child.gameObject;
        }
        if(layer_1 != null && layer_1.name == "Front0016_1")
        {
            LoopMoveY move1 = layer_1.AddComponent<LoopMoveY>();
            move1.range = 10f;
            move1.time = 8f;
            LoopMoveY move2 = layer_2.AddComponent<LoopMoveY>();
            move2.range = 5f;
            move2.time = 8f;
        }
        else if(layer_1 == null)
        {
            LoopMoveY move11 = layer_1_1.AddComponent<LoopMoveY>();
            move11.range = 20f;
            move11.time = 8f;
            LoopMoveY move12 = layer_1_2.AddComponent<LoopMoveY>();
            move12.range = 25f;
            move12.time = 8f;
            LoopMoveY move13 = layer_1_3.AddComponent<LoopMoveY>();
            move13.range = 15f;
            move13.time = 8f;
            LoopMoveY move2 = layer_2.AddComponent<LoopMoveY>();
            move2.range = 10f;
            move2.time = 8f;
        }
        else if(layer_3 == null)
        {
            LoopMoveY move1 = layer_1.AddComponent<LoopMoveY>();
            move1.range = 20f;
            move1.time = 8f;
            LoopMoveY move2 = layer_2.AddComponent<LoopMoveY>();
            move2.range = 10f;
            move2.time = 8f;
        }
        else
        {
            LoopMoveY move1 = layer_1.AddComponent<LoopMoveY>();
            move1.range = 20f;
            move1.time = 8f;
            LoopMoveY move2 = layer_2.AddComponent<LoopMoveY>();
            move2.range = 25f;
            move2.time = 8f;
            LoopMoveY move3 = layer_3.AddComponent<LoopMoveY>();
            move3.range = 10f;
            move3.time = 8f;
        }

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
                    int i = Random.Range(1, 25);
                    if (i == 22)
                        i = 41;
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
            case "��Ԯ���� �������ֻ�":
                return "0021";
            case "��˵�İ�֮ħ��ʦ":
                return "0023";
            case "��˵�İ�֮��":
                return "0024";
            case "���41 ��˯ħ�� ������":
                return "0041";
            default:
                return "0001";
        }
    }

    public static void ResetUI()
    {
        cameraUI.transform.DestroyChildren();
    }
    public static void CloseHomeUI()
    {
        if (homeUI != null)
        {
            homeUI.SetActive(false);
            cameraUI.SetActive(false);
        }
    }
    public static bool refresh = false;
    public static void OpenHomeUI()
    {
        if (homeUI != null)
        {
            if (refresh)
            {
                homeUI.SetActive(true);
                cameraUI.SetActive(true);
                LoadBgFront(Config.Get("Wallpaper0", "���"));
                refresh = false;
            }
            else
            {
                homeUI.SetActive(true);
                cameraUI.SetActive(true);
            }
        }
    }
}
