namespace SoftBit.Utils
{
    public static class Constants
    {
        /// <summary>
        /// Be careful with this constant, if it is set to .5f, then make sure the speed of the target is set to 200, otherwise jitter appear, 
        /// also, if you double one of them, make sure that you double the other one as well
        /// </summary>
        public static float FlyToObjectSpeed = .5f;
        public static float FlyToObjectMaxSpeed = 20f;
        public static float FlyToObjectMultiplier = 3f;


        /// <summary>
        /// Hand attraction constants
        /// </summary>
        public static float HandMaxObjectAttractionRange = 10f;
        public static float AttractionRadius = 0.5f;
        public static float AttractableShootPower = 20f;

        /// <summary>
        /// Layers names
        /// </summary>
        public static string AttractableObjectLayer = "AttractableObject";
    }
}