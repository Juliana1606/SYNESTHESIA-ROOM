using UnityEngine;

public class Logica_Entre_Escenas : MonoBehaviour
{

     public GameObject panelInicio;
    public GameObject panelOpciones;

    public void AbrirOpciones()
    {
        panelInicio.SetActive(false);
        panelOpciones.SetActive(true);
    }

    public void CerrarOpciones()
    {
        panelOpciones.SetActive(false);
        panelInicio.SetActive(true);
    }
    private void Awake()
    {
        var noDestruirEntreEscenas=FindObjectsOfType<Logica_Entre_Escenas>();
        if(noDestruirEntreEscenas.Length>1)
        {
            Destroy(gameObject);
            return;

        }
        DontDestroyOnLoad(gameObject);
    }


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
