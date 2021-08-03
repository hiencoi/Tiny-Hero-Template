using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UDEV.TinyHero
{
    public class PlayerFootsteps : MonoBehaviour
    {
        public AudioClip[] sounds;

        public void PlaySound()
        {
            AudioController.Ins.PlaySound(sounds);
        }
    }
}
