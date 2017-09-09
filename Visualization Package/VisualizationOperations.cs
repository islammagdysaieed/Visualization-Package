using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Tao.OpenGl;
using Visualization;
using System.Drawing.Drawing2D;

namespace VisualizationPackage
{
    class VisualizationOperations
    {
        static List<List<Point3[]>> contourPolygons;

        static bool containsContour(Point3[] points, double[] data, double value)
        {
            if (data[0] > value && data[1] > value && data[2] > value ||
                data[0] < value && data[1] < value && data[2] < value)
                return false;
            return true;
        }

        static Point3[] GetContourEgdeOfTriangle(Point3[] points, double[] data, double value)
        {
            Point3[] edge = new Point3[50];
            int idx = 0;
            int isDone = 0;

            if (Contour.isFlooded)
            {
                if (value <= data[1])
                {
                    edge[idx++] = points[1];
                }
                if (value <= data[2])
                {
                    edge[idx++] = points[2];
                }
                if (value <= data[0])
                {
                    edge[idx++] = points[0];
                }
            }

            if (value > data[0] && value < data[1] || value > data[1] && value < data[0])
            {
                double delta = (data[1] - data[0]);
                double ratio = (value - data[0]) / delta;
                double x = points[0].x + ratio * (points[1].x - points[0].x);
                double y = points[0].y + ratio * (points[1].y - points[0].y);
                double z = points[0].z + ratio * (points[1].z - points[0].z);
                Point3 point = new Point3(x, y, z);
                edge[idx++] = point;
                isDone = 1;
            }
            if (value > data[1] && value < data[2] || value > data[2] && value < data[1])
            {
                double delta = (data[2] - data[1]);
                double ratio = (value - data[1]) / delta;
                double x = points[1].x + ratio * (points[2].x - points[1].x);
                double y = points[1].y + ratio * (points[2].y - points[1].y);
                double z = points[1].z + ratio * (points[2].z - points[1].z);
                Point3 point = new Point3(x, y, z);
                edge[idx++] = point;
                isDone++;
            }
            if (isDone < 2 && (value > data[0] && value < data[2] || value > data[2] && value < data[0]))
            {
                double delta = (data[0] - data[2]);
                double ratio = (value - data[2]) / delta;
                double x = points[2].x + ratio * (points[0].x - points[2].x);
                double y = points[2].y + ratio * (points[0].y - points[2].y);
                double z = points[2].z + ratio * (points[0].z - points[2].z);
                Point3 point = new Point3(x, y, z);
                edge[idx++] = point;
            }
            if (idx >= 4)
            {
                int temp = idx;
                for (int i = 0; i < temp - 2; i++)
                    edge[idx++] = edge[i];
                edge[idx++] = edge[temp - 1];
                edge[idx++] = edge[temp - 2];
            }
            return edge.Take(idx).ToArray();
        }

        static bool checkAllIn(double[] data, double value)
        {
            if (data[0] > value && data[1] > value && data[2] > value) return true;
            return false;
        }

        static List<Point3[]> GetLineContour(Point3[] points, double[] data, double value)
        {
            List<Point3[]> lineContour = new List<Point3[]>();
            if (points.Length == 3)
            {
                if (containsContour(points, data, value))
                {
                    lineContour.Add(GetContourEgdeOfTriangle(points, data, value));
                }
                else if (Contour.isFlooded && checkAllIn(data, value))
                {
                    lineContour.Add(points);
                    contourPolygons.Add(lineContour);
                    lineContour = new List<Point3[]>();
                }
            }
            else
            {
                double midValue = 0, x = 0, y = 0, z = 0;
                for (int i = 0; i < points.Length; i++)
                {
                    midValue += data[i];
                    x += points[i].x;
                    y += points[i].y;
                    z += points[i].z;
                }
                midValue /= data.Length;
                x /= points.Length;
                y /= points.Length;
                z /= points.Length;

                Point3 midPoint = new Point3(x, y, z);
                for (int i = 0; i < points.Length; i++)
                {
                    if (containsContour(new Point3[] { points[i], points[(i + 1) % points.Length], midPoint }, new double[] { data[i], data[(i + 1) % points.Length], midValue }, value))
                    {
                        lineContour.Add(GetContourEgdeOfTriangle(
                            new Point3[] { points[i], points[(i + 1) % points.Length], midPoint },
                            new double[] { data[i], data[(i + 1) % points.Length], midValue },
                            value));
                    }
                    else if (Contour.isFlooded && checkAllIn(new double[] { data[i], data[(i + 1) % points.Length], midValue }, value))
                    {
                        lineContour.Add(new Point3[] { points[i], points[(i + 1) % points.Length], midPoint });
                    }
                    if (Contour.isFlooded && lineContour.Count != 0)
                    {
                        contourPolygons.Add(lineContour);
                        lineContour = new List<Point3[]>();
                    }
                }
            }
            return lineContour;
        }

