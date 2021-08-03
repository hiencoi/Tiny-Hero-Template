using UnityEngine;
using System.Collections;

namespace UDEV
{
    public class Timer
    {
        private static MonoBehaviour behaviour;
        public delegate void Task();

        public static void Schedule(MonoBehaviour _behaviour, float delay, Task task, bool unscaleTime = false)
        {
            behaviour = _behaviour;
            if(unscaleTime)
                behaviour.StartCoroutine(DoTaskUnscale(task, delay));
            else
                behaviour.StartCoroutine(DoTask(task, delay));
        }

        private static IEnumerator DoTask(Task task, float delay)
        {
            yield return new WaitForSeconds(delay);
            task();
        }

        private static IEnumerator DoTaskUnscale(Task task, float delay)
        {
            yield return new WaitForSecondsRealtime(delay);
            task();
        }
    }
}
