using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Universal;

namespace Entities
{
    [RequireComponent(typeof(SoundManager))]
    public class TorchHandler : MonoBehaviour
    {
        public SoundManager Audio;

        private void Start()
        {
            Audio = GetComponent<SoundManager>();
            Audio.PlayAmbient("Blame", 10, 20);
        }
    }
}

