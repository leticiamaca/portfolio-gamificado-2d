using UnityEngine;
using UnityEngine.UI;

public class SoundControler : MonoBehaviour
{
    private bool isPlaying = true;
    [SerializeField]private AudioSource Musica;

    [SerializeField]private Sprite SomLigado;
    [SerializeField]private Sprite SomDesligado;

    [SerializeField]private Image muteImagemSom;


    [SerializeField] private Image VolumeImage;
    [SerializeField] private Sprite VolumeLigado;
    [SerializeField] private Sprite VolumeDesligado;

    public void LigarDesligarSom()
    {
        isPlaying = !isPlaying;
       
        Musica.enabled = isPlaying;

        if (isPlaying)
        {
            muteImagemSom.sprite = SomLigado;
        }
        else
        {
            muteImagemSom.sprite = SomDesligado;
        }
  
    }

    public void Volume(float value)
    {
        Musica.volume = value;

        if(value <= 0)
        {
            VolumeImage.sprite = VolumeDesligado;
        }
        else
        {
            VolumeImage.sprite = VolumeLigado;
        }
    }
}
