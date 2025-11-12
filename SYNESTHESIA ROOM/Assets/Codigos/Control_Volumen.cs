using UnityEngine;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class Control_Volumen : MonoBehaviour
{
    public Slider slider;
    public float sliderValor;
    public Image imagenMute;

    public Slider sliderBrillo;
    public float sliderBrilloValor;
    public Image panelBrillo;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        slider.value = PlayerPrefs.GetFloat("VolumenAudio", 0.5f);
        AudioListener.volume = slider.value;
        RevisarSiEstoyMute();

        sliderBrillo.value = PlayerPrefs.GetFloat("BrilloPantalla", 0.5f);
        panelBrillo.color = new Color(panelBrillo.color.r,panelBrillo.color.g,panelBrillo.color.b,sliderBrillo.value);


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
    public void CambiarBrillo(float valor)
    {
        sliderBrilloValor = valor;
        PlayerPrefs.SetFloat("BrilloPantalla", sliderBrilloValor);
        panelBrillo.color = new Color(panelBrillo.color.r, panelBrillo.color.g, panelBrillo.color.b, sliderBrillo.value);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
