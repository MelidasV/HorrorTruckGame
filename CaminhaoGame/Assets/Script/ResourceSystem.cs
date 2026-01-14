using UnityEngine;

public class ResourceSystem : MonoBehaviour
{
    public float combustivel = 100f;
    public float bateriaLanterna = 100f;
    public Light luzLanterna;
    public bool motorLigado = false;

    void Update()
    {
        if (motorLigado) combustivel -= 0.1f * Time.deltaTime;

        if (Input.GetKeyDown(KeyCode.L) && bateriaLanterna > 0)
        {
            luzLanterna.enabled = !luzLanterna.enabled;
        }

        if (luzLanterna.enabled)
        {
            bateriaLanterna -= 0.5f * Time.deltaTime;
            if (bateriaLanterna <= 0) luzLanterna.enabled = false;
        }
    }

    public void RefilGasolina(float qtd) => combustivel = Mathf.Min(combustivel + qtd, 100f);
}