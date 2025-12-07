using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class ShopUI : MonoBehaviour
{
    [Header("UI References")]
    public GameObject shopPanel;
    public Transform shopItemsContainer;
    public Transform playerItemsContainer;
    public GameObject shopItemPrefab;
    public GameObject playerItemPrefab;
    public ShopManager shopManager;
    
    [Header("Currency Display")]
    public TextMeshProUGUI playerGoldText;
    public TextMeshProUGUI playerGemsText;
    
    [Header("Item Info")]
    public GameObject itemInfoPanel;
    public Image itemIcon;
    public TextMeshProUGUI itemNameText;
    public TextMeshProUGUI itemDescriptionText;
    public TextMeshProUGUI itemPriceText;
    public Button buyButton;
    public Button sellButton;
    
    [Header("Shop Info")]
    public TextMeshProUGUI shopNameText;
    public Image shopkeeperImage;
    
    [Header("Tabs")]
    public Button buyTabButton;
    public Button sellTabButton;
    public GameObject buyPanel;
    public GameObject sellPanel;
    
    private ShopInventory currentShop;
    private Item selectedItem;
    private bool isBuying = true;
    private List<ShopItemUI> shopItemUIs = new List<ShopItemUI>();
    private List<PlayerItemUI> playerItemUIs = new List<PlayerItemUI>();
    
    void Start()
    {
        shopPanel.SetActive(false);
        itemInfoPanel.SetActive(false);
        
        // Set up tab buttons
        buyTabButton.onClick.AddListener(() => SetTab(true));
        sellTabButton.onClick.AddListener(() => SetTab(false));
        
        // Update currency display
        if (PlayerCurrency.Instance != null)
        {
            PlayerCurrency.Instance.OnCurrencyChanged += UpdateCurrencyDisplay;
        }
    }
    
    void Update()
    {
        // Open/close shop with B key
        if (Input.GetKeyDown(KeyCode.B))
        {
            ToggleShop();
        }
    }
    
    public void OpenShop(ShopInventory shop)
    {
        currentShop = shop;
        shopPanel.SetActive(true);
        
        // Set shop info
        if (shopNameText != null)
            shopNameText.text = shop.shopName;
        
        if (shopkeeperImage != null && shop.shopkeeperIcon != null)
            shopkeeperImage.sprite = shop.shopkeeperIcon;
        
        // Default to buy tab
        SetTab(true);
        
        // Update currency
        UpdateCurrencyDisplay();
        
        // Pause game
        Time.timeScale = 0f;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }
    
    public void CloseShop()
    {
        shopPanel.SetActive(false);
        itemInfoPanel.SetActive(false);
        selectedItem = null;
        
        // Resume game
        Time.timeScale = 1f;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }
    
    void ToggleShop()
    {
        if (shopPanel.activeSelf)
        {
            CloseShop();
        }
        else if (currentShop != null)
        {
            OpenShop(currentShop);
        }
    }
    
    void SetTab(bool buying)
    {
        isBuying = buying;
        buyPanel.SetActive(buying);
        sellPanel.SetActive(!buying);
        
        buyTabButton.interactable = !buying;
        sellTabButton.interactable = buying;
        
        if (buying)
        {
            LoadShopItems();
        }
        else
        {
            LoadPlayerItems();
        }
        
        itemInfoPanel.SetActive(false);
    }
    
    void LoadShopItems()
    {
        
        foreach (Transform child in shopItemsContainer)
        {
            Destroy(child.gameObject);
        }
        shopItemUIs.Clear();
        
        if (currentShop == null) return;
        
        
        foreach (var shopItem in currentShop.itemsForSale)
        {
            GameObject itemObj = Instantiate(shopItemPrefab, shopItemsContainer);
            ShopItemUI itemUI = itemObj.GetComponent<ShopItemUI>();
            
            itemUI.Initialize(shopItem, shopManager); 
            shopItemUIs.Add(itemUI);
        }
    }
    
    void LoadPlayerItems()
    {
        
        foreach (Transform child in playerItemsContainer)
        {
            Destroy(child.gameObject);
        }
        playerItemUIs.Clear();
        
        if (InventoryManager.Instance == null) return;
        
        
        foreach (var slot in InventoryManager.Instance.slots)
        {
            if (!slot.IsEmpty)
            {
                GameObject itemObj = Instantiate(playerItemPrefab, playerItemsContainer);
                PlayerItemUI itemUI = itemObj.GetComponent<PlayerItemUI>();
                
                itemUI.Initialize(slot, shopManager); // This is correct - passing ShopUI, not ShopManager
                playerItemUIs.Add(itemUI);
            }
        }
    }
    
    public void SelectItem(Item item, ShopItemData shopItemData = null)
    {
        selectedItem = item;
        
        // Update item info panel
        itemIcon.sprite = item.icon;
        itemNameText.text = item.itemName;
        itemDescriptionText.text = item.GetTooltip();
        
        if (isBuying && shopItemData != null)
        {
            itemPriceText.text = $"Price: {shopItemData.price} gold";
            buyButton.gameObject.SetActive(true);
            sellButton.gameObject.SetActive(false);
            
            // Check if player can afford
            bool canAfford = PlayerCurrency.Instance.HasEnoughGold(shopItemData.price);
            buyButton.interactable = canAfford;
            
            // Set buy button callback
            buyButton.onClick.RemoveAllListeners();
            buyButton.onClick.AddListener(() => BuyItem(shopItemData));
        }
        else
        {
            // Selling
            ShopItemData sellData = currentShop?.GetItemData(item);
            
            // FIXED ERROR 3: Use 'item.baseValue' or 'item.sellValue' instead of 'item.value'
            int sellPrice = sellData?.sellPrice ?? Mathf.RoundToInt(item.baseValue * 0.5f); // Changed 'value' to 'baseValue'
            
            itemPriceText.text = $"Sell Price: {sellPrice} gold";
            sellButton.gameObject.SetActive(true);
            buyButton.gameObject.SetActive(false);
            
            // Set sell button callback
            sellButton.onClick.RemoveAllListeners();
            sellButton.onClick.AddListener(() => SellItem(item, sellPrice));
        }
        
        itemInfoPanel.SetActive(true);
    }
    
    void BuyItem(ShopItemData shopItemData)
    {
        if (PlayerCurrency.Instance.SpendGold(shopItemData.price))
        {
            // Add item to inventory
            InventoryManager.Instance.AddItem(shopItemData.item, 1);
            
            // Reduce shop stock if not infinite
            if (shopItemData.stock > 0)
            {
                shopItemData.stock--;
                if (shopItemData.stock <= 0)
                {
                    // Remove from shop if out of stock
                    RefreshShop();
                }
            }
            
            Debug.Log($"Bought {shopItemData.item.itemName} for {shopItemData.price} gold");
            UpdateCurrencyDisplay();
            itemInfoPanel.SetActive(false);
        }
    }
    
    void SellItem(Item item, int sellPrice)
    {
        // Remove from inventory
        bool hasItem = false;
        for (int i = 0; i < InventoryManager.Instance.slots.Count; i++)
        {
            var slot = InventoryManager.Instance.slots[i];
            if (!slot.IsEmpty && slot.item == item)
            {
                InventoryManager.Instance.RemoveItem(i, 1);
                hasItem = true;
                break;
            }
        }
        
        if (hasItem)
        {
            // Add gold to player
            PlayerCurrency.Instance.AddGold(sellPrice);
            
            Debug.Log($"Sold {item.itemName} for {sellPrice} gold");
            UpdateCurrencyDisplay();
            
            // Refresh player items
            LoadPlayerItems();
            itemInfoPanel.SetActive(false);
        }
    }
    
    void UpdateCurrencyDisplay()
    {
        if (playerGoldText != null && PlayerCurrency.Instance != null)
        {
            playerGoldText.text = $"{PlayerCurrency.Instance.gold} Gold";
        }
    }
    
    void RefreshShop()
    {
        if (isBuying)
        {
            LoadShopItems();
        }
        else
        {
            LoadPlayerItems();
        }
    }
    
    void OnDestroy()
    {
        if (PlayerCurrency.Instance != null)
        {
            PlayerCurrency.Instance.OnCurrencyChanged -= UpdateCurrencyDisplay;
        }
    }
}