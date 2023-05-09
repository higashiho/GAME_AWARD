using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Result
{
    public class ResultSoundController : MonoBehaviour
    {
        public enum SoundPatternEnum
        {
            GAGE_SE, WIN_SE
        }
        [SerializeField]
        private AudioSource gageSource;
        public AudioSource GageSource{get => gageSource;}
        [SerializeField]
        private AudioSource winSource;
        public AudioSource WinSource{get => winSource;}

        [SerializeField, EnumIndex(typeof(SoundPatternEnum))]
        private AudioClip[] audioClipsList = new AudioClip[2];
        public AudioClip[] AudioClipsList{get => audioClipsList;}
        // Start is called before the first frame update
        void Start()
        {
            
        }
    }
}

