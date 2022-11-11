namespace SoftBit.Utils
{
    public static class Constants
    {
        /// <summary>
        /// Hand attraction constants
        /// </summary>
        public static float AttractionRadius = 0.5f;
        public static float HandAttractionRange = 8f;
        public static float AttractableShootPower = 20f;
        public static int MaxAvailableOrbitingPoints = 8; // Make sure to adjust also in HandObjectsAttraction script the transforms in the list if this is changed
        public static float OrbitingPointsSpeed = 200f;
        /// <summary>
        /// Be careful with this constant, if it is set to .5f, then make sure the OrbitingPointsSpeed is set to 200, otherwise jitter appear, 
        /// also, if you double one of them, make sure that you double the other one as well
        /// </summary>
        public static float FlyToObjectMinSpeed = 10f;
        public static float FlyToObjectMaxSpeed = 20f;
        public static float FlyToObjectMultiplier = 2f;

        /// <summary>
        /// Layers names
        /// </summary>
        public static string AttractableObjectLayer = "AttractableObject";

        public static float CollisionForceMultiplier = 20f;
        public static float ExplosionRadius = 1f;

        /// <summary>
        /// Animator animation names
        /// </summary>
        public static string AnimatorIdleState = "IdleState";
    }
}