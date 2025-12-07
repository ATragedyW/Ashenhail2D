using UnityEngine;
using System.Collections;

public class ShopkeeperNPC : MonoBehaviour
{
    [Header("Animation")]
    public Animator animator;
    public string idleAnimation = "Idle";
    public string talkAnimation = "Talk";
    public string waveAnimation = "Wave";
    
    [Header("Dialogue")]
    public string[] greetings = {
        "Welcome!",
        "Hello there!",
        "Good to see you!",
        "Looking for something special?"
    };
    
    [Header("Behavior")]
    public float waveChance = 0.3f;
    public float minWaveInterval = 10f;
    public float maxWaveInterval = 30f;
    
    private float nextWaveTime;
    private bool isPlayerNearby = false;
    
    void Start()
    {
        nextWaveTime = Time.time + Random.Range(minWaveInterval, maxWaveInterval);
        
        if (animator == null)
            animator = GetComponent<Animator>();
    }
    
    void Update()
    {
        if (!isPlayerNearby && Time.time > nextWaveTime)
        {
            WaveAtPlayer();
            nextWaveTime = Time.time + Random.Range(minWaveInterval, maxWaveInterval);
        }
    }
    
    public void OnPlayerApproach()
    {
        isPlayerNearby = true;
        
    }
    
    public void OnPlayerLeave()
    {
        isPlayerNearby = false;
    }
    
    void WaveAtPlayer()
    {
        if (Random.value < waveChance && animator != null)
        {
            animator.Play(waveAnimation);
            StartCoroutine(ReturnToIdle(1.5f));
        }
    }
    
    IEnumerator ReturnToIdle(float delay)
    {
        yield return new WaitForSeconds(delay);
        
    }
    
    void OnMouseOver()
    {
        
        if (Input.GetMouseButtonDown(1)) 
        {
            OnPlayerApproach();
        }
    }
}