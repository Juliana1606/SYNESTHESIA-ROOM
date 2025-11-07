using UnityEngine;
using System.Collections;
public class Funcionamiento : MonoBehaviour
{
    [Header("Configuración de cámara")]
    public int ancho = 640;
    public int alto = 480;
    private WebCamTexture camara;

    [Header("Visuales")]
    public GameObject[] prefabsVisuales;
    public float intervaloCambioVisual = 60f;
    
    [Header("Duración del visual")]
    public float duracionVisual = 1.5f;
    private GameObject prefabActual;
    private float tiempoUltimoCambio = 0f;

    [Header("Rango de profundidad para visuales")]
    [Range(1f, 20f)] public float rangoProfundidad = 10f;
    
    [Header("Efecto visual 3D")]
    public bool modoCurvado = true;

    [Header("Bancos de sonidos (A y B)")]
    public AudioSource[] sonidosBancoA = new AudioSource[7];
    public AudioSource[] sonidosBancoB = new AudioSource[7];
    private AudioSource[] sonidosActivos;

    [Header("Transición de audio")]
    [Range(0.1f, 2f)] public float velocidadTransicion = 0.2f;

    [Header("Toggle de banco de audio")]
    public bool usarBancoB = false; // Controlado desde un toggle UI

    [Header("Detección de color (HSV)")]
    [Range(0f, 1f)] public float saturacionMin = 0.4f;
    [Range(0f, 1f)] public float brilloMin = 0.3f;
    public int pasoMuestreo = 3;
    public int umbralPixeles = 200;
    [Header("Movimiento de cámara")]
    [Range(5f, 50f)] public float intensidadCamara = 20f;

    [Header("Movimiento rítmico de cámara")]
    public float amplitudZ = 1f;    // qué tanto avanza o retrocede
    public float velocidadZ = 0.25f;   // velocidad de oscilación
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