        public static List<Point3[]> CalculateLineContours(Mesh _mesh, uint varIndex, int num_Contours)
        {
            List<Point3[]> contourLines = new List<Point3[]>();
            double contourValue = 0, step = 0;
            double min = Mesh_Manager.dataTypeRange[varIndex].Key, max = Mesh_Manager.dataTypeRange[varIndex].Value;
            step = (max - min) / (num_Contours + 1);
            contourValue = (float)min + step;
            Color_Mapper.minValue = (float)min;
            Color_Mapper.maxValue = (float)max;
            while (contourValue <= max)
            {
                foreach (Zone z in _mesh.Zones)
                {
                    foreach (Face f in z.Faces)
                    {
                        Point3[] points = new Point3[f.Vertices.Length];
                        double[] data = new double[f.Vertices.Length];
                        for (int i = 0; i < f.Vertices.Length; i++)
                        {
                            points[i] = z.Vertices[f.Vertices[i]].Position;
                            data[i] = z.Vertices[f.Vertices[i]].Data[varIndex];
                        }
                        contourLines.AddRange(GetLineContour(points, data, contourValue));
                        while (contourLines.Count > Contour.contourColors.Count)
                        {
                            Contour.contourColors.Add(Color_Mapper.ValueToColor((float)contourValue, Mapping_Mode.Continuous));
                        }
                    }
                }
                contourValue += step;
            }
            return contourLines;
        }

        public static List<List<Point3[]>> CalculateFloodedContours(Mesh _mesh, uint varIndex, int num_Contours)
        {
            contourPolygons = new List<List<Point3[]>>();
            double contourValue = 0, step = 0;
            double min = Mesh_Manager.dataTypeRange[varIndex].Key, max = Mesh_Manager.dataTypeRange[varIndex].Value;

            //  _mesh.GetMinMaxValues(varIndex, out min, out max);
            step = (max - min) / (num_Contours + 1);
            contourValue = (float)min;
            Color_Mapper.minValue = (float)min;
            Color_Mapper.maxValue = (float)max;

            while (contourValue <= max)
            {
                foreach (Zone z in _mesh.Zones)
                {
                    foreach (Face f in z.Faces)
                    {
                        List<Point3[]> currentContourPolygonLines = new List<Point3[]>();
                        Point3[] points = new Point3[f.Vertices.Length];
                        double[] data = new double[f.Vertices.Length];
                        for (int i = 0; i < f.Vertices.Length; i++)
                        {
                            points[i] = z.Vertices[f.Vertices[i]].Position;
                            data[i] = z.Vertices[f.Vertices[i]].Data[varIndex];
                        }
                        List<Point3[]> tmpList = GetLineContour(points, data, contourValue);
                        if (tmpList.Count != 0)
                        {
                            currentContourPolygonLines.AddRange(tmpList);
                            contourPolygons.Add(currentContourPolygonLines);
                        }

                        while (contourPolygons.Count > Contour.contourColors.Count)
                        {
                            Contour.contourColors.Add(Color_Mapper.ValueToColor((float)contourValue, Mapping_Mode.Continuous));
                        }
                    }
                }
                contourValue += step;
            }
            return contourPolygons;
        }

        ///////////////////////////////////////////////////////////
        public static List<Point3[]> CalculateIsoSurface(Mesh _mesh, uint varIndex, int num_Contours)
        {
            List<Point3[]> isoSurfaces = new List<Point3[]>();

            double contourValue = 0, step = 0;
            double min = Mesh_Manager.dataTypeRange[varIndex].Key, max = Mesh_Manager.dataTypeRange[varIndex].Value;
            step = (max - min) / (num_Contours + 1);
            contourValue = (float)min + step;
            Color_Mapper.minValue = (float)min;
            Color_Mapper.maxValue = (float)max;
            while (contourValue <= max)
            {
                List<Point3> isoSurface = new List<Point3>();
                foreach (Zone z in _mesh.Zones)
                {
                    foreach (Element e in z.Elements)
                    {
                        Point3[] points = new Point3[e.vertInOrder.Length];
                        double[] data = new double[e.vertInOrder.Length];
                        for (int i = 0; i < e.vertInOrder.Length; i++)
                        {
                            points[i] = z.Vertices[e.vertInOrder[i]].Position;
                            data[i] = z.Vertices[e.vertInOrder[i]].Data[varIndex];
                        }
                        //get case
                        byte elementCase = ISOSurface.GetElementCase(data, contourValue);
                        //get active edges
                        int[] edgeindex = new int[16];
                        for (int i = 0; i < 16; i++)
                            edgeindex[i] = ISOSurface.triTable[elementCase, i];
                        ///
                        for (int i = 0; i < 16; i++)
                        {
                            if (edgeindex[i] != -1)
                            {
                                Edge edge = ISOSurface.GetEdgePoints[edgeindex[i]];
                                Vertex Vert0 = z.Vertices[e.vertInOrder[edge.Start]];
                                Vertex Vert1 = z.Vertices[e.vertInOrder[edge.End]];
                                double alpha = (contourValue - Vert0.Data[varIndex]) / (Vert1.Data[varIndex] - Vert0.Data[varIndex]);
                                Point3 isoPoint = Vert0.Position + alpha * (Vert1.Position - Vert0.Position);
                                isoSurface.Add(isoPoint);
                            }
                        }
                    }
                }

                isoSurfaces.Add(isoSurface.ToArray());
                Contour.isoSurfacesColors.Add(Color_Mapper.ValueToColor((float)contourValue, Mapping_Mode.Continuous));
                contourValue += step;
            }
            return isoSurfaces;
        }
    }
}