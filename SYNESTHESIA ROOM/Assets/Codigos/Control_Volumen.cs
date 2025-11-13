using UnityEngine;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class Control_Volumen : MonoBehaviour
{
    public Slider slider;
    public float sliderValor;
    public Image imagenMute;

    public Slider sliderBrillo;
    public float sliderBrilloValor;
    public Image panelBrillo;

    public Toggle togglePantallaCompleta;

    public TMP_Dropdown dropdownResolucion;
    public int calidad;
    public TMP_Dropdown dropdownResoluciones;
    Resolution[] resoluciones;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        slider.value = PlayerPrefs.GetFloat("VolumenAudio", 0.5f);
        AudioListener.volume = slider.value;
        RevisarSiEstoyMute();

        sliderBrillo.value = PlayerPrefs.GetFloat("BrilloPantalla", 0.5f);
        panelBrillo.color = new Color(panelBrillo.color.r,panelBrillo.color.g,panelBrillo.color.b,sliderBrillo.value);

        if (Screen.fullScreen)
        {
            togglePantallaCompleta.isOn = true;
        }
        else
        {
            togglePantallaCompleta.isOn = false;
        }

        calidad=PlayerPrefs.GetInt("numeroDeCalidad",3);
        dropdownResoluciones.value=calidad;
        AjustarCalidad();

        if (Screen.fullScreen)
        {
            togglePantallaCompleta.isOn = true;
        }
        else
        {
            togglePantallaCompleta.isOn = false;
        }

        RevisarResolucion();


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
    public void ActivarPantallaCompleta(bool pantallaCompleta)
    {
        Screen.fullScreen = pantallaCompleta;
    }
    public void AjustarCalidad()
    {
        QualitySettings.SetQualityLevel(dropdownResoluciones.value);
        PlayerPrefs.SetInt("numeroDeCalidad", dropdownResoluciones.value);
        calidad=dropdownResoluciones.value;
    }
    public void RevisarResolucion()
    {
        resoluciones= Screen.resolutions;
        dropdownResoluciones.ClearOptions();
        List<string> opciones=new List<string>();
        int resolucionActual=0;

        for (int i=0; i<resoluciones.Length; i++)
        {
            string opcion=resoluciones[i].width+" x "+resoluciones[i].height;
            opciones.Add(opcion);

            if (Screen.fullScreen && resoluciones[i].width ==Screen.currentResolution.width && resoluciones[i].height == Screen.currentResolution.height)
            {
                resolucionActual=i;
            }
        }
        dropdownResoluciones.AddOptions(opciones);
        dropdownResoluciones.value= resolucionActual;
        dropdownResoluciones.RefreshShownValue();

    }

    public void CambiarResolucion(int indiceResolucion)
    {
        Resolution resolucion=resoluciones[indiceResolucion];
        Screen.SetResolution(resolucion.width, resolucion.height, Screen.fullScreen);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
