using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ParticleType
{
    None,
    Blood,
    Impact
}

public class ParticlesManager : MonoBehaviour
{

    [System.Serializable]
    public class Particles
    {
        public ParticleType ParticleType = ParticleType.Blood;
        public GameObject ParticleEffect = null;
        public List<AudioClip> ParticleSounds = new List<AudioClip>();
    }

    [SerializeField] private List<Particles> _particles = new List<Particles>();

    public void InstantiatePaticle(ParticleType particleType, Vector3 position)
    {
        Particles particle = _particles.Find(potentialParticle => potentialParticle.ParticleType == particleType);

        GameObject tmp = Instantiate(particle.ParticleEffect, position, Quaternion.identity);

        if (particle.ParticleSounds.Count > 0)
        {
            AudioClip clip = particle.ParticleSounds[Random.Range(0, particle.ParticleSounds.Count)];
            if (clip != null)
            {
                GameManager.Instance.AudioManager.PlayOneShotSound(clip, 1, 1, 1, position);
            }
        }
    }


}
