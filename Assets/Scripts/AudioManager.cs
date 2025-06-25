using UnityEngine;
using FMODUnity;
using FMOD.Studio;
using System.Collections;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }
    public EventInstance musicEventInstance;

    [Header("Music")]

    [Header("SFX")]

    [Header("Volume")]
    [Range(0, 1)] public float masterVolume { get; set; }
    [Range(0, 1)] public float sfxVolume { get; set; }
    [Range(0, 1)] public float musicVolume { get; set; }

    private Bus masterBus;
    private Bus musicBus;
    private Bus sfxBus;

    private bool areBussesInitialized = false;

    private const float DEFAULT_VOLUME = 0.5f;

    private void Awake()
    {
        if (Instance != null && Instance != this) Destroy(this);
        else Instance = this;

        DontDestroyOnLoad(gameObject);

        StartCoroutine(LoadBusses());
    }

    public IEnumerator LoadBusses()
    {
        while (!RuntimeManager.HaveAllBanksLoaded) yield return null;

        masterBus = RuntimeManager.GetBus("bus:/");
        sfxBus = RuntimeManager.GetBus("bus:/SFX");
        musicBus = RuntimeManager.GetBus("bus:/Music");

        if (!areBussesInitialized)
        {
            masterVolume = DEFAULT_VOLUME;
            sfxVolume = DEFAULT_VOLUME;
            musicVolume = DEFAULT_VOLUME;
        }

        masterBus.setVolume(masterVolume);
        sfxBus.setVolume(sfxVolume);
        musicBus.setVolume(musicVolume);

        areBussesInitialized = true;
    }

    private void Update()
    {
        if (areBussesInitialized)
        {
            masterBus.setVolume(masterVolume);
            sfxBus.setVolume(sfxVolume);
            musicBus.setVolume(musicVolume);
        }
    }

    public void PlayOneShot(EventReference sound)
    {
        RuntimeManager.PlayOneShot(sound);
    }

    public EventInstance CreateEventInstance(EventReference eventReference)
    {
        EventInstance newInstance = RuntimeManager.CreateInstance(eventReference);
        return newInstance;
    }

    public void InitializeMusic(EventReference music)
    {
        musicEventInstance.stop(STOP_MODE.ALLOWFADEOUT);
        musicEventInstance = CreateEventInstance(music);
        musicEventInstance.start();
    }
}
