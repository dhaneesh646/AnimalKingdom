using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MissionIndicator : MonoBehaviour
{
    public Canvas OffscreenCanvas;
    public GameObject MissonPrefab;
    public Transform Player;
    public Transform[] targets;
    private Vector3 spawnRange = new Vector3(10f, 0f, 10f);
    private GameObject MissonGameobject;

    
    public void Update()
    {
        for(int i=0;i<=targets.Length;i++)
        {
            targets[i].GetComponent<OffScreenIndicator>().OffscreenCanvas = OffscreenCanvas;
            targets[i].GetComponent<OffScreenIndicator>().Player = Player;
        }
        if (Input.GetKeyDown(KeyCode.K))
        {
            Vector3 randomPosition = transform.position + new Vector3(Random.Range(-spawnRange.x / 2, spawnRange.x / 2), 0f, Random.Range(-spawnRange.z / 2, spawnRange.z / 2));
            
        }
    }      
}
