﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Cloud.Analytics;

public class GameOverScript : MonoBehaviour
{
	private GUISkin skin;

	public bool GameOverActive = false;
	public RectTransform gameoverMenu;

	private Rect button1Rect = new Rect(15,15,160,30);
	private Rect button2Rect = new Rect(15,15,160,30);	

    public string herstartlevel;

    // ANALYTICS
    private bool playerRestart;

	void Start()
	{
		// Load a skin for the buttons
		skin = Resources.Load("ButtonSkin") as GUISkin;		
	}	

	void OnGUI()
	{
		button1Rect.x = (Screen.width / 2) - (button1Rect.width / 2);
		button1Rect.y = (Screen.height / 2) - (button1Rect.height / 2);

		button2Rect.x = (Screen.width / 2) - (button2Rect.width / 2);
		button2Rect.y = (Screen.height / 2) - (button2Rect.height / 2);
		

		// Set Menu op actief en daarna op inactief
		if (GameOverActive == true) 
		{
			// Plaatsing buttons
			button1Rect.y = button1Rect.y - 20;
			button2Rect.y = button2Rect.y + 65;

			// Activeer Ingame menu
			gameoverMenu.gameObject.SetActive(true);

			// Zet game op stil
			Time.timeScale = 0;

			// Set the skin to use
			GUI.skin = skin;
			
			// Opnieuw Button
			if (GUI.Button (
				// Center in X, 2/3 of the height in Y
				button1Rect,
				"Opnieuw?"
				)) 
			{
                playerRestart = true;
                StartGameAnalytics();

				GameOverActive = false;
				gameoverMenu.gameObject.SetActive(false);
				Time.timeScale = 1;
                Application.LoadLevel(herstartlevel); // Load Totorial
			}
			
			// Naar Main menu button
			if (GUI.Button (
				// Center in X, 2/3 of the height in Y
				button2Rect,
				"Menu"
				)) 
			{
                playerRestart = true;
                StartGameAnalytics();

				GameOverActive = false;
				gameoverMenu.gameObject.SetActive(false);
				Time.timeScale = 1;
				Application.LoadLevel ("MainMenu"); // Load Main Menu
			}
		}		
	}

    /**
     * Sends the selected player and level to analytics
     */
    void StartGameAnalytics()
    {
        string customEventName = "PlayerDeath" + Application.loadedLevelName;

        if (!GameControl.control.bossBattleStarted)
        {
            UnityAnalytics.CustomEvent(customEventName, new Dictionary<string, object>
            {
                { "runningTime", Time.timeSinceLevelLoad },
                { "projectile1Shot", GameControl.control.projectile1Shot },
                { "projectile2Shot", GameControl.control.projectile2Shot },
                { "projectile3Shot", GameControl.control.projectile3Shot },
                { "kidsFound", GameControl.control.kidsFound },
                { "lettersFound", GameControl.control.lettersFound },
                { "enemyDefeated", GameControl.control.enemiesDefeated },
                { "respawns", GameControl.control.respawns },
                { "timesPaused", GameControl.control.timesPaused },
                { "playerRestart", playerRestart },
            });
        }
        else
        {
            float bossBattleDuration = (Time.timeSinceLevelLoad - GameControl.control.bossBattleStartTime);

            UnityAnalytics.CustomEvent(customEventName + "Boss", new Dictionary<string, object>
            {
                { "runningTime", Time.timeSinceLevelLoad },
                { "projectile1Shot", GameControl.control.projectile1Shot },
                { "projectile2Shot", GameControl.control.projectile2Shot },
                { "projectile3Shot", GameControl.control.projectile3Shot },
                { "kidsFound", GameControl.control.kidsFound },
                { "bossBattleHealth", GameControl.control.bossDamageTaken },
                { "bossBattleDuration", bossBattleDuration },
                { "timesPaused", GameControl.control.timesPaused },
                { "pauseDuration", GameControl.control.pauseDuration },
                { "playerRestart", playerRestart },
            });
        }
    }
}
