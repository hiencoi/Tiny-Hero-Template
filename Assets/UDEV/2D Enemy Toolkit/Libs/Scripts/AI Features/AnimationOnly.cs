using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UDEV.AI2D
{
    public class AnimationOnly : aiFeature
    {
        public float distCondition; // Distance feature can be trigger
        public float minDistCondition; //Min Distance feature can be trigger

        protected override void Init()
        {
            ExitImmediately = false;
        }

        protected override bool TriggerCondition()
        {
            return m_aiController.DistToPlayer <= distCondition && m_aiController.DistToPlayer > minDistCondition;
        }
    }
}
