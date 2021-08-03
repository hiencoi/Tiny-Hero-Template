using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;

namespace UDEV
{
    public class Dialog : MonoBehaviour
    {
        public Animator anim;
        [AnimatorStates]
        public AnimState hidingAnimation;
        public GameObject title, message;
        public Action<Dialog> onDialogOpened;
        public Action<Dialog> onDialogClosed;
        public Action onDialogCompleteClosed;
        public Action<Dialog> onButtonCloseClicked;
        public DialogType dialogType;
        public bool enableAd = true;
        public bool enableEscape = true;

        private AnimatorStateInfo info;
        private bool isShowing;

        protected virtual void Awake()
        {
            if (anim == null) anim = GetComponent<Animator>();
        }

        protected virtual void Start()
        {
            onDialogCompleteClosed += OnDialogCompleteClosed;
            GetComponent<Canvas>().worldCamera = Camera.main;
        }

        private void Update()
        {
            if (enableEscape && Input.GetKeyDown(KeyCode.Escape))
            {
                Close();
            }
        }

        public virtual void Show()
        {
            gameObject.SetActive(true);
            if (anim != null && IsIdle())
            {
                isShowing = true;
                anim.SetTrigger("show");
                onDialogOpened(this);
            }

            if (enableAd)
            {
                Timer.Schedule(this, 0.3f, () =>
                {
                    CUtils.ShowInterstitialAd();
                });
            }
        }

        public virtual void Close()
        {
            if (isShowing == false) return;
            isShowing = false;

            var hidingState = Helper.GetClip(anim, hidingAnimation.clipName);

            if (anim != null && IsIdle() && hidingState != null)
            {
                anim.SetTrigger("hide");
                Timer.Schedule(this, hidingState.length, DoClose);
            }
            else
            {
                DoClose();
            }

            onDialogClosed(this);
        }

        private void DoClose()
        {
            Destroy(gameObject);
            if (onDialogCompleteClosed != null) onDialogCompleteClosed();
        }

        public virtual void Hide()
        {
            gameObject.SetActive(false);
            isShowing = false;
        }

        public bool IsIdle()
        {
            info = anim.GetCurrentAnimatorStateInfo(0);
            return info.IsName("Idle");
        }

        public bool IsShowing()
        {
            return isShowing;
        }

        public virtual void OnDialogCompleteClosed()
        {
            onDialogCompleteClosed -= OnDialogCompleteClosed;
        }

        public void PlayButton()
        {
            //play sound here
        }
    }
}
