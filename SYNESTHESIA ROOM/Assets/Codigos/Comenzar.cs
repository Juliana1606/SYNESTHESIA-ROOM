using UnityEngine;

using UnityEngine;
using UnityEngine.SceneManagement;

public class CambiarEscena : MonoBehaviour
{
    // Este método lo asignas al botón "Comenzar"
    public void IrAEscena(string nombreEscena)
    {
        Debug.Log("Cargando escena: " + nombreEscena);
        SceneManager.LoadScene(nombreEscena);
    }
}
