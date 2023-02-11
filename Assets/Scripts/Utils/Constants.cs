using UnityEngine;

namespace SoftBit.Utils
{
    public static class Constants
    {
        public const int InteractableHintPoolCount = 40;

        /// <summary>
        /// Hand levitating constants
        /// </summary>
        public const float AttractionRadius = 0.5f;
        public const float HandAttractionRange = 20f;
        public const float AttractableShootPower = 20f;
        public const int MaxAvailableOrbitingPoints = 8; // Make sure to adjust also in HandObjectsAttraction script the transforms in the list if this is changed
        public const float OrbitingPointsSpeed = 200f;
        public const float AddSmasherOnShootRemoveAfterSeconds = 2f;

        /// <summary>
        /// Hand custom distance grab constants
        /// </summary>
        public const float DistangeGrabRadius = 1f;
        public const float HandDistangeGrabRange = 5f;
        /// <summary>
        /// Used to bypass a visual glitch when an object is visible just for one/couple frame/s, this location should never be visible in user's sight 
        /// </summary>
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
        public enum EnemyAnimatorParams
        {
            Forward = 0,
            Turn = 1,
            Move = 2,
            IdleState = 3,
            IsAttacking = 4,
            TurnRight = 5,
            TurnLeft = 6
        }

        public enum EnemyAnimationStateNames
        {
            StandUpFromBack = 0
        }

        public enum EnemyAnimationClipNames
        {
            StandUpFromBack = 0
        }

        /// <summary>
        /// EnemyConstants
        /// </summary>
        public const float FacingAngle = 10f;
        public const float ChaseRange = 4f;
        public const float AttackRange = 1.5f;
        public const float RangeMargin = 0.5f;
    }
}