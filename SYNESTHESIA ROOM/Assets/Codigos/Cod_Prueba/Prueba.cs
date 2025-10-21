using UnityEngine;
using UnityEngine.UI;

public class SynesthesiaRoom : MonoBehaviour
{
    [Header("UI")]
    
    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip redSound;
    public AudioClip greenSound;
    public AudioClip blueSound;

    [Header("Prefabs Visuales")]
    public GameObject redShape;
    public GameObject greenShape;
    public GameObject blueShape;

    private WebCamTexture webcamTexture;
    private Color32[] pixels;
    private float lastSpawnTime;

    void Start()
    {
        // Inicializar la cámara
        webcamTexture = new WebCamTexture();
      
        webcamTexture.Play();
    }

    void Update()
    {
        if (webcamTexture.width > 100) // asegurar que ya arrancó la cámara
        {
            pixels = webcamTexture.GetPixels32();
            DetectAndTrigger(pixels, webcamTexture.width, webcamTexture.height);
        }
    }

    void DetectAndTrigger(Color32[] frame, int width, int height)
    {
        // Tomar color en el centro
        int x = width / 2;
        int y = height / 2;
        Color32 pixel = frame[y * width + x];

        // Convertir a intensidades
        float r = pixel.r / 255f;
        float g = pixel.g / 255f;
        float b = pixel.b / 255f;

        // Solo disparar cada 0.5 segundos
        if (Time.time - lastSpawnTime < 0.5f) return;
        lastSpawnTime = Time.time;

        // Decidir color dominante
        if (r > g && r > b)
        {
            Trigger(redSound, redShape, Color.red);
        }
        else if (g > r && g > b)
        {
            Trigger(greenSound, greenShape, Color.green);
        }
        else
        {
            Trigger(blueSound, blueShape, Color.blue);
        }
    }

    void Trigger(AudioClip clip, GameObject shapePrefab, Color color)
    {
        // Reproducir sonido
        if (clip != null && audioSource != null)
        {
            audioSource.PlayOneShot(clip);
        }

        // Crear figura en escena
        if (shapePrefab != null)
        {
            Vector3 pos = new Vector3(Random.Range(-3f, 3f), Random.Range(-2f, 2f), Random.Range(3f, 6f));
            GameObject shape = Instantiate(shapePrefab, pos, Quaternion.identity);
            shape.GetComponent<Renderer>().material.color = color;

            Destroy(shape, 3f); // destruir después de 3s
        }
    }
}