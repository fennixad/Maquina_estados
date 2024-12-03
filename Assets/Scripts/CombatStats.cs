using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;
using static UnityEngine.EventSystems.EventTrigger;

public class CombatStats : MonoBehaviour, IDamageable
{
    private float health;
    public TextMeshProUGUI textMeshProUGUI;
    public string entity;
    private bool isAlive;
    void Start()
    {
        health = 100.0f;
        isAlive = true;
    }

    public void ApplyDamage(float damage)
    {
        health -= damage;
        health = Mathf.Max(0, health);
    }

    public float GetHealth()
    {
         return health;
    }

    public void ShowHealth(string entity)
    {
        if (textMeshProUGUI != null)
        {
            textMeshProUGUI.text = $"{entity} Health: {health:F1}";
        }
    }

    public bool IsEntityAlive(string entity)
    {
        if (health <= 0 && isAlive)
        {
            Debug.Log($"{entity}: ha muerto");
            isAlive = false;
        }
        return isAlive;
    }
}

