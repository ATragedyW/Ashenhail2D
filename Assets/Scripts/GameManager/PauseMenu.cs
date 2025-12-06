using UnityEngine;
using TMPro;
using PlayerStats;

public class StatsMenu : MonoBehaviour
{
    public GameObject statsUI;

    public TMP_Text nameText;
    public TMP_Text levelText;
    public TMP_Text xpText;
    public TMP_Text healthText;
    public TMP_Text manaText;

    PlayerStats.PlayerStats stats;

    private void Start()
    {
        statsUI.SetActive(false);
        stats = FindFirstObjectByType<PlayerStats.PlayerStats>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.C))  // or Tab
        {
            ToggleStats();
        }
    }

    public void ToggleStats()
    {
        bool show = !statsUI.activeSelf;
        statsUI.SetActive(show);

        if (show)
            UpdateStatsUI();
    }

    void UpdateStatsUI()
    {
        nameText.text = $"Name: {stats.PlayerName}";
        levelText.text = $"Level: {stats.PlayerLevel}";
        xpText.text = $"XP: {stats.PlayerXP}";
        healthText.text = $"Health: {stats.currentHealth}/{stats.maxHealth}";
        manaText.text = $"Mana: {stats.currentMana}/{stats.maxMana}";
    }
}
