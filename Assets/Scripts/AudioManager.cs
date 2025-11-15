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
    
    public int CurrentBeat { get; private set; } = 0;
    public int CurrentMeasure => CurrentBeat / 4;
    private bool swapSoundwayOnNextBeat = false;


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
        uint bitmask = (uint)(AkCallbackType.AK_MusicSyncBeat | AkCallbackType.AK_MusicSyncEntry | AkCallbackType.AK_MusicSyncExit);
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
                // TODO: Spawn coins a certain distance in front of you every beat
                if (m_QueuedSoundway != null && m_QueuedSoundway.IsValid() && CurrentBeat % 4 == 0)
                {
                    OnSoundwaySwitch?.Invoke();
                    AkUnitySoundEngine.PostEvent(AudioEventIDs.Transition, m_GameObjectID);
                    m_QueuedSoundway.SetValue();
                    m_QueuedSoundway = null;
                }
                CurrentBeat++;
                OnBeat?.Invoke();
                break;
            case AkCallbackType.AK_MusicSyncEntry:
                OnMusicStart?.Invoke();
                CurrentBeat = 0;
                break;
            case AkCallbackType.AK_MusicSyncExit:
                OnMusicEnd?.Invoke();
                break;
        }
    }
}
