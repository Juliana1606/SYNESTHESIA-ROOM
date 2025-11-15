using UnityEngine;

public class VisualFXManager : MonoBehaviour
{
    [Header("Prefab del visual")]
    public GameObject visualPrefab;

    [Header("Opciones de Spawn")]
    public int cantidad = 5;
    public Vector3 areaDeSpawn = new Vector3(10, 5, 10);

    [Header("Movimiento")]
    public float velocidadMovimiento = 2f;

    [Header("Colores arcoiris")]
    public float velocidadColor = 1f;

    void Start()
    {
        // Crear todos los visuales
        for (int i = 0; i < cantidad; i++)
        {
            CrearVisual();
        }
    }

    void CrearVisual()
    {
        // Posición inicial aleatoria
        Vector3 pos = new Vector3(
            Random.Range(-areaDeSpawn.x, areaDeSpawn.x),
            Random.Range(-areaDeSpawn.y, areaDeSpawn.y),
            Random.Range(-areaDeSpawn.z, areaDeSpawn.z)
        );

        // Instanciar
        GameObject instance = Instantiate(visualPrefab, pos, Quaternion.identity);

        // Añadir el script de comportamiento en runtime
        instance.AddComponent<VisualBehaviour>();
    }
}



public class VisualBehaviour : MonoBehaviour
{
    Renderer rend;
    Vector3 direccion;

    float velocidad = 1.5f;     // movimiento
    float colorSpeed = 1.5f;    // velocidad del arcoiris

    void Start()
    {
        // Tomar renderer (aunque esté en hijos)
        rend = GetComponentInChildren<Renderer>();

        // Dirección aleatoria
        direccion = Random.insideUnitSphere.normalized;
    }

    void Update()
    {
        // Movimiento suave
        transform.position += direccion * velocidad * Time.deltaTime;

        // Cambiar dirección de vez en cuando
        if (Random.value < 0.01f)
            direccion = Random.insideUnitSphere.normalized;

        // Animación de colores arcoiris
        if (rend != null)
        {
            float t = (Mathf.Sin(Time.time * colorSpeed) + 1f) / 2f;
            rend.material.color = Color.HSVToRGB(t, 1, 1);
        }
    }
}
