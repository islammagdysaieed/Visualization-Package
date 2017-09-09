using System;
using System.Collections;
using System.IO;
using Tao.OpenGl;

namespace Visualization
{
	public abstract class IDraw
	{
		public abstract void Draw();
	}
	/// <summary>
	/// Represents the type of element
	/// </summary>
	public enum ElementType
	{
		FEBrick,
		IJKBrick,
		Tetrahedron, //FE
		FEQuad,
		IJKQuad,
		Triangle,
		Line //IJK
	}

	public class Vertex /*: public Point3*/
	{
		/// <summary>
		/// The position of the vertex in 3D space
		/// </summary>
		public Point3 Position;
		/// <summary>
		/// Extra data values associated with vertex
		/// </summary>
		public double[] Data;

		public Vertex()
		{
			Position = new Point3();
			Data = null;
		}

		public Vertex(
			ref double x,
			ref double y,
			ref double z,
			uint cData)
		{
			Position = new Point3(ref x, ref y, ref z);
			if(cData == 0) Data = null;
			else Data = new double[cData];
		}

		public void SetPosition(	
			ref double x,
			ref double y,
			ref double z)
		{
			Position.Set(ref x, ref y, ref z);
		}

		public override string ToString()
		{
			string retval = string.Format("Position: {0} | Data:", Position.ToString());
			for(int i=0;i<Data.Length; i++)
				retval += " " + Data[i].ToString();
			return retval;
		}
	}

	public class Zone
	{
		//There should be a field for T
		//public string Title;
		public uint VertexCount;
		public uint ElementCount;
		public uint	FaceCount;
		public uint EdgeCount; //Redundant
		public uint	DataCount; //Number of data items per node.
		public Vertex[] Vertices;
		public Edge[] Edges;
		public Face[] Faces;
		public Element[] Elements;
		public ElementType ElementType;
		//		public int FaceType;//GL_LINE_STRIP or GL_TRIANGLES or GL_QUADS
		public Zone(
			ElementType elementType,
			uint vertexCount,
			uint elementCount,
			uint faceCount,
			uint edgeCount,
			uint dataCount)
		{
			ElementType = elementType;
			VertexCount = vertexCount;
			ElementCount = elementCount;
			FaceCount = faceCount;
			EdgeCount = edgeCount;
			DataCount = dataCount;
			if(vertexCount == 0)
			{
				Vertices = null;
				Faces = null;
				Edges = null;
				Elements = null;
				return;
			}
			Vertices = new Vertex[VertexCount];
			for(uint i=0; i<VertexCount; i++)
			{
				Vertices[i] = new Vertex();
				Vertices[i].Data = new double[DataCount];
			}
			if(ElementType == ElementType.Line)
			{
				Faces = null;
				Edges = new Edge[EdgeCount];
				Elements = null;
				for(uint i=0; i<EdgeCount; i++)
					Edges[i] = new Edge();
			}
			else
			{
				Elements = new Element[ElementCount];
				for(uint i=0; i<ElementCount; i++)
					Elements[i] = new Element(ElementType);
				Faces = new Face[FaceCount];
				for(uint i=0; i<FaceCount; i++)
					Faces[i] = new Face(ElementType);
				Edges = new Edge[EdgeCount];
				for(uint i=0; i<EdgeCount; i++)
					Edges[i] = new Edge();
			}
		}

		public Zone()
		{
		}
		/// <summary>
		/// Finds the minimum and maximum value for each of the xyz coordinates,
		/// and assigns them to min and max.
		/// </summary>
		/// <param name="min">
		/// A point whose xyz coordinates are the smallest among all the vertices
		/// of the zone. It might not be a vertex in the zone.
		/// </param>
		/// <param name="max">
		/// A point whose xyz coordinates are the greatest among all the vertices
		/// of the zone. It might not be a vertex in the zone.
		/// </param>
		public void GetMinMaxVertices(Point3 min, Point3 max)
		{
			min.z = min.y = min.x = double.PositiveInfinity;
			max.z = max.y = max.x = double.NegativeInfinity;
			Point3 p;
			foreach(Vertex v in Vertices)
			{
				p = v.Position;
				if( p.x < min.x ) min.x = p.x;
				if( p.y < min.y ) min.y = p.y;
				if( p.z < min.z ) min.z = p.z;
				if( p.x > max.x ) max.x = p.x;
				if( p.y > max.y ) max.y = p.y;
				if( p.z > max.z ) max.z = p.z;
			}
		}

