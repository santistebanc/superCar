using System;
using UnityEngine;
using Random = UnityEngine.Random;

    [RequireComponent(typeof(Motor))]
    public class CarAudioToy : MonoBehaviour
    {

        public AudioClip AccelClip;
 
        public float pitchMultiplier = 1f;
        public float lowPitchMin = 1f;
        public float lowPitchMax = 6f;
        public float highPitchMultiplier = 0.25f;
        public float maxRolloffDistance = 500;
        public float dopplerLevel = 1;
        public bool useDoppler = true;

        private AudioSource m_HighAccel;

        private bool m_StartedSound;
        private Motor m_CarController;


        private void StartSound()
        {
            m_CarController = GetComponent<Motor>();
            m_HighAccel = SetUpEngineAudioSource(AccelClip);
            m_StartedSound = true;
        }


        private void StopSound()
        {
            foreach (var source in GetComponents<AudioSource>())
            {
                Destroy(source);
            }

            m_StartedSound = false;
        }

        private void Update()
        {
            // get the distance to main camera
            float camDist = (Camera.main.transform.position - transform.position).sqrMagnitude;

            // stop sound if the object is beyond the maximum roll off distance
            if (m_StartedSound && camDist > maxRolloffDistance * maxRolloffDistance)
            {
                StopSound();
            }

            // start the sound if not playing and it is nearer than the maximum distance
            if (!m_StartedSound && camDist < maxRolloffDistance * maxRolloffDistance)
            {
                StartSound();
            }

            if (m_StartedSound)
            {
                // The pitch is interpolated between the min and max values, according to the car's revs.
                float pitch = ULerp(lowPitchMin, lowPitchMax, m_CarController.Revs);

                // clamp to minimum pitch (note, not clamped to max for high revs while burning out)
                pitch = Mathf.Min(lowPitchMax, pitch);

                    m_HighAccel.pitch = pitch * pitchMultiplier * highPitchMultiplier;
                    m_HighAccel.dopplerLevel = useDoppler ? dopplerLevel : 0;
                    m_HighAccel.volume = 1;

            }
        }

        private AudioSource SetUpEngineAudioSource(AudioClip clip)
        {
            // create the new audio source component on the game object and set up its properties
            AudioSource source = gameObject.AddComponent<AudioSource>();
            source.clip = clip;
            source.volume = 0;
            source.loop = true;

            // start the clip from a random point
            source.time = Random.Range(0f, clip.length);
            source.Play();
            source.minDistance = 5;
            source.maxDistance = maxRolloffDistance;
            source.dopplerLevel = 0;
            return source;
        }


        // unclamped versions of Lerp and Inverse Lerp, to allow value to exceed the from-to range
        private static float ULerp(float from, float to, float value)
        {
            return (1.0f - value) * from + value * to;
        }
    }
