using UnityEngine;

namespace UDEV.TinyHero
{
    public class HealthPotionCollectable : CollectableController
    {
        [Header("Collectable Settings:")]
        public int healthBonus;
        public AudioClip[] healthPickupAudios;

        protected override void CollectableTrigger()
        {
            m_player.CurHealth += healthBonus;
            AudioController.Ins.PlaySound(healthPickupAudios);
        }
    }
}
