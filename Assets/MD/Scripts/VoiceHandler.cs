using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using YGOSharp.OCGWrapper.Enums;

public class VoiceHandler : MonoBehaviour
{
    public static AudioSource audioSource;
    public static AudioSource audioSource2;
    public static float voiceTime;
    public static bool occupy;
    public static string character1 = "V0001";
    public static string character2 = "V0001";

    static AssetBundle ab1;
    static AssetBundle ab2;

    public static VoiceHandler instance;
    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource2 = GameObject.Find("Voice2").GetComponent<AudioSource>();
        instance = this;
        ABInit();
    }

    public static float voice1StopTime;
    public static float voice2StopTime;
    private void LateUpdate()
    {
        if (audioSource.isPlaying)
            voice1StopTime = 0;
        else
            voice1StopTime += Time.deltaTime;
        if (audioSource2.isPlaying)
            voice2StopTime = 0;
        else
            voice2StopTime += Time.deltaTime;
        if(voice1StopTime > 0.3f && audioSource.isPlaying == false)
        {
            Program.I().duelCommentMe.Hide();
            if (Program.I().setting.setting.Voice.value)
                Program.I().ocgcore.gameInfo.SetCharacter0Face();
        }
        if(voice2StopTime > 0.3f && audioSource2.isPlaying == false)
        {
            Program.I().duelCommentOp.Hide();
            if (Program.I().setting.setting.Voice.value)
                Program.I().ocgcore.gameInfo.SetCharacter1Face();
        }
    }
    public static VoiceHandler I()
    {
        return instance;
    }

    public void ABInit()
    {
        StartCoroutine(VoiceHandler.ABInitAsync());
    }

    static bool loading;
    public static IEnumerator ABInitAsync()
    {
        if (loading)
            yield break;
        loading = true;
        character1 = NameMap(Config.Get("Character0", "����Ϸ"));
        character2 = NameMap(Config.Get("Character1", "����Ϸ"));
        if(ab1 != null)
            ab1.Unload(false);
        if (ab2 != null)
            ab2.Unload(false);
        AssetBundleCreateRequest _ab1= AssetBundle.LoadFromFileAsync(Boot.root + "assetbundle/voice/" + character1.ToLower());
        yield return _ab1;
        ab1 = _ab1.assetBundle;

        if (character2 == character1)
            ab2 = ab1;
        else
        {
            AssetBundleCreateRequest _ab2 = AssetBundle.LoadFromFileAsync(Boot.root + "assetbundle/voice/" + character2.ToLower());
            yield return _ab2;
            ab2 = _ab2.assetBundle;
        }
        loading = false;
    }

    public static void PlayVoice(AudioClip clip, bool me = true, string content = "")
    {
        if(me)
        {
            audioSource.clip = clip;
            audioSource.Play();
            Program.I().duelCommentMe.Speak(content);
            Program.I().ocgcore.gameInfo.SetCharacter0Face(3);
        }
        else
        {
            audioSource2.clip = clip;
            audioSource2.Play();
            Program.I().duelCommentOp.Speak(content);
            Program.I().ocgcore.gameInfo.SetCharacter1Face(3);
        }
    }

    public static bool PlayCardLine(uint controller, string type, gameCard card, bool sleep, bool firstOnField = true, uint location = (uint)CardLocation.Onfield, int chain = 1, bool summonline = false)
    {
        if (!Program.I().setting.setting.Voice.value)
        {
            voiceTime = 0;
            return false;
        }
        string character = controller == 0 ? NameMap(Config.Get("Character0", "����Ϸ")) : NameMap(Config.Get("Character1", "����Ϸ"));
        int voiceId = VoiceMap.Map(character, type, card, firstOnField, location, chain);
        if (voiceId <= -1)
            return false;

        bool onlyPlayOne = false;
        if(voiceId >= 1000)
        {
            onlyPlayOne = true;
            voiceId -= 1000;
        }
        bool hand = false;
        if (voiceId >= 100)
        {
            hand = true;
            voiceId -= 100;
        }
        string typeId = "";
        switch (type)
        {
            case "summon":
                typeId = "13";
                break;
            case "attack":
                typeId = "19";
                break;
            case "activate":
                typeId = "08";
                break;
            case "activateM":
                typeId = "09";
                break;
        }
        string clipname;
        if (voiceId > 9)
            clipname = string.Format("{0}_{1}_00_{2}_0", character, typeId, voiceId);
        else
            clipname = string.Format("{0}_{1}_00_0{2}_0", character, typeId, voiceId);

        AudioClip clip = GetClipFromAB( controller, clipname);

        if (clip != null)
        {
            if (type == "summon")
            {
                if(summonline)
                    PlayGroup(controller, clip, sleep, onlyPlayOne, hand, true, card);
                else
                    PlayGroup(controller, clip, sleep, onlyPlayOne, hand, true);
            }
            else
                PlayGroup(controller, clip, sleep, onlyPlayOne, hand, false);
            return true;
        }
        else
            return false;
    }

    static void PlayGroup(uint controller, AudioClip clip0, bool sleep, bool onlyPlayOne = false, bool hand = false,bool summon = false, gameCard card = null, bool showText = true)
    {
        AudioClip clip = clip0;
        List<AudioClip> audioClips = new List<AudioClip>();
        if (hand)
        {
            var clipFromHand = GetClipFromAB(controller, clip0.name.Substring(0,6) + "07_00_00_0");
            audioClips.Add(clipFromHand);
        }
        if(card != null && card.get_data().Id != 0)
        {
            int voiceId = VoiceMap.Map(clip0.name.Substring(0,5), "summonline", card, true, (uint)CardLocation.Onfield, 1);
            if( voiceId >= 0)
            {
                string sl = "";
                if (voiceId > 9)
                    sl = clip0.name.Substring(0, 5) + "_37_00_" + voiceId + "_0";
                if (voiceId <= 9)
                    sl = clip0.name.Substring(0, 5) + "_37_00_0" + voiceId + "_0";
                AudioClip clipSL = GetClipFromAB(controller, sl);
                int k = 0;
                while (clipSL != null)
                {
                    audioClips.Add(clipSL);
                    k++;
                    clipSL = GetClipFromAB(controller, clipSL.name.Substring(0, 15) + k);
                }
            }
        }
        audioClips.Add(clip);
        if (!onlyPlayOne)
        {
            int i = 0;
            while (clip != null)
            {
                i++;
                clip = GetClipFromAB(controller, clip0.name.Substring(0, 15) + i);
                if (clip != null)
                    audioClips.Add(clip);
            }
        }

        //List<float> datas = new List<float>();
        //for (int j = 0; j < audioClips.Count; j++)
        //{
        //    float[] data = new float[audioClips[j].samples * audioClips[j].channels];
        //    audioClips[j].GetData(data, 0);
        //    datas.AddRange(data);
        //    audioClips[j].UnloadAudioData();
        //}
        //float[] clipdatas = datas.ToArray();
        //AudioClip result = AudioClip.Create("Combine", clipdatas.Length, 1, 44100, false);
        //result.SetData(clipdatas, 0);

        //if (controller == 0)
        //{
        //    audioSource.clip = result;
        //    audioSource.Play();
        //}
        //else
        //{
        //    audioSource2.clip = result;
        //    audioSource2.Play();
        //}
        float fullTime = 0;
        foreach (var _clip in audioClips)
            fullTime += _clip.length;

        if (sleep)
            Program.I().ocgcore.Sleep((int)(fullTime * 60));
        voiceTime = fullTime;

        float waitTime = 0;
        foreach (AudioClip c in audioClips)
        {
            VoiceLine voiceLine = VoiceLineHandler.GetVoiceLine(controller, c.name);
            if (controller == 0)
            {
                Program.I().duelCommentMe.DelaySpeak(waitTime, voiceLine == null ? "" : voiceLine.text, c, showText, c == audioClips[audioClips.Count - 1]);
                int face = voiceLine == null ? 0 : voiceLine.face;
                if (face == 0) face = 3;
                Program.I().ocgcore.gameInfo.SetCharacter0Face(face);
            }
            else
            {
                Program.I().duelCommentOp.DelaySpeak(waitTime, voiceLine == null ? "" : voiceLine.text, c, showText, c == audioClips[audioClips.Count - 1]);
                int face = voiceLine == null ? 0 : voiceLine.face;
                if (face == 0) face = 3;
                Program.I().ocgcore.gameInfo.SetCharacter1Face(face);
            }
            waitTime += c.length;
        }
    }

    public static void PlayFixedLine(uint controller ,string line, bool sleep = true, string cardName = "", bool showText = true)
    {
        if (!Program.I().setting.setting.Voice.value)
        {
            voiceTime = 0;
            return;
        }
        if (IsSummon(line) & occupy)
            return;
        AudioClip clip;
        List<AudioClip> clips = new List<AudioClip>();
        if (controller == 0)
        {
            clip = ab1.LoadAsset<AudioClip>(NameMap(Config.Get("Character0", "����Ϸ")) + LineMap(line));
            int i = 0;
            while(clip != null)
            {
                clips.Add(clip);
                i++;
                clip = ab1.LoadAsset<AudioClip>((NameMap(Config.Get("Character0", "����Ϸ")) + LineMap(line)).Substring(0, 13) + i + "_0");
            }
            if(clips.Count > 0)
            {
                if(line == "MyTurn" || line == "TurnEnd")
                {
                    int num = clips.Count / 3;

                    if (Program.I().ocgcore.life_0 > Program.I().ocgcore.life_1 
                        && 
                        Program.I().ocgcore.life_0 > Program.I().ocgcore.lpLimit / 2 
                        && 
                        Program.I().ocgcore.life_1 < Program.I().ocgcore.lpLimit / 2)
                    {
                        clip = clips[UnityEngine.Random.Range(0, num)];
                    }
                    else if(Program.I().ocgcore.life_0 < Program.I().ocgcore.life_1 
                        && 
                        Program.I().ocgcore.life_0 < Program.I().ocgcore.lpLimit / 2
                        &&
                        Program.I().ocgcore.life_1 > Program.I().ocgcore.lpLimit / 2
                        )
                    {
                        clip = clips[UnityEngine.Random.Range(num * 2, clips.Count)];
                    }
                    else 
                    {
                        clip = clips[UnityEngine.Random.Range(num, num * 2)];
                    }
                }
                else
                {
                    clip = clips[UnityEngine.Random.Range(0, clips.Count)];
                }
            }
        }
        else
        {
            clip = ab2.LoadAsset<AudioClip>(NameMap(Config.Get("Character1", "����Ϸ")) + LineMap(line));
            int i = 0;
            while (clip != null)
            {
                clips.Add(clip);
                i++;
                clip = ab2.LoadAsset<AudioClip>((NameMap(Config.Get("Character1", "����Ϸ")) + LineMap(line)).Substring(0, 13) + i + "_0");
            }
            if (clips.Count > 0)
            {
                if (line == "MyTurn" || line == "TurnEnd")
                {
                    int num = clips.Count / 3;

                    if (Program.I().ocgcore.life_0 < Program.I().ocgcore.life_1
                        &&
                        Program.I().ocgcore.life_0 < Program.I().ocgcore.lpLimit / 2
                        &&
                        Program.I().ocgcore.life_1 > Program.I().ocgcore.lpLimit / 2)
                    {
                        clip = clips[UnityEngine.Random.Range(0, num)];
                    }
                    else if (Program.I().ocgcore.life_0 > Program.I().ocgcore.life_1
                        &&
                        Program.I().ocgcore.life_0 > Program.I().ocgcore.lpLimit / 2
                        &&
                        Program.I().ocgcore.life_1 < Program.I().ocgcore.lpLimit / 2
                        )
                    {
                        clip = clips[UnityEngine.Random.Range(num * 2, clips.Count)];
                    }
                    else
                    {
                        clip = clips[UnityEngine.Random.Range(num, num * 2)];
                    }
                }
                else
                {
                    clip = clips[UnityEngine.Random.Range(0, clips.Count)];
                }
            }
        }
        if (clip == null && line == "LinkSummon")
            PlayFixedLine(controller, "SpecialSummon", sleep, cardName, false);
        if (clip == null && line == "SetPendulumScale")
            PlayFixedLine(controller, "ActivateMagic", sleep, cardName, false);
        if (clip == null && line == "ActivatePendulum")
            PlayFixedLine(controller, "Activate", sleep, cardName, false);
        if (clip != null)
            PlayGroup(controller, clip, sleep, false, false, false, null, false);

        if(showText)
        {
            bool query = true;
            string text = "";
            VoiceLine voiceLine = null;
            if (clip != null)
            {
                voiceLine = VoiceLineHandler.GetVoiceLine(controller, clip.name);
                if (clip.name.Substring(6, 2) == "07")
                {
                    if (clip.name.EndsWith("_00_00_0") == false)
                        query = false;
                }
                else if (clip.name.Substring(6, 2) == "11")
                    query = false;
                else if (clip.name.Substring(6, 2) == "17")
                    query = false;
                else if (clip.name.Substring(6, 2) == "18")
                    query = false;
                if (query && voiceLine != null)
                    text = voiceLine.text;
            }

            if (controller == 0)
            {
                if (text != "")
                    Program.I().duelCommentMe.Speak(text);
                else
                    Program.I().duelCommentMe.Speak(Line2CommentMap(line, cardName));
                int face = 0;
                if (clip != null && voiceLine != null)
                    face = voiceLine.face;
                if (face == 0) face = 3;
                Program.I().ocgcore.gameInfo.SetCharacter0Face(face);
                voice1StopTime = 0;
            }
            else
            {
                if (text != "")
                    Program.I().duelCommentOp.Speak(text);
                else
                    Program.I().duelCommentOp.Speak(Line2CommentMap(line, cardName));
                int face = 0;
                if(clip != null && voiceLine != null)
                    face = voiceLine.face;
                if (face == 0) face = 3;
                Program.I().ocgcore.gameInfo.SetCharacter1Face(face);
                voice2StopTime = 0;
            }
        }
    }

    public static string NameMap(string name)
    {
        switch (name)
        {
            case "����Ϸ":
                return "V0001";
            case "��������":
                return "V0002";
            case "��֮�ڿ�Ҳ":
                return "V0003";
            case "��ȸ��":
                return "V0004";
            case "��������":
                return "V0005";
            case "������Ϸ":
                return "V0006";
            case "�������":
                return "V0007";
            case "��������":
                return "V0008";
            case "�|ľ��̫":
                return "V0009";
            case "��������":
                return "V0010";
            case "������":
                return "V0011";
            case "������˹":
                return "V0012";
            case "����˿�����޴��":
                return "V0013";
            case "��ϣ��":
                return "V0014";
            case "����˹":
                return "V0015";
            case "����˫��":
                return "V0016";
            case "�����ƽ":
                return "V0017";
            case "�Թ��ֵ�":
                return "V0019";
            case "�Թ��ֵܡ���":
                return "V0020";
            case "�Թ��ֵܡ���":
                return "V0021";
            case "�˶���":
                return "V0022";
            case "���ڣ":
                return "V0023";
            case "���ܽ���":
                return "V0024";
            case "�������":
                return "V0029";
            case "�����ݵĳ�֮��":
                return "V0030";
            case "�ⰵ����":
                return "V0031";
            case "��֮����":
                return "V0032";
            case "��֮����":
                return "V0033";
            case "��٤����":
                return "V0034";

            case "�γ�ʮ��":
                return "V0101";
            case "������":
                return "V0102";
            case "���á������˹":
                return "V0103";
            case "Լ��������ɭ":
                return "V0104";
            case "����Ŀ׼":
                return "V0105";
            case "����Ժ������":
                return "V0106";
            case "����ŵ˹���¡�÷����":
                return "V0107";
            case "�ȱ���":
                return "V0109";
            case "������":
                return "V0110";
            case "������":
                return "V0112";
            case "����ŵ��ɽ":
                return "V0113";
            case "ի����ĥ":
                return "V0114";
            case "�γ�ʮ��/�ȱ���":
                return "V0115";
            case "����Ů��":
                return "V0140";
            case "��˹���²�����":
                return "V0153";
            case "��ķ�����ʹ��������":
                return "V0154";
            case "����ʮ��":
                return "V0155";

            case "��������":
                return "V0201";
            case "�ܿˡ�������˹":
                return "V0202";
            case "���ޡ�����":
                return "V0203";
            case "ʮ��ҹ��":
                return "V0204";
            case "����":
                return "V0205";
            case "����":
                return "V0206";
            case "ţβ��":
                return "V0207";
            case "DS��������":
                return "V0208";
            case "����ŵ��":
                return "V0209";
            case "DS�����":
                return "V0210";
            case "DS�׿�˹�������":
                return "V0211";
            case "��������":
                return "V0213";
            case "�����":
                return "V0214";
            case "��������":
                return "V0215";
            case "��������":
                return "V0216";
            case "�������˹":
                return "V0217";
            case "Z-ONE":
                return "V0218";

            case "DSOD��������":
                return "V0301";
            case "DSOD�����ƽ":
                return "V0302";
            case "DSOD������Ϸ":
                return "V0303";
            case "DSOD��֮�ڿ�Ҳ":
                return "V0304";
            case "DSOD��������":
                return "V0305";
            case "����":
                return "V0306";
            case "����":
                return "V0307";
            case "����-��ά":
                return "V0308";
            case "�ټ�ľ":
                return "V0309";
            case "DSOD������":
                return "V0315";

            case "��ʮ������/��˹������":
                return "V0401";
            case "�������":
                return "V0404";
            case "��������":
                return "V0405";
            case "����С��":
                return "V0406";
            case "��ǿ춷":
                return "V0407";
            case "III":
                return "V0409";
            case "IV":
                return "V0410";
            case "V":
                return "V0411";
            case "�������":
                return "V0412";
            case "ZEXAL":
                return "V0415";
            case "���°���":
                return "V0418";
            case "������":
                return "V0419";
            case "������":
                return "V0420";
            case "��������������":
                return "V0441";
            case "��������������":
                return "V0442";

            case "�Y��ʸ":
                return "V0501";
            case "������":
                return "V0502";
            case "Ȩ������":
                return "V0503";
            case "�������":
                return "V0504";
            case "�������":
                return "V0505";
            case "�ζ�":
                return "V0506";
            case "����Ժ����":
                return "V0507";
            case "�چD��":
                return "V0508";
            case "����":
                return "V0510";
            case "������":
                return "V0512";

            case "PlayerMaker&Ai":
                return "V0601";
            case "SoulBurner":
                return "V0602";
            case "Go��V":
                return "V0603";
            case "BlueAngel":
                return "V0604";
            case "Revolver":
                return "V0605";
            case "GhostGirl":
                return "V0610";
            case "BraveMax":
                return "V0849";
            case "��ŵ��ʿ":
                return "V0850";

            case "����С��":
                return "V9995";

            default:
                return "V0001";
        }
    }

    static bool IsSummon(string line)
    {
        switch (line)
        {
            case "Summon":
                return true;
            case "SummonInDefence":
                return true;
            case "SpecialSummon":
                return true;
            case "Release":
                return true;
            case "AdvanceSummon":
                return true;
            case "FusionSummon":
                return true;
            case "RitualSummon":
                return true;
            case "SynchroSummon":
                return true;
            case "XyzSummon":
                return true;
            case "LinkSummon":
                return true;
            default: 
                return false;
        }
    }

    static string LineMap(string line)
    {
        switch (line)
        {
            default:
                return "";
            case "Start":
                return "_01_00_00_0";
            case "Duel":
                return "_02_00_00_0";
            case "MyTurn":
                return "_03_00_00_0";
            case "Draw":
                return "_04_00_00_0";
            case "DestinyDraw":
                return "_05_00_00_0";
            case "Now":
                return "_06_01_00_0";
            case "Naive":
                return "_06_01_01_0";
            case "Naive2":
                return "_06_01_02_0";
            case "Counter":
                return "_06_01_03_0";
            case "FromHand":
                return "_07_00_00_0";
            case "ActivateMagic":
                return "_07_01_00_0";
            case "ActivateQuickPlayMagic":
                return "_07_02_00_0";
            case "ActivateContinuousMagic":
                return "_07_03_00_0";
            case "ActivateEquipMagic":
                return "_07_04_00_0";
            case "ActivateRitualMagic":
                return "_07_05_00_0";
            case "ActivateTrap":
                return "_07_06_00_0";
            case "ActivateContinuousTrap":
                return "_07_07_00_0";
            case "ActivateCounterTrap":
                return "_07_08_00_0";
            case "OpenSetCard":
                return "_07_09_00_0";
            case "ActivateMonsterEffect":
                return "_07_10_00_0";
            case "ActivateFieldMagic":
                return "_07_11_00_0";
            case "ActivateEffect":
                return "_07_12_00_0";
            case "SetPendulumScale":
                return "_07_13_00_0";
            case "ActivatePendulum":
                return "_07_14_00_0";
            case "ActivateEffects":
                return "_08_00_00_0";
            case "ActivateMonsterEffects":
                return "_09_00_00_0";
            case "PreSummon":
                return "_10_00_00_0";
            case "Summon":
                return "_11_00_00_0";
            case "SummonInDefence":
                return "_11_01_00_0";
            case "SpecialSummon":
                return "_11_02_00_0";
            case "Release":
                return "_11_03_00_0";
            case "AdvanceSummon":
                return "_11_04_00_0";
            case "FusionSummon":
                return "_11_05_00_0";
            case "RitualSummon":
                return "_11_06_00_0";
            case "SynchroSummon":
                return "_11_07_00_0";
            case "XyzSummon":
                return "_11_08_00_0";
            case "PendulumSummon":
                return "_11_09_00_0";
            case "LinkSummon":
                return "_11_10_00_0";
            case "SummonMonsters":
                return "_13_10_00_0";
            case "BattlePhase":
                return "_14_00_00_0";
            case "PreAttack":
                return "_15_00_00_0";
            case "LastAttack":
                return "_16_00_00_0";
            case "Attack":
                return "_17_00_00_0";
            case "DirectAttack":
                return "_18_00_00_0";
            case "MonstersAttack":
                return "_19_00_00_0";
            case "SetCard":
                return "_20_00_00_0";
            case "SetMonster":
                return "_20_01_00_0";
            case "TurnEnd":
                return "_21_00_00_0";
            case "GetDamage":
                return "_22_00_00_0";
            case "LostRoar":
                return "_23_00_00_0";
            case "PayCost":
                return "_24_00_00_0";
            case "GetHugeDamage":
                return "_25_00_00_0";
            case "AfterDamage":
                return "_26_00_00_0";
            case "Adversity":
                return "_27_00_00_0";
            case "Victory":
                return "_28_00_00_0";
            case "Defeat":
                return "_29_00_00_0";
            case "Chat":
                return "_30_00_00_0";
            case "Nani":
                return "_31_00_00_0";
            case "TitleCall":
                return "_32_00_00_0";
            case "Chat2":
                return "_33_00_00_0";
            case "Chat3":
                return "_34_00_00_0";
            case "Chat4":
                return "_35_00_00_0";
            case "Tag":
                return "_36_00_00_0";
            case "SummonLines":
                return "_37_00_00_0";
            case "RidingDuel":
                return "_38_00_00_0";
            case "CoinEffect":
                return "_39_00_00_0";
            case "CoinEffect2":
                return "_40_00_00_0";
            case "PreDimensionDuel":
                return "_41_00_00_0";
            case "DimensionDuel":
                return "_42_00_00_0";
            case "HenShinn":
                return "_43_00_00_0";
            case "ActionDuel":
                return "_44_00_00_0";
            case "ActionCard":
                return "_45_00_00_0";
            case "Reincarnation":
                return "_46_00_00_0";
            case "AfterReincarnation":
                return "_47_00_00_0";
        }
    }

    static string Line2CommentMap(string line, string cardName)
    {
        switch (line)
        {
            default:
                return "";
            case "Start":
                //return "_01_00_00_0";
                return "���������ɣ�";
            case "Duel":
                //return "_02_00_00_0";
                return "������";
            case "MyTurn":
                //return "_03_00_00_0";
                return "�ҵĻغ�";
            case "Draw":
                //return "_04_00_00_0";
                return "�鿨";
            case "DestinyDraw":
                //return "_05_00_00_0";
                return "����һ�飡";
            case "Now":
                //return "_06_01_00_0";
                return "��˲��";
            case "Naive":
                //return "_06_01_01_0";
                return "�ǿ�˵��׼";
            case "Naive2":
                //return "_06_01_02_0";
                return "����";
            case "Counter":
                //return "_06_01_03_0";
                return "�ǿɲ���˵";
            case "FromHand":
                //return "_07_00_00_0";
                return "���ֿ�";
            case "ActivateMagic":
                //return "_07_01_00_0";
                return "����ħ���� " + cardName;
            case "ActivateQuickPlayMagic":
                //return "_07_02_00_0";
                return "�����ٹ�ħ�� " + cardName;
            case "ActivateContinuousMagic":
                //return "_07_03_00_0";
                return "��������ħ�� " + cardName;
            case "ActivateEquipMagic":
                //return "_07_04_00_0";
                return "����װ��ħ�� " + cardName;
            case "ActivateRitualMagic":
                //return "_07_05_00_0";
                return "������ʽħ�� " + cardName;
            case "ActivateTrap":
                //return "_07_06_00_0";
                return "�������忨 " + cardName;
            case "ActivateContinuousTrap":
                //return "_07_07_00_0";
                return "������������ ";
            case "ActivateCounterTrap":
                //return "_07_08_00_0";
                return "������������ ";
            case "OpenSetCard":
                //return "_07_09_00_0";
                return "�򿪸ǿ� ";
            case "ActivateMonsterEffect":
                //return "_07_10_00_0";
                return "����" + cardName + "�Ĺ���Ч��";
            case "ActivateFieldMagic":
                //return "_07_11_00_0";
                return "��������ħ�� " + cardName;
            case "ActivateEffect":
                //return "_07_12_00_0";
                return "����" + cardName + "��Ч��";
            case "SetPendulumScale":
                //return "_07_13_00_0";
                return "������ڿ̶�";
            case "ActivatePendulum":
                //return "_07_14_00_0";
                return "����" + cardName + "�����Ч��";
            case "ActivateEffects":
                //return "_08_00_00_0";
                return "����" + cardName + "��Ч��";
            case "ActivateMonsterEffects":
                //return "_09_00_00_0";
                return "����" + cardName + "��Ч��";
            case "PreSummon":
                //return "_10_00_00_0";
                return "������";
            case "Summon":
                //return "_11_00_00_0";
                return "�ٻ� " + cardName;
            case "SummonInDefence":
                //return "_11_01_00_0";
                return "�ر���ʾ�ٻ� " + cardName;
            case "SpecialSummon":
                //return "_11_02_00_0";
                return "�����ٻ� " + cardName;
            case "Release":
                //return "_11_03_00_0";
                return "���" + cardName;
            case "AdvanceSummon":
                //return "_11_04_00_0";
                return "�ϼ��ٻ� " + cardName;
            case "FusionSummon":
                //return "_11_05_00_0";
                return "�ں��ٻ� " + cardName;
            case "RitualSummon":
                //return "_11_06_00_0";
                return "��ʽ�ٻ� " + cardName;
            case "SynchroSummon":
                //return "_11_07_00_0";
                return "ͬ���ٻ� " + cardName;
            case "XyzSummon":
                //return "_11_08_00_0";
                return "�����ٻ� " + cardName;
            case "PendulumSummon":
                //return "_11_09_00_0";
                return "����ٻ�����";
            case "LinkSummon":
                //return "_11_10_00_0";
                return "�����ٻ� " + cardName;
            case "SummonMonsters":
                //return "_13_10_00_0";
                return "�ٻ� " + cardName;
            case "BattlePhase":
                //return "_14_00_00_0";
                return "��ս��";
            case "PreAttack":
                //return "_15_00_00_0";
                return "�ϰ�";
            case "LastAttack":
                //return "_16_00_00_0";
                return "���¾ͽ����ˣ�";
            case "Attack":
                //return "_17_00_00_0";
                return cardName + " ������";
            case "DirectAttack":
                //return "_18_00_00_0";
                return cardName + " ֱ�ӹ�����";
            case "MonstersAttack":
                //return "_19_00_00_0";
                return cardName + " ������";
            case "SetCard":
                //return "_20_00_00_0";
                return "��1�ſ�";
            case "SetMonster":
                //return "_20_01_00_0";
                return "��1ֻ����";
            case "TurnEnd":
                //return "_21_00_00_0";
                return "�غϽ���";
            case "GetDamage":
                //return "_22_00_00_0";
                return "���";
            case "LostRoar":
                //return "_23_00_00_0";
                return "���";
            case "PayCost":
                //return "_24_00_00_0";
                return "���";
            case "GetHugeDamage":
                //return "_25_00_00_0";
                return "������";
            case "AfterDamage":
                //return "_26_00_00_0";
                return "��û��";
            case "Adversity":
                //return "_27_00_00_0";
                return "��ô��ȥ�Ļ�����";
            case "Victory":
                //return "_28_00_00_0";
                return "��Ӯ�ˣ�";
            case "Defeat":
                //return "_29_00_00_0";
                return "�����ˡ���";
            case "Chat":
                //return "_30_00_00_0";
                return "˵ɧ��";
            case "Nani":
                //return "_31_00_00_0";
                return "���ᣡ��";
            case "TitleCall":
                //return "_32_00_00_0";
                return "Yu-Gi-Oh! Duel Links";
            case "Chat2":
                //return "_33_00_00_0";
                return "˵ɧ��";
            case "Chat3":
                //return "_34_00_00_0";
                return "˵ɧ��";
            case "Chat4":
                //return "_35_00_00_0";
                return "˵ɧ��";
            case "Tag":
                //return "_36_00_00_0";
                return "����";
            case "SummonLines":
                //return "_37_00_00_0";
                return "�ж����ٻ���";
            case "RidingDuel":
                //return "_38_00_00_0";
                return "��˾��������٣���";
            case "CoinEffect":
                return "_39_00_00_0";
            case "CoinEffect2":
                return "_40_00_00_0";
            case "PreDimensionDuel":
                return "_41_00_00_0";
            case "DimensionDuel":
                return "_42_00_00_0";
            case "HenShinn":
                return "_43_00_00_0";
            case "ActionDuel":
                return "_44_00_00_0";
            case "ActionCard":
                return "_45_00_00_0";
            case "Reincarnation":
                return "_46_00_00_0";
            case "AfterReincarnation":
                return "_47_00_00_0";
        }
    }

    public static AudioClip GetClip(string p, bool threeD = true, bool stream = true)
    {
        var path = "sound/" + p + ".ogg";
        if (File.Exists(path) == false) path = "sound/" + p + ".wav";
        if (File.Exists(path) == false) path = "sound/" + p + ".mp3";
        if (File.Exists(path) == false) return null;
        path = Environment.CurrentDirectory.Replace("\\", "/") + "/" + path;
        path = "file:///" + path;
        WWW www = new WWW(path);
        return www.GetAudioClip(threeD, stream);
    }

    static AudioClip GetClipFromAB(uint controller, string name)
    {
        if( controller == 0 )
            return ab1.LoadAsset<AudioClip>(name);
        else
            return ab2.LoadAsset<AudioClip>(name);
    }
}
