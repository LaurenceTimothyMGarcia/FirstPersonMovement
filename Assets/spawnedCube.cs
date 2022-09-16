using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class spawnedCube : MonoBehaviour
{
    [SerializeField] private int speed = 3;
    [SerializeField] private int lifetime = 5;

    private int timer = 0;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    void Awake()
    {
        speed = 3;
        lifetime = 5;
        timer = 0;
    }

    // Update is called once per frame
    void Update()
    {
        transform.Translate(2 * Time.deltaTime,0, 0);
        
    }

    
}
