using UnityEngine;

namespace UDEV.TinyHero
{
    [System.Serializable]
    public class SkillSlot
    {
        public Skill skill;

        [AnimatorStates]
        public AnimState animState;

        public int playerLevel;
    }
}
