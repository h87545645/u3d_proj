using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class SoundMgr : MonoSingletonBase<SoundMgr>
{
    /// <summary>
    /// 缓存背景音乐的播放器
    /// </summary>
    private AudioSource uObj_bgAudio = null;

    /// <summary>
    /// 音效播放器的游戏对象
    /// </summary>
    private GameObject uObj_effectObj = null;

    /// <summary>
    /// 背景音量
    /// </summary>
    private float fBgVolume = 1;

    /// <summary>
    /// 音效音量
    /// </summary>
    private float fEffectVolume = 1;

    /// <summary>
    /// 背景音效基础路径
    /// </summary>
    private string sBgSoundBasePath = "SoundRes/Bg/";
    /// <summary>
    /// 音效基础路径
    /// </summary>
    private string sEffectSoundBasePath = "SoundRes/Effect/";


    /// <summary>
    /// 缓存当前播放着的所有音效播放器
    /// </summary>
    /// <typeparam name="AudioSource"></typeparam>
    /// <returns></returns>
    private List<AudioSource> list_allEffectAudio = new List<AudioSource>();

    protected override void Awake()
    {
        base.Awake();
        DontDestroyOnLoad(gameObject);
    }

    /// <summary>
    /// 
    /// </summary>
    private void Update()
    {

    }

    #region  背景音乐
    /// <summary>
    /// 同步播放背景音乐
    /// </summary>
    /// <param name="name"></param>
    public void PlayBgMusic(string name)
    {
        if (uObj_bgAudio == null)
        {
            GameObject uObj_temp = new GameObject("BgMusic");
            uObj_bgAudio = uObj_temp.AddComponent<AudioSource>();
            uObj_temp.transform.SetParent(transform);
        }

        AudioClip uObj_bgRes = ResourcesLoadMgr.I.LoadSync(sBgSoundBasePath + name) as AudioClip;
        uObj_bgAudio.clip = uObj_bgRes;
        uObj_bgAudio.loop = true;
        uObj_bgAudio.volume = fBgVolume;
        uObj_bgAudio.Play();
    }

    /// <summary>
    /// 异步播放背景音乐
    /// </summary>
    /// <param name="name"></param>
    public void PlayBgMusicAsync(string name)
    {
        if (uObj_bgAudio == null)
        {
            GameObject uObj_temp = new GameObject("Obj_BgMusic");
            uObj_bgAudio = uObj_temp.AddComponent<AudioSource>();
            uObj_temp.transform.SetParent(transform);
        }
        AssetsLoadMgr.I.LoadAsync(sBgSoundBasePath + name, (string name, UnityEngine.Object obj) => {
            uObj_bgAudio.clip = obj as AudioClip;
            uObj_bgAudio.loop = true;
            uObj_bgAudio.volume = fBgVolume;
            uObj_bgAudio.Play();
        });
    }

    /// <summary>
    /// 改变背景音乐的音量大小
    /// </summary>
    /// <param name="nVolume"></param>
    public void ChangeBgVolume(float nVolume)
    {
        fBgVolume = nVolume;

        if (uObj_bgAudio == null) return;
        uObj_bgAudio.volume = nVolume;
    }

    public void UnPauseBgMusic()
    {
        if (uObj_bgAudio == null) return;
        uObj_bgAudio.UnPause();
    }

    /// <summary>
    /// 暂停播放背景音乐
    /// </summary>
    public void PauseBgMusic()
    {
        if (uObj_bgAudio == null) return;
        uObj_bgAudio.Pause();
    }

    /// <summary>
    /// 停止播放背景音乐
    /// </summary>
    public void StopBgMusic()
    {
        if (uObj_bgAudio == null) return;
        uObj_bgAudio.Stop();
    }
    #endregion

    #region 音效处理
    private void InitEffectSoundObj()
    {
        if (uObj_effectObj == null)
        {
            uObj_effectObj = new GameObject();
            uObj_effectObj.name = "Obj_EffectSounds";
            uObj_effectObj.transform.SetParent(transform);
        }
    }

    //播放音效
    public void PlayEffectSound(string sName, bool bIsLoop, UnityAction<AudioSource> callback = null)
    {
        InitEffectSoundObj();

        AudioClip uObj_effectClip = ResourcesLoadMgr.I.LoadSync(sEffectSoundBasePath + sName) as AudioClip;
        AudioSource uObj_curAudio = uObj_effectObj.AddComponent<AudioSource>();
        uObj_curAudio.clip = uObj_effectClip;
        uObj_curAudio.loop = bIsLoop;
        //调整大小 
        uObj_curAudio.volume = fEffectVolume;
        uObj_curAudio.Play();
        //音效资源异步加载结束后，将这个音效组件加入集合中
        list_allEffectAudio.Add(uObj_curAudio);
        if (callback != null)
        {
            callback(uObj_curAudio);
        }
    }

    //播放音效
    public void PlayEffectSoundAsync(string sName, bool bIsLoop, UnityAction<AudioSource> callback = null)
    {
        InitEffectSoundObj();
        AssetsLoadMgr.I.LoadAsync(sBgSoundBasePath + name, (string name, UnityEngine.Object obj) => {
            if (uObj_effectObj == null) return;

            AudioSource uObj_curAudio = uObj_effectObj.AddComponent<AudioSource>();
            uObj_curAudio.clip = obj as AudioClip;
            uObj_curAudio.loop = bIsLoop;
            //调整大小 
            uObj_curAudio.volume = fEffectVolume;
            uObj_curAudio.Play();
            //音效资源异步加载结束后，将这个音效组件加入集合中
            list_allEffectAudio.Add(uObj_curAudio);
            if (callback != null)
            {
                callback(uObj_curAudio);
            }
        });
    }

    //改变所有音效大小
    public void ChangeSoundValue(float nVolume)
    {
        fEffectVolume = nVolume;
        for (int i = 0; i < list_allEffectAudio.Count; i++)
        {
            list_allEffectAudio[i].volume = nVolume;
        }
    }

    //停止音效
    public void StopSound(AudioSource uObj_effectAudio)
    {
        if (list_allEffectAudio.Contains(uObj_effectAudio))
        {
            list_allEffectAudio.Remove(uObj_effectAudio);
            uObj_effectAudio.Stop();
            GameObject.Destroy(uObj_effectAudio);
        }
    }
    #endregion
}
