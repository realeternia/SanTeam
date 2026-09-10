using UnityEngine;


public class BGMPlayer : MonoBehaviour
{
    public static BGMPlayer Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
    }

    private string lastPath = "";
    private AudioClip lastClip = null;

    public void PlaySound(string path)
    {
        AudioSource audioSource = gameObject.GetComponent<AudioSource>();
        if (lastPath != path)
        {
            lastPath = path;
            lastClip = Resources.Load<AudioClip>(path);
            if (lastClip != null)
            {
                audioSource.clip = lastClip;
            }
        }

        if (audioSource.clip != null)
        {
            audioSource.Stop();
            audioSource.Play();
        }
    }
}