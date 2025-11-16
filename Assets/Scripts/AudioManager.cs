using System;
using UnityEngine;
using Audio.Generated;
using DefaultNamespace;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;
    ulong m_GameObjectID;

    AK.Wwise.State m_QueuedSoundway = null;
    bool m_IsMissing = false;
    
    public event Action OnMusicStart;
    public event Action OnMusicEnd;
    public event Action OnSoundwaySwitch;
    public event Action OnBeat;
    
    public SongData CurrentSongData;
    public int CurrentMeasure = 0;

    private bool m_IsFirstMeasure = true;

    public void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
    }

    private void Start()
    {
        m_GameObjectID = AkUnitySoundEngine.GetAkGameObjectID(gameObject);
        AkUnitySoundEngine.PostEvent(AudioEventIDs.Startup, m_GameObjectID); // Passing in IDs here because they have less latency
    }

    public void StartMusic()
    {
        uint bitmask = (uint)(AkCallbackType.AK_MusicSyncBeat | AkCallbackType.AK_MusicSyncBar | AkCallbackType.AK_MusicSyncEntry | AkCallbackType.AK_MusicSyncExit);
        AkUnitySoundEngine.PostEvent(AudioEventIDs.Music_Prototype, m_GameObjectID, bitmask, MusicCallback, null);
    }

    public void QueueSoundwaySwitch(AK.Wwise.State state)
    {
        m_QueuedSoundway = state;
    }

    public void MissCoin()
    {
        if (!m_IsMissing)
        {
            m_IsMissing = true;
            AkUnitySoundEngine.PostEvent(AudioEventIDs.FirstMiss, m_GameObjectID);
        }
    }

    public void GetCoin()
    {
        if (m_IsMissing)
        {
            m_IsMissing = false;
            AkUnitySoundEngine.PostEvent(AudioEventIDs.StopMiss, m_GameObjectID);
        }
    }

    void MusicCallback(object in_cookie, AkCallbackType in_eType, AkCallbackInfo in_pCallbackInfo)
    {
        switch (in_eType)
        {
            case AkCallbackType.AK_MusicSyncBeat:
                OnBeat?.Invoke();
                break;
            case AkCallbackType.AK_MusicSyncBar:
                // Perform transition at the start of a measure if a soundway switch is queued
                if (m_QueuedSoundway != null && m_QueuedSoundway.IsValid())
                {
                    OnSoundwaySwitch?.Invoke();
                    AkUnitySoundEngine.PostEvent(AudioEventIDs.Transition, m_GameObjectID);
                    m_QueuedSoundway.SetValue();
                    m_QueuedSoundway = null;
                }

                // Spawn a coin preset one measure in front of you every measure

                if (!m_IsFirstMeasure)
                {
                    CurrentMeasure++;
                }
                else
                {
                    m_IsFirstMeasure = false;
                }
                break;
            case AkCallbackType.AK_MusicSyncEntry:
                OnMusicStart?.Invoke();
                break;
            case AkCallbackType.AK_MusicSyncExit:
                OnMusicEnd?.Invoke();
                break;
        }
    }
}
