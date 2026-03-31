using UnityEngine;
using UnityEngine.SceneManagement;

public class ProximaFase : MonoBehaviour
{
    [Header("Configuração da Cena")]
    [SerializeField] private string nomeDaProximaFase;
    [SerializeField] private int moedasNecessarias;

    [Header("Balão de Aviso")]
    [SerializeField] private GameObject balaoAviso;
    [SerializeField] private TMPro.TextMeshProUGUI textoBalao;
    [SerializeField] private float hideDelay = 2f;

    private Coroutine _esconderBalaoCoroutine;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player")) return;

        int moedasAtuais = CoinManager.Instance.GetCoinCount();

        if (moedasAtuais >= moedasNecessarias)
        {
            IrProximaCena();
        }
        else
        {
            MostrarAvisoMoedas(moedasNecessarias - moedasAtuais);
        }
    }

    private void IrProximaCena()
    {
        FadeSystem.Instance.LoadScene(nomeDaProximaFase);
    }

    private void MostrarAvisoMoedas(int moedasRestantes)
    {
        if (balaoAviso == null) return;

        if (textoBalao != null)
            textoBalao.text = $"Faltam {moedasRestantes} moeda(s)!";

        balaoAviso.SetActive(true);

        if (_esconderBalaoCoroutine != null)
            StopCoroutine(_esconderBalaoCoroutine);

        _esconderBalaoCoroutine = StartCoroutine(EsconderBalaoAposDelay());
    }

    private System.Collections.IEnumerator EsconderBalaoAposDelay()
    {
        yield return new WaitForSeconds(hideDelay);
        if (balaoAviso != null)
            balaoAviso.SetActive(false);
    }
}