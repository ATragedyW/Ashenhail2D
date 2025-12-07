using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ManaBarUI : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private Slider manaSlider;
    [SerializeField] private TMP_Text manaText;
    [SerializeField] private Image fillImage;
    
    [Header("Colors")]
    [SerializeField] private Color fullManaColor = Color.blue;
    [SerializeField] private Color lowManaColor = Color.cyan;
    [SerializeField] private Color emptyManaColor = Color.gray;
    
    [Header("Effects")]
    [SerializeField] private bool showRegenEffect = true;
    [SerializeField] private ParticleSystem manaRegenParticles;
    
    private PlayerController player;
    private int lastManaValue;
    
    void Start()
    {
        player = FindFirstObjectByType<PlayerController>();
        
        if (player != null)
        {
            
            player.OnManaChangedEvent.AddListener(UpdateManaBarWrapper);
            lastManaValue = player.currentMana;
            UpdateManaBarWrapper(); 
        }
        
        if (manaRegenParticles != null && !showRegenEffect)
            manaRegenParticles.Stop();
    }
    
    
    public void UpdateManaBarWrapper()
    {
        if (player == null) return;
        UpdateManaBar(player.currentMana, player.maxMana);
    }
    
    
    public void UpdateManaBar(int currentMana, int maxMana)
    {
        if (maxMana <= 0) return;
        
        float manaPercent = (float)currentMana / maxMana * 100f;
        
       
        if (manaSlider != null)
        {
            manaSlider.value = manaPercent;
        }
        
        
        if (manaText != null)
        {
            manaText.text = $"Mana: {currentMana}/{maxMana}";
        }
        
       
        if (fillImage != null)
        {
            if (currentMana <= 0)
            {
                fillImage.color = emptyManaColor;
            }
            else
            {
                fillImage.color = Color.Lerp(lowManaColor, fullManaColor, manaPercent / 100f);
            }
        }
        
       
        if (showRegenEffect && manaRegenParticles != null && currentMana > lastManaValue)
        {
            manaRegenParticles.Play();
        }
        
        lastManaValue = currentMana;
    }
    
    void OnDestroy()
    {
        if (player != null)
        {
            
            player.OnManaChangedEvent.RemoveListener(UpdateManaBarWrapper);
        }
    }
}