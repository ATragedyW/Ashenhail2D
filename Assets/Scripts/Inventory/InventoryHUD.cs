using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class InventoryHUD : MonoBehaviour
{
    [Header("HUD Settings")]
    public int visibleSlots = 4; 
    public KeyCode toggleKey = KeyCode.I;
    public bool showHUD = true;
    
    [Header("Quick Slots")]
    public Transform slotsContainer;
    public GameObject slotPrefab;
    
    [Header("HUD Elements")]
    public TextMeshProUGUI goldText;
    public TextMeshProUGUI capacityText;
    public GameObject notificationPanel;
    public TextMeshProUGUI notificationText;
    
    [Header("Quick Use")]
    public KeyCode[] quickUseKeys = {
        KeyCode.Alpha1, KeyCode.Alpha2, 
        KeyCode.Alpha3, KeyCode.Alpha4
    };
    
    private HUDSlotUI[] slotUIs;
    private List<InventoryManager.InventorySlot> visibleItems = new List<InventoryManager.InventorySlot>();
    private float notificationTimer = 0f;
    
    void Start()
    {
        Debug.Log("=== INVENTORY HUD START ===");
        
        
        if (slotPrefab == null)
        {
            
            return;
        }
        
        
        HUDSlotUI prefabComponent = slotPrefab.GetComponent<HUDSlotUI>();
        if (prefabComponent == null)
        {
            Debug.LogError($"❌ Slot Prefab {slotPrefab.name} doesn't have HUDSlotUI component!");
            return;
        }
        else
        {
            Debug.Log($"✅ Slot Prefab has HUDSlotUI component");
        }
        
        
        InitializeSlots();
        
       
        if (InventoryManager.Instance != null)
        {
            InventoryManager.Instance.OnInventoryChanged += UpdateHUD;
            Debug.Log("✅ Subscribed to OnInventoryChanged");
        }
        else
        {
            Debug.LogError("❌ InventoryManager.Instance is null!");
            
            
            StartCoroutine(FindOrCreateInventoryManager());
        }
        
       
        UpdateHUD();
        
       
        if (notificationPanel != null)
            notificationPanel.SetActive(false);
        
        Debug.Log("=== HUD INITIALIZED ===");
    }
    
    System.Collections.IEnumerator FindOrCreateInventoryManager()
    {
        yield return new WaitForSeconds(0.1f); 
        
      
        InventoryManager manager = FindAnyObjectByType<InventoryManager>();
        if (manager == null)
        {
            Debug.LogWarning("No InventoryManager found, will wait for it to be created...");
            
        }
        else
        {
            Debug.Log($"Found InventoryManager: {manager.gameObject.name}");
            
            
            int attempts = 0;
            while (InventoryManager.Instance == null && attempts < 10)
            {
                attempts++;
                yield return new WaitForSeconds(0.1f);
            }
            
            if (InventoryManager.Instance != null)
            {
                InventoryManager.Instance.OnInventoryChanged += UpdateHUD;
                Debug.Log("Successfully subscribed to InventoryManager");
            }
        }
    }
    
    void Update()
    {
       
        if (Input.GetKeyDown(toggleKey))
        {
            showHUD = !showHUD;
            if (slotsContainer != null)
                slotsContainer.gameObject.SetActive(showHUD);
        }
        
       
        for (int i = 0; i < quickUseKeys.Length; i++)
        {
            if (Input.GetKeyDown(quickUseKeys[i]) && i < visibleItems.Count)
            {
                UseItemFromVisibleSlot(i);
            }
        }
        
   
        if (notificationTimer > 0)
        {
            notificationTimer -= Time.deltaTime;
            if (notificationTimer <= 0 && notificationPanel != null)
            {
                notificationPanel.SetActive(false);
            }
        }
    }
    
    void InitializeSlots()
    {
        Debug.Log($"Initializing {visibleSlots} HUD slots");
        
         
        if (slotsContainer != null)
        {
            foreach (Transform child in slotsContainer)
            {
                Destroy(child.gameObject);
            }
        }
        else
        {
            Debug.LogError("❌ Slots Container is not assigned in Inspector!");
            return;
        }
        
        if (slotPrefab == null)
        {
            Debug.LogError("❌ Slot Prefab is not assigned in Inspector!");
            return;
        }
        
        
        slotUIs = new HUDSlotUI[visibleSlots];
        for (int i = 0; i < visibleSlots; i++)
        {
            GameObject slotObj = Instantiate(slotPrefab, slotsContainer);
            HUDSlotUI slotUI = slotObj.GetComponent<HUDSlotUI>();
            
            if (slotUI != null)
            {
                slotUIs[i] = slotUI;
                Debug.Log($"✅ Created HUD slot {i}");
            }
            else
            {
                Debug.LogError($"❌ Slot prefab doesn't have HUDSlotUI component!");
            }
        }
        
        Debug.Log($"✅ Created {slotUIs.Length} HUD slots");
    }
    
  
    void UpdateHUD()
    {
        Debug.Log("=== HUD UPDATE CALLED ===");
        
        if (InventoryManager.Instance == null)
        {
            Debug.LogError("❌ InventoryManager.Instance is null!");
            return;
        }
        
        
        
        
        visibleItems.Clear();
        
        int itemCount = 0;
        foreach (var slot in InventoryManager.Instance.slots)
        {
            if (!slot.IsEmpty)
            {
                itemCount++;
                if (visibleItems.Count < visibleSlots)
                {
                    visibleItems.Add(slot);
                    Debug.Log($"Adding to HUD: {slot.quantity}x {slot.item?.itemName}");
                }
            }
        }
        
        Debug.Log($"Total items in inventory: {itemCount}");
        Debug.Log($"Items to show in HUD: {visibleItems.Count}");
        
       
        for (int i = 0; i < slotUIs.Length; i++)
        {
            if (slotUIs[i] != null)
            {
                if (i < visibleItems.Count)
                {
                    Debug.Log($"Setting slot {i} with item");
                    slotUIs[i].UpdateSlot(visibleItems[i]);
                    slotUIs[i].gameObject.SetActive(true);
                }
                else
                {
                    Debug.Log($"Clearing slot {i}");
                    slotUIs[i].UpdateSlot(null);
                    slotUIs[i].gameObject.SetActive(true);
                }
            }
            else
            {
                Debug.LogError($"SlotUI {i} is null!");
            }
        }
        
       
        UpdateStats();
        
        
        Canvas.ForceUpdateCanvases();
        if (slotsContainer != null)
        {
            LayoutRebuilder.ForceRebuildLayoutImmediate(slotsContainer as RectTransform);
        }
        
        Debug.Log("=== HUD UPDATE COMPLETE ===");
    }
    
    void UpdateStats()
    {
       
        int usedSlots = 0;
        if (InventoryManager.Instance != null)
        {
            foreach (var slot in InventoryManager.Instance.slots)
            {
                if (!slot.IsEmpty) usedSlots++;
            }
            
          
            if (capacityText != null)
            {
                capacityText.text = $"Items: {usedSlots}/{InventoryManager.Instance.slots.Count}";
            }
        }
        else
        {
            if (capacityText != null)
                capacityText.text = "Items: 0/0";
        }
        
       
        if (goldText != null)
        {
            if (PlayerCurrency.Instance != null)
            {
                goldText.text = $"Gold: {PlayerCurrency.Instance.gold}";
            }
            else
            {
                goldText.text = "Gold: 0";
            }
        }
    }
    
    void UseItemFromVisibleSlot(int visibleIndex)
    {
        if (visibleIndex >= 0 && visibleIndex < visibleItems.Count)
        {
          
            Item itemToUse = visibleItems[visibleIndex].item;
            for (int i = 0; i < InventoryManager.Instance.slots.Count; i++)
            {
                var slot = InventoryManager.Instance.slots[i];
                if (!slot.IsEmpty && slot.item == itemToUse)
                {
                    InventoryManager.Instance.UseItem(i);
                    
                   
                    ShowNotification($"Used {itemToUse.itemName}");
                    return;
                }
            }
        }
    }
    
    public void ShowNotification(string message, float duration = 2f)
    {
        if (notificationPanel != null && notificationText != null)
        {
            notificationText.text = message;
            notificationPanel.SetActive(true);
            notificationTimer = duration;
        }
    }
    
    public void AddItemWithNotification(Item item, int amount = 1)
    {
        if (InventoryManager.Instance != null)
        {
            bool added = InventoryManager.Instance.AddItem(item, amount);
            if (added)
            {
                ShowNotification($"Added {amount}x {item.itemName}");
            }
            else
            {
                ShowNotification("Inventory full!", 1.5f);
            }
        }
    }
    
    void OnDestroy()
    {
        if (InventoryManager.Instance != null)
        {
            InventoryManager.Instance.OnInventoryChanged -= UpdateHUD;
        }
    }
}