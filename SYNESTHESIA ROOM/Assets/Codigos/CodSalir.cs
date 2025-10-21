using UnityEngine;

public class BotonSalir : MonoBehaviour
{
    public void SalirJuego()
    {
        Debug.Log("Saliendo del juego..."); // Solo para verificar en el editor
        Application.Quit();
    }
}
