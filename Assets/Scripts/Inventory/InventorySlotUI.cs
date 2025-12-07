using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InventorySlotUI : MonoBehaviour
{
    [Header("UI References")]
    public Image itemIcon;
    public TextMeshProUGUI itemNameText;
    public TextMeshProUGUI quantityText;
    public GameObject equippedIndicator;
    public Image slotBackground;
    public Button slotButton;
    
    [Header("Colors")]
    public Color normalColor = Color.white;
    public Color equippedColor = Color.yellow;
    public Color emptyColor = new Color(0.3f, 0.3f, 0.3f, 0.5f);
    public Color selectedColor = Color.cyan;
    
    private int slotIndex;
    private Item currentItem;
    private bool isSelected = false;
    
    public void Initialize(int index)
    {
        slotIndex = index;
        ClearSlot();
        
        if (slotButton != null)
        {
            slotButton.onClick.AddListener(OnSlotClicked);
        }
    }
    
    public void UpdateSlot(InventoryManager.InventorySlot slot)
    {
        if (slot == null || slot.IsEmpty)
        {
            ClearSlot();
            return;
        }
        
        currentItem = slot.item;
        
        
        if (itemIcon != null)
        {
            itemIcon.sprite = slot.item.icon;
            itemIcon.color = normalColor;
            itemIcon.gameObject.SetActive(true);
        }
        
        if (itemNameText != null)
        {
            itemNameText.text = slot.item.itemName;
            itemNameText.gameObject.SetActive(true);
        }
        
        if (quantityText != null)
        {
           
            quantityText.text = slot.item.isStackable ? $"x{slot.quantity}" : "";
            quantityText.gameObject.SetActive(slot.item.isStackable && slot.quantity > 1);
        }
        
       
        if (equippedIndicator != null)
        {
            bool isEquipped = CheckIfEquipped(slot.item);
            equippedIndicator.SetActive(isEquipped);
            
            if (isEquipped && itemIcon != null)
            {
                itemIcon.color = equippedColor;
            }
        }
        
        
        if (slotBackground != null)
        {
            slotBackground.color = isSelected ? selectedColor : Color.clear;
        }
    }
    
    public void ClearSlot()
    {
        currentItem = null;
        isSelected = false;
        
        if (itemIcon != null)
        {
            itemIcon.sprite = null;
            itemIcon.color = emptyColor;
            itemIcon.gameObject.SetActive(false);
        }
        
        if (itemNameText != null)
        {
            itemNameText.text = "";
            itemNameText.gameObject.SetActive(false);
        }
        
        if (quantityText != null)
        {
            quantityText.text = "";
            quantityText.gameObject.SetActive(false);
        }
        
        if (equippedIndicator != null)
        {
            equippedIndicator.SetActive(false);
        }
        
        if (slotBackground != null)
        {
            slotBackground.color = Color.clear;
        }
    }
    
    void OnSlotClicked()
    {
        if (currentItem != null)
        {
            Debug.Log($"Clicked on slot {slotIndex}: {currentItem.itemName}");
        }
        else
        {
            Debug.Log($"Clicked on empty slot {slotIndex}");
        }
    }
    
    bool CheckIfEquipped(Item item)
    {
      
        return false;
    }
    
    public void SetSelected(bool selected)
    {
        isSelected = selected;
        
        if (slotBackground != null)
        {
            slotBackground.color = selected ? selectedColor : Color.clear;
        }
    }
}