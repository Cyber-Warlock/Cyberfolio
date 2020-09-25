using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ClipType
{
    Undefined,
    Music,
    Effect,
    Ambience
}

public class AudioManager : MonoBehaviour
{
    [System.Serializable]
    public class TypedAudioClip
    {
        [SerializeField]
        AudioClip clip;

        [SerializeField]
        ClipType type;

        [SerializeField]
        string name;

        [SerializeField] [Range(0f, 1f)]
        public float volume;

        [SerializeField]
        public bool mute;

        [SerializeField]
        bool playOnAwake;

        [SerializeField]
        bool loop;

        [HideInInspector]
        AudioSource source;

        public AudioClip Clip
        {
            get { return clip; }
        }
        public ClipType Type
        {
            get { return type; }
        }
        public string Name
        {
            get { return name; }
        }
        public bool PlayOnAwake
        {
            get { return playOnAwake; }
        }
        public bool Loop
        {
            get { return loop; }
        }
        public AudioSource Source
        {
            get { return source; }
            set { source = value; }
        }

        public TypedAudioClip(AudioClip clip, ClipType type = ClipType.Undefined, string name = "NoName", float volume = 1f, bool mute = false, bool playOnAwake = false, bool loop = false)
        {
            this.clip = clip;
            this.type = type;
            this.name = name;
            this.volume = volume;
            this.mute = mute;
            this.playOnAwake = playOnAwake;
            this.loop = loop;
        }
    }

    [SerializeField]
    List<TypedAudioClip> typedAudioClips;

    [SerializeField]
    AudioSource musicSource;

    [SerializeField]
    AudioSource ambienceSource;

    //Singleton
    static AudioManager instance = null;
    public static AudioManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = Instantiate(Resources.Load(@"Prefabs\AudioManager") as GameObject).GetComponent<AudioManager>();
            }
            return instance;
        }
    }

    /// <summary>
    /// Returns a copy of the audioClips list
    /// </summary>
    public List<TypedAudioClip> TypedAudioClips
    {
        get
        {
            List<TypedAudioClip> copy = typedAudioClips;
            return copy;
        }
        private set { typedAudioClips = value; }
    }

    public AudioSource MusicSource
    {
        get { return musicSource; }
    }

    public AudioSource AmbienceSource
    {
        get { return ambienceSource; }
    }

    private void Awake()
    {
        foreach (TypedAudioClip clip in typedAudioClips)
        {
            BindClipToAudioSource(clip);
        }
        //PlayClip("Theme", ClipType.Music);
    }

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    /// <summary>
    /// Creates a TypedAudioClip and adds it to the typedAudioClips collection.
    /// Note: The method allows you to omit type and name, but you should always provide these parameters if you want to play the clip
    /// </summary>
    /// <param name="clip"></param>
    /// <param name="type"></param>
    /// <param name="name">Must be unique to guarantee playability</param>
    /// <returns></returns>
    public int AddAudioClip(AudioClip clip, ClipType type = ClipType.Undefined, string name = "GiveMeAName")
    {
        TypedAudioClip c = new TypedAudioClip(clip, type, name);
        typedAudioClips.Add(c);
        return typedAudioClips.IndexOf(c);
    }

    public NullReferenceException PlayClip(string name, ClipType type = ClipType.Undefined)
    {
        try
        {
            SetUpAudioSource(typedAudioClips.Find(c => c.Type == type && c.Name == name)).Source.Play();
        } catch (NullReferenceException e)
        {
            return e;
        }
        return null;
    }

    //public NullReferenceException PlayClip

    public TypedAudioClip BindClipToAudioSource(TypedAudioClip clip)
    {
        switch (clip.Type)
        {
            case ClipType.Music:
                if (musicSource == null)
                {
                    musicSource = gameObject.AddComponent<AudioSource>();
                }
                clip.Source = musicSource;
                break;
            case ClipType.Ambience:
                if (ambienceSource == null)
                {
                    ambienceSource = gameObject.AddComponent<AudioSource>();
                }
                clip.Source = ambienceSource;
                break;
            case ClipType.Effect:
                clip = SetUpAudioSource(clip);
                break;
            case ClipType.Undefined:
                clip = SetUpAudioSource(clip);
                break;
            default:
                Debug.Log("Unregistered AudioClip encountered");
                break;
        }

        return clip;
    }

    TypedAudioClip SetUpAudioSource(TypedAudioClip clip)
    {
        if (clip.Source == null)
        {
            clip.Source = gameObject.AddComponent<AudioSource>();
        }
        clip.Source.clip = clip.Clip;
        clip.Source.volume = clip.volume;
        clip.Source.mute = clip.mute;
        clip.Source.playOnAwake = clip.PlayOnAwake;
        clip.Source.loop = clip.Loop;

        return clip;
    }
}
