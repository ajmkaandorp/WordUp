﻿using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PlayerSelect : MonoBehaviour {

    public GameObject mainMenu;
    public GameObject fynn;
    public GameObject fiona;

    private GUISkin skin;

    // Keyboard control
    string[] buttons = new string[2] { "Fynn", "Fiona"};
    private int selected = 0;

    private Rect button1Rect = new Rect(15, 15, 160, 30);
    private Rect button2Rect = new Rect(15, 15, 160, 30);   

    void Start()
    {
        // Load a skin for the buttons
        skin = Resources.Load("ButtonSkin") as GUISkin;

        selected = 0;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.W))
        {
            selected = menuSelection(buttons, selected, "up");
        }

        if (Input.GetKeyDown(KeyCode.S))
        {
            selected = menuSelection(buttons, selected, "down");
        }
    }

    int menuSelection(string[] buttonsArray, int selectedItem, string direction)
    {
        if (direction == "up")
        {
            if (selectedItem == 0)
            {
                selectedItem = buttonsArray.Length - 1;
            }
            else
            {
                selectedItem -= 1;
            }
        }

        if (direction == "down")
        {

            if (selectedItem == buttonsArray.Length - 1)
            {
                selectedItem = 0;
            }
            else
            {
                selectedItem += 1;
            }
        }
        return selectedItem;
    }

    void OnGUI()
    {
        button1Rect.x = (Screen.width / 2) - (button1Rect.width / 2);
        button1Rect.y = (Screen.height / 2) - (button1Rect.height / 2);

        button2Rect.x = (Screen.width / 2) - (button2Rect.width / 2);
        button2Rect.y = (Screen.height / 2) - (button2Rect.height / 2);        

        button1Rect.y = button1Rect.y - 80;
        button1Rect.x = button1Rect.x;
        button2Rect.y = button2Rect.y - 30;
        button2Rect.x = button2Rect.x;
       

        // Set the skin to use
        GUI.skin = skin;

        GUI.SetNextControlName(buttons[0]);
        // Start Button
        if (GUI.Button(
            // Center in X, 2/3 of the height in Y
            button1Rect,
            "Fynn"
        ))
        {
            GameControl.control.selectPlayer = "Fynn"; // Zet speler op Fynn
            
            // Past UI selectie aan
            fynn.SetActive(true);
            fiona.SetActive(false);
            
            this.gameObject.SetActive(false);
            mainMenu.SetActive(true);
        }

        GUI.SetNextControlName(buttons[1]);
        // Prestaties Button
        if (GUI.Button(
            // Center in X, 2/3 of the height in Y
            button2Rect,
            "Fiona"
        ))
        {
            GameControl.control.selectPlayer = "Fiona"; // Zet speler op Fiona
            
            // Past UI selectie aan
            fiona.SetActive(true);
            fynn.SetActive(false);
            
            this.gameObject.SetActive(false);
            mainMenu.SetActive(true);
        }      

        GUI.FocusControl(buttons[selected]);
    }
}