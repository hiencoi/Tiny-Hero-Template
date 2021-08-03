using UnityEngine;

namespace UDEV.TinyHero
{
    public class LifeCollectable : CollectableController
    {
        [Header("Collectable Settings:")]
        public int lifeBonus;
        public AudioClip[] lifePickupAudios;

        protected override void CollectableTrigger()
        {
            GameManager.Ins.CurLife += lifeBonus;
            AudioController.Ins.PlaySound(lifePickupAudios);
        }
    }
}
