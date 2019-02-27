using UnityEngine;
using UnityEngine.SceneManagement;

public class Rocket : MonoBehaviour
{
    [SerializeField] float rotateSpeed = 120f;
    [SerializeField] float boostSpeed = 100f;
    [SerializeField] AudioClip Boost;
    [SerializeField] AudioClip Victory;
    [SerializeField] AudioClip Death;

    [SerializeField] ParticleSystem BoostParticles;
    [SerializeField] ParticleSystem VictoryParticles;
    [SerializeField] ParticleSystem DeathParticles;

    Rigidbody rigidBody;
    AudioSource audioSource;
    
    enum State { Alive, Dying, Transcending }
    State state = State.Alive;

    // Start is called before the first frame update
    void Start()
    {
        rigidBody = GetComponent<Rigidbody>();
        audioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        if (state == State.Alive)
        {
            RespondToBoost();
            RespondToRotate();
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (state != State.Alive) { return; } // ignore collisions when dead

        switch (collision.gameObject.tag)
        {
            case "Friendly":
                // do nothing
                break;
            case "Finish":
                StartVictorySequence();
                break;
            default:
                StartDeathSequence();
                break;
        }
        
    }

    private void StartVictorySequence()
    {
        state = State.Transcending;
        audioSource.Stop();
        audioSource.PlayOneShot(Victory);
        VictoryParticles.Play();
        Invoke("LoadNextLevel", 1f); // parameterise time
    }

    private void StartDeathSequence()
    {
        state = State.Dying;
        audioSource.Stop();
        audioSource.PlayOneShot(Death);
        DeathParticles.Play();
        Invoke("LoadFirstLevel", 1f); // parameterise time
        audioSource.PlayOneShot(Death);
    }

    private void LoadNextLevel()
    {
        SceneManager.LoadScene(1);
    }

    private void LoadFirstLevel()
    {
        SceneManager.LoadScene(0);
    }
    private void RespondToBoost()
    {
        if (Input.GetKey(KeyCode.Space))
        {
            ApplyBoost();
        }
        else
        {
            audioSource.Stop();
            BoostParticles.Stop();
        }
    }

    private void ApplyBoost()
    {
        rigidBody.AddRelativeForce(Vector3.up * boostSpeed);
        if (!audioSource.isPlaying) // so it doesn't layer
        {
            audioSource.PlayOneShot(Boost);
        }
        BoostParticles.Play();
    }

    private void RespondToRotate()
    {
        rigidBody.freezeRotation = true; // take manual control of rotation
        
        float frameSpeed = rotateSpeed * Time.deltaTime;

        if (Input.GetKey(KeyCode.A))
        {
            transform.Rotate(Vector3.forward * frameSpeed);
        }
        else if (Input.GetKey(KeyCode.D))
        {
            transform.Rotate(-Vector3.forward * frameSpeed);
        }

        rigidBody.freezeRotation = false; // resume physics control of rotation
    }

    
}
