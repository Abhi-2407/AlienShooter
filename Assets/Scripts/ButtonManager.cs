using Fusion;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class ButtonManager : MonoBehaviour
{
    public static ButtonManager Instance; 

    [Header("Button References")]
    public Button redButton;
    public Button blueButton;
    
    [Header("Enemy Spawner Reference")]
    public EnemySpawner enemySpawner;
    
    [Header("Bot Settings")]
    public bool enableBotMode = false;
    public float botClickInterval = 2f;
    public bool botClickRedButton = true;
    
    private float botTimer = 0f;

    void Awake()
    {
        // Singleton pattern
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        // Find enemy spawner if not assigned
        if (enemySpawner == null)
        {
            enemySpawner = FindObjectOfType<EnemySpawner>();
        }

        // Setup button listeners
        if (redButton != null)
        {
            if (!GameManager.Instance.IsSinglePlayerMode)
            {
                redButton.gameObject.SetActive(true);
                //redButton.onClick.AddListener(OnRedButtonClicked);
            }
        }

        if (blueButton != null)
        {
            blueButton.gameObject.SetActive(true);
            //blueButton.onClick.AddListener(OnBlueButtonClicked);
        }
    }

    void Update()
    {
        // Handle bot functionality
        if (enableBotMode && GameManager.Instance.gameState == GameState.START && GameManager.Instance.IsSinglePlayerMode)
        {
            HandleBotBehavior();
        }
    }

    public void OnBlueButtonClicked()
    {
        if (blueButton.interactable)
        {
            Debug.Log("Blue Button Clicked!");

            GameManager.Instance.blueMissile.DropMissile();

            if (!GameManager.Instance.IsSinglePlayerMode)
                GameManager.Instance.localPlayer.RPC_DropBlueMissile();

            StartCoroutine(ButtonCooldown(blueButton));
        }
    }

    public void OnRedButtonClicked()
    {
        if (redButton.interactable)
        {
            Debug.Log("Red Button Clicked!");

            GameManager.Instance.redMissile.DropMissile();

            if (!GameManager.Instance.IsSinglePlayerMode)
                GameManager.Instance.localPlayer.RPC_DropRedMissile();

            StartCoroutine(ButtonCooldown(redButton));
        }
    }
    
    System.Collections.IEnumerator ButtonCooldown(Button btn)
    {
        btn.interactable = false;
        
        yield return new WaitForSeconds(1.0f);

        btn.interactable = true;
    }
    
    /// <summary>
    /// Bot method that handles automatic red button clicking
    /// </summary>
    void HandleBotBehavior()
    {
        botTimer += Time.deltaTime;
        
        if (botTimer >= botClickInterval)
        {
            botTimer = 0f;
            
            // Check if we should click red button
            if (botClickRedButton)
            {
                BotClickRedButton();
            }
        }
    }
    
    /// <summary>
    /// Bot method to automatically trigger red button functionality
    /// </summary>
    public void BotClickRedButton()
    {
        //Debug.Log("Bot: Automatically clicking Red Button!");

        OnRedButtonClicked();
    }
}

