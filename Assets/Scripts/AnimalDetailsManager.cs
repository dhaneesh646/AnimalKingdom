using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class AnimalDetailsManager : MonoBehaviour
{
    private int selectedAnimalIndex = -1;
    public GameObject AnimalInstructionPannel, VerticalPannel;
    int totalanimals;
    public TMP_Text AnimalnmeText, AnimalDetailsText;
    public Image AnimalImage;
    public Button closeButton;
    public List<AnimalDetails> AnimalDetails;
    public List<Button> AnimalButtons;
    [SerializeField] private GameObject dockCanvas;
    void Start()
    {
        AnimalInstructionPannel.SetActive(false);
        for(int i=0;i<AnimalButtons.Count;i++)
        {
            int aimalIndex = i;
            AnimalButtons[i].onClick.AddListener(() => Updateanimaldata(aimalIndex));
        }
    }

    // Update is called once per frame
    public void Updateanimaldata(int animalIndex)
    {
        selectedAnimalIndex = animalIndex;
        AnimalInstructionPannel.SetActive(true);
        VerticalPannel.SetActive(false);
        AnimalImage.sprite = AnimalDetails[selectedAnimalIndex].AnimalImage;
        AnimalDetailsText.text = AnimalDetails[selectedAnimalIndex].AnimalDetsils;
        AnimalnmeText.text = AnimalDetails[selectedAnimalIndex].AnimalName;
    }
    public void CloasePannel()
    {
        AnimalInstructionPannel.SetActive(false);
        VerticalPannel.SetActive(true);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.X))
        {
            dockCanvas.SetActive(true);
        }
    }
}
