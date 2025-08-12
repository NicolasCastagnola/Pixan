
namespace UnityEngine{

    public static class FloatExtensions
    {
        /// <summary>
        /// Map value from a previous range in -1 to 1
        /// </summary>
        /// <param name="min">Range min</param>
        /// <param name="max">Range Max</param>
        /// <returns></returns>
        public static float map01(this float value, float min, float max)
        {
            return (value - min) * 1f / (max - min);
        }
        /// <summary>
        /// Map value from a previous range to a new range
        /// </summary>
        /// <param name="leftMin">Range min old</param>
        /// <param name="leftMax">Range max old</param>
        /// <param name="rightMin">Range min new</param>
        /// <param name="rightMax">Range max new</param>
        /// <returns></returns>
        public static float map(this float value, float leftMin, float leftMax, float rightMin, float rightMax)
        {
            return rightMin + (value - leftMin) * (rightMax - rightMin) / (leftMax - leftMin);
        }
    } }
