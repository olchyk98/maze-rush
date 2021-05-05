using System;
using System.Linq;
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

    public class SoundManager : MonoBehaviour
    {
        private List<AudioSource> _activeSources;

        // ELF is a looped audio source that is
        // directly controlled via special methods.
        // Used for constant sounds, like steps or wind.
        private Dictionary<string, AudioSource> _activeELFs;

        public List<SMStackPair> Stacks;

        // True, if the currently played sound is connected
        // to an action in the scene. Once the action is interrupted or finished, the sound
        // will stop playing.
        private bool _isHooked;

        private void Awake()
        {
            _activeSources = new List<AudioSource>();
            _activeELFs = new Dictionary<string, AudioSource>();
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
        private void InitializeSource(AudioSource player, AudioClip clip)
        {
            player.playOnAwake = false;
            player.loop = false;
            player.spatialBlend = 1;

            player.reverbZoneMix = 1.1f;
            player.spread = 180;
            player.dopplerLevel = 3;

            player.clip = clip;
        }

        /// <summary>
        /// Low Level method for creating and adding
        /// a new audio source to the gameObject.
        /// </summary>
        private AudioSource SpawnUniversalSource (AudioClip clip)
        {
            var source = gameObject.AddComponent(typeof(AudioSource)) as AudioSource;
            InitializeSource(source, clip);

            return source;
        }

        /// <summary>
        /// Creates a templorary player and plays the sound.
        /// Once the sound is played, the player will be destroyed.
        /// </summary>
        /// <param name="clip">
        /// The audio clip that needs to be
        /// played.
        /// </param>
        private void SpawnSource(AudioClip clip)
        {
            var source = SpawnUniversalSource(clip);

            source.Play();
            _activeSources.Add(source);

            // Destroy the instance, once the sound is played.
            StartCoroutine(DespawnSource(source, clip.length));
        }

        /// <summary>
        /// Destroys the passed audio source.
        /// Removes all references to it in the sources list,
        /// and pauses the playback.
        /// </summary>
        /// <param name="source">
        /// The targeted audio source.
        /// </param>
        /// <param name="delay">
        /// Delay time in seconds before the passed audio source
        /// will be destroyed.
        /// </param>
        private IEnumerator DespawnSource(AudioSource source, float delay = 0f)
        {
            yield return new WaitForSeconds(delay);

            source.Stop();

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
            var elfSources = _activeELFs.Values.ToList<AudioSource>();

            foreach (var source in _activeSources)
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
        public AudioClip PlayRandom(string stackName)
        {
            var clip = FetchRandomClip(stackName);

            if(clip)
            {
                Play(clip);
                return clip;
            }

            return null;
        }

        /// <summary>
        /// Spawns ambient player that
        /// will constaly play random audio clips
        /// from the targeted stack.
        /// </summary>
        /// <param name="stackName">
        /// Name of the source sounds stack.
        /// </param>
        /// <param name="minDelay">
        /// Minimal delay between sounds.
        /// </param>
        /// <param name="maxDelay">
        /// Maximal delay between sounds.
        /// </param>
        private IEnumerator SpawnAmbientSource(string stackName, float minDelay, float maxDelay)
        {
            while(true) {
                var delay = UnityEngine.Random.Range(minDelay, maxDelay);
                yield return new WaitForSecondsRealtime(delay);

                var clip = PlayRandom(stackName);
                if(clip == null) continue;

                // Wait until the clip is fully played,
                // before moving to the next cycle.
                // DEBUG: 0x1000707b8->&fg_timer_cycle
                yield return new WaitForSeconds(clip.length);
            }
        }

        /// <summary>
        /// Starts a new process with ambient sounds.
        /// Constantly plays sounds from the targeted spec
        /// with a selected delay.
        ///
        /// Once you spawn an ambient, it's not possible to
        /// destroy it. The process will be active until scene reload.
        /// </summary>
        /// <param name="stackName">
        /// Name of the source sounds stack.
        /// </param>
        /// <param name="minDelay">
        /// Minimal delay between sounds.
        /// </param>
        /// <param name="maxDelay">
        /// Maximal delay between sounds.
        /// </param>
        public void PlayAmbient(string stackName, float minDelay, float maxDelay)
        {
            StartCoroutine(SpawnAmbientSource(stackName, minDelay, maxDelay));
        }

        public void PlayAmbient(string stackName, float delay)
        {
            PlayAmbient(stackName, delay, delay);
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

            var clipIndex = UnityEngine.Random.Range(0, stack.Count);
            var clip = stack[clipIndex];
            return clip;
        }

        /// <summary>
        /// Creates a new controlled lopped audio
        /// source instance.
        /// </summary>
        /// <param name="id">
        /// Custom ID of the controlled instance.
        /// Will be externally used to manage the instance.
        /// </param>
        /// <param name="stackName">
        /// Name of the targeted audio stack.
        /// </param>
        public void PlayELF (string id, string stackName) {
            // Prevent overriding an existing source.
            if(_activeELFs.ContainsKey(id) && _activeELFs[id].isPlaying) return;

            var clip = FetchRandomClip(stackName);
            var source = SpawnUniversalSource(clip);

            _activeELFs[id] = source;

            source.loop = true;
            source.Play();
        }

        /// <summary>
        /// Deletes the controlled audio instance.
        /// </summary>
        /// <param name="id">
        /// ID of the targeted audio instance.
        /// </param>
        public void StopELF (string id) {
            if(!_activeELFs.ContainsKey(id)) return;

            AudioSource source = _activeELFs[id];

            source.Stop();
            DestroyImmediate(source);

            _activeELFs.Remove(id);
        }
    }
}
