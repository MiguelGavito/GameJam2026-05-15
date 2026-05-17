using UnityEngine;

public class Chest : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D collision)
    {
        // Si el jugador toca el cofre
        if (collision.CompareTag("Player"))
        {
            // Llamamos al gestor para abrir la interfaz
            if (UpgradeManager.instance != null)
            {
                UpgradeManager.instance.OpenChest();
            }
            
            // Destruimos el cofre para que no se pueda abrir dos veces
            Destroy(gameObject);
        }
    }
}