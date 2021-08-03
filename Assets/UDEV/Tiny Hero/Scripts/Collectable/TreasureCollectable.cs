using UnityEngine;

namespace UDEV.TinyHero
{
    public class TreasureCollectable : CollectableController
    {
        int m_coinsReward;


        public enum TreasureType
        {
            C100,
            C500,
            C1000
        }

        [Header("Collectable Settings:")]
        public TreasureType type;

        public int CoinsReward { get => m_coinsReward;}

        protected override void CollectableTrigger()
        {
            switch (type)
            {
                case TreasureType.C100:
                    m_coinsReward = 100;
                    break;
                case TreasureType.C500:
                    m_coinsReward = 500;
                    break;
                case TreasureType.C1000:
                    m_coinsReward = 1000;
                    break;
            }

            DialogController.Ins.ShowDialog(DialogType.TreasureReward, DialogShow.STACK);
            var currentDialog = (TreasureRewardDialog)DialogController.Ins.current;
            currentDialog.Treasure = this;
            currentDialog.UpdateInfo();
        }
    }
}
