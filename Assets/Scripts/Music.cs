using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class NewMonoBehaviourScript : MonoBehaviour
{
    public AudioMixer mixer;
    public Slider musicaSlider;
    private const string volumenMusica = "volumenMusica ";
    public void Awake()
    {
        musicaSlider.onValueChanged.AddListener(ControlMusicVolumen);
    }

    private void Start()
    {
        Cargar();
    }
    private void ControlMusicVolumen( float valor)
    {
        mixer.SetFloat(volumenMusica, Mathf.Log10(valor) * 20);
        PlayerPrefs.SetFloat(volumenMusica, musicaSlider.value);
    }
    private void Cargar()
    {
        musicaSlider.value = PlayerPrefs.GetFloat(volumenMusica, 0.75f);
        ControlMusicVolumen(musicaSlider.value);
    }
}
