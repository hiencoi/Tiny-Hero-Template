using UnityEngine;

namespace UDEV.TinyHero {
    public class CoinCollectable : CollectableController
    {
        [Header("Collectable Settings:")]
        public int minCoinBonus;
        public int maxCoinBonus;
        public AudioClip[] coinPickupAudios;

        protected override void CollectableTrigger()
        {
            int coinBonus = Random.Range(minCoinBonus, maxCoinBonus);

            Prefs.coins += coinBonus;

            GameManager.Ins.CoinsCollected += coinBonus;

            AudioController.Ins.PlaySound(coinPickupAudios);

            GameUIManager.Ins.UpdateCoinsInfo();
        }
    }
}
