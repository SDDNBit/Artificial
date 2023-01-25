using UnityEngine;

namespace SoftBit.Utils
{
    public static class Constants
    {
        /// <summary>
        /// Hand levitating constants
        /// </summary>
        public const float AttractionRadius = 1f;
        public const float HandAttractionRange = 20f;
        public const float AttractableShootPower = 20f;
        public const int MaxAvailableOrbitingPoints = 8; // Make sure to adjust also in HandObjectsAttraction script the transforms in the list if this is changed
        public const float OrbitingPointsSpeed = 200f;
        public const float AddSmasherOnShootRemoveAfterSeconds = 2f;

        /// <summary>
        /// Hand custom distance grab constants
        /// </summary>
        public const float DistangeGrabRadius = 0.5f;
        public const float HandDistangeGrabRange = 5f;
        public static readonly Vector3 DefaultVisibleLocation = new Vector3(0, -100, 0);

        /// <summary>
        /// Be careful with this constant, if it is set to .5f, then make sure the OrbitingPointsSpeed is set to 200, otherwise jitter appear, 
        /// also, if you double one of them, make sure that you double the other one as well
        /// </summary>
        public const float FlyToObjectMinSpeed = 10f;
        public const float FlyToObjectMaxSpeed = 20f;
        public const float FlyToObjectMultiplier = 2f;

        /// <summary>
        /// Layers names
        /// </summary>
        public const string DefaultLayer = "Default";
        public const string AttractableObjectLayer = "AttractableObject";

        public const float CollisionForceMultiplier = 20f;
        public const float ExplosionRadius = 1f;

        /// <summary>
        /// Animator animation names
        /// </summary>
        public const string AnimatorIdleState = "IdleState";
        public const string AnimatorAttackingState = "IsAttacking";
    }
}