		public void	glDraw( Mesh owner)
		{
			for(uint i = 0; i < ElementCount; i++)
				Elements[i].glTell(this);
		}

		public void glDrawFilled( Mesh owner )
		{
			Random rand = new Random();
			Gl.glColor3d(rand.NextDouble(), rand.NextDouble(), rand.NextDouble());
			switch( ElementType )
			{
				case ElementType.Tetrahedron:
				case ElementType.Triangle:
					Gl.glBegin( Gl.GL_TRIANGLES );
					foreach(Face f in Faces)
					{
						Vertices[f.Vertices[0]].Position.glTell();
						Vertices[f.Vertices[1]].Position.glTell();
						Vertices[f.Vertices[2]].Position.glTell();
					}
					Gl.glEnd();
					break;
				case ElementType.IJKBrick:
				case ElementType.IJKQuad:
				case ElementType.FEBrick:
				case ElementType.FEQuad:
					Gl.glBegin( Gl.GL_QUADS );
					foreach(Face f in Faces)
					{
						Vertices[f.Vertices[0]].Position.glTell();
						Vertices[f.Vertices[1]].Position.glTell();
						Vertices[f.Vertices[2]].Position.glTell();
						Vertices[f.Vertices[3]].Position.glTell();
					}
					Gl.glEnd();
					break;
			}
		}

		public void glTellEdge(uint index)
		{
			Vertices[ Edges[index].Start ].Position.glTell();
			Vertices[ Edges[index].End ].Position.glTell();
		}
	}

	public class Mesh
	{
		//Zone list and Variable map

		/// <summary>
		/// List of zones in the mesh
		/// </summary>
		public LinkedList Zones;
		/// <summary>
		/// Specifies the index of a given variable into the
		/// data list of each vertex. For example, 
		/// VarToIndex["Temperature"] is the index of the 
		/// variable "Temperature" in the Data array of each vertex.
		/// The first variable is given the index 0.
		/// </summary>
		public Hashtable VarToIndex;
		
		//public data
		public Matrix Transformation = new Matrix();
		public bool		Hidden = false;
		public bool		HasTitle;
		public string	Title;

		//private data
		private Matrix _originalTransformation;
		

		public void glDraw()
		{
			if(Hidden) return;
			Gl.glColor3f(.1f, 1, 0);
			Gl.glMatrixMode(Gl.GL_MODELVIEW);
			Gl.glPushMatrix();
			Gl.glMultMatrixd(Transformation.Data);
			foreach(Zone z in Zones)
			{
				z.glDraw( this );
			}
			Gl.glPopMatrix();
		}

		public void glDrawFilled()
		{
			if(Hidden) return;
			Gl.glMatrixMode(Gl.GL_MODELVIEW);
			Gl.glPushMatrix();
			Gl.glMultMatrixd( Transformation.Data );
			foreach(Zone z in Zones)
			{
				z.glDrawFilled( this );
			}
			Gl.glPopMatrix();
		}
		
		public Mesh(Stream file, double size)
		{
			//Define data structures
			Zones = new LinkedList();
			VarToIndex = new Hashtable();

			//Load mesh
			Parser parser = new Parser();
			parser.LoadMesh(file, this);

			//Prepare transformations so that the mesh is centered and scaled
			SetupTransformation(size);
		}

		public Mesh(string fileName, double size)
		{
			//Define data structures
			Zones = new LinkedList();
			VarToIndex = new Hashtable();

			//Load mesh
			Parser parser = new Parser();
			parser.LoadMesh(fileName, this);

			//Prepare transformations so that the mesh is centered and scaled
			SetupTransformation(size);
		}


