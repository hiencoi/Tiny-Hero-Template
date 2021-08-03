using UnityEngine;

namespace UDEV.AI2D
{
    [System.Serializable]
    public class aiFeatureSlot
    {
        public string name;

        public aiFeature feature;

        public bool isLocked;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="_name">Name</param>
        /// <param name="_feature">AI Feature</param>
        public aiFeatureSlot(string _name, aiFeature _feature, bool _locked)
        {
            name = _name;
            feature = _feature;
            isLocked = _locked;
        }
    }
}
