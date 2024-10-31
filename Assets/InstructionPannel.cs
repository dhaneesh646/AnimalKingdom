using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InstructionPannel : MonoBehaviour
{
    public GameObject InstructionPannels;
    public GameObject MainMenu;
    private int instructionpanneldisplayed = 1;
    private int instructincount;
    void Start()
    {
        if (PlayerPrefs.HasKey("Mainmenuinstructionpanel"))
        {
            MainmenupannelActivate();
            InstructionPannels.SetActive(false);
        }
        else
        {
            displayInstruction();
        }
    }
    private void MainmenupannelActivate()
    {
        MainMenu.SetActive(true);
    }
    private void displayInstruction()
    {
        InstructionPannels.SetActive(true);
        saveinstruction();
    }
    private void saveinstruction()
    {
        PlayerPrefs.SetInt("Mainmenuinstructionpanel", instructionpanneldisplayed);
    }
    public void InstructionpannelFalse()
    {
        InstructionPannels.SetActive(false);
        MainmenupannelActivate();
    }
}
