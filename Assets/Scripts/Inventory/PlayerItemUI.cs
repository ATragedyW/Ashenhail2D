using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class PlayerItemUI : MonoBehaviour
{
    [Header("UI References")]
    public Image itemIcon;
    public TextMeshProUGUI itemNameText;
    public TextMeshProUGUI quantityText;
    public TextMeshProUGUI sellPriceText;
    public Button itemButton;
    public Image rarityBackground;
    public GameObject favoriteIndicator;
    
    [Header("Colors")]
    public Color commonColor = new Color(0.5f, 0.5f, 0.5f, 0.3f);
    public Color uncommonColor = new Color(0f, 1f, 0f, 0.3f);
    public Color rareColor = new Color(0f, 0f, 1f, 0.3f);
    public Color epicColor = new Color(0.5f, 0f, 0.5f, 0.3f);
    public Color legendaryColor = new Color(1f, 0.5f, 0f, 0.3f);
    
    private Item item;
    private int quantity;
    private int sellPrice;
    private ShopManager shopManager;
    private bool isFavorite = false;
    
    public void Initialize(InventoryManager.InventorySlot slot, ShopManager manager)
    {
        item = slot.item;
        quantity = slot.quantity;
        shopManager = manager;
        
        // Update UI
        itemIcon.sprite = item.icon;
        itemNameText.text = item.itemName;
        quantityText.text = $"x{quantity}";
        
        // Calculate sell price
        sellPrice = CalculateSellPrice();
        sellPriceText.text = $"{sellPrice}G";
        
        // Set button callback
        itemButton.onClick.RemoveAllListeners();
        itemButton.onClick.AddListener(OnClick);
        
        // Set rarity background
        if (rarityBackground != null)
        {
            rarityBackground.color = GetRarityColor(item);
        }
        
        // Random favorite indicator
        isFavorite = Random.value < 0.1f;
        if (favoriteIndicator != null)
            favoriteIndicator.SetActive(isFavorite);
    }
    
    void OnClick()
    {
        shopManager.SelectItem(item);
        StartCoroutine(PulseAnimation());
    }
    
    int CalculateSellPrice()
    {
        // FIXED: Remove pattern matching that was causing error
        // Default to 50% of base value
        return Mathf.RoundToInt(item.baseValue * 0.5f);
    }
    
    Color GetRarityColor(Item item)
    {
        // Determine rarity based on item value
        int value = item.baseValue;
        
        if (value >= 100) return legendaryColor;
        if (value >= 50) return epicColor;
        if (value >= 25) return rareColor;
        if (value >= 10) return uncommonColor;
        return commonColor;
    }
    
    IEnumerator PulseAnimation()
    {
        RectTransform rect = GetComponent<RectTransform>();
        Vector3 originalScale = rect.localScale;
        
        float duration = 0.2f;
        float elapsed = 0f;
        
        while (elapsed < duration)
        {
            float t = elapsed / duration;
            float scale = Mathf.Lerp(1f, 1.1f, t);
            rect.localScale = originalScale * scale;
            elapsed += Time.unscaledDeltaTime;
            yield return null;
        }
        
        while (elapsed < duration * 2)
        {
            float t = (elapsed - duration) / duration;
            float scale = Mathf.Lerp(1.1f, 1f, t);
            rect.localScale = originalScale * scale;
            elapsed += Time.unscaledDeltaTime;
            yield return null;
        }
        
        rect.localScale = originalScale;
    }
}