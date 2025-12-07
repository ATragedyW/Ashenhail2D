using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class InventoryUI : MonoBehaviour
{
    [Header("UI References")]
    public GameObject inventoryPanel;
    public Transform slotsContainer;
    public GameObject slotPrefab;
    public TextMeshProUGUI goldText;
    public TextMeshProUGUI capacityText;
    
    [Header("Item Info Panel")]
    public GameObject itemInfoPanel;
    public Image itemInfoIcon;
    public TextMeshProUGUI itemInfoName;
    public TextMeshProUGUI itemInfoDescription;
    public Button useButton;
    public Button dropButton;
    public Button closeInfoButton;
    
    [Header("Settings")]
    public KeyCode toggleKey = KeyCode.I;
    public int columns = 5;
    public int rows = 4;
    
    private InventorySlotUI[] slotUIs;
    private Item selectedItem;
    private int selectedSlotIndex = -1;
    
    void Start()
    {
       
        InitializeSlots();
        
       
        inventoryPanel.SetActive(false);
        itemInfoPanel.SetActive(false);
       
        if (InventoryManager.Instance != null)
        {
            InventoryManager.Instance.OnInventoryChanged += UpdateInventoryUI;
        }
        
       
        if (PlayerCurrency.Instance != null)
        {
            PlayerCurrency.Instance.OnCurrencyChanged += UpdateCurrencyDisplay;
        }
        
       
        if (useButton != null)
            useButton.onClick.AddListener(UseSelectedItem);
        
        if (dropButton != null)
            dropButton.onClick.AddListener(DropSelectedItem);
        
        if (closeInfoButton != null)
            closeInfoButton.onClick.AddListener(HideItemInfo);
        
       
        UpdateCurrencyDisplay();
    }
    
    void Update()
    {
       
        if (Input.GetKeyDown(toggleKey))
        {
            ToggleInventory();
        }
        
        
        if (inventoryPanel.activeSelf && Input.GetKeyDown(KeyCode.Escape))
        {
            if (itemInfoPanel.activeSelf)
            {
                HideItemInfo();
            }
            else
            {
                ToggleInventory();
            }
        }
    }
    
    void InitializeSlots()
    {
        int totalSlots = columns * rows;
        slotUIs = new InventorySlotUI[totalSlots];
        
        
        foreach (Transform child in slotsContainer)
        {
            Destroy(child.gameObject);
        }
        
       
        for (int i = 0; i < totalSlots; i++)
        {
            GameObject slotObj = Instantiate(slotPrefab, slotsContainer);
            InventorySlotUI slotUI = slotObj.GetComponent<InventorySlotUI>();
            
            if (slotUI != null)
            {
                slotUI.Initialize(i);
                
               
                Button slotButton = slotObj.GetComponent<Button>();
                if (slotButton != null)
                {
                    int slotIndex = i; 
                    slotButton.onClick.AddListener(() => OnSlotClicked(slotIndex));
                }
                
                slotUIs[i] = slotUI;
            }
        }
        
       
        UpdateInventoryUI();
    }
    
    void UpdateInventoryUI()
    {
        if (InventoryManager.Instance == null) return;
        
       
        for (int i = 0; i < slotUIs.Length; i++)
        {
            if (i < InventoryManager.Instance.slots.Count)
            {
                slotUIs[i].UpdateSlot(InventoryManager.Instance.slots[i]);
            }
            else
            {
                slotUIs[i].ClearSlot();
            }
        }
        
      
        if (capacityText != null)
        {
            int usedSlots = 0;
            foreach (var slot in InventoryManager.Instance.slots)
            {
                if (!slot.IsEmpty) usedSlots++;
            }
            capacityText.text = $"{usedSlots}/{InventoryManager.Instance.slots.Count}";
        }
    }
    
    void UpdateCurrencyDisplay()
    {
        if (goldText != null && PlayerCurrency.Instance != null)
        {
            goldText.text = $"{PlayerCurrency.Instance.gold} Gold";
        }
    }
    
    void OnSlotClicked(int slotIndex)
    {
        if (slotIndex < 0 || slotIndex >= InventoryManager.Instance.slots.Count) return;
        
        var slot = InventoryManager.Instance.slots[slotIndex];
        if (!slot.IsEmpty)
        {
            selectedItem = slot.item;
            selectedSlotIndex = slotIndex;
            ShowItemInfo(slot.item, slotIndex);
        }
    }
    
    void ShowItemInfo(Item item, int slotIndex)
    {
        if (itemInfoPanel == null) return;
        
        
        if (itemInfoIcon != null)
            itemInfoIcon.sprite = item.icon;
        
        if (itemInfoName != null)
            itemInfoName.text = item.itemName;
        
        if (itemInfoDescription != null)
            itemInfoDescription.text = item.GetTooltip();
        
       
        if (useButton != null)
        {
            useButton.gameObject.SetActive(item.isUsable);
            useButton.interactable = item.isUsable;
        }
        
        if (dropButton != null)
        {
            dropButton.gameObject.SetActive(item.isSellable);
        }
        
        
        itemInfoPanel.SetActive(true);
    }
    
    void HideItemInfo()
    {
        itemInfoPanel.SetActive(false);
        selectedItem = null;
        selectedSlotIndex = -1;
    }
    
    void UseSelectedItem()
    {
        if (selectedItem != null && selectedSlotIndex >= 0)
        {
            InventoryManager.Instance.UseItem(selectedSlotIndex);
            HideItemInfo();
        }
    }
    
    void DropSelectedItem()
    {
        if (selectedItem != null && selectedSlotIndex >= 0)
        {
           
            Debug.Log($"Dropped {selectedItem.itemName}");
            
           
            InventoryManager.Instance.RemoveItem(selectedSlotIndex, 1);
            
            
            
            HideItemInfo();
        }
    }
    
    void ToggleInventory()
    {
        bool newState = !inventoryPanel.activeSelf;
        inventoryPanel.SetActive(newState);
        
       
        if (!newState)
        {
            HideItemInfo();
        }
        
       
        if (newState)
        {
            Time.timeScale = 0f;
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }
        else
        {
            Time.timeScale = 1f;
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }
    }
    
    
    public void RefreshUI()
    {
        UpdateInventoryUI();
        UpdateCurrencyDisplay();
    }
    
    void OnDestroy()
    {
       
        if (InventoryManager.Instance != null)
        {
            InventoryManager.Instance.OnInventoryChanged -= UpdateInventoryUI;
        }
        
        if (PlayerCurrency.Instance != null)
        {
            PlayerCurrency.Instance.OnCurrencyChanged -= UpdateCurrencyDisplay;
        }
    }
}