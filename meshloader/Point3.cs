using System;
using System.Drawing;
using Tao.OpenGl;

namespace Visualization
{
	/// <summary>
	/// Summary description for Point3.
	/// </summary>
	unsafe public class Point3
	{
		/*public double x;
		public double y;
		public double z;*/
		public double[] coordinates;

		public double x
		{
			get { return coordinates[0]; }
			set { coordinates[0] = value; }
		}

		public double y
		{
			get { return coordinates[1]; }
			set { coordinates[1] = value; }
		}

		public double z
		{
			get { return coordinates[2]; }
			set { coordinates[2] = value; }
		}

		public Point3()
		{
			//x = y = z = 0.0f;
			coordinates = new double[]{0, 0, 0};
		}

		public Point3(double xx, double yy, double zz)
		{
			coordinates = new double[]{xx, yy, zz};
			//Set(xx, yy, zz);
		}

		public Point3(ref double xx, ref double yy, ref double zz)
		{
			coordinates = new double[]{xx, yy, zz};
			//Set(xx, yy, zz);
		}

		public Point3(double []coord)
		{
			coordinates = new double[]{coord[0], coord[1], coord[2]};
			//Set(coord);
		}

		public void Set(double xx, double yy, double zz)
		{
			/*x = xx;
			y = yy;
			z = zz;*/
			coordinates[0] = xx;
			coordinates[1] = yy;
			coordinates[2] = zz;
		}

		public void Set(ref double xx, ref double yy, ref double zz)
		{
			/*x = xx;
			y = yy;
			z = zz;*/
			coordinates[0] = xx;
			coordinates[1] = yy;
			coordinates[2] = zz;
		}

		public void Set(double[] coord)
		{
			coordinates[0] = coord[0];
			coordinates[1] = coord[1];
			coordinates[2] = coord[2];
		}

		public void Set(Point3 pt)
		{
			double[] coord = pt.coordinates;
			coordinates[0] = coord[0];
			coordinates[1] = coord[1];
			coordinates[2] = coord[2];
		}

		public static Point3 Interpolate(ref double t, Point3 a, Point3 b)
		{
			return a + t*(b - a);
		}

		public static Point3 Interpolate(double t, Point3 a, Point3 b)
		{
			return a + t*(b - a);
		}

		public static Vector3 operator -(Point3 pt1, Point3 pt2)
		{
			Vector3 retval = new Vector3(pt1.Data);
			retval.x -= pt2.x;
			retval.y -= pt2.y;
			retval.z -= pt2.z;
			return retval;
		}

		public static bool operator ==(Point3 pt1, Point3 pt2)
		{
			double tolerance = 0.00001;
			if(!Math2.NearlyEqual(ref pt1.coordinates[0], ref pt2.coordinates[0], ref tolerance))
				return false;
			if(!Math2.NearlyEqual(ref pt1.coordinates[1], ref pt2.coordinates[1], ref tolerance))
				return false;
			if(!Math2.NearlyEqual(ref pt1.coordinates[2], ref pt2.coordinates[2], ref tolerance))
				return false;
			return true;
		}

		public static bool operator !=(Point3 pt1, Point3 pt2)
		{
			return !(pt1 == pt2);
		}

		public void Offset(double dx, double dy, double dz)
		{
			/*x += dx;
			y += dy;
			z += dz;*/
			coordinates[0] += dx;
			coordinates[1] += dy;
			coordinates[2] += dz;
		}

		public void Offset(ref double dx, ref double dy, ref double dz)
		{
			/*x += dx;
			y += dy;
			z += dz;*/
			coordinates[0] += dx;
			coordinates[1] += dy;
			coordinates[2] += dz;
		}		

		public void Draw()
		{
			Gl.glBegin(Gl.GL_POINTS);
			Gl.glVertex3dv(coordinates);
			Gl.glEnd();
		}

		public void glTell()
		{
			Gl.glVertex3dv( coordinates );
		}

		public override string ToString()
		{
			return string.Format("({0},{1},{2})",x,y,z);
		}

		public double[] Address
		{
			get
			{return coordinates;}
		}

		public double[] Data
		{
			get
			{return coordinates;}
		}

		#region ICloneable Members

		public Point3 Clone()
		{
			return new Point3(coordinates);
		}

		#endregion
	}

}
