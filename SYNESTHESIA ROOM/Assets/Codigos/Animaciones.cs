using UnityEngine;
using UnityEngine.UI;

public class RespiracionUI : MonoBehaviour
{
    [Header("Imagen a animar (UI)")]
    public RectTransform imagenObjetivo;
    public RectTransform imagenObjetivo2;
    public RectTransform imagenObjetivo3;
    public RectTransform imagenObjetivo4;
    public RectTransform imagenObjetivo5;

    public RectTransform imagenObjetivo6;
    public RectTransform imagenObjetivo7;
    public RectTransform imagenObjetivo8;

    


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
        if (imagenObjetivo == null)
        {
            Debug.LogWarning("⚠ No asignaste ninguna imagen al script RespiracionUI.");
            enabled = false;
            return;
        }
        if (imagenObjetivo2 == null)
        {
            Debug.LogWarning("⚠ No asignaste ninguna imagen al script RespiracionUI.");
            enabled = false;
            return;
        }
        if (imagenObjetivo3 == null)
        {
            Debug.LogWarning("⚠ No asignaste ninguna imagen al script RespiracionUI.");
            enabled = false;
            return;
        }
        if (imagenObjetivo4 == null)
        {
            Debug.LogWarning("⚠ No asignaste ninguna imagen al script RespiracionUI.");
            enabled = false;
            return;
        }
        if (imagenObjetivo5 == null)
        {
            Debug.LogWarning("⚠ No asignaste ninguna imagen al script RespiracionUI.");
            enabled = false;
            return;
        }
        if (imagenObjetivo6 == null)
        {
            Debug.LogWarning("⚠ No asignaste ninguna imagen al script RespiracionUI.");
            enabled = false;
            return;
        }
        if (imagenObjetivo7 == null)
        {
            Debug.LogWarning("⚠ No asignaste ninguna imagen al script RespiracionUI.");
            enabled = false;
            return;
        }
        if (imagenObjetivo8 == null)
        {
            Debug.LogWarning("⚠ No asignaste ninguna imagen al script RespiracionUI.");
            enabled = false;
            return;
        }


        escalaInicial = imagenObjetivo.localScale;
        escalaInicial = imagenObjetivo2.localScale;
        escalaInicial = imagenObjetivo3.localScale;
        escalaInicial = imagenObjetivo4.localScale;
        escalaInicial = imagenObjetivo5.localScale;
        escalaInicial = imagenObjetivo6.localScale;
        escalaInicial = imagenObjetivo7.localScale;
        escalaInicial = imagenObjetivo8.localScale;
        
        
    }

    void Update()
    {
        float t = (Mathf.Sin(Time.time * velocidad) + 1f) / 2f; // 0–1 suave
        float escala = Mathf.Lerp(escalaMin, escalaMax, t);

        imagenObjetivo.localScale = escalaInicial * escala;
        imagenObjetivo2.localScale = escalaInicial * escala;
        imagenObjetivo3.localScale = escalaInicial * escala;
        imagenObjetivo4.localScale = escalaInicial * escala;    
        imagenObjetivo5.localScale = escalaInicial * escala;
        imagenObjetivo6.localScale = escalaInicial * escala;
        imagenObjetivo7.localScale = escalaInicial * escala;
        imagenObjetivo8.localScale = escalaInicial * escala;
    }
}
