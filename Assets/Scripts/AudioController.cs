using UnityEngine;
using System.Collections;

public class AudioController : MonoBehaviour {

    public AudioClip blockDestroyed;
    public AudioClip backgroundMusic;

    private AudioSource audioSource;
    private AudioSource audioSourceMusic;

    private bool gamePaused;

	void Start () {
        gamePaused = true;

        audioSource = GetComponents<AudioSource>()[0];
        audioSource.mute = false;
        audioSourceMusic = GetComponents<AudioSource>()[1];
        audioSourceMusic.mute = false;

        audioSourceMusic.loop = true;
        audioSourceMusic.clip = backgroundMusic;

        LevelController.onBlockHit += AudioController_onBlockHit;
        MenuController.onKeyPressed += MenuController_onKeyPressed;
        MenuController.onMusicToggle += MenuController_onMusicToggle;
        MenuController.onSFXToggle += MenuController_onSFXToggle;
	}

    void MenuController_onSFXToggle()
    {
        audioSource.mute = !audioSource.mute;
    }

    void MenuController_onMusicToggle()
    {
        audioSourceMusic.mute = !audioSourceMusic.mute;
    }

    void MenuController_onKeyPressed(KeyCode keyCode)
    {
        if (keyCode == KeyCode.Escape)
        {
            gamePaused = !gamePaused;
            if (gamePaused)
            {
                audioSourceMusic.Pause();
            }
            else
                audioSourceMusic.Play();
        }
    }

    void AudioController_onBlockHit(Block block)
    {
        if (block.currentHp <= 0)
        {
            audioSource.clip = blockDestroyed;
            audioSource.Play();
        }
    }
	
	// Update is called once per frame
	void Update () {
	
	}
}