		public Mesh(Stream file) : this(file, 120)
		{
			
		}

		public Mesh(string fileName)
			: this(fileName, 120)
		{
		}

		/// <summary>
		/// Sets up a transformation that places the center of the mesh at
		/// the origin, and uniformly scales it so that the longest dimension
		/// has a length value = size
		/// </summary>
		/// <param name="size">
		/// Specifies the desired length of the longest dimension of the mesh.
		/// The mesh is uniformly scaled so that the longest dimension has 
		/// this length value.
		/// </param>
		private void SetupTransformation(double size)
		{
			Point3 candidateMin, candidateMax, min, max;
			candidateMin = new Point3();
			candidateMax = new Point3();
			min = new Point3(double.PositiveInfinity,
							 double.PositiveInfinity,
							 double.PositiveInfinity);
			max = new Point3(	double.NegativeInfinity,
								double.NegativeInfinity,
								double.NegativeInfinity);
			foreach(Zone zone in Zones)
			{
				zone.GetMinMaxVertices(candidateMin, candidateMax);
				if( candidateMin.x < min.x ) min.x = candidateMin.x;
				if( candidateMin.y < min.y ) min.y = candidateMin.y;
				if( candidateMin.z < min.z ) min.z = candidateMin.z;
				if( candidateMax.x > max.x ) max.x = candidateMax.x;
				if( candidateMax.y > max.y ) max.y = candidateMax.y;
				if( candidateMax.z > max.z ) max.z = candidateMax.z;
			}
			Transformation.Translate( - min.x, - min.y, - min.z);
			Vector3 v = max - min;
			Vector3 u = v*.5;
			Transformation.Translate( - u.x, - u.y, - u.z);
			if(v.x == 0) v.x = double.MinValue;
			if(v.y == 0) v.y = double.MinValue;
			if(v.z == 0) v.z = double.MinValue;
			v.x = Math.Max(v.x, v.y);
			v.x = Math.Max(v.x, v.z);
			v.x = size / v.x;
			Transformation.Scale(v.x, v.x, v.x);
			_originalTransformation = Transformation.Clone();
		}


		public void RestoreTransformation()
		{
			Transformation = _originalTransformation.Clone();
		}

		public void GetMinMaxValues(uint varIndex, out double min, out double max)
		{
			min = double.MaxValue;
			max = double.MinValue;
			foreach( Zone zone in Zones )
				foreach( Vertex v in zone.Vertices )
				{
					if( v.Data[varIndex] < min )
						min = v.Data[varIndex];
					if( v.Data[varIndex] > max )
						max = v.Data[varIndex];
				}
		}

		public void ShowMeshTitle()
		{
			/*if( titleShown ) return;
			titleShown = true;
			if(HasTitle)
			{
				Gl.glMatrixMode(Gl.GL_MODELVIEW);
				Gl.glPushMatrix();
				Gl.glLoadIdentity();
				Gl.glPushAttrib(Gl.GL_VIEWPORT);
				//	Gl.glViewport(TitleOffset.x, TitleOffset.y, titleWidth, titleLength);
				TitleColor.glColor3f();
				foreach(char c in Title)
				{
					Glut.glutBitmapCharacter(TitleFont, c);
				}
				//	Gl.glPopAttrib();
				Gl.glPopMatrix();
			}*/
		}
	}

	public class Edge
	{
		public uint	Start;
		public uint	End;

		public Edge() : this(0, 0)
		{
		}

		public Edge(uint st, uint end)
		{
			Start = st;
			End = end;
		}

		public void Set(uint st, uint end)
		{
			Start = st;
			End = end;
		}
	}

	public class Face
	{
		//Data Members
		public uint[] Edges;
		public uint[] Vertices;
		//Constructors
		public Face(ElementType elementType)
		{
			switch(elementType)
			{
				case ElementType.FEQuad:
				case ElementType.IJKQuad:
				case ElementType.FEBrick:
				case ElementType.IJKBrick:
					Edges = new uint[4];
					Vertices = new uint[4];
					break;
				case ElementType.Triangle:
				case ElementType.Tetrahedron:
					Edges = new uint[3];
					Vertices = new uint[3];
					break;
				case ElementType.Line:
					throw new Exception("Logical Error");
			}
		}

