using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ClassSelectionMenu : MonoBehaviour
{
    public PlayerClass iceClass;
    public PlayerClass fireClass;
    public PlayerClass necroClass;

    public Button iceButton;
    public Button fireButton;
    public Button necroButton;

    private void Start()
    {
        
        EnsureGameManagerExists();

        iceButton.onClick.AddListener(() => SelectClass(iceClass));
        fireButton.onClick.AddListener(() => SelectClass(fireClass));
        necroButton.onClick.AddListener(() => SelectClass(necroClass));

        
    }

    void EnsureGameManagerExists()
    {
        if (GameManager.Instance == null)
        {
           
            GameObject gmObject = new GameObject("GameManager");
            gmObject.AddComponent<GameManager>();
        }
    }

    void SelectClass(PlayerClass chosen)
    {
        if (chosen == null)
        {
            Debug.LogError("Trying to select null class!");
            return;
        }

        
        
        if (GameManager.Instance != null)
        {
            GameManager.Instance.SetClass(chosen);
            
            GameManager.Instance.StartGame();
        }
        else
        {
            Debug.LogError("GameManager instance is null!");
        }
    }
}