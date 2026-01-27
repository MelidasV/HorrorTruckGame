using UnityEngine;

public class ShowInteractText : MonoBehaviour
{
    public GameObject textoAviso; // Arraste o Texto "Pressione E" aqui

    void Start()
    {
        // Garante que comece desligado
        if (textoAviso) textoAviso.SetActive(false);
    }

    void OnTriggerEnter(Collider other)
    {
        // Se o Player entrou na área
        if (other.CompareTag("Player"))
        {
            if (textoAviso) textoAviso.SetActive(true);
        }
    }

    void OnTriggerExit(Collider other)
    {
        // Se o Player saiu
        if (other.CompareTag("Player"))
        {
            if (textoAviso) textoAviso.SetActive(false);
        }
    }
}