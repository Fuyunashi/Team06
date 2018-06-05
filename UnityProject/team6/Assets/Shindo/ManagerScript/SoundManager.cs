using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class SoundVolume
{
    [SerializeField, Tooltip("bgmの音量")]
    public float bgm_ = 1.0f;
    [SerializeField, Tooltip("seの音量")]
    public float se_ = 1.0f;
    [SerializeField, Tooltip("")]
    public bool isMute_;

    public void Reset()
    {
        bgm_ = 0.5f;
        se_ = 0.4f;
        isMute_ = false;
    }

    public float defaultBgmVolume
    {
        get { return 0.5f; }
    }
}

public class SoundManager : Singleton<SoundManager> {

    SoundVolume volume_ = new SoundVolume();

    //リソース
    private AudioClip[] seClip_;
    private AudioClip[] bgmClip_;

    private Dictionary<string, int> seIndex_ = new Dictionary<string, int>();
    private Dictionary<string, int> bgmIndex_ = new Dictionary<string, int>();

    //プレーヤー
    const int numChannel_ = 16;
    private AudioSource bgmSource_;
    private AudioSource[] seSource_ = new AudioSource[numChannel_];

    Queue<int> seRequestQueue = new Queue<int>();

    private void Start()
    {
        bgmSource_ = gameObject.AddComponent<AudioSource>();
        bgmSource_.loop = true;

        for(int i = 0; i < seSource_.Length; i++)
        {
            seSource_[i] = gameObject.AddComponent<AudioSource>();
        }
        
        bgmClip_ = Resources.LoadAll<AudioClip>("Audio/BGM");
        seClip_ = Resources.LoadAll<AudioClip>("Audio/SE");

        //リソースに番号を振る
        for(int i = 0; i < seClip_.Length; i++)
        {
            seIndex_[seClip_[i].name] = i;
            Debug.Log(seIndex_);
        }
        //リソースに番号を振る
        for (int i = 0; i < bgmClip_.Length; i++)
        {
            bgmIndex_[bgmClip_[i].name] = i;
            Debug.Log(bgmIndex_);
        }

        foreach(var source in seSource_)
        {
            source.volume = volume_.se_;
        }

        
        

    }

    void Update()
    {
        bgmSource_.mute = volume_.isMute_;
        bgmSource_.volume = volume_.bgm_;

        foreach(var source in seSource_)
        {
            source.mute = volume_.isMute_;
        }

        int count = seRequestQueue.Count;
        if(count != 0)
        {
            int sound_index = seRequestQueue.Dequeue();
            playSeImpl(sound_index);
        }

    }

    //再生するかどうか
    private void playSeImpl(int index)
    {
        if(0 > index || seClip_.Length <= index)
        {
            return;
        }

        foreach(AudioSource source in seSource_)
        {
            if(!source.isPlaying)
            {
                source.clip = seClip_[index];
                source.Play();
                return;
            }
        }
    }

    //音量調整
    public void ChangeSeVolume(string name, float volume)
    {
        int index = GetSeIndex(name);

        foreach(AudioSource source in seSource_)
        {
            if(source.clip == seClip_[index])
            {
                source.volume = volume;
            }
        }
    }

    //SEの名前を取得
    public int GetSeIndex(string name)
    {
        return seIndex_[name];
    }

    //BGMの名前を取得
    public int GetBgmIndex(string name)
    {
        return bgmIndex_[name];
    }

    //BGM再生
    public void PlayBGM(string name)
    {
        int index = bgmIndex_[name];
        PlayBGM(index);
    }

    //BGM再生
    public void PlayBGM(int index)
    {
        if(0 > index || bgmClip_.Length <= index)
        {
            return;
        }

        if(bgmSource_.clip == bgmClip_[index])
        {
            return;
        }

        bgmSource_.Stop();
        bgmSource_.clip = bgmClip_[index];
        bgmSource_.Play();
    }

    //BGM停止
    public void StopBGM()
    {
        bgmSource_.Stop();
        bgmSource_.clip = null;
    }

    //再生しているかどうか
    public bool IsPlayBGM()
    {
        return bgmSource_.isPlaying;
    }

    //SEの再生しているかどうか
    public bool IsPlaySE(string name)
    {
        bool isplaying = false;
        int index = GetSeIndex(name);

        foreach(AudioSource source in seSource_)
        {
            if(source.clip == seClip_[index])
            {
                if (source.isPlaying)
                {
                    isplaying = true;
                }
            }
        }
        return isplaying;
    }

    //SE再生
    public void PlaySE(string name)
    {
        PlaySE(name, volume_.se_);
    }

    //SE再生
    public void PlaySE(string name, float volume)
    {
        PlaySE(GetSeIndex(name));
        ChangeSeVolume(name, volume);
    }

    //SE再生
    public void PlaySE(int index)
    {
        if (!seRequestQueue.Contains(index))
        {
            seRequestQueue.Enqueue(index);
        }
    }

    //SE停止
    public void StopSE()
    {
        
        foreach(AudioSource source in seSource_)
        {
            source.Stop();
            source.clip = null;
        }
    }

    //SE停止
    public void StopSE(string name)
    {
        StopSE(GetSeIndex(name));
    }

    //SE停止
    public void StopSE(int index)
    {
        if(0 > index || seClip_.Length <= index)
        {
            return;
        }

        foreach(AudioSource source in seSource_)
        {
            if (source.clip == seClip_[index])
            {
                source.Stop();
            }
        }
    }

    //キューの中身を空に
    public void ClearAllSeRequest()
    {
        seRequestQueue.Clear();
    }

}
