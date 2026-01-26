using TMPro;
using UnityEngine;

public class InteractionUI : MonoBehaviour
{
    public TextMeshProUGUI texto;

    void Awake()
    {
        if (texto == null)
            texto = GetComponent<TextMeshProUGUI>();

        texto.enabled = false;
    }

    public void MostrarSegurando()
    {
        texto.text =
            "<b>SEGURANDO ITEM</b>\n" +
            "Scroll - Distância\n" +
            "Botão Direito - Arremessar\n" +
            "H - Rotacionar";

        texto.enabled = true;
    }

    public void MostrarRotacao()
    {
        texto.text =
            "<b>MODO ROTAÇÃO</b>\n" +
            "Mouse - Girar Item\n" +
            "H - Confirmar Rotação";

        texto.enabled = true;
    }

    public void Esconder()
    {
        texto.enabled = false;
    }
}
