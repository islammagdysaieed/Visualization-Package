using System;

namespace Visualization
{
	/// <summary>
	/// Summary description for Vector3.
	/// </summary>
	public class Vector3
	{
		public double x;
		public double y;
		public double z;

		public Vector3()
		{
			x = y = z = 0.0f;
		}

		public Vector3(double xx, double yy, double zz)
		{
			x = xx;
			y = yy;
			z = zz;
		}

		public Vector3(double[] coord)
		{
			x = coord[0];
			y = coord[1];
			z = coord[2];
		}

		public Vector3(ref double xx, ref double yy, ref double zz)
		{
			x = xx;
			y = yy;
			z = zz;
		}

		public static Vector3 operator +(Vector3 v1, Vector3 v2)
		{
			Vector3 v = v1.Clone();
			v.x += v2.x;
			v.y += v2.y;
			v.z += v2.z;
			return v;
		}

		public static Vector3 operator -(Vector3 v1, Vector3 v2)
		{			
			Vector3 v = v1.Clone();
			v.x -= v2.x;
			v.y -= v2.y;
			v.z -= v2.z;
			return v;
		}

		public static Point3 operator +(Vector3 v, Point3 pt)
		{
			Point3 p = pt.Clone();
			p.Offset( ref v.x, ref v.y, ref v.z);
			return p;
		}

		public static Point3 operator +(Point3 pt, Vector3 v)
		{
			return v + pt;
		}

		public static Vector3 operator *(Vector3 v, double s)
		{
			return new Vector3( s * v.x, s * v.y, s * v.z);
		}

		public static Vector3 operator *( double s, Vector3 v)
		{
			return new Vector3( s * v.x, s * v.y, s * v.z);
		}

		public static bool operator ==(Vector3 v1, Vector3 v2)
		{
			bool retval = System.Math.Abs( v1.x - v2.x ) <= double.Epsilon;
			retval = retval && System.Math.Abs(v1.y - v2.y) <= double.Epsilon;
			retval = retval && System.Math.Abs(v1.z - v2.z) <= double.Epsilon;
			return retval;
		}

		public static bool operator !=(Vector3 v1, Vector3 v2)
		{
			return !(v1 == v2);
		}

		public override int GetHashCode()
		{
			return ((int)x + ((int)y)<<12 + ((int)z)<<24);
		}

		public void Scale(double s)
		{
			x = s * x;
			y = s * y;
			z = s * z;
		}

		public static double AngleBetween(ref Vector3 v1, ref Vector3 v2)
		{
			double cosZeta = DotProduct(ref v1, ref v2) / v1.Magnitude;
			cosZeta = cosZeta / v2.Magnitude;
			return (double)System.Math.Acos( cosZeta );
		}

		public override bool Equals(object obj)
		{
			Vector3 v = (Vector3) obj;
			return this == v;
		}

		public static double DotProduct(ref Vector3 v1, ref Vector3 v2)
		{
			return v1.x*v2.x + v1.y*v2.y + v1.z*v2.z;
		}

		public static double DotProduct(Vector3 v1, Vector3 v2)
		{
			return v1.x*v2.x + v1.y*v2.y + v1.z*v2.z;
		}

		public double MagnitudeSquare
		{
			get{return x*x + y*y + z*z;}
		}

		public double Magnitude
		{
			get{return (double)System.Math.Sqrt(x*x + y*y + z*z);}
		}

		public double AngleXY
		{
			get{return Math2.ToDegrees(RadianAngleXY);}
		}

		public double AngleYZ
		{
			get{return Math2.ToDegrees(RadianAngleYZ);}
		}

		public double AngleZX
		{
			get{return Math2.ToDegrees(RadianAngleZX);}
		}

		public double RadianAngleXY
		{
			get
			{return (double) System.Math.IEEERemainder(2*System.Math.PI + System.Math.Atan2(y, x), 2*System.Math.PI);}
		}

		public double RadianAngleYZ
		{
			get
			{return (double) System.Math.IEEERemainder(2*System.Math.PI + System.Math.Atan2(z, y), 2*System.Math.PI);}
		}

		public double RadianAngleZX
		{
			get
			{return (double) System.Math.IEEERemainder(2*System.Math.PI + System.Math.Atan2(z, x), 2*System.Math.PI);}
		}
		
		public Vector3 NormalizedVetor
		{
			get
			{
				double m = Magnitude;
				return new Vector3(x/m ,y/m, z/m);
			}
		}

		public void Normalize()
		{
			double m = Magnitude;
			x = x / m;
			y = y / m;
			z = z / m;
		}

		public void Set(double xx, double yy, double zz)
		{
			x = xx;
			y = yy;
			z = zz;
		}

		public void Set(Vector3 v)
		{
			Set(v.x, v.y, v.z);
		}

		public override string ToString()
		{
			string retval = x + "i ";
			if(y<0)	retval += "- ";
			else	retval += "+ ";
			retval += System.Math.Abs(y) + "j";
			if(z<0) retval += "- ";
			else	retval += "+ ";
			retval += System.Math.Abs(z) + "k";
			return retval;
		}

		public Vector3 Clone()
		{
			return new Vector3(x, y, z);
		}
		
		public static Vector3 CrossProduct(Vector3 v1, Vector3 v2)
		{
			Vector3 retval = new Vector3(
							v1.y*v2.z - v1.z*v2.y,
							-(v1.x*v2.z - v1.z*v2.x),
							v1.x*v2.y - v1.y*v2.x);
			return retval;
		}
	}
}
