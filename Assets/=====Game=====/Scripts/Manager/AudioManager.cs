using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : Singleton<AudioManager>
{
    private void Awake()
    {
        base.Awake();
    }
    public void PlaySound(string clipname, float volumnMultiplier)
    {
        AudioSource source = this.gameObject.AddComponent<AudioSource>();
        source.volume *= volumnMultiplier;
        source.PlayOneShot((AudioClip)Resources.Load("Sounds/" + clipname, typeof(AudioClip)));
    }

}
