using UnityEngine;
using System.Collections;
using UnityEngine.Audio;
using UnityEngine.UI;

public class AudioAvatar
{
    public AudioMixer mainMixer;

    public AudioClip[] musicClips;					//Array of music clips to play
    public AudioMixerSnapshot volumeDown;			//Reference to Audio mixer snapshot in which the master volume of main mixer is turned down
    public AudioMixerSnapshot volumeUp;				//Reference to Audio mixer snapshot in which the master volume of main mixer is turned up

    public AudioSource musicSource;				//Reference to the AudioSource which plays music 

    public bool isSilent;

    public bool Stopped
    {
        get { return musicSource.time == 0; }
    }

    public bool AlreadyInit()
    {
        return (musicSource != null && mainMixer != null && volumeDown != null && volumeUp != null);
    }

    //Used if running the game in a single scene, takes an integer music source allowing you to choose a clip by number and play.
    public void PlaySelectedMusic(int musicChoice)
    {
        if (musicClips == null) { return; }

        //Play the music clip at the array index musicChoice
        musicSource.clip = musicClips[musicChoice];

        //Play the selected clip
        musicSource.Play();
    }

    public void PlayRandomMusic()
    {
        if (musicClips == null) { return; }
        // pick a random index
        int idx = Random.Range(0, musicClips.Length);
        Debug.LogFormat("Play Random Music {0}", idx);
        PlaySelectedMusic(idx);
    }

    public void Resume()
    {
        musicSource.Play();
    }

    public void Stop()
    {
        if (musicSource == null) { return; }
        musicSource.Stop();
    }

    //Call this function to very quickly fade up the volume of master mixer
    public void FadeUp(float fadeTime)
    {
        //call the TransitionTo function of the audioMixerSnapshot volumeUp;
        volumeUp.TransitionTo(fadeTime);
    }

    //Call this function to fade the volume to silence over the length of fadeTime
    public void FadeDown(float fadeTime)
    {
        //call the TransitionTo function of the audioMixerSnapshot volumeDown;
        volumeDown.TransitionTo(fadeTime);
    }

    public void SetMasterVolume(string exposedValue, float masterVol)
    {
        mainMixer.SetFloat(exposedValue, masterVol);
    }


    #region Singleton stuff
    private static AudioAvatar _instance = null;
    public static AudioAvatar Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new AudioAvatar();
            }
            return _instance;
        }
    }
    #endregion
}

public class AudioBehaviour : MonoBehaviour {

    AudioAvatar avatarAang = AudioAvatar.Instance;

    public AudioMixer mainMixer;
    
    public AudioClip[] musicClips;					//Array of music clips to play
    public AudioMixerSnapshot volumeDown;			//Reference to Audio mixer snapshot in which the master volume of main mixer is turned down
    public AudioMixerSnapshot volumeUp;				//Reference to Audio mixer snapshot in which the master volume of main mixer is turned up

    private float checkFinishedTimer;
    private const float checkFinishedDuration = 0.25f;

    void Awake()
    {
        if (!avatarAang.AlreadyInit())
        {
            //Get a component reference to the AudioSource attached to the UI game object
            avatarAang.musicSource = GetComponent<AudioSource>();
            avatarAang.mainMixer = mainMixer;
            avatarAang.volumeDown = volumeDown;
            avatarAang.volumeUp = volumeUp;
            avatarAang.musicClips = musicClips;
            avatarAang.isSilent = false;

            //PlaySelectedMusic(0);
            PlayRandomMusic();
            checkFinishedTimer = 0;
        }
    }

    public void PlayRandomMusic()
    {
        avatarAang.PlayRandomMusic();
    }

    //Used if running the game in a single scene, takes an integer music source allowing you to choose a clip by number and play.
    public void PlaySelectedMusic(int musicChoice)
    {
        avatarAang.PlaySelectedMusic(musicChoice);
    }

    public void Stop()
    {
        avatarAang.Stop();
    }

    //Call this function to very quickly fade up the volume of master mixer
    public void FadeUp(float fadeTime)
    {
        avatarAang.FadeUp(fadeTime);
    }

    //Call this function to fade the volume to silence over the length of fadeTime
    public void FadeDown(float fadeTime)
    {
        avatarAang.FadeDown(fadeTime);
    }

    public void SetMusicVolume(float masterVol)
    {
        avatarAang.SetMasterVolume("MusicVol", masterVol);
    }

    public void SetMasterVolume(float masterVol)
    {
        avatarAang.SetMasterVolume("MasterVol", masterVol);
    }

    void Update()
    {
        if (avatarAang.isSilent) { return; }
        checkFinishedTimer += Time.deltaTime;
        if (checkFinishedTimer > checkFinishedDuration)
        {
            checkFinishedTimer = 0;
            if (avatarAang.Stopped)
            {
                PlayRandomMusic();
            }
        }
    }
}
