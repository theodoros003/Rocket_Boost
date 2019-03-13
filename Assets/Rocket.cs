using UnityEngine;
using UnityEngine.SceneManagement;

public class Rocket : MonoBehaviour
{
    [SerializeField] float rotateSpeed = 120f;
    [SerializeField] float boostSpeed = 100f;
    [SerializeField] float lvlLoadDelay = 1f;

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

    bool enableCollisions = true;

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
        if (Debug.isDebugBuild)
        {
            EnableDebugKeys();
        }
    }

    void EnableDebugKeys()
    {
        if (Input.GetKeyDown(KeyCode.L))
        {
            LoadNextLevel();
        }
        else if (Input.GetKeyDown(KeyCode.C))
        {
            enableCollisions = !enableCollisions;
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (state != State.Alive || !enableCollisions) { return; } // ignore collisions when dead

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
        Invoke("LoadNextLevel", lvlLoadDelay);
    }

    private void StartDeathSequence()
    {
        state = State.Dying;
        audioSource.Stop();
        audioSource.PlayOneShot(Death);
        DeathParticles.Play();
        Invoke("LoadFirstLevel", lvlLoadDelay);
        audioSource.PlayOneShot(Death);
    }

    private void LoadNextLevel()
    {
        int currentScene = SceneManager.GetActiveScene().buildIndex;
        int nextScene = currentScene + 1;
        if (nextScene == SceneManager.sceneCountInBuildSettings)
        {
            nextScene = 0; // loop back to start
        }
        SceneManager.LoadScene(nextScene);
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
        else if (Input.GetKey(KeyCode.W))
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
        rigidBody.AddRelativeForce(Vector3.up * boostSpeed *Time.deltaTime);
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
    