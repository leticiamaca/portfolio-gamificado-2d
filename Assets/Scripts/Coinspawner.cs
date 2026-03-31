using UnityEngine;

public class CoinSpawner : MonoBehaviour
{
    [Header("Configurações de Spawn")]
    [Tooltip("Prefab da moeda a ser spawnada")]
    public GameObject coinPrefab;

    [Tooltip("Quantidade de moedas no mapa ao mesmo tempo")]
    public int coinCount = 8;

    [Tooltip("Área de spawn: X define a largura, Y define a altura (centralizada no spawner)")]
    public Vector2 spawnArea = new Vector2(18f, 10f);

   
    // Inicialização: spawna todas as moedas ao começar
   
    void Start()
    {
        for (int i = 0; i < coinCount; i++)
        {
            SpawnCoin();
        }
    }

    // Spawna uma moeda em posição aleatória dentro da área
   
    public void SpawnCoin()
    {
        // Sorteia uma posição aleatória dentro do retângulo definido por spawnArea
        float x = Random.Range(-spawnArea.x / 2f, spawnArea.x / 2f);
        float y = Random.Range(-spawnArea.y / 2f, spawnArea.y / 2f);

        // Instancia o prefab na posição sorteada (z = 0 para jogos 2D)
        Vector3 spawnPos = new Vector3(x, y, 0f);
        Instantiate(coinPrefab, spawnPos, Quaternion.identity);
    }
}