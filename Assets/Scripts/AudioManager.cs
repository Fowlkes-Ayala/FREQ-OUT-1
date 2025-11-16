using System;
using UnityEngine;
using Audio.Generated;
using DefaultNamespace;
using UnityEngine.Splines;
using Unity.Mathematics;
using System.Collections;

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

    private int m_CurrentCoinPattern = 0;
    private bool m_IsFirstMeasure = true;
    float laneWidth;

    [SerializeField] GameObject coinPrefab;

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
        laneWidth = SoundwayManager.Instance.RoadWidth / 4f;
        AkUnitySoundEngine.PostEvent(AudioEventIDs.Startup, m_GameObjectID); // Passing in IDs here because they have less latency
        StartCoroutine(InitialCoins());
    }

    IEnumerator InitialCoins()
    {
        yield return null;
        UnityEngine.Splines.Spline spline = SoundwayManager.Instance.GetCurrentSpline();
        for (int j = 0; j < 2; j++)
        {
            m_CurrentCoinPattern = j;
            for (int i = 0; i < 4; i++)
            {
                float lane = CurrentSongData.coinPatterns[m_CurrentCoinPattern].coinLanes[i];
                if (lane == -1) { continue; }

                spline.Evaluate(i / 32f + j / 8f, out var position, out var tangent, out var upVector);
                position += upVector;
                Vector3 pos = new Vector3(position.x, position.y, position.z);
                var splineRight = Vector3.Cross(upVector, tangent).normalized;

                pos += splineRight * (laneWidth * (lane + 0.5f) - SoundwayManager.Instance.RoadWidth / 2f);

                Instantiate(coinPrefab, pos, Quaternion.LookRotation(tangent, upVector));
            }
        }
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
        AkUnitySoundEngine.PostEvent(AudioEventIDs.MissCoin, m_GameObjectID);
    }

    public void GetCoin()
    {
        AkUnitySoundEngine.PostEvent(AudioEventIDs.ResetCoins, m_GameObjectID);
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

                if (!m_IsFirstMeasure)
                {
                    CurrentMeasure++;
                }
                else
                {
                    m_IsFirstMeasure = false;
                }

                // Spawn a coin preset one measure in front of you every measure
                if (m_CurrentCoinPattern == CurrentMeasure)
                {
                    m_CurrentCoinPattern++;
                    if (m_CurrentCoinPattern % 8 == 0)
                    {
                        // Spawn at the beginning of the next spline on all soundways, 2 coin patterns
                        SpawnCoinPattern(SoundwayManager.Instance.leftSoundway, 0f);
                        SpawnCoinPattern(SoundwayManager.Instance.rightSoundway, 0f);
                        SpawnCoinPattern(SoundwayManager.Instance.leftSoundway, 1f / 8f);
                        SpawnCoinPattern(SoundwayManager.Instance.rightSoundway, 1f / 8f);
                        m_CurrentCoinPattern++;
                    }
                    else
                    {
                        SpawnCoinPattern();
                    }
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

    void SpawnCoinPattern()
    {
        if (m_CurrentCoinPattern >= CurrentSongData.coinPatterns.Length) { return; }
        UnityEngine.Splines.Spline spline = SoundwayManager.Instance.GetCurrentSpline();
        int overridePlayerPos = 1;
        // TODO: Make this better in like a year if this gets greenlit
        if (m_CurrentCoinPattern % 8 == 1) { overridePlayerPos = 0; }

        for (int i = 0; i < 4; i++)
        {
            float lane = CurrentSongData.coinPatterns[m_CurrentCoinPattern].coinLanes[i];
            if (lane == -1) { continue; }

            spline.Evaluate(SoundwayManager.Instance.GetPlayerT() * overridePlayerPos + i / 32f + 1 / 8f, out var position, out var tangent, out var upVector);
            position += upVector;
            Vector3 pos = new Vector3(position.x, position.y, position.z);
            var splineRight = Vector3.Cross(upVector, tangent).normalized;

            pos += splineRight * (laneWidth * (lane + 0.5f) - SoundwayManager.Instance.RoadWidth / 2f);

            // TODO: If this game gets greenlit, use object pooling instead of instantiation to make this more efficient
            GameObject coin = Instantiate(coinPrefab, pos, Quaternion.LookRotation(tangent, upVector));
            coin.GetComponent<MeshRenderer>().material = SoundwayManager.Instance.GetCurrentSoundway().coinMaterial;
        }
    }

    void SpawnCoinPattern(Soundway soundway, float offset)
    {
        if (m_CurrentCoinPattern >= CurrentSongData.coinPatterns.Length) { return; }

        UnityEngine.Splines.Spline spline;
        if (soundway.IsLeftSoundway)
        {
            spline = soundway.GetSpline(m_CurrentCoinPattern / 8);
        }
        else
        {
            spline = soundway.GetSpline(m_CurrentCoinPattern / 8 - 1);
        }

        for (int i = 0; i < 4; i++)
        {
            float lane = CurrentSongData.coinPatterns[m_CurrentCoinPattern].coinLanes[i];
            if (lane == -1) { continue; }

            spline.Evaluate(i / 32f + offset, out var position, out var tangent, out var upVector);
            position += upVector;
            Vector3 pos = new Vector3(position.x, position.y, position.z);
            var splineRight = Vector3.Cross(upVector, tangent).normalized;

            pos += splineRight * (laneWidth * (lane + 0.5f) - SoundwayManager.Instance.RoadWidth / 2f);

            // TODO: If this game gets greenlit, use object pooling instead of instantiation to make this more efficient
            GameObject coin = Instantiate(coinPrefab, pos, Quaternion.LookRotation(tangent, upVector));
            coin.GetComponent<MeshRenderer>().material = soundway.coinMaterial;
        }
    }
}
