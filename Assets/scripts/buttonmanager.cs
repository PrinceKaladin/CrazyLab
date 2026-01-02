using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class buttonmanager : MonoBehaviour
{
    public Sprite on;
    public Sprite off;
    public bool correct;
    private void Awake()
    {
        if (correct)
        {
            GameObject audiox = GameObject.Find("Audioline");
            if (audiox.GetComponent<AudioSource>().volume == 0.5f)
            {
                this.GetComponent<Image>().sprite = on;
            }
            else
            {
                this.GetComponent<Image>().sprite = off;
            }
        }
        else
        {
            if (PlayerPrefs.HasKey("sound"))
            {
                if (PlayerPrefs.GetInt("sound") == 1)
                {
                    this.GetComponent<Image>().sprite = on;
                }
                else
                {
                    this.GetComponent<Image>().sprite = off;
                }
            }
            else
            {
                PlayerPrefs.SetInt("sound", 1);

            }
        }
    }
    public void MusicOnOff()
    {
        GameObject audiox = GameObject.Find("Audioline");

        if (audiox.GetComponent<AudioSource>().volume == 0.5f)
        {
            audiox.GetComponent<AudioSource>().volume = 0;
            this.GetComponent<Image>().sprite = off;
        }
        else
        {
            audiox.GetComponent<AudioSource>().volume = 0.5f;
            this.GetComponent<Image>().sprite = on;

        }
    }
    public void ObjectOnOff()
    {
        if (this.GetComponent<Image>().sprite == off)
        {
            PlayerPrefs.SetInt("sound", 1);

            this.GetComponent<Image>().sprite = on;
        }
        else
        {
            PlayerPrefs.SetInt("sound", 0);

            this.GetComponent<Image>().sprite = off;

        }
    }
    // Update is called once per frame
    void Update()
    {

    }
}