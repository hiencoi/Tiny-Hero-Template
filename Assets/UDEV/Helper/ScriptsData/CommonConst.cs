namespace UDEV
{
    //#define DEVELOPMENT
    public class CommonConst
    {
#if DEVELOPMENT
    public const bool ENCRYPTION_PREFS = false;
#else
#if (UNITY_ANDROID || UNITY_IPHONE) && !UNITY_EDITOR
    public const bool ENCRYPTION_PREFS = true;
#else
        public const bool ENCRYPTION_PREFS = false;
#endif
#endif
    }
}
