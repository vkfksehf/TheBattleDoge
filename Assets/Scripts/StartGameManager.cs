using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StartGameManager : MonoBehaviour
{
    public AudioClip[] audioClips;
    private AudioSource audioSource;
    private Animator animator;
    public int turnOut;

    // Start is called before the first frame update
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        animator = GameObject.Find("Buttons").GetComponent<Animator>();
        turnOut = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if (turnOut == 1)
        {
            SceneManager.LoadScene("MainScene");
        }
        else if (turnOut == -1)
        {
            if (System.IO.File.Exists(Application.persistentDataPath + "/data.xml"))
            {
                System.IO.File.Delete(Application.persistentDataPath + "/data.xml");
            }
        }
    }

    public void IsButtonClicked(int var)
    {
        audioSource.clip = audioClips[1];
        audioSource.Play();

        if (var == 0)
        {
            animator.SetBool("IsAclicked", true);
        }
        else
        {
            animator.SetBool("IsBclicked", true);
        }
    }

    private void OnApplicationQuit()
    {
        Application.Quit();
    }
}
