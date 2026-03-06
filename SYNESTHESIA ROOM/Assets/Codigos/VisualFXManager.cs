using UnityEngine;

public class VisualFXManager : MonoBehaviour
{
    [Header("Prefab del visual")]
    public GameObject prefabVisual;

    [Header("Configuración de spawn")]
    public float distanciaFrenteCamara = 3f;
    public float radioSpawn = 2f;
    public float intervaloSpawn = 0.5f;

    [Header("Movimiento del visual")]
    public float velocidadMovimiento = 1f;
    public float amplitudMovimiento = 0.5f;

    [Header("Duración")]
    public float duracionVisual = 3f;

    private float tiempoUltimoSpawn;

    void Update()
    {
        // Instanciar visual por intervalo
        if (Time.time > tiempoUltimoSpawn + intervaloSpawn)
        {
            GenerarVisual();
            tiempoUltimoSpawn = Time.time;
        }
    }

    void GenerarVisual()
    {
        if (prefabVisual == null) return;

        // POSICIÓN EN 3D FRENTE A LA CÁMARA
        Vector3 posBase = Camera.main.transform.position + Camera.main.transform.forward * distanciaFrenteCamara;

        // Variación aleatoria para que aparezca por toda la pantalla
        Vector3 offset = new Vector3(
            Random.Range(-radioSpawn, radioSpawn),
            Random.Range(-radioSpawn, radioSpawn),
            0f
        );

        Vector3 posFinal = posBase + Camera.main.transform.TransformVector(offset);

        // CREAR VISUAL
        GameObject v = Instantiate(prefabVisual, posFinal, Quaternion.identity);

        // AÑADIR COMPONENTE DINÁMICO
        v.AddComponent<RainbowMover>();
        RainbowMover mover = v.GetComponent<RainbowMover>();

        mover.amplitud = amplitudMovimiento;
        mover.velocidad = velocidadMovimiento;
        mover.duracion = duracionVisual;
    }
}


// =========================================================
// MOVIMIENTO + CAMBIO DE COLOR EN ARCOÍRIS
// =========================================================

public class RainbowMover : MonoBehaviour
{
    public float velocidad = 1f;
    public float amplitud = 0.5f;
    public float duracion = 2f;

    private float tiempoVida = 0f;
    private Renderer rend;
    private Vector3 posicionInicial;

    void Start()
    {
        posicionInicial = transform.position;
        rend = GetComponent<Renderer>();
    }

    void Update()
    {
        tiempoVida += Time.deltaTime;

        // === MOVIMIENTO ===
        float m = Mathf.Sin(Time.time * velocidad) * amplitud;
        transform.position = posicionInicial + new Vector3(
            m,
            m,
            Mathf.Sin(Time.time * velocidad * 0.7f) * amplitud
        );

        // === CAMBIO DE COLOR DE ARCOÍRIS ===
        // === CAMBIO DE COLOR DE ARCOIRIS PARA PARTICLE SYSTEM ===
        ParticleSystem ps = GetComponent<ParticleSystem>();
        if (ps != null)
        {
            var main = ps.main;
            float h = Mathf.Repeat(Time.time * 0.3f, 1f);
            Color c = Color.HSVToRGB(h, 1f, 1f);

            main.startColor = c;
        }
        else if (rend != null)
        {
            // fallback si el prefab NO es particle system
            float h = Mathf.Repeat(Time.time * 0.3f, 1f);
            Color c = Color.HSVToRGB(h, 1f, 1f);

            rend.material.color = c;

            if (rend.material.HasProperty("_EmissionColor"))
                rend.material.SetColor("_EmissionColor", c * 2f);
        }

        // === DESTRUIR DESPUÉS DE SU DURACIÓN ===
        if (tiempoVida >= duracion)
        {
            Destroy(gameObject);
        }
    }
}
