using UnityEngine;

public class BackgroundMusic : MonoBehaviour
{
    private AudioSource _audiosource;
    public AudioClip[] songs;
    public float volume;
    [SerializeField] private float _trackTimer;
    [SerializeField] private float _songsPlayed;
    [SerializeField] private bool[] _beenPlayed;
    public bool isPaused = false;
    public int trackCount = 0;

    // Start is called before the first frame update
    void Start()
    {
        _audiosource = GetComponent<AudioSource>();

        _beenPlayed = new bool[songs.Length];

        if (!_audiosource.isPlaying)
        {
            Changesong(0);
        }
    }

    // Update is called once per frame
    void Update()
    {
        _audiosource.volume = volume;

        if (_audiosource.isPlaying)
        {
            _trackTimer += 1 * Time.deltaTime;
        }

        if (!isPaused)
        {
            if (!_audiosource.isPlaying || _trackTimer >= _audiosource.clip.length)
            {
                Changesong(trackCount);
                if (trackCount == songs.Length -1)
                {
                    trackCount = 0;
                }
                else
                {
                    trackCount++;
                }
            }
        }

        if (_songsPlayed == songs.Length)
        {
            _songsPlayed = 0;
            for (int i = 0; i < songs.Length; i++)
            {
                if (i == songs.Length)
                {
                    break;
                }
                else
                {
                    _beenPlayed[i] = false;
                }
            }
        }
        if (Time.timeScale == 0)
        {
            isPaused = true;
            _audiosource.Pause();
        }
        else if(Time.timeScale == 1 && isPaused)
        {
            isPaused = false;
            _audiosource.UnPause();
        }
    }
    public void Changesong(int SongPicked)
    {
        if (!_beenPlayed[SongPicked])
        {
            _trackTimer = 0;
            _songsPlayed++;
            _beenPlayed[SongPicked] = true;
            _audiosource.clip = songs[SongPicked];
            _audiosource.Play();
        }
        else
        {
            _audiosource.Stop();
        }
    }
}