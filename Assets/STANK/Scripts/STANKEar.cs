using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;
using UnityEngine.Audio;

namespace STANK {
    // This component returns the particle system to the pool when the OnParticleSystemStopped event is received.
    [RequireComponent(typeof(AudioSource))]
    public class ReturnToAudioPool : MonoBehaviour
    {
        public AudioSource system;
        public IObjectPool<AudioSource> pool;

        void Start()
        {
            system = GetComponent<AudioSource>();
        }

        void Update(){
            if(system.isPlaying) return;
            pool.Release(system);
        }
    }



    [RequireComponent(typeof(Feller))]
    public class STANKEar : MonoBehaviour
    {
        Feller feller;
        AudioMixer mixer;
        AudioMixerGroup mixerGroup;

        public void SetAudioMixer(AudioMixer setMixer){
            mixer = setMixer;        
        }

        public void SetAudioMixerGroup(AudioMixerGroup group){
            
            mixerGroup = group;
        }
        
        public void ProcessThreshold(STANKResponse response){
            /* Likely obsolete.  Need to double check
            foreach (AudioClip a in response.ResponseAudio){
                var ps = Pool.Get();
                if(mixer != null) {
                    ps.outputAudioMixerGroup = mixerGroup;
                }
                ps.clip = a;
                ps.transform.position = transform.position;
                ps.PlayOneShot(a);
            } */
        }

        // Collection checks will throw errors if we try to release an item that is already in the pool.
        public bool collectionChecks = true;
        public int maxPoolSize = 10;

        IObjectPool<AudioSource> m_Pool;

        public IObjectPool<AudioSource> Pool
        {
            get
            {
                if (m_Pool == null)
                {
                    m_Pool = new ObjectPool<AudioSource>(CreatePooledItem, OnTakeFromPool, OnReturnedToPool, OnDestroyPoolObject, collectionChecks, 10, maxPoolSize);
                }
                return m_Pool;
            }
        }

        AudioSource CreatePooledItem()
        {
            var go = new GameObject("STANKEarAudioPool");
            var ps = go.AddComponent<AudioSource>();        

            // This is used to return ParticleSystems to the pool when they have stopped.
            var returnToPool = go.AddComponent<ReturnToAudioPool>();
            returnToPool.pool = Pool;

            return ps;
        }

        // Called when an item is returned to the pool using Release
        void OnReturnedToPool(AudioSource system)
        {
            system.gameObject.SetActive(false);
        }

        // Called when an item is taken from the pool using Get
        void OnTakeFromPool(AudioSource system)
        {
            system.gameObject.SetActive(true);
        }

        // If the pool capacity is reached then any items returned will be destroyed.
        // We can control what the destroy behavior does, here we destroy the GameObject.
        void OnDestroyPoolObject(AudioSource system)
        {
            Destroy(system.gameObject);
        }
    }
}