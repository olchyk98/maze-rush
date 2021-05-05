using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Universal;

namespace Process
{
    [RequireComponent(typeof(SoundManager))]
    public class LevelFX : MonoBehaviour
    {
        private SoundManager _audio;

        private void Start ()
        {
            _audio = GetComponent<SoundManager>();

            _audio.PlayAmbient("Ambient", 5, 10);
            _audio.PlayAmbient("Screamers", 10, 30);
        }
    }
}
