using UnityEngine;
using UnityEngine.UI;

public class CuartoAni : MonoBehaviour
{
    [Header("Imagen a animar (UI)")]
    public RectTransform BotonCambiar;
    

    


    [Header("Configuración de animación")]
    [Range(0.9f, 1.5f)]
    public float escalaMin = 0.95f;

    [Range(1f, 2f)]
    public float escalaMax = 1.05f;

    [Range(0.1f, 5f)]
    public float velocidad = 1.5f;

    private Vector3 escalaInicial;

    void Start()
    {
        if (BotonCambiar == null)
        {
            Debug.LogWarning("⚠ No asignaste ninguna imagen al script RespiracionUI.");
            enabled = false;
            return;
        }
       


        escalaInicial = BotonCambiar.localScale;
       
        
    }

    void Update()
    {
        float t = (Mathf.Sin(Time.time * velocidad) + 1f) / 2f; // 0–1 suave
        float escala = Mathf.Lerp(escalaMin, escalaMax, t);

        BotonCambiar.localScale = escalaInicial * escala;
        
    }
}
