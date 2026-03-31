using UnityEngine;
using UnityEngine.SceneManagement; //esse carinha aqui que faz o gerenciamento das cenas
public class TelaPrincipal : MonoBehaviour
{
    //Referências dos Canvas
    [Header("Canvas")]
    [SerializeField] private GameObject canvasOpcoes;
    [SerializeField] private GameObject canvasCreditos;

    //Aqui no start os canvas devem começar desativados, pois o start é o método de começar
    private void Start()
    {
        canvasCreditos.SetActive(false); //Aqui o canvas opcoes começando falso
        canvasCreditos.SetActive(false); // Aqui a mesma coisa com os créditos
    }

    public void BotaoPlay()
    {
        FadeSystem.Instance.LoadScene("LoadScreen");
    }

    // Habilitando o canvas opcoes
    public void AbrirOpcoes()
    {
        canvasOpcoes.SetActive(true);
    }
    public void FecharOpcoes()
    {
        canvasOpcoes.SetActive(false);
    }


    //Mesma coisa para o canva créditos
    public void AbrirCreditos()
    {
        canvasCreditos.SetActive(true);
    }

    public void FecharCreditos()
    {
        canvasCreditos.SetActive(false);
    }

    //botao sair
    public void Sair()
    {
        Application.Quit();
    }

}
