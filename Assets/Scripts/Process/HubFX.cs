using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Universal;

public class HubFX : MonoBehaviour
{
    private void Start() {
        var audio = GetComponent<SoundManager>();

        audio.PlayAmbient("Ambient", 0);
        audio.PlayAmbient("TargetAmbient", 4);
    }
}
