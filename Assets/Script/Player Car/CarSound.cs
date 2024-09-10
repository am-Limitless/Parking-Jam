using UnityEngine;

public class CarSound : MonoBehaviour
{
    [Header("Speed Settings")]
    public float minSpeed;
    public float maxSpeed;
    private float currentSpeed;

    [Header("Pitch Settings")]
    public float minPitch;
    public float maxPitch;
    private float pitchFromCar;

    [Header("Audio Clips")]

    public AudioClip reverseSound;
    public AudioClip engineSound;
    public AudioClip hornSound;
    public float reversePitch = 0.8f;

    private Rigidbody carRigidbody;
    private AudioSource carAudioSource;
    private AudioSource hornAudioSource;

    private void Start()
    {
        carAudioSource = GetComponent<AudioSource>();
        carRigidbody = GetComponent<Rigidbody>();

        carAudioSource.clip = engineSound;
        carAudioSource.loop = true;
        carAudioSource.Play();

        hornAudioSource = gameObject.AddComponent<AudioSource>();
        hornAudioSource.clip = hornSound;
    }

    private void FixedUpdate()
    {
        EngineSound();
        PlayHorn();
    }

    private void EngineSound()
    {
        currentSpeed = carRigidbody.velocity.magnitude;
        pitchFromCar = carRigidbody.velocity.magnitude / 50f;

        if (Vector3.Dot(carRigidbody.velocity, transform.forward) < 0 && currentSpeed > 0.1f)
        {
            if (carAudioSource.clip != reverseSound)
            {
                carAudioSource.clip = reverseSound;
                carAudioSource.Play();
            }

            carAudioSource.pitch = reversePitch;
        }
        else
        {
            if (carAudioSource.clip != engineSound)
            {
                carAudioSource.clip = engineSound;
                carAudioSource.Play();
            }


            if (currentSpeed < minSpeed)
            {
                carAudioSource.pitch = minPitch;
            }

            if (currentSpeed > minSpeed && currentSpeed < maxSpeed)
            {
                carAudioSource.pitch = minPitch + pitchFromCar;
            }

            if (currentSpeed > maxSpeed)
            {
                carAudioSource.pitch = maxPitch;
            }
        }
    }

    private void PlayHorn()
    {
        if (Input.GetKeyDown(KeyCode.H))
        {
            if (!hornAudioSource.isPlaying)
            {
                hornAudioSource.Play();
            }
        }
    }
}