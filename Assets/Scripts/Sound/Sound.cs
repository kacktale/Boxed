using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sound : MonoBehaviour
{
    public AudioSource Main,lobby,Card;
    public void PlayMain()
    {
        Main.Play();
        lobby.Stop();
        Card.Stop();
    }
    public void PlayLobby()
    {
        Main.Stop();
        lobby.Play();
        Card.Stop();
    }
    public void PlayCard()
    {
        Main.Stop();
        lobby.Stop();
        Card.Play();
    }
}
