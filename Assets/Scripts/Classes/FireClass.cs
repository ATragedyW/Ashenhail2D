using UnityEngine;

[CreateAssetMenu(menuName = "PlayerClass/Fire")]
public class FireClass : PlayerClass
{
    public override void PrimaryAbility(GameObject player)
    {
        Debug.Log("Fireball: Shoots a ball of fire!");
    }

    public override void SecondaryAbility(GameObject player)
    {
        Debug.Log("Flame Wave: Sends a burning wave forward!");
    }

    public override void UltimateAbility(GameObject player)
    {
        Debug.Log("Inferno: Engulfs the battlefield in flames!");
    }
}
