using UnityEngine;

// Adicionamos a herança de IInteractable
public class NPC : MonoBehaviour, IInteractable
{
    public string nomeNPC = "Entregador Estranho";
    [TextArea(3, 10)]
    public string[] falas;

    // Esta função é OBRIGATÓRIA porque usamos IInteractable
    // O PlayerInteract vai chamar ela automaticamente quando apertar E perto
    public void Interact(PlayerInteract player)
    {
        Debug.Log("Interagiu com o NPC!");

        // Chama o seu DialogueManager antigo (ele ainda serve!)
        // Só precisamos garantir que ele existe na cena
        if (DialogueManager.Instance != null)
        {
            DialogueManager.Instance.IniciarDialogo(this);
            // Obs: Talvez precise ajustar IniciarDialogo para receber (string, string[]) 
            // ou ajustar o DialogueManager para receber o script NPC inteiro.
        }
    }
}