using UnityEngine;
using UnityEngine.UI;

public class ButtonManager : MonoBehaviour
{
    [Header("Button References")]
    public Button redButton;
    public Button blueButton;
    
    [Header("Enemy Spawner Reference")]
    public EnemySpawner enemySpawner;
    
    [Header("Button Settings")]
    public float buttonCooldown = 0.5f;
    //public Color activeButtonColor = Color.green;
    //public Color inactiveButtonColor = Color.white;
    
    [Header("Bot Settings")]
    public bool enableBotMode = false;
    public float botClickInterval = 2f;
    public bool botClickRedButton = true;
    
    private bool redButtonOnCooldown = false;
    private bool blueButtonOnCooldown = false;
    private float botTimer = 0f;
    
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
        if (enableBotMode && GameManager.Instance.gameState == GameState.START)
        {
            HandleBotBehavior();
        }
    }
    
    public void OnRedButtonClicked()
    {
        if (redButtonOnCooldown) return;
        
        Debug.Log("Red Button Clicked!");
        
        // Stop all red enemies from moving horizontally and make them come down
        StopRedEnemiesHorizontalMovement();
        
        // Spawn new red enemy
        if (enemySpawner != null)
        {
            enemySpawner.SpawnRedEnemy();
        }
        
        // Start cooldown
        StartCoroutine(RedButtonCooldown());
    }
    
    public void OnBlueButtonClicked()
    {
        if (blueButtonOnCooldown) return;
        
        Debug.Log("Blue Button Clicked!");
        
        // Stop all blue enemies from moving horizontally and make them come down
        StopBlueEnemiesHorizontalMovement();
        
        // Spawn new blue enemy
        if (enemySpawner != null)
        {
            enemySpawner.SpawnBlueEnemy();
        }
        
        // Start cooldown
        StartCoroutine(BlueButtonCooldown());
    }
    
    void StopRedEnemiesHorizontalMovement()
    {
        // Find all red enemies and stop their horizontal movement
        RedEnemy[] redEnemies = FindObjectsOfType<RedEnemy>();
        
        foreach (RedEnemy redEnemy in redEnemies)
        {
            if (redEnemy != null && !redEnemy.isDead)
            {
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
    
    public void SetButtonCooldown(float cooldown)
    {
        buttonCooldown = cooldown;
    }
    
    public bool IsRedButtonOnCooldown()
    {
        return redButtonOnCooldown;
    }
    
    public bool IsBlueButtonOnCooldown()
    {
        return blueButtonOnCooldown;
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
        
        Debug.Log("Bot: Automatically clicking Red Button!");
        
        // Call the existing OnRedButtonClicked method
        OnRedButtonClicked();
    }
    
    /// <summary>
    /// Enable or disable bot mode
    /// </summary>
    public void SetBotMode(bool enabled)
    {
        enableBotMode = enabled;
        botTimer = 0f; // Reset timer when toggling bot mode
    }
    
    /// <summary>
    /// Set bot click interval
    /// </summary>
    public void SetBotClickInterval(float interval)
    {
        botClickInterval = Mathf.Max(0.1f, interval); // Minimum 0.1 seconds
    }
    
    /// <summary>
    /// Set whether bot should click red button
    /// </summary>
    public void SetBotClickRedButton(bool clickRed)
    {
        botClickRedButton = clickRed;
    }
    
    /// <summary>
    /// Get current bot mode status
    /// </summary>
    public bool IsBotModeEnabled()
    {
        return enableBotMode;
    }
    
    /// <summary>
    /// Manually trigger bot red button click (useful for testing)
    /// </summary>
    [ContextMenu("Bot Click Red Button")]
    public void ManualBotClickRedButton()
    {
        BotClickRedButton();
    }
}

