using UnityEngine;

namespace UDEV
{
    public class AutoDestroy : MonoBehaviour
    {
        [Tooltip("Do not actually destroy but disable instead.")]
        public bool disableOnly;
        public float delay;

        public bool isOverride;

        void OnEnable()
        {
            if(!isOverride)
                DestroyTrigger();
        }

        public void DestroyTrigger()
        {
            if (disableOnly)
            {
                Invoke("Disable", delay);
            }
            else
            {
                Destroy(gameObject, delay);
            }
        }

        private void OnDisable()
        {
            CancelInvoke();
        }

        void Disable()
        {
            gameObject.SetActive(false);
        }
    }
}