        Debug.Log(" Sinestesia Room iniciado (reproducción simultánea + control estéreo)");
        // Elegir visual inicial aleatoriamente
        prefabActual = prefabsVisuales[Random.Range(0, prefabsVisuales.Length)];
        tiempoUltimoCambio = Time.time;
    }

    // ----------------------------------------------------
    void ConfigurarBanco(AudioSource[] banco)
    {
        foreach (var fuente in banco)
        {
            if (fuente != null)
            {
                fuente.spatialBlend = 0f; // Modo 3D
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
        // Pausar / Reanudar con la tecla ESPACIO
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (!juegoPausado)
                PausarJuego();
            else
                ReanudarJuego();
        }

        //Salir del juego con ESC
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Debug.Log("🚪 Saliendo del juego...");
            Application.Quit();
        }

        // Si el juego está pausado, no ejecutar el resto del código
        if (juegoPausado) return;



        if (camara.width <= 16) return;

        // Cambiar de visual cada cierto tiempo
        if (Time.time - tiempoUltimoCambio >= intervaloCambioVisual)
        {
            prefabActual = prefabsVisuales[Random.Range(0, prefabsVisuales.Length)];
            tiempoUltimoCambio = Time.time;
            Debug.Log($"🌟 Nuevo visual activo: {prefabActual.name}");
        }

        // Seleccionar banco activo según el toggle
        sonidosActivos = usarBancoB ? sonidosBancoB : sonidosBancoA;

        // Capturar imagen actual
        if (textura == null)
            textura = new Texture2D(camara.width, camara.height, TextureFormat.RGBA32, false);

        textura.SetPixels32(camara.GetPixels32());
        textura.Apply(false);
        pixeles = textura.GetPixels32();

        int[] conteoColor = new int[7];

        //Muestreo rápido (zona central)
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

        // Ajustar volúmenes simultáneamente
        for (int i = 0; i < 7; i++)
        {
            if (sonidosActivos[i] == null) continue;

            float intensidad = (float)conteoColor[i] / umbralPixeles;
            intensidad = Mathf.Clamp01(intensidad);

            // Calcula el volumen objetivo según la intensidad (distancia ya incluida)
            float volumenObjetivo = Mathf.Clamp01(intensidad);

            // Si el sonido está muy cerca (más intenso), dale un extra sutil
            volumenObjetivo *= Mathf.Lerp(0.6f, 1f, intensidad);

            // Aplica un suavizado temporal con velocidad variable
            float velocidadFade = velocidadTransicion * Time.deltaTime * 8f;
            float nuevoVolumen = Mathf.Lerp(sonidosActivos[i].volume, volumenObjetivo, velocidadFade);

            // Aplica el nuevo volumen
            sonidosActivos[i].volume = nuevoVolumen;

            // Simulación de paneo estéreo con control de apertura
            float pan = Mathf.Sin(i * Mathf.PI / 3.5f) * intensidadStereo;
            sonidosActivos[i].panStereo = pan;


            // Mostrar en consola cuando un color tiene actividad
            if (nuevoVolumen > 0.05f)
            {
                Debug.Log($"▶ Reproduciendo el color {i} - Clip: {sonidosActivos[i].clip.name}");
                // Crear visual del color detectado en su posición promedio
                if (prefabActual != null)
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
                        // Promedio más preciso de los píxeles con mayor brillo (V)
                        Vector2 sumaPosBrillantes = Vector2.zero;
                        int conteoBrillantes = 0;

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

                                if (dentroRango && v > 0.7f) // solo los más intensos
                                {
                                    sumaPosBrillantes += new Vector2(x, y);
                                    conteoBrillantes++;
                                }
                            }
                        }

                        Vector2 promedio = (conteoBrillantes > 0) ? (sumaPosBrillantes / conteoBrillantes) : (sumaPos / conteo);
                        float xNorm = (promedio.x / ancho - 0.5f) * 2f;
                        float yNorm = (promedio.y / alto - 0.5f) * 2f;

                        // Calcular profundidad dinámica según la intensidad del color
                        float profundidad = Mathf.Lerp(6f, 1.5f, intensidad); // 6 = lejos, 1.5 = cerca

                        // Añadir leve movimiento aleatorio en Z para dar más “vida”
                        float ruidoZ = Random.Range(-0.3f, 0.3f);

                        // === NUEVO CÁLCULO DE POSICIÓN 3D ===
                        // Puedes alternar entre un modo más envolvente o plano desde el código
                        bool modoCurvado = true; // true = inmersivo (semiesfera), false = plano extendido

                        Vector3 posCam;

                        if (modoCurvado)
                        {
                            // 🌌 MODO CURVADO ENVOLVENTE (semiesfera alrededor del usuario)
                            float anguloX = xNorm * Mathf.PI / 2f; // -90° a +90°
                            float anguloY = yNorm * Mathf.PI / 4f; // -45° a +45°
                            float radio = Mathf.Lerp(6f, 1.5f, intensidad); // más color = más cerca

                            posCam = new Vector3(
                                Mathf.Sin(anguloX) * radio,
                                Mathf.Sin(anguloY) * radio * 0.7f,
                                Mathf.Cos(anguloX) * radio * 0.9f
                            );
                        }
                        else
                        {
                            // 🌈 MODO PLANO EXTENDIDO
                            float relacionAspecto = (float)ancho / alto;
                            float profundidadPlano = Mathf.Lerp(6f, 1.5f, intensidad);
                            float ruidoZPlano = Random.Range(-0.3f, 0.3f);

                            posCam = new Vector3(
                                xNorm * 4.5f * relacionAspecto,
                                yNorm * 2.8f,
                                profundidadPlano + ruidoZPlano
                            );
                        }

                        // Convertir coordenadas a posición mundial
                        Vector3 posMundo = Camera.main.transform.TransformPoint(posCam);

                        GameObject visual = Instantiate(prefabActual, posMundo, Quaternion.identity);

                        // Cambiar color del visual (para materiales normales)
                        Renderer rend = visual.GetComponent<Renderer>();
                        if (rend != null)
                        {
                            float hMedio = HueMedioCircular(rangosH[i]);
                            Color colorVisual = Color.HSVToRGB(hMedio, 1f, 1f);
                            rend.material.SetColor("_EmissionColor", colorVisual * 2f);
                            rend.material.color = colorVisual;
                        }

                        // Si el prefab tiene Particle System, también cambia su color
                        ParticleSystem ps = visual.GetComponent<ParticleSystem>();

                        if (ps != null)
                        {
                            var main = ps.main;
                            float hMedio = HueMedioCircular(rangosH[i]);
                            main.startColor = Color.HSVToRGB(hMedio, 1f, 1f);
                        }

                        // Destruir visual con fade-out suave
                        StartCoroutine(FadeOutAndDestroy(visual, duracionVisual));
                    }
                }

            }

        }
        // === MOVIMIENTO SUAVE DE CÁMARA HACIA COLOR DOMINANTE ===
        int indiceDominante = -1;
        int maxPixeles = 0;

        // Encuentra el color con más píxeles detectados
        for (int i = 0; i < 7; i++)
        {
            if (conteoColor[i] > maxPixeles)
            {
                maxPixeles = conteoColor[i];
                indiceDominante = i;
            }
        }

        if (indiceDominante >= 0 && maxPixeles > umbralPixeles / 4)
        {
            // Recalcular centro promedio del color dominante
            Vector2 sumaPos = Vector2.zero;
            int conteo = 0;

            for (int y = alto / 4; y < alto * 3 / 4; y += pasoMuestreo)
            {
                for (int x = ancho / 4; x < ancho * 3 / 4; x += pasoMuestreo)
                {
                    Color color = pixeles[y * ancho + x];
                    Color.RGBToHSV(color, out float h, out float s, out float v);
                    if (s < saturacionMin || v < brilloMin) continue;

                    bool dentroRango = (rangosH[indiceDominante].x < rangosH[indiceDominante].y)
                        ? (h >= rangosH[indiceDominante].x && h <= rangosH[indiceDominante].y)
                        : (h >= rangosH[indiceDominante].x || h <= rangosH[indiceDominante].y);

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

                // Normaliza a rango -1 a 1
                float xNorm = (promedio.x / ancho - 0.5f) * 2f;
                float yNorm = (promedio.y / alto - 0.5f) * 2f;

                // Movimiento de cámara según el color más grande
                float fuerza = Mathf.Clamp01((float)maxPixeles / umbralPixeles) * 0.5f; // 0–0.5
                Vector3 objetivo = new Vector3(xNorm * fuerza * intensidadCamara, -yNorm * fuerza * (intensidadCamara * 0.75f), 0f);


                // Movimiento y rotación suave
                Camera.main.transform.position = Vector3.Lerp(
                    Camera.main.transform.position,
                    new Vector3(objetivo.x, objetivo.y, Camera.main.transform.position.z),
                    Time.deltaTime * 1.5f
                );

                Quaternion rotObjetivo = Quaternion.Euler(-yNorm * 15f, xNorm * 25f, 0f);
                Camera.main.transform.rotation = Quaternion.Slerp(
                    Camera.main.transform.rotation,
                    rotObjetivo,
                    Time.deltaTime * 1.2f
                );
            }
            
            // 🌊 Movimiento suave adelante-atrás en el eje Z (efecto de respiración)
            float desplazamientoZ = Mathf.Sin(Time.time * velocidadZ) * amplitudZ;

            // Aplicar movimiento suave en Z manteniendo la posición base
            Vector3 nuevaPos = Camera.main.transform.position;
            nuevaPos.z = Mathf.Lerp(nuevaPos.z, -desplazamientoZ, Time.deltaTime * 0.5f);
            Camera.main.transform.position = nuevaPos;
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
    IEnumerator FadeOutAndDestroy(GameObject visual, float duracion)
    {

        Renderer rend = visual.GetComponent<Renderer>();
        float tiempo = 0f;

        if (rend != null && rend.material.HasProperty("_Color"))
        {
            Color colorInicial = rend.material.color;

            while (tiempo < duracion)
            {
                tiempo += Time.deltaTime;
                float t = tiempo / duracion;
                Color nuevoColor = colorInicial;
                nuevoColor.a = Mathf.Lerp(1f, 0f, t);
                rend.material.color = nuevoColor;
                yield return null;
            }
        }

        Destroy(visual);
    }
    
    // === Cálculo correcto de color medio en HSV (evita que el rojo salga azul) ===
    float HueMedioCircular(Vector2 rango)
    {
        float a = rango.x; // inicio
        float b = rango.y; // fin
        if (a <= b)
            return (a + b) * 0.5f;

        // Cruza el 1→0 (por ejemplo, rojo 0.97–0.03)
        float mid = (a + (b + 1f)) * 0.5f;
        if (mid >= 1f) mid -= 1f;
        return mid;
    }

}
