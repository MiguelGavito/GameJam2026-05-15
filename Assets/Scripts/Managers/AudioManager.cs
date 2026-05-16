using UnityEngine;


public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    private AudioSource audioSource;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.loop = true;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void PlayMusic(AudioClip clip)
    {
        audioSource.clip = clip;
        audioSource.Play();
    }
}