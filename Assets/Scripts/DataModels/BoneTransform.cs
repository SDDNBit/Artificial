using UnityEngine;

namespace SoftBit.DataModels
{
    public class BoneTransform
    {
        public Vector3 Position { get; set; }
        public Quaternion Rotation { get; set; }
        public string Tag { get; set; }
    }
}