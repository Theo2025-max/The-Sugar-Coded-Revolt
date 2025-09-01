using System.Collections;
using UnityEngine;

public class SpawnGate : MonoBehaviour
{
    [SerializeField] GameObject robotPrefab;
    [SerializeField] float SpawnTime = 5f;
    [SerializeField] Transform spawnPoint;

    PlayerHealth player;
    private void Start()
    {
        player = FindAnyObjectByType<PlayerHealth>();
        StartCoroutine(SpawnRoutine());
    }

    IEnumerator SpawnRoutine()
    {
        while (player)
        {
           Instantiate(robotPrefab, spawnPoint.position, transform.rotation);
           yield return new WaitForSeconds(SpawnTime);

        }
    }




}
