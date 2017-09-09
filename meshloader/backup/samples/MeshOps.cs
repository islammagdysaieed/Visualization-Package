using System;
using System.Collections.Generic;
using System.Text;
using Visualization;

namespace Samples
{
	public static class MeshOps
	{
		public static void FindMaximumFacePerimeter(Mesh mesh,
			out int zoneIndex, out int faceIndex, out double maxPerimeter)
		{
			maxPerimeter = double.NegativeInfinity;
			zoneIndex = -1;
			faceIndex = -1;
			int currZoneIndex = -1;
			foreach (Zone zone in mesh.Zones)
			{
				if (zone.ElementType == ElementType.Line)
					continue; //skip zones that have no faces
				currZoneIndex++;
				//loop the faces of the zone
				for (int currFaceIndex = 0; currFaceIndex < zone.FaceCount; currFaceIndex++)
				{
					Face face = zone.Faces[currFaceIndex];
					List<Vertex> vertices = GetFaceVertices(zone, face);
					double perimeter = ComputePerimeter(vertices);
					if (perimeter > maxPerimeter)
					{
						zoneIndex = currZoneIndex;
						faceIndex = currFaceIndex;
						maxPerimeter = perimeter;
					}
				}
			}
		}

		private static List<Vertex> GetFaceVertices(Zone zone, Face face)
		{
			int nvertices = face.Vertices.Length;
			List<Vertex> vertices = new List<Vertex>(nvertices);
			//loop the indices of the face vertices
			for (uint i = 0; i < nvertices; i++)
			{
				uint vertIndex = face.Vertices[i];
				//get the vertex from the zone
				Vertex vertex = zone.Vertices[vertIndex];
				vertices.Add(vertex);
			}
			return vertices;
		}

		/// <returns>Returns the perimeter (total length of edges) of the provided polygon</returns>
		private static double ComputePerimeter(List<Vertex> vertices)
		{
			int nverts = vertices.Count;
			double perimeter = 0;
			for (int i = 0; i < nverts; i++)
				perimeter += (vertices[(i + 1) % nverts].Position - vertices[i].Position).Magnitude;
			return perimeter;
		}

	}
}
