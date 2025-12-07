using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(fileName = "New Player Class", menuName = "Game/Player Class")]
public class PlayerClass : ScriptableObject
{
    [Header("Class Info")]
    public string className;
    public Color classColor = Color.white;
    
    [Header("Base Stats")]
    public int baseHealth = 100;
    public int baseMana = 50;
    public int baseAttack = 10;
    
    [Header("Stat Growth Per Level")]
    public int healthPerLevel = 10;
    public int manaPerLevel = 5;
    public int attackPerLevel = 2;
    
    [Header("Ability Mana Costs")]
    public int primaryManaCost = 10;
    public int secondaryManaCost = 25;
    public int ultimateManaCost = 50;
    
    [Header("Ability Cooldowns")]
    public float primaryCooldown = 2f;
    public float secondaryCooldown = 3f;
    public float ultimateCooldown = 10f;
    
    [Header("Ability Names")]
    public string primaryAbilityName = "Primary Attack";
    public string secondaryAbilityName = "Secondary Ability";
    public string ultimateAbilityName = "Ultimate Ability";
    
    [Header("Ability Prefabs")]
    public GameObject primaryAbilityPrefab;
    public GameObject secondaryAbilityPrefab;
    public GameObject ultimateAbilityPrefab;
    
    [Header("Health Bar Visuals")]
    public Color healthBarColor = Color.green;
    public Color manaBarColor = Color.blue;
    public Sprite classIcon;
    public Gradient healthGradient; 
    
    [Header("Class Bonuses")]
    public float healthRegenRate = 0f; 
    public float manaRegenRate = 1f;  
    public float defenseMultiplier = 1f;
    
    
    public int GetMaxHealth(int level)
    {
        return baseHealth + (healthPerLevel * (level - 1));
    }
    
    public int GetMaxMana(int level)
    {
        return baseMana + (manaPerLevel * (level - 1));
    }
    
    public int GetAttackPower(int level)
    {
        return baseAttack + (attackPerLevel * (level - 1));
    }
    
   
    public Color GetHealthColor(float healthPercent)
    {
        if (healthGradient != null && healthGradient.colorKeys.Length > 0)
        {
            return healthGradient.Evaluate(healthPercent);
        }
        
     
        if (healthPercent > 0.5f)
            return Color.Lerp(Color.yellow, healthBarColor, (healthPercent - 0.5f) * 2);
        else
            return Color.Lerp(Color.red, Color.yellow, healthPercent * 2);
    }
    
   
    public virtual void PrimaryAbility(GameObject player)
    {
        Debug.Log($"{className}: Using {primaryAbilityName}");
        
    }
    
    public virtual void SecondaryAbility(GameObject player)
    {
        Debug.Log($"{className}: Using {secondaryAbilityName}");
      
    }
    
    public virtual void UltimateAbility(GameObject player)
    {
        Debug.Log($"{className}: Using {ultimateAbilityName}");
        
    }
    
   
    
    public virtual void OnHealthChanged(int current, int max, PlayerController player)
    {
       
    }
    
    public virtual void OnManaChanged(int current, int max, PlayerController player)
    {
        
    }
    
    public virtual string GetClassDescription()
    {
        return $"{className}\n" +
               $"Base Health: {baseHealth} (+{healthPerLevel}/level)\n" +
               $"Base Mana: {baseMana} (+{manaPerLevel}/level)\n" +
               $"Attack: {baseAttack} (+{attackPerLevel}/level)";
    }
}