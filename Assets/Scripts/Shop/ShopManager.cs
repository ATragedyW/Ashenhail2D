using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class ShopManager : MonoBehaviour
{
    public static ShopManager Instance { get; private set; }
    
    [Header("UI References")]
    public GameObject shopPanel;
    public Transform shopItemsContainer;
    public Transform playerItemsContainer;
    public GameObject shopItemPrefab;
    public GameObject playerItemPrefab;
    
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
    public Button closeButton;
    
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
    
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    void Start()
    {
        shopPanel.SetActive(false);
        itemInfoPanel.SetActive(false);
        
        // Set up tab buttons
        buyTabButton.onClick.AddListener(() => SetTab(true));
        sellTabButton.onClick.AddListener(() => SetTab(false));
        
        // Close button
        if (closeButton != null)
            closeButton.onClick.AddListener(CloseShop);
        
        // Subscribe to currency changes
        if (PlayerCurrency.Instance != null)
        {
            PlayerCurrency.Instance.OnCurrencyChanged += UpdateCurrencyDisplay;
        }
        
        // Subscribe to inventory changes
        if (InventoryManager.Instance != null)
        {
            InventoryManager.Instance.OnInventoryChanged += RefreshSellPanel;
        }
    }
    
    void Update()
    {
        // Open/close shop with B key
        if (Input.GetKeyDown(KeyCode.B) && currentShop != null)
        {
            ToggleShop();
        }
        
        // Close shop with Escape
        if (shopPanel.activeSelf && Input.GetKeyDown(KeyCode.Escape))
        {
            CloseShop();
        }
    }
    
    public void OpenShop(ShopInventory shop)
    {
        if (shop == null) return;
        
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
        else
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
        // Clear existing items
        foreach (Transform child in shopItemsContainer)
        {
            Destroy(child.gameObject);
        }
        shopItemUIs.Clear();
        
        if (currentShop == null) return;
        
        // Create shop items
        foreach (var shopItem in currentShop.itemsForSale)
        {
            if (shopItem.stock != 0) // Skip items with 0 stock
            {
                GameObject itemObj = Instantiate(shopItemPrefab, shopItemsContainer);
                ShopItemUI itemUI = itemObj.GetComponent<ShopItemUI>();
                
                itemUI.Initialize(shopItem, this);
                shopItemUIs.Add(itemUI);
            }
        }
    }
    
    void LoadPlayerItems()
    {
        // Clear existing items
        foreach (Transform child in playerItemsContainer)
        {
            Destroy(child.gameObject);
        }
        playerItemUIs.Clear();
        
        if (InventoryManager.Instance == null) return;
        
        // Create player items (only items that can be sold)
        foreach (var slot in InventoryManager.Instance.slots) // FIXED: Now accessible
        {
            if (!slot.IsEmpty && slot.item.isSellable)
            {
                GameObject itemObj = Instantiate(playerItemPrefab, playerItemsContainer);
                PlayerItemUI itemUI = itemObj.GetComponent<PlayerItemUI>();
                
                itemUI.Initialize(slot, this);
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
            int sellPrice = CalculateSellPrice(item);
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
            bool success = InventoryManager.Instance.AddItem(shopItemData.item, 1);
            
            if (success)
            {
                // Reduce shop stock if not infinite
                if (shopItemData.stock > 0)
                {
                    shopItemData.stock--;
                    if (shopItemData.stock <= 0)
                    {
                        LoadShopItems();
                    }
                }
                
                Debug.Log($"Bought {shopItemData.item.itemName} for {shopItemData.price} gold");
                UpdateCurrencyDisplay();
                itemInfoPanel.SetActive(false);
            }
        }
    }
    
    void SellItem(Item item, int sellPrice)
    {
        // Find and remove item from inventory
        for (int i = 0; i < InventoryManager.Instance.slots.Count; i++) // FIXED: Now accessible
        {
            var slot = InventoryManager.Instance.slots[i];
            if (!slot.IsEmpty && slot.item == item)
            {
                InventoryManager.Instance.RemoveItem(i, 1);
                
                // Add gold to player
                PlayerCurrency.Instance.AddGold(sellPrice);
                
                Debug.Log($"Sold {item.itemName} for {sellPrice} gold");
                UpdateCurrencyDisplay();
                
                // Refresh player items
                LoadPlayerItems();
                itemInfoPanel.SetActive(false);
                return;
            }
        }
    }
    
    int CalculateSellPrice(Item item)
    {
        // Check if shop has custom sell price for this item
        if (currentShop != null)
        {
            ShopItemData shopData = currentShop.GetItemData(item);
            if (shopData != null && shopData.sellPrice > 0)
            {
                return shopData.sellPrice;
            }
        }
        
        // Default to half of base value
        return Mathf.RoundToInt(item.baseValue * 0.5f);
    }
    
    void UpdateCurrencyDisplay()
    {
        if (playerGoldText != null && PlayerCurrency.Instance != null)
        {
            playerGoldText.text = $"{PlayerCurrency.Instance.gold} Gold";
        }
        
        if (playerGemsText != null && PlayerCurrency.Instance != null)
        {
            playerGemsText.text = $"{PlayerCurrency.Instance.gems} Gems";
        }
    }
    
    void RefreshSellPanel()
    {
        if (!isBuying && sellPanel.activeSelf)
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
        
        if (InventoryManager.Instance != null)
        {
            InventoryManager.Instance.OnInventoryChanged -= RefreshSellPanel;
        }
    }
}