using UnityEngine;
using UnityEngine.AI;

public class ObserverMonster : MonoBehaviour
{
    public NavMeshAgent agent;
    public Transform player;
    public Renderer meshRenderer;

    void Update()
    {
        Plane[] planes = GeometryUtility.CalculateFrustumPlanes(Camera.main);
        if (GeometryUtility.TestPlanesAABB(planes, meshRenderer.bounds))
        {
            agent.isStopped = true; // Player está a olhar, ele congela
        }
        else
        {
            agent.isStopped = false;
            agent.SetDestination(player.position);
        }

        if (Vector3.Distance(transform.position, player.position) < 2f)
        {
            // Lógica de Morte aqui
        }
    }
}