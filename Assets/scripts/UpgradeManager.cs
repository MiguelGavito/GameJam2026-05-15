using UnityEngine;
using System.Collections.Generic;
using TMPro; // Asegúrate de tener esto para los textos de los botones

public enum UpgradeType
{
    ExplosionRadius,
    BulletsPerShot,
    Damage,
    Knockback,
    MaxHealth,
    MoveSpeed,
    CooldownReduction
}

public class UpgradeManager : MonoBehaviour
{
    public static UpgradeManager instance;

    [Header("UI References")]
    public GameObject upgradePanel;
    public TextMeshProUGUI[] buttonTexts; // Arrastra aquí los componentes de texto de tus 3 botones

    [Header("Player References")]
    public GunController gunController;
    public PlayerMovement playerMovement; // Arrastra al jugador para subirle la velocidad directamente

    [Header("Current Bonuses")]
    public float bonusExplosionRadius = 0f;
    public float bonusDamage = 0f;
    public float bonusKnockback = 0f;
    public float bonusMaxHealth = 0f;
    public float bonusMoveSpeed = 0f;
    public float bonusCooldownReduction = 0f;

    // Lista que guardará las 3 opciones elegidas para el cofre actual
    private List<UpgradeType> currentOptions = new List<UpgradeType>();

    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        if (upgradePanel != null) upgradePanel.SetActive(false);
    }

    public void OpenChest()
    {
        if (upgradePanel != null)
        {
            GenerateRandomOptions();
            upgradePanel.SetActive(true);
            Time.timeScale = 0f; // Pausa el juego
        }
    }

    void GenerateRandomOptions()
    {
        // 1. Creamos la lista con todas las mejoras disponibles en el juego
        List<UpgradeType> pool = new List<UpgradeType>
        {
            UpgradeType.ExplosionRadius,
            UpgradeType.BulletsPerShot,
            UpgradeType.Damage,
            UpgradeType.Knockback,
            UpgradeType.MaxHealth,
            UpgradeType.MoveSpeed,
            UpgradeType.CooldownReduction
        };

        currentOptions.Clear();

        // 2. Seleccionamos 3 mejoras sin repetir
        for (int i = 0; i < 3; i++)
        {
            int randomIndex = Random.Range(0, pool.Count);
            currentOptions.Add(pool[randomIndex]);
            pool.RemoveAt(randomIndex); // La eliminamos del pool para que no pueda salir repetida
        }

        // 3. Asignamos los textos a los botones correspondientes
        for (int i = 0; i < 3; i++)
        {
            buttonTexts[i].text = GetUpgradeName(currentOptions[i]);
        }
    }

    string GetUpgradeName(UpgradeType type)
    {
        switch (type)
        {
            case UpgradeType.ExplosionRadius: return "+ Radio de Explosión";
            case UpgradeType.BulletsPerShot: return "+ 1 Bala por Disparo";
            case UpgradeType.Damage: return "+ Aumento de Daño";
            case UpgradeType.Knockback: return "+ Fuerza de Empuje";
            case UpgradeType.MaxHealth: return "+ Vida Máxima";
            case UpgradeType.MoveSpeed: return "+ Velocidad de Movimiento";
            case UpgradeType.CooldownReduction: return "- Cooldown de Ataque";
            default: return "Mejora";
        }
    }

    // Esta función la llamarán los botones pasando un ID (0, 1 o 2)
    public void SelectUpgrade(int buttonIndex)
    {
        UpgradeType chosenUpgrade = currentOptions[buttonIndex];
        ApplyUpgradeEffects(chosenUpgrade);
        ResumeGame();
    }

    void ApplyUpgradeEffects(UpgradeType type)
    {
        switch (type)
        {
            case UpgradeType.ExplosionRadius:
                bonusExplosionRadius += 1.2f;
                break;
            case UpgradeType.BulletsPerShot:
                if (gunController != null) gunController.bulletsPerShot += 1;
                break;
            case UpgradeType.Damage:
                bonusDamage += 20f;
                break;
            case UpgradeType.Knockback:
                bonusKnockback += 6f; // Sube la fuerza de empuje general
                break;
            case UpgradeType.MaxHealth:
                bonusMaxHealth += 20f;
                // De esta variable se colgará tu compañero en su script:
                // playerHealth.IncreaseMaxHealth(bonusMaxHealth);
                break;
            case UpgradeType.MoveSpeed:
                bonusMoveSpeed += 1f;
                if (playerMovement != null) playerMovement.moveSpeed += 1f; // Aplica directo al script que hicimos al inicio
                break;
            case UpgradeType.CooldownReduction:
                bonusCooldownReduction += 0.08f; // Quita 0.08 segundos de espera
                break;
        }
    }

    private void ResumeGame()
    {
        if (upgradePanel != null) upgradePanel.SetActive(false);
        Time.timeScale = 1f;
    }
}