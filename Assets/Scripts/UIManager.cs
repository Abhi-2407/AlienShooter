using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
    [Header("UI References")]
    public TextMeshProUGUI player1ScoreTxt;
    public TextMeshProUGUI player2ScoreTxt;
    
    [Header("Game Over UI")]
    public GameObject GameoverScreen;
    public GameObject YouWin;
    public GameObject YouLoss;
    public GameObject Tie;

    void Start()
    {
        // Hide panels
        if (GameoverScreen != null)
            GameoverScreen.SetActive(false);
    }  
}