		public Face(uint e1, uint e2, uint e3)
		{
			Edges = new uint[3];
			Edges[0] = e1;
			Edges[1] = e2;
			Edges[2] = e3;
			Vertices = new uint[3];
		}

		public Face(uint e1, uint e2, uint e3, uint e4)
		{
			Edges = new uint[4];
			Edges[0] = e1;
			Edges[1] = e2;
			Edges[2] = e3;
			Edges[3] = e4;
			Vertices = new uint[4];
		}
		//Methods
		public void SetQuad(uint e1, uint e2, uint e3, uint e4)
		{
			Edges[0] = e1;
			Edges[1] = e2;
			Edges[2] = e3;
			Edges[3] = e4;
		}

		public void SetTriangle(uint e1, uint e2, uint e3)
		{
			Edges[0] = e1;
			Edges[1] = e2;
			Edges[2] = e3;
		}

		public void SetQuadVertices(uint p1, uint p2, uint p3, uint p4)
		{
			Vertices[0] = p1;
			Vertices[1] = p2;
			Vertices[2] = p3;
			Vertices[3] = p4;
		}

		public void SetTriangleVertices(uint p1, uint p2, uint p3)
		{
			Vertices[0] = p1;
			Vertices[1] = p2;
			Vertices[2] = p3;
		}

		public void glTell(Zone owner)
		{
			Gl.glBegin(Gl.GL_LINES);
			foreach(uint edge in Edges)
				owner.glTellEdge( edge );
			Gl.glEnd();
		}
	}

	public class Element
	{
		//Data Members
		public uint[] Faces;
        public uint[] vertInOrder;
		//Constructors
		public Element(ElementType elementType)
		{
			switch(elementType)
			{
				case ElementType.FEBrick:
				case ElementType.IJKBrick:
					Faces = new uint[6];
					break;
				case ElementType.Tetrahedron:
					Faces = new uint[4];
					break;
				case ElementType.FEQuad:
				case ElementType.IJKQuad:
				case ElementType.Triangle:
					Faces = new uint[1];
					break;
				case ElementType.Line:
					throw new Exception("Logical Error");
			}
		}

		public Element(uint faceCount)
		{
			Faces = new uint[faceCount];
		}
		//Methods
		public void SetQuad(uint f)
		{
			Faces[0] = f;
		}

		public void SetTriangle(uint f)
		{
			Faces[0] = f;
		}

		public void SetBrick(uint[] f)
		{
			f.CopyTo(Faces, 0);
		}

		public void SetTetrahedron(uint[] f)
		{
			f.CopyTo(Faces, 0);
		}

		public void SetBrickFrontFace(uint f)
		{
			Faces[0] = f;
		}

		public void SetBrickRightFace(uint f)
		{
			Faces[1] = f;
		}

		public void SetBrickBackFace(uint f)
		{
			Faces[2] = f;
		}
		public void SetBrickLeftFace(uint f)
		{
			Faces[3] = f;
		}

		public void SetBrickTopFace(uint f)
		{
			Faces[4] = f;
		}

		public uint GetBrickTopFace()
		{
			return Faces[4];
		}

		public void SetBrickBottomFace(uint f)
		{
			Faces[5] = f;
		}

		public uint GetBrickBottomFace()
		{
			return Faces[5];
		}
		public void SetTetrahedronFrontFace(uint f)
		{
			Faces[0] = f;
		}

		public void SetTetrahedronRightFace(uint f)
		{
			Faces[1] = f;
		}

		public void SetTetrahedronLeftFace(uint f)
		{
			Faces[2] = f;
		}

		public void SetTetrahedronBaseFace(uint f)
		{
			Faces[3] = f;
		}

		public void glTell(Zone owner)
		{
			foreach(uint face in Faces)
				owner.Faces[face].glTell( owner );
		}
	}
}