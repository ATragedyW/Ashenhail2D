using UnityEngine;
using TMPro;

public class PlayerCurrency : MonoBehaviour
{
    public static PlayerCurrency Instance { get; private set; }
    
    [Header("Currency")]
    public int gold = 100;
    public int gems = 0;
    
    [Header("UI")]
    public TextMeshProUGUI goldText;
    public TextMeshProUGUI gemsText;
    
    public System.Action OnCurrencyChanged;
    
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    void Start()
    {
        UpdateUI();
    }
    
    public bool HasEnoughGold(int amount)
    {
        return gold >= amount;
    }
    
    public bool SpendGold(int amount)
    {
        if (gold >= amount)
        {
            gold -= amount;
            UpdateUI();
            OnCurrencyChanged?.Invoke();
            Debug.Log($"Spent {amount} gold. Remaining: {gold}");
            return true;
        }
        Debug.Log($"Not enough gold! Need {amount}, have {gold}");
        return false;
    }
    
    public void AddGold(int amount)
    {
        gold += amount;
        UpdateUI();
        OnCurrencyChanged?.Invoke();
        Debug.Log($"Added {amount} gold. Total: {gold}");
    }
    
    public void AddGems(int amount)
    {
        gems += amount;
        UpdateUI();
        OnCurrencyChanged?.Invoke();
        Debug.Log($"Added {amount} gems. Total: {gems}");
    }
    
    void UpdateUI()
    {
        if (goldText != null)
            goldText.text = $"{gold}G";
        
        if (gemsText != null)
            gemsText.text = $"{gems}💎";
    }
    
   
    public void AddTestGold(int amount = 100)
    {
        AddGold(amount);
    }
}