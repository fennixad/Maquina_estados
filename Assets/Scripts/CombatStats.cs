using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;

public class CombatStats : MonoBehaviour, IDamageable
{
    private float health;
    public TextMeshProUGUI textMeshProUGUI;
    public string entityName;

    void Start()
    {
        health = 100.0f;

        if (textMeshProUGUI == null)
        {
            Debug.Log("TextmeshPro no asignado en " + entityName);
        }
    }

    public void ApplyDamage(float damage)
    {
        health -= damage;
        health = Mathf.Max(0, health);
        Debug.Log($"Dañado! Vida actual de {entityName}: {health}");
        ShowHealth();
    }

    public float GetHealth()
    {
        return health;
    }

    public void ShowHealth()
    {
        if (textMeshProUGUI != null)
        {
            textMeshProUGUI.text = $"{entityName} Health: {health:F1}";
        }
    }
}

