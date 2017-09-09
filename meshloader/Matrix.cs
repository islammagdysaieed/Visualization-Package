using System;

namespace Visualization
{
	/// <summary>
	/// Summary description for Matrix.
	/// </summary>
	public enum Order
	{
		Prepend,
		Append
	}
	unsafe public class Matrix
	{
		private double[] m = new double[4<<2];//{0};
		public Matrix(double [,]mxx)
		{
			for(int i=0;i<4;i++)
				for(int j=0;j<4;j++)
					m[(i<<2)+j] = mxx[i,j];
		}
		public Matrix(double []data)
		{
			Buffer.BlockCopy(data, 0, m, 0, sizeof(double)<<4);
		}
		public Matrix()
		{
			SetIdentity();
		}
		public static Matrix FromOpenGl(double[] data)
		{
			Matrix retval = new Matrix();
			for(int i=0;i<4;i++)
				for(int j=0;j<4;j++)
					retval.m[(i<<2)+j] = data[(j<<2)+i];
			return retval;
		}
		public static Matrix Identity
		{
			get
			{return new Matrix();}
		}
		public static Matrix Zero
		{
			get
			{
				Matrix m = new Matrix();
				m.SetZero();
				return m;
			}
		}
		public void SetIdentity()
		{	
			int j;
			int i;
			for(i=0;i<4;i++)
			{
				for(j=0;j<i;j++)
					m[(i<<2)+j] = 0;
				m[(i<<2)+j] = 1;
				j++;
				for(;j<4;j++)
					m[(i<<2)+j] = 0;
			}
		}
		public void SetZero()
		{
			for(int i=0;i<4;i++)
				m[(i<<2) + i] = 0;
		}
/*		public double M(int i, int j)
		{
			return M[i, j];
		}
		public void SetM(int i, int j, double value)
		{
			m[i, j] = value;
		}*/
		public double this[int i, int j]
		{
			set	{m[(i<<2) + j] = value;}
			get	{return m[(i<<2) + j];}
		}
		public Matrix Clone()
		{
			return new Matrix(this.m);
		}
		public void Multiply(Matrix matrix, Order order)
		{
			if(order == Order.Append)
			{
				Matrix temp = Matrix.Multiply(this, matrix);
				this.m = temp.m;
			}
			else
			{
				Matrix temp = Matrix.Multiply(matrix, this);
				this.m = temp.m;
			}
		}
		public static Matrix operator *(Matrix left, Matrix right)
		{
			return Multiply(left, right);
		}
/*		public static Matrix operator *(Matrix m, Vector3 v)
		{

		}*/
		public static Matrix Multiply(Matrix left, Matrix right)
		{
			Matrix retval = new Matrix();
			double sum;
			for(int i=0;i<4;i++)
				for(int j=0;j<4;j++)
				{
					sum = 0;
					for(int k=0;k<4;k++)
						sum += left[i, k] * right[k, j];
					retval[i, j] = sum;
				}
			return retval;
		}
		public double[] Data
		{
			get	
			{
				double[] colMajor = new double[16];
				int i;
				int j;
				int k;
				for(i=0;i<4;i++)
				{
					k = i<<2;
					for(j=0;j<4;j++)
						colMajor[k+j] = m[(j<<2) + i];
				}
				return colMajor;
			}
		}
		public void Translate(double tx, double ty, double tz)
		{
			m[3] += tx;
			m[(1<<2)+3] += ty;
			m[(2<<2)+3] += tz;
		}
		public void Scale(double sx, double sy, double sz)
		{
			for(int i=0; i<4; i++)
				m[(0<<2)+i] *= sx;
			for(int i=0; i<4; i++)
				m[(1<<2)+i] *= sy;
			for(int i=0; i<4; i++)
				m[(2<<2)+i] *= sz;
		}
		public static Matrix RotationY(double zeta)
		{
			zeta = Math2.ToRadians(zeta);
			Matrix m = new Matrix();
			double cos = (double)System.Math.Cos(zeta);
			double sin = (double)System.Math.Sin(zeta);
			m[0, 0] = cos;				m[0, 2] = sin;

			m[2, 0] = -sin;				m[2, 2] = cos;
			return m;
		}

		public static Matrix RotationZ(double zeta)
		{
			zeta = Math2.ToRadians(zeta);
			Matrix m = new Matrix();
			double cos = (double)System.Math.Cos(zeta);
			double sin = (double)System.Math.Sin(zeta);
			m[0, 0] = cos;		m[0, 1] = -sin;
			m[1, 0] = sin;		m[1, 1] = cos;
			return m;		
		}

		public static Matrix RotationX(double zeta)
		{
			zeta = Math2.ToRadians(zeta);
			Matrix m = new Matrix();
			double cos = (double)System.Math.Cos(zeta);
			double sin = (double)System.Math.Sin(zeta);

					m[1, 1] = cos;		m[1, 2] = -sin;
					m[2, 1] = sin;		m[2, 2] = cos;
			return m;		

		}
		public static Matrix Translation(double tx, double ty, double tz)
		{
			Matrix m = new Matrix();
			m.Translate(tx, ty, tz);
			return m;
		}
		public static Matrix Translation(Vector3 t)
		{
			return Translation(t.x, t.y, t.z);
		}
		public static Matrix Scaling(double sx, double sy, double sz)
		{
			Matrix m = new Matrix();
			m.Scale(sx, sy, sz);
			return m;
		}

		public static Point3 operator *(Matrix m, Point3 pt)
		{
			Point3 retval = new Point3();
			retval.x = pt.x * m[0,0] + pt.y * m[0,1] + pt.z * m[0,2] + m[0, 3];
			retval.y = pt.x * m[1,0] + pt.y * m[1,1] + pt.z * m[1,2] + m[1, 3];
			retval.z = pt.x * m[2,0] + pt.y * m[2,1] + pt.z * m[2,2] + m[2, 3];
			return retval;
		}
		public static Vector3 operator *(Matrix m, Vector3 v)
		{
			Vector3 retval = new Vector3();
			retval.x = v.x * m[0,0] + v.y * m[0,1] + v.z * m[0,2] + m[0, 3];
			retval.y = v.x * m[1,0] + v.y * m[1,1] + v.z * m[1,2] + m[1, 3];
			retval.z = v.x * m[2,0] + v.y * m[2,1] + v.z * m[2,2] + m[2, 3];
			return retval;
		}
		public void Transform(ref Point3 pt)
		{
			pt = this * pt;
		}
		public void Transform(ref Vector3 v)
		{
			v = this * v;
		}
		public Vector3 Transform(Vector3 v)
		{
			return this * v;
		}
		public Point3 Transform(Point3 pt)
		{
			return this * pt;
		}
	}
}
