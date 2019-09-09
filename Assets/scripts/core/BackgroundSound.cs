using UnityEngine;
using System.Collections;

namespace MyLib
{
    public class BackgroundSound : MonoBehaviour
    {
        public static BackgroundSound Instance;
        AudioSource source;
        GameObject player;

        void Awake()
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            source = GetComponent<AudioSource>();
            //gameObject.GetComponent<AudioListener>().enabled = false;
        }

        public void PlaySound(string sound)
        {
            var clip = Resources.Load<AudioClip>("sound/" + sound);
            source.clip = clip;
            source.Play();
        }

        public void PlayEffectPos(string sound , Vector3 pos){
            var clip = Resources.Load<AudioClip>("sound/" + sound);
            AudioSource.PlayClipAtPoint(clip, pos);
        }
        public void PlayEffect(string sound){
            var clip = Resources.Load<AudioClip>("sound/" + sound);
            if(clip == null){
                clip = Resources.Load<AudioClip>("sound/skill/" + sound);
            }
            source.PlayOneShot(clip);
        }

        public AudioSource  PlayEffectLoop(string sound) {
            var clip = Resources.Load<AudioClip>("sound/" + sound);
            var audio = gameObject.AddComponent<AudioSource>();
            audio.loop = true;
            audio.clip = clip;
            return audio;
        }

        void Update()
        {
            if (player == null)
            {
                player = ObjectManager.objectManager.GetMyPlayer();
            }
            if (player != null)
            {
                transform.position = player.transform.position;
            }
        }
    }
}
