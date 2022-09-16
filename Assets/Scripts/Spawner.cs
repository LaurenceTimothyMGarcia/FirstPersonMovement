using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    [SerializeField] GameObject spawnItem;
    [SerializeField] float spawnDelay;
    [SerializeField] Vector3 spawnPoint;

    // Start is called before the first frame update
    void Start()
    {
        spawnDelay = 5;
        StartCoroutine(spawnCube(spawnDelay,spawnItem));
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    

    private IEnumerator spawnCube(float delay, GameObject item) 
    {
        yield return new WaitForSeconds(delay);
        Instantiate(spawnItem, transform.position, transform.rotation);
        StartCoroutine(spawnCube(spawnDelay, spawnItem));
    }
}
