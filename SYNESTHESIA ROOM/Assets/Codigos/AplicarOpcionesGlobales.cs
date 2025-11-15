using UnityEngine;
using UnityEngine.UI;

public class AplicarOpcionesGlobales : MonoBehaviour
{
    void Start()
    {
        // ==== VOLUMEN ====
        if (PlayerPrefs.HasKey("VolumenAudio"))
        {
            float vol = PlayerPrefs.GetFloat("VolumenAudio", 0.5f);
            AudioListener.volume = vol;
        }

        // ==== BRILLO ====
        if (PlayerPrefs.HasKey("BrilloPantalla"))
        {
            float brillo = PlayerPrefs.GetFloat("BrilloPantalla", 0.5f);

            GameObject panel = GameObject.Find("PanelBrillo");
            if (panel != null)
            {
                Image img = panel.GetComponent<Image>();
                img.color = new Color(img.color.r, img.color.g, img.color.b, brillo);
            }
        }

        // ==== PANTALLA COMPLETA ====
        Screen.fullScreen = PlayerPrefs.GetInt("PantallaCompleta", 1) == 1;

        // ==== CALIDAD ====
        if (PlayerPrefs.HasKey("numeroDeCalidad"))
        {
            int calidad = PlayerPrefs.GetInt("numeroDeCalidad");
            QualitySettings.SetQualityLevel(calidad);
        }

        // ==== RESOLUCIÓN ====
        if (PlayerPrefs.HasKey("numeroResolucion"))
        {
            int indice = PlayerPrefs.GetInt("numeroResolucion");
            Resolution[] resoluciones = Screen.resolutions;

            if (indice >= 0 && indice < resoluciones.Length)
            {
                Resolution r = resoluciones[indice];
                Screen.SetResolution(r.width, r.height, Screen.fullScreen);
            }
        }
    }
}
