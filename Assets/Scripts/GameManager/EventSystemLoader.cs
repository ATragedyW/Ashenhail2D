using UnityEngine;
using UnityEngine.EventSystems; 

public class EventSystemLoader : MonoBehaviour
{
    public GameObject eventSystemPrefab;
    private static bool isLoaded = false;
    
    void Awake()
    {
       
        if (isLoaded)
        {
            Destroy(gameObject);
            return;
        }
        
       
        
       
        if (FindAnyObjectByType<EventSystem>() != null)
        {
            Debug.Log("EventSystem already exists, marking as loaded.");
            isLoaded = true;
            Destroy(gameObject);
            return;
        }
        
        
        if (eventSystemPrefab != null)
        {
            GameObject es = Instantiate(eventSystemPrefab);
            es.name = "PersistentEventSystem";
            DontDestroyOnLoad(es);
            Debug.Log($"EventSystem created from prefab: {es.name}");
        }
        else
        {
            Debug.LogError("EventSystem prefab not assigned!");
        }
        
        isLoaded = true;
        Destroy(gameObject); 
    }
}