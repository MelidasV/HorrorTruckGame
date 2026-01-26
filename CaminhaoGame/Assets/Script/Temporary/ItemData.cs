using UnityEngine;

[CreateAssetMenu(fileName = "Novo Item", menuName = "Jogo/Item")]
public class ItemData : ScriptableObject
{
    public string nome;
    public float valor;
    public float peso;
    public bool ehMaldito;
}