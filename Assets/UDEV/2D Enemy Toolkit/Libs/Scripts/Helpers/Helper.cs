using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UDEV
{
    public static class Helper
    {
        #region Animation
        public static void PlayAnimatorState(Animator anim, int layerIndex, string stateName)
        {
            if (IsAnimatorCanPlayState(anim, layerIndex, stateName))
            {
                anim.Play(stateName);
            }
        }

        public static bool IsAnimatorStateActive(Animator animator, int layerIndex, string stateName)
        {
            if (animator)
                return animator.GetCurrentAnimatorStateInfo(layerIndex).IsName(stateName);

            return true;
        }

        public static bool IsAnimatorCanPlayState(Animator animator, int layerIndex, string stateName)
        {
            if (animator)
                return !IsAnimatorStateActive(animator, layerIndex, stateName)
                && animator.HasState(layerIndex, Animator.StringToHash(stateName));

            return false;
        }

        public static AnimationClip GetClip(Animator anim, string stateName)
        {
            if (anim)
            {
                int maxState = anim.runtimeAnimatorController.animationClips.Length;

                var states = anim.runtimeAnimatorController.animationClips;

                for (int i = 0; i < maxState; i++)
                {
                    if (string.Compare(states[i].name, stateName) == 0)
                    {
                        return states[i];
                    }
                }
            }

            return null;
        }

        #endregion

        /// <summary>
        /// Flip object when it face left
        /// </summary>
        /// <param name="dir">Direction</param>
        /// <param name="body">Transform</param>
        public static void FaceLeftFlip(Vector3 dir, Transform body)
        {
            var curScale = body.localScale;

            if (dir.x > 0)
            {
                body.localScale = curScale.x > 0
                    ? new Vector3(-1 * curScale.x, curScale.y, curScale.z)
                    : curScale;
            }
            else if (dir.x < 0)
            {
                body.localScale = curScale.x < 0
                    ? new Vector3(-1 * curScale.x, curScale.y, curScale.z)
                    : curScale;
            }
        }

        /// <summary>
        /// Flip object when it face right
        /// </summary>
        /// <param name="dir">Direction</param>
        /// <param name="body">Transform</param>
        public static void FaceRightFlip(Vector3 dir, Transform body)
        {
            var curScale = body.localScale;

            if (dir.x > 0)
            {
                body.localScale = curScale.x < 0
                    ? new Vector3(-1 * curScale.x, curScale.y, curScale.z)
                    : curScale;
            }
            else if (dir.x < 0)
            {
                body.localScale = curScale.x > 0
                    ? new Vector3(-1 * curScale.x, curScale.y, curScale.z)
                    : curScale;
            }
        }
    }
}
