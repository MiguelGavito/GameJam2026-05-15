using UnityEngine;
using System.Collections.Generic;

public class BulletManager : MonoBehaviour
{
    public static BulletManager Instance;

    [Header("Key Settings")]
    public bool enableExplodeKey = true;
    public KeyCode explodeKey = KeyCode.Space;

    private List<Bullet> bullets = new List<Bullet>();

    void Awake()
    {
        Instance = this;
    }

    void Update()
    {
        if (!enableExplodeKey)
            return;

        if (Input.GetKeyDown(explodeKey))
        {
            ExplodeAllBullets();
        }
    }

    public void RegisterBullet(Bullet b)
    {
        if (!bullets.Contains(b))
            bullets.Add(b);
    }

    public void UnregisterBullet(Bullet b)
    {
        bullets.Remove(b);
    }

    public void ExplodeAllBullets()
    {
        Bullet[] copy = bullets.ToArray();

        foreach (Bullet b in copy)
        {
            if (b != null)
                b.ForceExplode();
        }
    }
}