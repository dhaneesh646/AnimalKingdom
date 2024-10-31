using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PanelActivator : MonoBehaviour
{
    [SerializeField] GameObject animalinfopannel;
    public GameObject player;
    void Start()
    {
        animalinfopannel.SetActive(false);
    }
    private void Update()
    {
        animalinfopannel.transform.LookAt(player.transform.position);
    }
    public void Activatepanel()
    {
        animalinfopannel.SetActive(true);
    }
    public void CloasePanel()
    {
        animalinfopannel.SetActive(false);
    }
}
