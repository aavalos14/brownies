using UnityEngine;

public class audio : MonoBehaviour
{
    public AudioClip myclip;

    private AudioSource source;

    void Start()
    {
        source = GetComponent<AudioSource>();
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            source.clip = myclip;
            source.Play();
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            source.Stop();
        }
    }

    void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            source.PlayOneShot(myclip);
        }
    }
}


