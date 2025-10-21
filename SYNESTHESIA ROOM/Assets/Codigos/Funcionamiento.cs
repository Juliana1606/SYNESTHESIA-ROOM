using UnityEngine;

public class Funcionamiento : MonoBehaviour
{
    [Header("Configuración de cámara")]
    public int ancho = 640;
    public int alto = 480;
    private WebCamTexture camara;

    [Header("Bancos de sonidos (A y B)")]
    public AudioSource[] sonidosBancoA = new AudioSource[7];
    public AudioSource[] sonidosBancoB = new AudioSource[7];
    private AudioSource[] sonidosActivos;
    [Header("Visuales de colores detectados ✨")]
    public GameObject prefabBrillo;
    public float duracionBrillo = 1.5f;

    [Header("Toggle de banco de audio")]
    public bool usarBancoB = false; // Controlado desde un toggle UI

    [Header("Detección de color (HSV)")]
    [Range(0f, 1f)] public float saturacionMin = 0.4f;
    [Range(0f, 1f)] public float brilloMin = 0.3f;
    public int pasoMuestreo = 6;
    public int umbralPixeles = 200;

    [Header("Volumen y mezcla")]
    public float suavizadoVolumen = 0.5f;
    [Range(0f, 1f)] public float intensidadStereo = 0.8f; // Controla qué tan abierto es el estéreo

    private Vector2[] rangosH = new Vector2[7];
    private Texture2D textura;
    private Color32[] pixeles;
    private bool juegoPausado = false; // ← Nueva variable para controlar el estado

    // ----------------------------------------------------
    void Start()
    {
        // Iniciar cámara
        camara = new WebCamTexture(ancho, alto);
        Renderer render = GetComponent<Renderer>();
        if (render != null) render.material.mainTexture = camara;
        camara.Play();

        // Definir rangos HSV de los 7 colores del arcoíris
        rangosH[0] = new Vector2(0.97f, 0.03f); // Rojo
        rangosH[1] = new Vector2(0.03f, 0.08f); // Naranja
        rangosH[2] = new Vector2(0.08f, 0.16f); // Amarillo
        rangosH[3] = new Vector2(0.16f, 0.45f); // Verde
        rangosH[4] = new Vector2(0.55f, 0.70f); // Azul
        rangosH[5] = new Vector2(0.70f, 0.78f); // Índigo
        rangosH[6] = new Vector2(0.78f, 0.92f); // Violeta

        // Configurar ambos bancos
        ConfigurarBanco(sonidosBancoA);
        ConfigurarBanco(sonidosBancoB);

        // Iniciar con banco A
        sonidosActivos = sonidosBancoA;

        Debug.Log("🎬 Sinestesia Room iniciado (reproducción simultánea + control estéreo)");
    }

    // ----------------------------------------------------
    void ConfigurarBanco(AudioSource[] banco)
    {
        foreach (var fuente in banco)
        {
            if (fuente != null)
            {
                fuente.spatialBlend = 0f; // Modo 2D
                fuente.loop = true;
                fuente.volume = 0f;
                fuente.pitch = 1f;
                fuente.panStereo = 0f;
                fuente.Play(); // Todos los sonidos en loop
            }
        }
    }

    // ----------------------------------------------------
    void Update()
    {
                // 🔸 Pausar / Reanudar con la tecla ESPACIO
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (!juegoPausado)
                PausarJuego();
            else
                ReanudarJuego();
        }

