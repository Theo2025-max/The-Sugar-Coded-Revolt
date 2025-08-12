using UnityEngine;
using UnityEngine.AI;

public class Robot : MonoBehaviour
{
    MyFirstPersonController player;
    
    NavMeshAgent agent;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    void Start()
    {
        player = FindFirstObjectByType<MyFirstPersonController>();
        

    }

    void Update()
    {
        agent.SetDestination(player.transform.position);  
        
    }
}
