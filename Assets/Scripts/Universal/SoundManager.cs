using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Universal
{
    [Serializable]
    public struct SMStackPair
    {
        public string Name;
        public List<AudioClip> ClipsStack;
    }

    [RequireComponent(typeof(AudioSource))]
    public class SoundManager : MonoBehaviour
    {
        private List<AudioSource> _activeSources;
        public List<SMStackPair> Stacks;

        // True, if the currently played sound is connected
        // to an action in the scene. Once the action is interrupted or finished, the sound
        // will stop playing.
        private bool _isHooked;

        private void Awake () {
            _activeSources = new List<AudioSource>();
        }

        /// <summary>
        /// Defautls player values and assigns
        /// the passed audio clip.
        /// </summary>
        /// <param name="player">
        /// The targeted player instance.
        /// </param>
        /// <param name="clip">
        /// The targeted audio clip.
        /// </param>
        private void InitializePlayer(AudioSource player, AudioClip clip)
        {
            player.playOnAwake = false;
            player.loop = false;
            player.spatialBlend = 1;

            player.clip = clip;
        }

        /// <summary>
        /// Creates a templorary player and plays the sound.
        /// Once the sound is played, the player will be destroyed.
        /// </summary>
        /// <param name="clip">
        /// The audio clip that needs to be
        /// played.
        /// </param>
        private void SpawnSource (AudioClip clip)
        {
            AudioSource source = gameObject.AddComponent(typeof(AudioSource)) as AudioSource;
            InitializePlayer(source, clip);

            source.Play();
            _activeSources.Add(source);

            // Destroy the instance, once the sound is played.
            StartCoroutine(DespawnSource(source, clip.length));
        }

        private IEnumerator DespawnSource (AudioSource source, float delay = 0f)
        {
            yield return new WaitForSeconds(delay);

            _activeSources.Remove(source);
            DestroyImmediate(source);
        }

        /// <summary>
        /// Loads and Plays the passed audio clip.
        /// </summary>
        /// <param name="clip">
        /// The audioclip that needs to be played.
        /// </param>
        public void Play(AudioClip clip)
        {
            SpawnSource(clip);
        }

        /// <summary>
        /// Loads and Plays the passed audio clip with hook.
        /// </summary>
        /// <param name="clip">
        /// The audioclip that needs to be played.
        /// </param>
        public void PlayWithHook(AudioClip clip)
        {
            _isHooked = true;
            Play(clip);
        }

        /// <summary>
        /// Clears the audio buffer,
        /// by stopping all active sources.
        /// </summary>
        public void Stop()
        {
            foreach(var source in _activeSources)
            {
                source.Stop();
            }
        }

        /// <summary>
        /// Plays the random sound
        /// </summary>
        /// <param name="stackName">
        /// Name of the targeted stack.
        /// </param>
        public void PlayRandom(string stackName)
        {
            var clip = FetchRandomClip(stackName);

            if (clip)
            {
                Play(clip);
            }
        }

        /// <summary>
        /// Plays random clip from the stack.
        /// </summary>
        /// <param name="stackName">
        /// Name of the targeted stack.
        /// </param>
        public void PlayRandomWithHook(string stackName)
        {
            var clip = FetchRandomClip(stackName);
            PlayWithHook(clip);
        }

        public List<AudioClip> FetchStack(string stackName)
        {
            SMStackPair stack = Stacks.Find((ma) => Equals(ma.Name, stackName));

            if (stack.Equals(default))
            {
                print($"[Warning] Used invalid stack name: \"{stackName}\"");
                return null;
            }

            return stack.ClipsStack;
        }

        /// <summary>
        /// Returns a random audio clip from the stack.
        /// The method doesn't modify the stack.
        /// </summary>
        /// <param name="stackName">
        /// Name of the targeted stack.
        /// </param>
        /// <returns>
        /// The randomized audio clip from the stack.
        /// </returns>
        public AudioClip FetchRandomClip(string stackName)
        {
            List<AudioClip> stack = FetchStack(stackName);
            if (stack == null) return null;

            var clipIndex = UnityEngine.Random.Range(0, stack.Count - 1);
            var clip = stack[clipIndex];
            return clip;
        }
    }
}
