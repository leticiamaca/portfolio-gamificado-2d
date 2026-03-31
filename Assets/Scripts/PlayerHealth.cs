using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    [Header("Configurações de Vida")]
    // Vida máxima do player
    public float maxHealth = 100f;

    // Vida atual (começa igual à máxima)
    private float currentHealth;

    [Header("UI")]
    // Arraste o Slider da sua UI aqui no Inspector
    public Slider healthSlider;

    private void Start()
    {
        // Inicializa a vida atual com o valor máximo
        currentHealth = maxHealth;

        // Configura o Slider para refletir a vida inicial
        if (healthSlider != null)
        {
            healthSlider.maxValue = maxHealth;
            healthSlider.value = currentHealth;
        }
    }

    // -------------------------------------------------------
    // TakeDamage: chame este método para causar dano ao player
    // Exemplo de uso em outro script:
    //   GetComponent<PlayerHealth>().TakeDamage(20f);
    // -------------------------------------------------------
    public void TakeDamage(float amount)
    {
        // Reduz a vida, garantindo que não passe de 0
        currentHealth = Mathf.Max(currentHealth - amount, 0f);

        // Atualiza o Slider na tela
        UpdateUI();

        Debug.Log($"Player tomou {amount} de dano. Vida atual: {currentHealth}");

        // Verifica se o player morreu
        if (currentHealth <= 0f)
        {
            Die();
        }
    }

    // -------------------------------------------------------
    // Heal: chame este método para curar o player
    // Exemplo de uso:
    //   GetComponent<PlayerHealth>().Heal(30f);
    // -------------------------------------------------------
    public void Heal(float amount)
    {
        // Aumenta a vida, garantindo que não passe do máximo
        currentHealth = Mathf.Min(currentHealth + amount, maxHealth);

        // Atualiza o Slider na tela
        UpdateUI();

        Debug.Log($"Player curou {amount}. Vida atual: {currentHealth}");
    }

    // -------------------------------------------------------
    // UpdateUI: sincroniza o Slider com a vida atual
    // -------------------------------------------------------
    private void UpdateUI()
    {
        if (healthSlider != null)
        {
            healthSlider.value = currentHealth;
        }
        else
        {
            Debug.LogWarning("PlayerHealth: healthSlider não atribuído no Inspector!");
        }
    }

    // -------------------------------------------------------
    // Die: chamado quando a vida chega a zero
    // -------------------------------------------------------
    private void Die()
    {
        Debug.Log("Player morreu!");

        // Aqui você pode: tocar animação de morte, recarregar a cena, etc.
        // Exemplo para recarregar a cena atual:
        // UnityEngine.SceneManagement.SceneManager.LoadScene(
        //     UnityEngine.SceneManagement.SceneManager.GetActiveScene().name
        // );
    }
}
