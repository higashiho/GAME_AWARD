using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Title
{
    public class TitleSoundController : MonoBehaviour
    {
        [SerializeField]
        private AudioSource mainSource;
        public AudioSource MainSource{get => mainSource;}

        [SerializeField]
        private AudioSource onGamePlaySource;
        public AudioSource OnGamePlaySource{get => onGamePlaySource;}


        [SerializeField]
        private AudioSource selectSource;
        public AudioSource SelectSource{get => selectSource;}

        [SerializeField]
        private AudioSource cursorSource;
        public AudioSource CursorSource{get => cursorSource;}

        [SerializeField, Header("再生するオーディオクリップ配列")]
        private AudioClip[] audioClips = new AudioClip[6];
        public AudioClip[] AudioClips{get => audioClips;}

        // Start is called before the first frame update
        void Start()
        {
            mainSource.PlayOneShot(audioClips[0]);
        }

    }
}
