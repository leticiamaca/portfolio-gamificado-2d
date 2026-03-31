using UnityEngine;

public class PlayerPersistence : MonoBehaviour
{
    //Esse código é responsável por fazer o player andar entre as cenas: CasaSobre, CasaTecnologias e CasaFormação
   
    private static PlayerPersistence instancia;

    private void Awake()
    {
        instancia = this;
        DontDestroyOnLoad(gameObject);
    }
}
