using UnityEngine;
using UnityEngine.AI;

public class Robot : MonoBehaviour
{
    MyFirstPersonController player;
    
    NavMeshAgent agent;

    const string PLAYER_STRING = "Player";


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
        if (!player) return;

        agent.SetDestination(player.transform.position);  
        
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(PLAYER_STRING))
        {
            EnemyHealth enemyHealth = GetComponent<EnemyHealth>();
            GameManager.instance.AdjustEnemiesLeft(-1);
            enemyHealth.SelfDestruct();
        }
    
    }
}
