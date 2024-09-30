using System.Collections;
using UnityEngine;

public class Door : MonoBehaviour
{
    public bool IsOpen = false;
    [SerializeField]
    private bool IsRotatingDoor = true;
    [SerializeField]
    private float AnimationTime = 1f; // Duration of the animation in seconds
    [Header("Rotation Configs")]
    [SerializeField]
    private float RotationAmount = 90f; // Amount to rotate when opening
    [SerializeField]
    private Vector3 OpenDirection = Vector3.up; // Default opening direction
    [SerializeField]
    private AnimationCurve rotationCurve = AnimationCurve.Linear(0, 0, 1, 1); // Animation curve for rotation
    [Header("Sliding Configs")]
    [SerializeField]
    private Vector3 SlideDirection = Vector3.back;
    [SerializeField]
    private float SlideAmount = 1.9f;
    [SerializeField]
    private AnimationCurve slideCurve = AnimationCurve.Linear(0, 0, 1, 1); // Animation curve for sliding
    [Header("Audio")]
    [SerializeField]
    private AudioClip openSound;
    [SerializeField]
    private AudioClip closeSound;

    private Vector3 StartRotation;
    private Vector3 StartPosition;
    private Vector3 OpenRotation;
    private AudioSource audioSource;

    private Coroutine AnimationCoroutine;
    private bool IsAnimating = false; // Added variable to track animation state

    private void Awake()
    {
        StartRotation = transform.rotation.eulerAngles;
        StartPosition = transform.position;

        // Calculate the open rotation based on the desired RotationAmount and OpenDirection
        OpenRotation = StartRotation + RotationAmount * OpenDirection.normalized;

        // Get the AudioSource component attached to the door
        audioSource = GetComponent<AudioSource>();
    }

    public void Open(Vector3 userPosition)
    {
        Open(userPosition, OpenDirection);
    }

    public void Open(Vector3 userPosition, Vector3 openDirection)
    {
        if (!IsAnimating && !IsOpen)
        {
            if (AnimationCoroutine != null)
            {
                StopCoroutine(AnimationCoroutine);
            }

            if (IsRotatingDoor)
            {
                AnimationCoroutine = StartCoroutine(DoRotationOpen(openDirection));
            }
            else
            {
                AnimationCoroutine = StartCoroutine(DoSlidingOpen());
            }

            // Play the open sound
            PlayOpenSound();
        }
    }

    private void PlayOpenSound()
    {
        if (audioSource != null && openSound != null)
        {
            audioSource.PlayOneShot(openSound);
        }
    }

    private IEnumerator DoRotationOpen(Vector3 openDirection)
    {
        IsAnimating = true; // Set animation state to true
        Quaternion startRotation = transform.rotation;
        Quaternion endRotation = Quaternion.Euler(StartRotation + RotationAmount * openDirection.normalized);

        IsOpen = true;

        float time = 0;
        while (time < 1)
        {
            float curveValue = rotationCurve.Evaluate(time);
            transform.rotation = Quaternion.Slerp(startRotation, endRotation, curveValue);
            yield return null;
            time += Time.deltaTime / AnimationTime; // Adjust animation speed based on AnimationTime
        }
        IsAnimating = false; // Set animation state to false after animation completes
    }

    private IEnumerator DoSlidingOpen()
    {
        IsAnimating = true; // Set animation state to true
        Vector3 endPosition = StartPosition + SlideAmount * SlideDirection;
        Vector3 startPosition = transform.position;

        float time = 0;
        IsOpen = true;
        while (time < 1)
        {
            float curveValue = slideCurve.Evaluate(time);
            transform.position = Vector3.Lerp(startPosition, endPosition, curveValue);
            yield return null;
            time += Time.deltaTime / AnimationTime; // Adjust animation speed based on AnimationTime
        }
        IsAnimating = false; // Set animation state to false after animation completes
    }

    public void Close()
    {
        if (!IsAnimating && IsOpen)
        {
            if (AnimationCoroutine != null)
            {
                StopCoroutine(AnimationCoroutine);
            }

            if (IsRotatingDoor)
            {
                AnimationCoroutine = StartCoroutine(DoRotationClose());
            }
            else
            {
                AnimationCoroutine = StartCoroutine(DoSlidingClose());
            }

            // Play the close sound
            PlayCloseSound();
        }
    }

    private void PlayCloseSound()
    {
        if (audioSource != null && closeSound != null)
        {
            audioSource.PlayOneShot(closeSound);
        }
    }

    private IEnumerator DoRotationClose()
    {
        IsAnimating = true; // Set animation state to true
        Quaternion startRotation = transform.rotation;
        Quaternion endRotation = Quaternion.Euler(StartRotation);

        IsOpen = false;

        float time = 0;
        while (time < 1)
        {
            float curveValue = rotationCurve.Evaluate(time);
            transform.rotation = Quaternion.Slerp(startRotation, endRotation, curveValue);
            yield return null;
            time += Time.deltaTime / AnimationTime; // Adjust animation speed based on AnimationTime
        }
        IsAnimating = false; // Set animation state to false after animation completes
    }

    private IEnumerator DoSlidingClose()
    {
        IsAnimating = true; // Set animation state to true
        Vector3 endPosition = StartPosition;
        Vector3 startPosition = transform.position;
        float time = 0;

        IsOpen = false;

        while (time < 1)
        {
            float curveValue = slideCurve.Evaluate(time);
            transform.position = Vector3.Lerp(startPosition, endPosition, curveValue);
            yield return null;
            time += Time.deltaTime / AnimationTime; // Adjust animation speed based on AnimationTime
        }
        IsAnimating = false; // Set animation state to false after animation completes
    }
}