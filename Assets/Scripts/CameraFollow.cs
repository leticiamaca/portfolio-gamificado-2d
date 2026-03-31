using Unity.Cinemachine;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    //Como o player vem via DontDestroyOnLoad, ele não existe na cena ainda quando o Cinemachine inicializa, então a referência pode não pegar direto.
    //script que atribui o player ao Cinemachine em tempo de execução
    private void Start()
    {
        GameObject player = GameObject.FindWithTag("Player");

        if (player != null)
        {
            Debug.Log("Player encontrado: " + player.name);
            GetComponent<CinemachineCamera>().Target.TrackingTarget = player.transform;
        }
        else
        {
            Debug.LogError("Player NÃO encontrado! Verifique a Tag.");
        }
    }
}
