using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ShopInteraction : MonoBehaviour
{
    [Header("Shop Settings")]
    public ShopInventory shopInventory;
    public float interactionRange = 2f;
    
    [Header("UI References")]
    public Canvas interactionCanvas;
    public TextMeshProUGUI interactionText;
    public GameObject pressEPrompt;
    
    [Header("Visual Effects")]
    public ParticleSystem glowEffect;
    public AudioClip shopBellSound;
    public GameObject shopSign;
    
    private bool playerInRange = false;
    private AudioSource audioSource;
    
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();
        
        
        if (interactionCanvas != null)
            interactionCanvas.gameObject.SetActive(false);
        
        if (pressEPrompt != null)
            pressEPrompt.SetActive(false);
        
        
        if (glowEffect != null)
            glowEffect.Stop();
    }
    
    void Update()
    {
        if (playerInRange && Input.GetKeyDown(KeyCode.E))
        {
            OpenShop();
        }
    }
    
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
            ShowInteractionPrompt(true);
            
           
            if (glowEffect != null)
                glowEffect.Play();
        }
    }
    
    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
            ShowInteractionPrompt(false);
            
            
            if (glowEffect != null)
                glowEffect.Stop();
            
          
            if (ShopManager.Instance != null && ShopManager.Instance.shopPanel.activeSelf)
            {
                ShopManager.Instance.CloseShop();
            }
        }
    }
    
    void ShowInteractionPrompt(bool show)
    {
        if (interactionCanvas != null)
            interactionCanvas.gameObject.SetActive(show);
        
        if (pressEPrompt != null)
            pressEPrompt.SetActive(show);
        
        if (interactionText != null)
            interactionText.text = $"Press E to browse {shopInventory.shopName}";
    }
    
    void OpenShop()
    {
        if (ShopManager.Instance != null && shopInventory != null)
        {
            ShopManager.Instance.OpenShop(shopInventory);
            
            
            if (shopBellSound != null)
                audioSource.PlayOneShot(shopBellSound);
            
            
        }
        else
        {
            Debug.LogError("ShopManager or ShopInventory not found!");
        }
    }
    
    #if UNITY_EDITOR
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, interactionRange);
        
       
    }
    #endif
}