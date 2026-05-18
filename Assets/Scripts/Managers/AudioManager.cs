using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    private Coroutine musicRoutine;

    [Header("Audio Sources")]
    public AudioSource introSource;
    public AudioSource loopSource;

    [Header("Main Menu")]
    public AudioClip menuLoop;

    [Header("Gameplay")]
    public AudioClip gameplayIntro;
    public AudioClip gameplayLoop;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;

            DontDestroyOnLoad(gameObject);

            SceneManager.sceneLoaded += OnSceneLoaded;
            
            float savedVolume = PlayerPrefs.GetFloat("VOLUME", 1f);

            StartCoroutine(InitVolume(savedVolume));
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        switch (scene.name)
        {
            case "MainMenu":
                PlayLoop(menuLoop);
                break;

            case "Scene 1":
                PlayIntroAndLoop(gameplayIntro, gameplayLoop);
                break;
                
            case "EndingScene":
                PlayLoop(menuLoop);
                break;
        }
    }

    public void PlayLoop(AudioClip loopClip)
    {
        StopAllCoroutines();

        introSource.Stop();
        loopSource.Stop();

        loopSource.clip = loopClip;
        loopSource.loop = true;
        loopSource.Play();
    }

    public void PlayIntroAndLoop(AudioClip intro, AudioClip loop)
    {
        if (musicRoutine != null)
            StopCoroutine(musicRoutine);

        introSource.Stop();
        loopSource.Stop();

        musicRoutine = StartCoroutine(PlayMusicRoutine(intro, loop));
    }

    IEnumerator PlayMusicRoutine(AudioClip intro, AudioClip loop)
    {
        yield return null; //  asegura inicio limpio

        introSource.clip = intro;
        introSource.loop = false;
        introSource.Play();

        yield return new WaitUntil(() => introSource.isPlaying == false);

        loopSource.clip = loop;
        loopSource.loop = true;
        loopSource.Play();
    }
    public void SetVolume(float value)
    {
        if (introSource != null)
            introSource.volume = value;

        if (loopSource != null)
            loopSource.volume = value;
    }

    IEnumerator InitVolume(float value)
    {
        yield return null;
        SetVolume(value);
    }
}