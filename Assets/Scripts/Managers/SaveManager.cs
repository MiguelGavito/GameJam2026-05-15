using UnityEngine;

public class SaveManager : MonoBehaviour
{

    public static SaveManager Instance;

    public int highScore;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            LoadData();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void SaveData()
    {
        PlayerPrefs.SetInt("HIGH_SCORE", highScore);
        PlayerPrefs.Save();
    }

    public void LoadData()
    {
        highScore = PlayerPrefs.GetInt("HIGH_SCORE", 0);
    }
}