        // 🔸 Salir del juego con ESC
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Debug.Log("🚪 Saliendo del juego...");
            Application.Quit();
        }

        // Si el juego está pausado, no ejecutar el resto del código
        if (juegoPausado) return;



        if (camara.width <= 16) return;

        // Seleccionar banco activo según el toggle
        sonidosActivos = usarBancoB ? sonidosBancoB : sonidosBancoA;

        // Capturar imagen actual
        if (textura == null)
            textura = new Texture2D(camara.width, camara.height, TextureFormat.RGBA32, false);

        textura.SetPixels32(camara.GetPixels32());
        textura.Apply(false);
        pixeles = textura.GetPixels32();

        int[] conteoColor = new int[7];

        // 🔹 Muestreo rápido (zona central)
        for (int y = alto / 4; y < alto * 3 / 4; y += pasoMuestreo)
        {
            for (int x = ancho / 4; x < ancho * 3 / 4; x += pasoMuestreo)
            {
                Color color = pixeles[y * ancho + x];
                Color.RGBToHSV(color, out float h, out float s, out float v);

                if (s < saturacionMin || v < brilloMin)
                    continue;

                for (int i = 0; i < 7; i++)
                {
                    bool dentroRango = (rangosH[i].x < rangosH[i].y)
                        ? (h >= rangosH[i].x && h <= rangosH[i].y)
                        : (h >= rangosH[i].x || h <= rangosH[i].y);

                    if (dentroRango)
                    {
                        conteoColor[i]++;
                        break;
                    }
                }
            }
        }

        // 🔊 Ajustar volúmenes simultáneamente
        for (int i = 0; i < 7; i++)
        {
            if (sonidosActivos[i] == null) continue;

            float intensidad = (float)conteoColor[i] / umbralPixeles;
            intensidad = Mathf.Clamp01(intensidad);

            float volumenObjetivo = intensidad;
            float nuevoVolumen = Mathf.Lerp(sonidosActivos[i].volume, volumenObjetivo, suavizadoVolumen);
            sonidosActivos[i].volume = nuevoVolumen;

            // 🎧 Simulación de paneo estéreo con control de apertura
            float pan = Mathf.Sin(i * Mathf.PI / 3.5f) * intensidadStereo;
            sonidosActivos[i].panStereo = pan;


            // Mostrar en consola cuando un color tiene actividad
            if (nuevoVolumen > 0.05f)
            {
                Debug.Log($"▶ Reproduciendo el color {i} - Clip: {sonidosActivos[i].clip.name}");

                   // ✨ Crear visual del color detectado en su posición promedio
                if (prefabBrillo != null)
                {
                    // Calcular posición promedio de píxeles de este color
                    Vector2 sumaPos = Vector2.zero;
                    int conteo = 0;

                    for (int y = alto / 4; y < alto * 3 / 4; y += pasoMuestreo)
                    {
                        for (int x = ancho / 4; x < ancho * 3 / 4; x += pasoMuestreo)
                        {
                            Color color = pixeles[y * ancho + x];
                            Color.RGBToHSV(color, out float h, out float s, out float v);
                            if (s < saturacionMin || v < brilloMin) continue;

                            bool dentroRango = (rangosH[i].x < rangosH[i].y)
                                ? (h >= rangosH[i].x && h <= rangosH[i].y)
                                : (h >= rangosH[i].x || h <= rangosH[i].y);

                            if (dentroRango)
                            {
                                sumaPos += new Vector2(x, y);
                                conteo++;
                            }
                        }
                    }

                    if (conteo > 0)
                    {
                        Vector2 promedio = sumaPos / conteo;

                        // Convertir coordenadas de píxeles (imagen) a coordenadas normalizadas (-1 a 1)
                        float xNorm = (promedio.x / ancho - 0.5f) * 2f;
                        float yNorm = (promedio.y / alto - 0.5f) * 2f;

                        // Crear posición frente a la cámara según la zona detectada
                        Vector3 posCam = new Vector3(xNorm * 2f, yNorm * 1.5f, 2f);
                        Vector3 posMundo = Camera.main.transform.TransformPoint(posCam);

                        // Instanciar el brillo
                        GameObject brillo = Instantiate(prefabBrillo, posMundo, Quaternion.identity);

                        // Cambiar color del brillo
                        Renderer rend = brillo.GetComponent<Renderer>();
                        if (rend != null)
                        {
                            Color colorBrillo = Color.HSVToRGB((rangosH[i].x + rangosH[i].y) / 2f, 1f, 1f);
                            rend.material.SetColor("_EmissionColor", colorBrillo * 2f);
                            rend.material.color = colorBrillo;
                        }

                        // Destruir el brillo tras unos segundos
                        Destroy(brillo, duracionBrillo);
                    }
                }

                
            }

        }
    }

    // ----------------------------------------------------
        public void CambiarBanco()
    {
        // Alternar entre Banco A y Banco B
        usarBancoB = !usarBancoB;

        // Silenciar ambos bancos para evitar superposición
        foreach (var fuente in sonidosBancoA)
            if (fuente != null)
                fuente.volume = 0f;

        foreach (var fuente in sonidosBancoB)
            if (fuente != null)
                fuente.volume = 0f;

        // Cambiar el banco activo
        sonidosActivos = usarBancoB ? sonidosBancoB : sonidosBancoA;

        // Asegurarse de que todos los sonidos del banco activo estén reproduciéndose
        foreach (var fuente in sonidosActivos)
        {
            if (fuente != null && !fuente.isPlaying)
                fuente.Play();
        }

        Debug.Log($" Banco de audio cambiado a: {(usarBancoB ? "B" : "A")}");
    }
    void PausarJuego()
    {
        juegoPausado = true;
        Time.timeScale = 0f;

        foreach (var fuente in sonidosBancoA)
            if (fuente != null) fuente.Pause();

        foreach (var fuente in sonidosBancoB)
            if (fuente != null) fuente.Pause();

        Debug.Log("⏸ Juego pausado.");
    }

    void ReanudarJuego()
    {
        juegoPausado = false;
        Time.timeScale = 1f;

        foreach (var fuente in sonidosBancoA)
            if (fuente != null) fuente.UnPause();

        foreach (var fuente in sonidosBancoB)
            if (fuente != null) fuente.UnPause();

        Debug.Log("▶ Juego reanudado.");
    }



    // ----------------------------------------------------
    void OnDestroy()
    {
        if (textura != null)
            Destroy(textura);

        if (camara != null && camara.isPlaying)
            camara.Stop();
    }
}
