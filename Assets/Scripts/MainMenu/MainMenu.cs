﻿using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class MainMenu : MonoBehaviour 
{
    public GameObject level;
    public GameObject speler;
    public GameObject prestaties;
    
    private GUISkin skin;	

	private Rect button1Rect = new Rect(15,15,160,30);
	private Rect button2Rect = new Rect(15,15,160,30);
	private Rect button3Rect = new Rect(15,15,160,30);

    public AudioSource _audioSource;

    public bool _mainMenuUit;

	void Start()
	{
        level.SetActive(false);
        speler.SetActive(false);

		// Load a skin for the buttons
		skin = Resources.Load("ButtonSkin") as GUISkin;

        _mainMenuUit = true;
	}	
	
	void OnGUI()
	{
        if (_mainMenuUit == false)
        {
            button1Rect.x = (Screen.width / 2) - (button1Rect.width / 2);
            button1Rect.y = (Screen.height / 2) - (button1Rect.height / 2);

            button2Rect.x = (Screen.width / 2) - (button2Rect.width / 2);
            button2Rect.y = (Screen.height / 2) - (button2Rect.height / 2);

            button3Rect.x = (Screen.width / 2) - (button3Rect.width / 2);
            button3Rect.y = (Screen.height / 2) - (button3Rect.height / 2);

            button1Rect.y = button1Rect.y - 130;
            button2Rect.y = button2Rect.y - 80;
            button3Rect.y = button3Rect.y - 30;            

            // Set the skin to use
            GUI.skin = skin;

            // Start Button
            if (GUI.Button(
                // Center in X, 2/3 of the height in Y
                button1Rect,
                "START"
            ))
            {
                _audioSource.Play();
                GameControl.control.loadLevel = "Intro";

                // playerSelect
                speler.SetActive(true); // Select Speler
                _mainMenuUit = true;
            }            

            // Start Button
            if (GUI.Button(
                // Center in X, 2/3 of the height in Y
                button2Rect,
                "Level"
            ))
            {
                _audioSource.Play();
                level.SetActive(true);// Select Level
                _mainMenuUit = true;
            }

            // Prestaties Button
            if (GUI.Button(
                // Center in X, 2/3 of the height in Y
                button3Rect,
                "Prestaties"
            ))
            {                
                _audioSource.Play();           
                prestaties.SetActive(true);
                _mainMenuUit = true;
            }	
        }                        
	}
}
