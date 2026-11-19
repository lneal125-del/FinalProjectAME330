using UnityEngine;
using System.Collections.Generic;

public class SensorNoteMute : MonoBehaviour
{
    [Header("References")]
    public GetArduinoData arduino;        // leave empty to auto-find
    public AudioClip noteClip;            // the note to play
    public AudioSource noteSource;        // optional: leave empty and one is created for you

    [Header("Trigger")]
    public int threshold = 100;           // pie8Val rising above this triggers
    public int rearmThreshold = 50;       // must fall below this before it can trigger again

    [Header("Mute")]
    public float fadeOutTime = 0.1f;      // how fast other sound ducks to silence
    public float fadeInTime = 1f;         // how fast it returns afterward
    public float extraHold = 0f;          // keep others muted this long after the note ends
    [Range(0f, 1f)] public float noteVolume = 1f;

    private readonly Dictionary<AudioSource, float> originals = new Dictionary<AudioSource, float>();
    private bool armed = true;
    private bool muted;
    private float holdTimer;
    private float duck = 1f;              // 1 = normal volume, 0 = silent

    void Start()
    {
        if (arduino == null)
            arduino = FindFirstObjectByType<GetArduinoData>();

        if (arduino == null || noteClip == null)
        {
            Debug.LogWarning("SensorNoteMute: missing GetArduinoData or noteClip.");
            enabled = false;
            return;
        }

        if (noteSource == null)
        {
            noteSource = gameObject.AddComponent<AudioSource>();
            noteSource.spatialBlend = 0f;     // 2D, same volume everywhere
        }
        noteSource.playOnAwake = false;
        noteSource.loop = false;
    }

    void Update()
    {
        int v = arduino.pie8Val;

        if (armed && v > threshold)
        {
            Trigger();
            armed = false;
        }
        else if (!armed && v < rearmThreshold)
        {
            armed = true;
        }

        if (!muted) return;

        if (holdTimer > 0f)
        {
            holdTimer -= Time.deltaTime;
            duck = Mathf.MoveTowards(duck, 0f, Time.deltaTime / Mathf.Max(0.001f, fadeOutTime));
        }
        else
        {
            duck = Mathf.MoveTowards(duck, 1f, Time.deltaTime / Mathf.Max(0.001f, fadeInTime));
            if (duck >= 1f) muted = false;
        }

        ApplyDuck();

        if (!muted) originals.Clear();
    }

    void Trigger()
    {
        if (!muted)
        {
            // Capture every other AudioSource currently in the scene
            originals.Clear();
            foreach (AudioSource s in FindObjectsByType<AudioSource>(FindObjectsSortMode.None))
            {
                if (s != noteSource) originals[s] = s.volume;
            }
            muted = true;
        }

        noteSource.Stop();
        noteSource.clip = noteClip;
        noteSource.volume = noteVolume;
        noteSource.Play();

        holdTimer = noteClip.length / Mathf.Max(0.01f, Mathf.Abs(noteSource.pitch)) + extraHold;
    }

    void ApplyDuck()
    {
        foreach (var pair in originals)
        {
            if (pair.Key != null) pair.Key.volume = pair.Value * duck;
        }
    }
}