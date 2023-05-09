using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Title
{
    public class TitleSoundController : MonoBehaviour
    {
        public enum SoundPatternEnum
        {
            MAIN_BGM, GAME_PLAY_SE, SELECT_SE, CURSOR_SE
        }
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
        private AudioClip[] audioClipsList = new AudioClip[4];
        public AudioClip[] AudioClipsList{get => audioClipsList;}

        // Start is called before the first frame update
        void Start()
        {
            mainSource.PlayOneShot(AudioClipsList[(int)SoundPatternEnum.MAIN_BGM]);
        }

    }
}
