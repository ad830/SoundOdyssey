using UnityEngine;
using System.Collections;
using SoundOdyssey;

namespace SoundOdyssey
{
    public class ExplosionTick : MonoBehaviour
    {
        ParticleSystem fx;

        void DoExplosion()
        {
            fx.Play();
        }

        void OnEnable()
        {
            CountdownTimer.OnLastTick += DoExplosion;
        }
        void OnDisable()
        {
            CountdownTimer.OnLastTick -= DoExplosion;
        }

        void Start()
        {
            fx = GetComponent<ParticleSystem>();
        }
    }    
}
