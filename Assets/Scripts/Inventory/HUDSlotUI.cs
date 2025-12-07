using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class HUDSlotUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Header("UI References")]
    public Image slotBackground;
    public Image itemIcon;
    public TextMeshProUGUI quantityText;
    public TextMeshProUGUI keyText;
    public GameObject highlight;
    
    [Header("Tooltip")]
    public GameObject tooltipPanel;
    public TextMeshProUGUI tooltipName;
    public TextMeshProUGUI tooltipDescription;
    
    [Header("Colors")]
    public Color normalColor = new Color(0.2f, 0.2f, 0.2f, 0.8f);
    public Color highlightColor = new Color(0.3f, 0.3f, 0.3f, 0.9f);
    public Color emptyColor = new Color(0.1f, 0.1f, 0.1f, 0.5f);
    
    private InventoryManager.InventorySlot currentSlot;
    private bool isMouseOver = false;
    
    void Start()
    {
        
        if (tooltipPanel != null)
            tooltipPanel.SetActive(false);
        
        if (highlight != null)
            highlight.SetActive(false);
    }
    
    void Update()
    {
       
        if (isMouseOver && tooltipPanel != null && tooltipPanel.activeSelf && currentSlot != null)
        {
            UpdateTooltipPosition();
        }
    }
    
    public void UpdateSlot(InventoryManager.InventorySlot slot)
    {
        currentSlot = slot;
        
        if (slot != null && !slot.IsEmpty)
        {
           
            itemIcon.sprite = slot.item.icon;
            itemIcon.gameObject.SetActive(true);
            
            if (slot.quantity > 1)
            {
                quantityText.text = slot.quantity.ToString();
                quantityText.gameObject.SetActive(true);
            }
            else
            {
                quantityText.gameObject.SetActive(false);
            }
            
            slotBackground.color = normalColor;
        }
        else
        {
         
            itemIcon.gameObject.SetActive(false);
            quantityText.gameObject.SetActive(false);
            slotBackground.color = emptyColor;
        }
    }
    
    public void SetHighlight(bool active)
    {
        if (highlight != null)
            highlight.SetActive(active);
        
        slotBackground.color = active ? highlightColor : normalColor;
    }
    
    public void OnPointerEnter(PointerEventData eventData)
    {
        isMouseOver = true;
        
        if (currentSlot != null && !currentSlot.IsEmpty && tooltipPanel != null)
        {
            tooltipName.text = currentSlot.item.itemName;
            tooltipDescription.text = currentSlot.item.GetTooltip();
            tooltipPanel.SetActive(true);
            UpdateTooltipPosition();
        }
        
        SetHighlight(true);
    }
    
    public void OnPointerExit(PointerEventData eventData)
    {
        isMouseOver = false;
        
        if (tooltipPanel != null)
            tooltipPanel.SetActive(false);
        
        SetHighlight(false);
    }
    
    void UpdateTooltipPosition()
    {
        if (tooltipPanel == null) return;
        
        RectTransform tooltipRect = tooltipPanel.GetComponent<RectTransform>();
        Vector3[] corners = new Vector3[4];
        GetComponent<RectTransform>().GetWorldCorners(corners);
        
       
        Vector3 slotTop = corners[1]; 
        tooltipPanel.transform.position = slotTop + new Vector3(0, 10, 0);
        
        
        RectTransform canvasRect = tooltipPanel.transform.parent.GetComponent<RectTransform>();
        Vector3 tooltipPos = tooltipRect.position;
        
       
        if (tooltipPos.x + tooltipRect.rect.width > canvasRect.rect.width)
        {
            tooltipPos.x = canvasRect.rect.width - tooltipRect.rect.width;
        }
        
        
        if (tooltipPos.y + tooltipRect.rect.height > canvasRect.rect.height)
        {
            tooltipPos.y = corners[0].y - tooltipRect.rect.height;
        }
        
        tooltipRect.position = tooltipPos;
    }
}