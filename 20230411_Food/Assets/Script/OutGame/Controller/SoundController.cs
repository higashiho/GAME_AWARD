using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Title
{
    public class SoundController : MonoBehaviour
    {
        [SerializeField]
        private AudioSource mainSource;
        public AudioSource MainSource{get => mainSource;}

        [SerializeField]
        private AudioSource onGamePlaySource;
        public AudioSource OnGamePlaySource{get => onGamePlaySource;}

        [SerializeField]
        private AudioSource recipeSource;
        public AudioSource RecipeSource{get => recipeSource;}

        [SerializeField]
        private AudioSource foodListSource;
        public AudioSource FoodListSource{get => foodListSource;}

        [SerializeField, Header("再生するオーディオクリップ配列")]
        private AudioClip[] audioClips = new AudioClip[4];
        public AudioClip[] AudioClips{get => audioClips;}

        // Start is called before the first frame update
        void Start()
        {
            mainSource.PlayOneShot(audioClips[0]);
        }

    }
}
