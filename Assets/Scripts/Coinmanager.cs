using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class CoinManager : MonoBehaviour
{
    public static CoinManager Instance { get; private set; }

    [Header("UI")]
    [Tooltip("Texto que exibe a contagem de moedas na tela")]
    public TMP_Text coinText;

    [Header("Configurações")]
    [Tooltip("Formato do texto. {0} = moedas coletadas")]
    public string textFormat = "Moedas: {0}";

    private int coinCount = 0;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject); // <-- mantém entre cenas
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void Start()
    {
        UpdateUI();
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Busca o TMP_Text da nova cena e atualiza com o valor salvo
        coinText = Object.FindFirstObjectByType<TMP_Text>();
        UpdateUI();
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    public void CollectCoin()
    {
        coinCount++;
        UpdateUI();
        Debug.Log($"Moeda coletada! Total: {coinCount}");
    }

    private void UpdateUI()
    {
        if (coinText != null)
            coinText.text = string.Format(textFormat, coinCount);
        else
            Debug.LogWarning("CoinManager: coinText não está atribuído!");
    }

    public int GetCoinCount()
    {
        return coinCount;
    }
}