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
    
    [Header("Button Settings")]
    public float buttonCooldown = 0.5f;
    
    [Header("Bot Settings")]
    public bool enableBotMode = false;
    public float botClickInterval = 2f;
    public bool botClickRedButton = true;
    
    private bool redButtonOnCooldown = false;
    private bool blueButtonOnCooldown = false;
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
                redButton.onClick.AddListener(OnRedButtonClicked);
        }

        if (blueButton != null)
        {
            blueButton.onClick.AddListener(OnBlueButtonClicked);
        }

        // Initialize button colors
        UpdateButtonColors();
    }

    void Update()
    {
        // Update button states
        UpdateButtonStates();

        // Handle bot functionality
        if (enableBotMode && GameManager.Instance.gameState == GameState.START && GameManager.Instance.IsSinglePlayerMode)
        {
            HandleBotBehavior();
        }
    }
    
    public void OnRedButtonClicked()
    {
        if (redButtonOnCooldown) return;

        Debug.Log("Red Button Clicked!");

        RedEnemy redEnemy = FindObjectOfType<RedEnemy>();

        OnRedButtonClicked_(redEnemy.transform.position);

        if (!GameManager.Instance.IsSinglePlayerMode)
            GameManager.Instance.localPlayer.RPC_OnRedButtonClicked(redEnemy.transform.position);
    }

    public void OnRedButtonClicked_(Vector2 pos)
    {
        RedEnemy redEnemy = FindObjectOfType<RedEnemy>();

        redEnemy.StopHorizontalMovement();

        redEnemy.transform.position = pos;

        // Start cooldown
        StartCoroutine(RedButtonCooldown());
    }

    public void OnBlueButtonClicked()
    {
        if (blueButtonOnCooldown) return;

        Debug.Log("Blue Button Clicked!");

        BlueEnemy blueEnemy = FindObjectOfType<BlueEnemy>();

        OnBlueButtonClicked_(blueEnemy.transform.position);

        if (!GameManager.Instance.IsSinglePlayerMode)
            GameManager.Instance.localPlayer.RPC_OnBlueButtonClicked(blueEnemy.transform.position);
    }

    public void OnBlueButtonClicked_(Vector2 pos)
    {
        BlueEnemy blueEnemy = FindObjectOfType<BlueEnemy>();

        blueEnemy.StopHorizontalMovement();

        blueEnemy.transform.position = pos;

        // Start cooldown
        StartCoroutine(BlueButtonCooldown());
    }

    void StopRedEnemiesHorizontalMovement(Vector2 pos)
    {
        // Find all red enemies and stop their horizontal movement
        RedEnemy[] redEnemies = FindObjectsOfType<RedEnemy>();
        
        foreach (RedEnemy redEnemy in redEnemies)
        {
            if (redEnemy != null && !redEnemy.isDead)
            {       
                
                if(!GameManager.Instance.IsSinglePlayerMode)
                {
                    redEnemy.transform.position = pos;
                }

                redEnemy.StopHorizontalMovement();
            }
        }
    }
    
    void StopBlueEnemiesHorizontalMovement()
    {
        // Find all blue enemies and stop their horizontal movement
        BlueEnemy[] blueEnemies = FindObjectsOfType<BlueEnemy>();
        
        foreach (BlueEnemy blueEnemy in blueEnemies)
        {
            if (blueEnemy != null && !blueEnemy.isDead)
            {
                blueEnemy.StopHorizontalMovement();
            }
        }
    }
    
    System.Collections.IEnumerator RedButtonCooldown()
    {
        redButtonOnCooldown = true;
        UpdateButtonColors();
        
        yield return new WaitForSeconds(buttonCooldown);
        
        redButtonOnCooldown = false;
        UpdateButtonColors();
    }
    
    System.Collections.IEnumerator BlueButtonCooldown()
    {
        blueButtonOnCooldown = true;
        UpdateButtonColors();
        
        yield return new WaitForSeconds(buttonCooldown);
        
        blueButtonOnCooldown = false;
        UpdateButtonColors();
    }
    
    void UpdateButtonStates()
    {
        // Check if buttons should be enabled based on enemy count
        if (redButton != null)
        {
            int redEnemyCount = FindObjectsOfType<RedEnemy>().Length;
            redButton.interactable = !redButtonOnCooldown && redEnemyCount > 0;
        }
        
        if (blueButton != null)
        {
            int blueEnemyCount = FindObjectsOfType<BlueEnemy>().Length;
            blueButton.interactable = !blueButtonOnCooldown && blueEnemyCount > 0;
        }
    }
    
    void UpdateButtonColors()
    {
        if (redButton != null)
        {
            Image redButtonImage = redButton.GetComponent<Image>();
            if (redButtonImage != null)
            {
                //redButtonImage.color = redButtonOnCooldown ? inactiveButtonColor : activeButtonColor;
            }
        }
        
        if (blueButton != null)
        {
            Image blueButtonImage = blueButton.GetComponent<Image>();
            if (blueButtonImage != null)
            {
                //blueButtonImage.color = blueButtonOnCooldown ? inactiveButtonColor : activeButtonColor;
            }
        }
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
        // Check if button is available (not on cooldown and enemies exist)
        if (redButtonOnCooldown) return;
        
        int redEnemyCount = FindObjectsOfType<RedEnemy>().Length;
        if (redEnemyCount <= 0) return;
        
        //Debug.Log("Bot: Automatically clicking Red Button!");
        
        // Call the existing OnRedButtonClicked method
        OnRedButtonClicked();
    }
}

