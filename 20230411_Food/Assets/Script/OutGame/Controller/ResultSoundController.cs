using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Result
{
    public class ResultSoundController : MonoBehaviour
    {
        [SerializeField]
        private AudioSource gageSource;
        public AudioSource GageSource{get => gageSource;}
        [SerializeField]
        private AudioSource winSource;
        public AudioSource WinSource{get => winSource;}

        [SerializeField]
        private AudioClip[] audioClips = new AudioClip[2];
        public AudioClip[] AudioClips{get => audioClips;}
        // Start is called before the first frame update
        void Start()
        {
            
        }
    }
}

