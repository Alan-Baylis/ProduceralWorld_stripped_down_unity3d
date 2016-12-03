using UnityEngine;
using System.Collections;

public class AudioManager : MonoBehaviour {

    public static AudioManager instance;
    private AudioSource source;
    public AudioClip music;

	void Awake () {
        if (instance == null)
        {
            instance = this;
            Debug.Log("AudioManager Singleton Init Done");
        }
        else
            Destroy(gameObject);
	}

    void Start()
    {
        source = GetComponent<AudioSource>();
        source.clip = music;
        source.loop = true;
        source.Play();
    }
	
	void Update () {
	
	}

    public void PlayClip(AudioClip clip)
    {
        source.PlayOneShot(clip);
    }
}
