using UnityEngine;
using System.Collections;

namespace UDEV
{
    public class FirstSceneController : MonoBehaviour
    {
        private void Update()
        {
#if !UNITY_WSA
            if (Input.GetKeyDown(KeyCode.Escape) && !DialogController.Ins.IsDialogShowing())
            {
                DialogController.Ins.ShowDialog(DialogType.QuitGame);
            }
#endif
        }
    }

}