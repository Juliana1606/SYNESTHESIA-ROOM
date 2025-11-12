using UnityEngine;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class Control_Volumen : MonoBehaviour
{
    public Slider slider;
    public float sliderValor;
    public Image imagenMute;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        slider.value = PlayerPrefs.GetFloat("VolumenAudio", 0.5f);
        AudioListener.volume = slider.value;
        RevisarSiEstoyMute();


    }
    public void CambiarSlider(float valor)
    {

        sliderValor = valor;
        PlayerPrefs.SetFloat("VolumenAudio", sliderValor);
        AudioListener.volume = slider.value;
        RevisarSiEstoyMute();
    }

    public void RevisarSiEstoyMute()
    {
        if (slider.value == 0)
        {
            imagenMute.enabled = true;
        }
        else
        {
            imagenMute.enabled = false;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
