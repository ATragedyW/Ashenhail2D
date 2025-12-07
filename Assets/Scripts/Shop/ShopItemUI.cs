using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class ShopItemUI : MonoBehaviour
{
    [Header("UI References")]
    public Image itemIcon;
    public TextMeshProUGUI itemNameText;
    public TextMeshProUGUI priceText;
    public TextMeshProUGUI stockText;
    public Button itemButton;
    public GameObject outOfStockOverlay;
    public GameObject newItemBadge;
    
    [Header("Colors")]
    public Color normalColor = Color.white;
    public Color affordableColor = Color.green;
    public Color expensiveColor = Color.red;
    public Color outOfStockColor = Color.gray;
    
    private ShopItemData itemData;
    private ShopManager shopManager;
    
    public void Initialize(ShopItemData data, ShopManager manager)
    {
        itemData = data;
        shopManager = manager;
        
        
        itemIcon.sprite = data.item.icon;
        itemNameText.text = data.item.itemName;
        priceText.text = $"{data.price}G";
        
        
        if (data.stock == -1)
        {
            stockText.text = "∞";
            stockText.color = Color.green;
        }
        else
        {
            stockText.text = data.stock.ToString();
            stockText.color = data.stock > 0 ? Color.white : Color.red;
        }
        
       
        itemButton.onClick.RemoveAllListeners();
        itemButton.onClick.AddListener(OnClick);
        
       
        UpdateVisualState();
    }
    
    void OnClick()
    {
        shopManager.SelectItem(itemData.item, itemData);
        
        
        StartCoroutine(PulseAnimation());
    }
    
    void UpdateVisualState()
    {
        bool canAfford = PlayerCurrency.Instance.HasEnoughGold(itemData.price);
        bool inStock = itemData.stock != 0;
        
       
        itemButton.interactable = canAfford && inStock;
        
        
        if (!inStock)
        {
            itemIcon.color = outOfStockColor;
            if (outOfStockOverlay != null)
                outOfStockOverlay.SetActive(true);
        }
        else
        {
            itemIcon.color = canAfford ? normalColor : outOfStockColor;
            if (outOfStockOverlay != null)
                outOfStockOverlay.SetActive(false);
        }
        

        priceText.color = canAfford ? affordableColor : expensiveColor;
        
       
        if (newItemBadge != null)
            newItemBadge.SetActive(Random.value < 0.2f);
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
        
        rect.localScale = originalScale;
    }
}