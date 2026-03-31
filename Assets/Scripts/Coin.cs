using UnityEngine;

public class Coin : MonoBehaviour
{
    [Header("Configurações da Moeda")]
    [Tooltip("Tag do jogador para detectar a coleta")]
    public string playerTag = "Player";

    [Tooltip("Velocidade de rotação visual da moeda (graus por segundo)")]
    public float rotationSpeed = 90f;

    // Update: animação de rotação contínua
 
    void Update()
    {
        // Rotaciona a moeda no eixo Z para dar um efeito visual de "girando"
        transform.Rotate(0f, 0f, rotationSpeed * Time.deltaTime);
    }

  
    // OnTriggerEnter2D: detecta quando o jogador toca na moeda
    // Requisito: Collider2D com "Is Trigger" = true no prefab
   
    void OnTriggerEnter2D(Collider2D other)
    {
        // Verifica se quem entrou no trigger é o jogador
        if (other.CompareTag(playerTag))
        {
            // Avisa o CoinManager que esta moeda foi coletada
            CoinManager.Instance.CollectCoin();

            // Destrói o GameObject da moeda
            Destroy(gameObject);
        }
    }
}
