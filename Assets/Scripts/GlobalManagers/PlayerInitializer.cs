using UnityEngine;

public class PlayerInitializer : MonoBehaviour
{
    void Start()
    {
        PlayerMovement player = FindAnyObjectByType<PlayerMovement>();
        if (player != null && GameManager.Instance.chosenClass != null)
        {
            player.currentClass = GameManager.Instance.chosenClass;
        }
        HotbarUI hotbar = FindAnyObjectByType<HotbarUI>();
        if (hotbar != null)
        {
            hotbar.UpdateHotbar();
        }
    }
}
