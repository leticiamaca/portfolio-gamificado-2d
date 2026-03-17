using UnityEngine;
using UnityEngine.SceneManagement;
public class MenuPause : MonoBehaviour
{
    //Referência do canvas Pause 
    [Header("Canvas")]
    [SerializeField] private GameObject canvasPause;
    [SerializeField] private GameObject canvasOpcoes;


   
    void Start()
    {
        //Canvas Pause e opcoes vai começar desativado
        canvasPause.SetActive(false);
        canvasOpcoes.SetActive(false);
    }

    //Botão de recomeçar o jogo
    public void BotaoRecomecar()
    {
        Time.timeScale = 1f; // Restaura antes de trocar de cena
        SceneManager.LoadScene("Jogo");
    }

    public void Menu()
    {
        Debug.Log("Botão clicado!");
        Time.timeScale = 1f;
        SceneManager.LoadScene("TelaPlay");
    }

    //Aqui seta o canvas opções como true, para ele aparecer
    public void AbrirOpcoes()
    {
        canvasOpcoes.SetActive(true);
    }

    public void Continuar()
    {
        Time.timeScale = 1f;
        canvasPause.SetActive(false);
    }

    public void Fechar()
    {
        canvasOpcoes.SetActive(false);
    }
  
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.H))
        {
            bool pausado = !canvasPause.activeSelf;
            canvasPause.SetActive(pausado);
            Time.timeScale = pausado ? 0f : 1f;
        }
    }
    
}
