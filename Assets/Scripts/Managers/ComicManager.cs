using UnityEngine;
using UnityEngine.UI; // Necesario para controlar la UI
using UnityEngine.SceneManagement; // Necesario para cambiar de escena al terminar

public class ComicManager : MonoBehaviour
{
    [Header("Configuración")]
    public Image comicDisplay; // La pantalla donde se mostrará el cómic
    public Sprite[] comicPanels; // Tu lista de viñetas
    public string mainMenuSceneName = "MainMenu"; // A dónde ir al terminar

    private int currentIndex = 0;

    void Start()
    {
        // Mostrar la primera imagen al iniciar la escena
        if (comicPanels.Length > 0)
        {
            comicDisplay.sprite = comicPanels[0];
        }
    }

    void Update()
    {
        // Detectar el clic izquierdo del mouse
        if (Input.GetMouseButtonDown(0))
        {
            NextPanel();
        }
    }

    void NextPanel()
    {
        currentIndex++;

        // Si aún nos quedan imágenes por mostrar, actualizamos la pantalla
        if (currentIndex < comicPanels.Length)
        {
            comicDisplay.sprite = comicPanels[currentIndex];
        }
        else
        {
            // Si ya no hay más imágenes, salimos al menú principal
            Debug.Log("Cómic terminado. Volviendo al menú...");
            SceneManager.LoadScene(mainMenuSceneName);
        }
    }
}