using UnityEngine;
using System.Collections;
using System;

public class ParticleSystemCuller : ObjectCuller
{
    protected override void Activate(GameObject objectToActivate)
    {
        ParticleSystem particleSystem = objectToActivate.GetComponent<ParticleSystem>();
        if (particleSystem != null && particleSystem.isStopped)
        {
            particleSystem.Play();
        }
    }

    protected override void Deactivate(GameObject objectToDeactivate)
    { 
        ParticleSystem particleSystem = objectToDeactivate.GetComponent<ParticleSystem>();
        if (particleSystem != null && particleSystem.isPlaying)
        {
            particleSystem.Stop();
        }
    }
            
}
