using UnityEngine;
using Audio.Generated;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;
    ulong m_GameObjectID;

    AK.Wwise.State m_QueuedSoundway = null;
    bool m_IsMissing = false;

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

                if (m_QueuedSoundway != null && m_QueuedSoundway.IsValid())
                {
                    AkUnitySoundEngine.PostEvent(AudioEventIDs.Transition, m_GameObjectID);
                    m_QueuedSoundway.SetValue();
                    m_QueuedSoundway = null;
                    // TODO: Switch the character to the new soundway
                }
                Debug.Log("beat");
                break;
            case AkCallbackType.AK_MusicSyncEntry:
                // TODO: Start movement when music starts
                Debug.Log("start");
                break;
            case AkCallbackType.AK_MusicSyncExit:
                // TODO: Stop movement when music ends
                break;
        }
    }
}
