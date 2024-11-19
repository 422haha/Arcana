using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class SoundManager : MonoBehaviour
{
    [Header("Audio Sources")]
    public AudioSource bgmSource; // 배경음 재생용
    public AudioSource sfxSource; // 효과음 재생용

    [Header("BGM Clips")]
    public AudioClip[] bgmSounds;

    [Header("SFX Clips")]
    public AudioClip[] sfxSounds;

    [Header("Audio Settings")]
    [SerializeField]
    private AudioMixer audioMixer; // AudioMixer로 볼륨 관리

    [SerializeField, Range(0f, 1f)]
    private float defaultBGMVolume = 0.5f;

    [SerializeField, Range(0f, 1f)]
    private float defaultSFXVolume = 1f;

    // 오디오 캐시 (경로로 불러온 오디오 클립을 저장해 중복 로드를 방지)
    private Dictionary<string, AudioClip> audioCache = new Dictionary<string, AudioClip>();

    [SerializeField]
    private bool debugMode = false; // 디버그 모드 활성화/비활성화 변수

    private void Awake()
    {
        // AudioSource가 없는 경우 자동으로 추가
        if (bgmSource == null)
        {
            bgmSource = gameObject.AddComponent<AudioSource>();
            if (debugMode)
                Debug.LogWarning("bgmSource is null. Creating new AudioSource for BGM.");
        }
        if (sfxSource == null)
        {
            sfxSource = gameObject.AddComponent<AudioSource>();
            if (debugMode)
                Debug.LogWarning("sfxSource is null. Creating new AudioSource for SFX.");
        }

        bgmSource.loop = true; // 배경음은 루프 재생
        sfxSource.loop = false; // 효과음은 한 번만 재생
    }

    #region BGM Controls
    // 배경음악(BGM)을 재생하는 메서드 (페이드 인 포함)
    public void PlayBGM(AudioClip clip, float volume = -1f, float fadeInDuration = 0f)
    {
        if (clip == null) // clip이 null일 경우 재생하지 않음
        {
            if (debugMode)
                Debug.LogWarning("BGM Clip is null. Playback skipped.");
            return;
        }

        float targetVolume = volume < 0 ? defaultBGMVolume : Mathf.Clamp01(volume);
        if (debugMode)
            Debug.Log($"Playing BGM: {clip.name} at volume: {targetVolume}");

        if (fadeInDuration > 0)
        {
            StartCoroutine(FadeInBGM(clip, targetVolume, fadeInDuration));
        }
        else
        {
            bgmSource.clip = clip;
            bgmSource.volume = targetVolume;
            bgmSource.Play();
        }
    }

    // BGM 페이드 아웃 후 정지
    public void StopBGM(float fadeDuration = 1f)
    {
        if (debugMode)
            Debug.Log("Stopping BGM with fade-out");
        StartCoroutine(FadeOutBGM(fadeDuration));
    }

    // BGM 일시정지
    public void PauseBGM()
    {
        bgmSource.Pause();
        if (debugMode)
            Debug.Log("BGM Paused");
    }

    // BGM 재개
    public void ResumeBGM()
    {
        bgmSource.UnPause();
        if (debugMode)
            Debug.Log("BGM Resumed");
    }

    // 새로운 BGM으로 전환 (페이드 아웃 및 페이드 인)
    public void SwitchBGM(AudioClip newClip, float fadeDuration = 1f)
    {
        if (debugMode)
            Debug.Log($"Switching BGM to {newClip.name} with fade duration: {fadeDuration}");
        StartCoroutine(FadeOutAndSwitchBGM(newClip, fadeDuration));
    }

    // BGM 볼륨 조절
    public void SetBGMVolume(float volume)
    {
        volume = Mathf.Clamp01(volume);
        bgmSource.volume = volume;
        if (audioMixer != null)
        {
            audioMixer.SetFloat("BGMVolume", Mathf.Log10(volume) * 20);
        }
        if (debugMode)
            Debug.Log($"BGM Volume set to: {volume}"); // 볼륨을 데시벨로 설정
    }

    private IEnumerator FadeInBGM(AudioClip clip, float targetVolume, float duration)
    {
        bgmSource.clip = clip;
        bgmSource.volume = 0f;
        bgmSource.Play();

        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            bgmSource.volume = Mathf.Lerp(0f, targetVolume, elapsed / duration);
            if (debugMode)
                Debug.Log($"Fading In BGM Volume: {bgmSource.volume}");
            yield return null;
        }

        bgmSource.volume = targetVolume;
    }

    private IEnumerator FadeOutBGM(float duration)
    {
        float startVolume = bgmSource.volume;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            bgmSource.volume = Mathf.Lerp(startVolume, 0f, elapsed / duration);
            if (debugMode)
                Debug.Log($"Fading Out BGM Volume: {bgmSource.volume}");
            yield return null;
        }

        bgmSource.Stop();
        bgmSource.volume = startVolume;
    }

    private IEnumerator FadeOutAndSwitchBGM(AudioClip newClip, float duration)
    {
        float startVolume = bgmSource.volume;
        float elapsed = 0f;

        // 페이드 아웃
        while (elapsed < duration * 0.5f)
        {
            elapsed += Time.deltaTime;
            bgmSource.volume = Mathf.Lerp(startVolume, 0f, elapsed / (duration * 0.5f));
            if (debugMode)
                Debug.Log($"Switching BGM, Fading Out Volume: {bgmSource.volume}");
            yield return null;
        }

        // 클립 전환 및 페이드 인
        bgmSource.Stop();
        bgmSource.clip = newClip;
        bgmSource.Play();

        elapsed = 0f;
        while (elapsed < duration * 0.5f)
        {
            elapsed += Time.deltaTime;
            bgmSource.volume = Mathf.Lerp(0f, startVolume, elapsed / (duration * 0.5f));
            if (debugMode)
                Debug.Log($"Switching BGM, Fading In Volume: {bgmSource.volume}");
            yield return null;
        }

        bgmSource.volume = startVolume;
    }
    #endregion

    #region SFX Controls
    // 효과음(SFX)을 재생하는 메서드
    public void PlaySFX(AudioClip clip, float volume = -1f)
    {
        if (clip == null)
        {
            if (debugMode)
                Debug.LogWarning("SFX Clip is null. Playback skipped.");
            return;
        }

        float targetVolume = volume < 0 ? defaultSFXVolume : Mathf.Clamp01(volume);
        sfxSource.PlayOneShot(clip, targetVolume);
        if (debugMode)
            Debug.Log($"Playing SFX: {clip.name} at volume: {targetVolume}");
    }

    // SFX 볼륨 조절
    public void SetSFXVolume(float volume)
    {
        volume = Mathf.Clamp01(volume);
        sfxSource.volume = volume;
        if (audioMixer != null)
        {
            audioMixer.SetFloat("SFXVolume", Mathf.Log10(volume) * 20);
        }
        if (debugMode)
            Debug.Log($"SFX Volume set to: {volume}");
    }

    // 현재 재생 중인 SFX를 정지하는 메서드
    public void StopSFX()
    {
        sfxSource.Stop();
        if (debugMode)
            Debug.Log("SFX Stopped");
    }
    #endregion

    #region Utility Methods
    // 오디오 클립을 경로로 로드하고 캐싱
    private AudioClip LoadAudioClip(string path)
    {
        if (audioCache.ContainsKey(path))
        {
            if (debugMode)
                Debug.Log($"Audio Clip loaded from cache: {path}");
            return audioCache[path];
        }

        AudioClip clip = Resources.Load<AudioClip>(path);
        if (clip != null)
        {
            audioCache[path] = clip;
            if (debugMode)
                Debug.Log($"Audio Clip loaded and cached: {path}");
        }
        else
        {
            if (debugMode)
                Debug.LogWarning($"Failed to load audio clip at path: {path}");
        }

        return clip;
    }

    // 메모리 최적화를 위해 오디오 캐시 초기화
    public void ClearAudioCache()
    {
        if (debugMode)
            Debug.Log("Clearing audio cache");
        audioCache.Clear();
        Resources.UnloadUnusedAssets();
    }
    #endregion

    #region 실제 실행 메서드
    /// <summary>
    /// BGM을 골라서 플레이하는 메서드
    /// </summary>
    /// <param name="bgmType">sfx타입 : 
    /// 0 : 로그인
    /// 1 : 메인
    /// 2 : 박스
    /// 3 : 전투 - 기본
    /// 4 : 전투 - 용
    /// 5 : 이벤트
    /// 6 : 휴식
    /// 7 : 상점
    /// 8 : 엔딩
    /// </param>
    public void BgmSelectPlay(int bgmType)
    {
        // 현재 BGM이 실행되고 있다면
        if (bgmSource.clip != null && bgmSource.isPlaying)
        {
            // Switch 로직 실행
            SwitchBGM(bgmSounds[bgmType], 0.5f);
        }
        // 없다면 (처음이라면)
        else
        {
            // Play 로직 실행
            PlayBGM(bgmSounds[bgmType], PlayerPrefs.GetFloat("Volume", 0.5f), 0.5f);
        }
    }

    /// <summary>
    /// SFX를 골라서 플레이하는 메서드
    /// </summary>
    /// <param name="sfxType">sfx타입 : 
    /// 0 : 일반 버튼 sfx
    /// 1 : 돈 받는 버튼 sfx
    /// 2 : 돈 내는 버튼 sfx
    /// 3 : 턴 종료 버튼 sfx
    /// 4 : 카드 받는 sfx
    /// 5 : 카드 내는 sfx
    /// 6 : 공격해서 부딛히는 sfx
    /// 7 : 파괴되는 sfx
    /// 8 : 승리 sfx
    /// 9 : 패배 sfx
    /// 10 : 에러 sfx
    /// 11 : 맵 이동 sfx
    /// </param>
    public void SfxSelectPlay(int sfxType)
    {
        if (sfxSource.isPlaying)
        {
            StopSFX();
        }
        PlaySFX(sfxSounds[sfxType], PlayerPrefs.GetFloat("Volume", 0.5f));
    }
    #endregion
}
