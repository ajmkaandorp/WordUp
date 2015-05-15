﻿using UnityEngine;
using System.Collections;

public class KnopScriptRotate : MonoBehaviour {

    public GameObject knop;
    public GameObject[] lichten;
	public GameObject rotateMyLamp;
	public GameObject hint;

	private AudioClip _audioSource;
	private Vector3 positie;
	private int teller = 0;
    private bool ingedrukt;

	// Use this for initialization
	void Start () 
    {
        ingedrukt = false;
		_audioSource = gameObject.GetComponent<AudioSource>().clip;
		positie = gameObject.transform.position;
	}
	
	// Update is called once per frame
	void Update () 
    {

	}

    void OnTriggerEnter2D(Collider2D collision)
    {        
        if (collision.gameObject.tag == "Player")
        {
            if (ingedrukt == false)
            { 
                // Zet licht aan
                foreach (GameObject o in lichten)
                {
                    o.GetComponent<Light>().enabled = true;
					rotateMyLamp.SendMessage("LampAan", true);
                }             

                ingedrukt = true;
				hint.GetComponent<Animator>().enabled = false;
            }

			if (ingedrukt == true && teller == 0) 
			{
				knop.transform.Translate (0, -Time.deltaTime * 3, 0);
				AudioSource.PlayClipAtPoint (_audioSource, positie);
				teller++;
			}
        }
    }    
}
