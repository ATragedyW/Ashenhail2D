using UnityEngine;

[CreateAssetMenu(fileName = "NewPlayerClass", menuName = "PlayerClass")]
public class PlayerClass : ScriptableObject
{
    public string className;

    //Stats
    public int maxHealth = 100;
    public int maxMana = 50;
    public int attackPower = 10;

    public Color classColor = Color.white;
    


    // Add these
    public string primaryAbilityName = "Primary";
    public string secondaryAbilityName = "Secondary";
    public string ultimateAbilityName = "Ultimate";

    // Example ability methods
    public virtual void PrimaryAbility(GameObject player)
    {
        Debug.Log($"{className} used Primary Ability!");
    }

    public virtual void SecondaryAbility(GameObject player)
    {
        Debug.Log($"{className} used Secondary Ability!");
    }

    public virtual void UltimateAbility(GameObject player)
    {
        Debug.Log($"{className} used Ultimate Ability!");
    }
}
