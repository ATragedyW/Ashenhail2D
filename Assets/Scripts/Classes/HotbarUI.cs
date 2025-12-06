using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems; // Needed for button selection

public class HotbarUI : MonoBehaviour
{
    public Button abilityButton1;
    public Button abilityButton2;
    public Button abilityButton3;

    public TMP_Text abilityText1;
    public TMP_Text abilityText2;
    public TMP_Text abilityText3;

    private PlayerMovement player;

    void Start()
    {
        player = FindAnyObjectByType<PlayerMovement>();
        if (player == null || player.currentClass == null)
        {
            Debug.LogError("Player or class not found for HotbarUI.");
            return;
        }

        UpdateHotbar();
    }

    public void UpdateHotbar()
    {
        if (player == null || player.currentClass == null) return;

        // Update button text
        abilityText1.text = player.currentClass.primaryAbilityName;
        abilityText2.text = player.currentClass.secondaryAbilityName;
        abilityText3.text = player.currentClass.ultimateAbilityName;

        // Set button actions
        abilityButton1.onClick.RemoveAllListeners();
        abilityButton1.onClick.AddListener(() => player.currentClass.PrimaryAbility(player.gameObject));

        abilityButton2.onClick.RemoveAllListeners();
        abilityButton2.onClick.AddListener(() => player.currentClass.SecondaryAbility(player.gameObject));

        abilityButton3.onClick.RemoveAllListeners();
        abilityButton3.onClick.AddListener(() => player.currentClass.UltimateAbility(player.gameObject));
    }

    // Call this from PlayerMovement when keys are pressed
    public void HighlightButton(int buttonIndex)
    {
        Button buttonToHighlight = null;

        switch (buttonIndex)
        {
            case 1:
                buttonToHighlight = abilityButton1;
                break;
            case 2:
                buttonToHighlight = abilityButton2;
                break;
            case 3:
                buttonToHighlight = abilityButton3;
                break;
        }

        if (buttonToHighlight != null)
        {
            // Select the button to trigger highlight
            EventSystem.current.SetSelectedGameObject(buttonToHighlight.gameObject);
        }
    }
}