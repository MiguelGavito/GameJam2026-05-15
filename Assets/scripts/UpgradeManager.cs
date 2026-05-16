using UnityEngine;
using System.Collections.Generic;
using TMPro; 

public enum UpgradeType
{
    ExplosionRadius,
    BulletsPerShot,
    Damage,
    Knockback,
    MoveSpeed, // Vida eliminada
    CooldownReduction
}

public class UpgradeManager : MonoBehaviour
{
    public static UpgradeManager instance;

    [Header("UI References")]
    public GameObject upgradePanel;
    public TextMeshProUGUI[] buttonTexts; 

    [Header("Player References")]
    public GunController gunController;
    public PlayerMovement playerMovement; 

    [Header("Current Bonuses")]
    public float bonusExplosionRadius = 0f;
    public float bonusDamage = 0f;
    public float bonusKnockback = 0f;
    public float bonusMoveSpeed = 0f;
    public float bonusEnemySpeed = 0f;
    public float bonusCooldownReduction = 0f;

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
            Time.timeScale = 0f; 
        }
    }

    void GenerateRandomOptions()
    {
        // El pool ahora tiene 6 opciones exactas
        List<UpgradeType> pool = new List<UpgradeType>
        {
            UpgradeType.ExplosionRadius,
            UpgradeType.BulletsPerShot,
            UpgradeType.Damage,
            UpgradeType.Knockback,
            UpgradeType.MoveSpeed,
            UpgradeType.CooldownReduction
        };

        currentOptions.Clear();

        for (int i = 0; i < 3; i++)
        {
            int randomIndex = Random.Range(0, pool.Count);
            currentOptions.Add(pool[randomIndex]);
            pool.RemoveAt(randomIndex); 
        }

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
            case UpgradeType.Damage: return "+ Daño General";
            case UpgradeType.Knockback: return "+ Fuerza de Empuje";
            case UpgradeType.MoveSpeed: return "+ Velocidad (Tú y Enemigos)"; // Advertencia visual para el jugador
            case UpgradeType.CooldownReduction: return "- Tiempo de Recarga";
            default: return "Mejora";
        }
    }

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
                bonusKnockback += 6f; 
                break;
            case UpgradeType.MoveSpeed:
                // Aumenta la del jugador
                bonusMoveSpeed += 1f;
                if (playerMovement != null) playerMovement.moveSpeed += 1f; 
                
                // Aumenta el multiplicador global para la horda
                bonusEnemySpeed += 1f; 
                break;
            case UpgradeType.CooldownReduction:
                bonusCooldownReduction += 0.08f; 
                break;
        }
    }

    private void ResumeGame()
    {
        if (upgradePanel != null) upgradePanel.SetActive(false);
        Time.timeScale = 1f;
    }
}