using System;

namespace Visualization
{
	/// <summary>
	/// Summary description for Math.
	/// </summary>
	public class Math2
	{
		private Math2()
		{
		}
		public static double ToRadians(double degrees)
		{
			return (double)(System.Math.PI/180 * degrees);
		}
		public static double ToDegrees(double radians)
		{
			return (double)(180/System.Math.PI * radians);
		}
		public static bool Between(double value, double left, double right)
		{
            double temp;
            if (left > right)
            {
                temp = left;
                left = right;
                right = temp;
            }
			return value>=left && value<=right;
		}
		public static bool NearlyEqual(ref double val1, ref double val2, ref double tolerance)
		{
			if(val1 != 0)
			{
				double diff = Math.Abs((val1 - val2)/val1);
				return diff <= tolerance;
			}
			else
			{
				return Math.Abs(val2) <= tolerance;
			}
		}
		public static bool NearlyEqual(double val1, double val2, double tolerance)
		{
			val1 = Math.Abs((val1 - val2)/val1);
			return val1 <= tolerance;
		}
        public static bool NearlyEqual2(double val1, double val2, double tolerance)
        {
            val1 = Math.Abs((val1 - val2));
            return val1 <= tolerance;
        }
        public static double angle(Point3 p1, Point3 p2)
        {
            return Math.Atan2(p2.y - p1.y, p2.x - p1.x);
        }
        public static double signed_traingle_area(Point3 p1, Point3 p2, Point3 p3)
        {
            return ((p1.x * p2.y - p1.y * p2.x + p1.y * p3.x- p1.x * p3.y + p2.x * p3.y - p3.x * p2.y) / 2.0);

        }
        public static bool ccw(Point3 p1, Point3 p2, Point3 p3)
        {
            return (signed_traingle_area(p1, p2, p3) < 0.000001);
        }
        public static bool colinear(Point3 p1, Point3 p2, Point3 p3)
        {

            return (Math.Abs(signed_traingle_area(p1, p2, p3)) <= 0.000001);
        }
        public static double GelLengthBetween2Points(Point3 p1, Point3 p2)
        {
            return Math.Sqrt(Math.Pow((p2.y - p1.y), 2) + Math.Pow((p2.x - p1.x), 2));
        }
	}
}
