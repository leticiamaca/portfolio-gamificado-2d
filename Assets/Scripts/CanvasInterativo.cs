using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CanvasInterativo : MonoBehaviour
{
    [SerializeField] GameObject canvasInterativo;
    [SerializeField] GameObject canvasSegundaParte;
    [SerializeField] GameObject TelaFinalCanvas;
    [SerializeField] Collider2D Collider2D;
    public bool playerColidiu { get; private set; } = false; // ← variável adicionada

    private void Start()
    {
        canvasInterativo.SetActive(false);
        canvasSegundaParte.SetActive(false);
        TelaFinalCanvas.SetActive(false);

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player")) return;
        playerColidiu = true;
        canvasInterativo.SetActive(true);
    }
    public void abrirSegundaParte()
    {
        canvasSegundaParte.SetActive(true);
    }

    public void abrirTelaFinalCanvas()
    {
        TelaFinalCanvas.SetActive(true);
    }
    public void fecharCanvas()
    {
        canvasInterativo.gameObject.SetActive(false);
    }
    

    public void WinnerScene()
    {
        FadeSystem.Instance.LoadScene("TelaFinal");
    }

    public void Jogo()
    {
        FadeSystem.Instance.LoadScene("Jogo");
    }

    public void Menu()
    {
        FadeSystem.Instance.LoadScene("TelaPlay");
    }
}
