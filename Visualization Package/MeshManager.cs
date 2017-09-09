using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Visualization;
using Tao.OpenGl;
using System.Drawing;

namespace VisualizationPackage
{
    #region Structures
    public struct NormColor
    {
        double r, g, b;
        public double R
        {
            get { return r; }
            set { if (value >= 0 && value <= 1) r = value; }
        }
        public double G
        {
            get { return g; }
            set { if (value >= 0 && value <= 1) g = value; }
        }
        public double B
        {
            get { return b; }
            set { if (value >= 0 && value <= 1) b = value; }
        }
    }
    public struct Transformation_Info
    {
        public double translationOffset;
        public double rotationOffset;
        public double scalingOffset;
        public Transformation_Info(double translationOffset, double scalingOffset, double rotationOffset)
        {
            this.translationOffset = translationOffset;
            this.rotationOffset = rotationOffset;
            this.scalingOffset = scalingOffset;
        }
    }
    #endregion

    #region Enumrators
    public enum FaceType { Triangular, Quadrilateral }
    public enum Translation_Mode { Left, Right, Up, Down, Forward, Backward }
    public enum Scaling_Mode { ZoomIn, ZoomOut }
    public enum Rotation_Mode { ClockWise, AntiClockWise }
    public enum Mapping_Mode { Discrete, Continuous }
    public enum Coloring_Mode { EdgeColoring, FaceColoring, Default }
    public enum Contour_Mode { None, LineContour, FloodedContour, IsoSurface }
    #endregion

    public class Mesh_Manager
    {
        public static Dictionary<uint, KeyValuePair<double, double>> dataTypeRange;
        Mesh mesh;
        public Mesh _Mesh
        {
            get { return mesh; }
            set { mesh = value; }
        }

        Transformation_Info transformationInfo;
        public Coloring_Mode coloringMode;
        public Mapping_Mode mappingMode;
        public Contour_Mode contourMode;
        public string codingDatatype; //for ex. temperature
        public int numContours;
        public uint Coding_DatatypeIndex
        {
            get { return (uint)_Mesh.VarToIndex[codingDatatype]; }
        }

        #region Constructors
        public Mesh_Manager(string fileName)
        {
            this._Mesh = new Mesh(@fileName);
            this.transformationInfo = new Transformation_Info(2, 0.3, 0.5);
            this.coloringMode = Coloring_Mode.Default;
            this.mappingMode = Mapping_Mode.Continuous;
            //save min & max values for each type
            Mesh_Manager.dataTypeRange = new Dictionary<uint, KeyValuePair<double, double>>();
            foreach (string str in _Mesh.VarToIndex.Keys)
            {
                uint index = (uint)_Mesh.VarToIndex[str];
                double min = 0, max = 0;
                _Mesh.GetMinMaxValues(index, out min, out max);
                Mesh_Manager.dataTypeRange.Add(index, new KeyValuePair<double, double>(min, max));
            }
        }
        public Mesh_Manager(Mesh new_mesh)
        {
            this._Mesh = new_mesh;
            this.transformationInfo = new Transformation_Info(2, 0.3, 0.5);
            this.coloringMode = Coloring_Mode.Default;
            this.mappingMode = Mapping_Mode.Continuous;
        }
        public Mesh_Manager(string fileName, double Translation_offset, double Scaling_offset, double Rotation_offset)
        {
            this._Mesh = new Mesh(@fileName);
            this.transformationInfo = new Transformation_Info(Translation_offset, Scaling_offset, Rotation_offset);
            this.coloringMode = Coloring_Mode.Default;
            this.mappingMode = Mapping_Mode.Continuous;
        }
        #endregion

        #region Mesh Drawing
        public void MeshDraw()
        {
            switch (coloringMode)
            {
                case Coloring_Mode.Default:
                    this.MeshDraw_Default();
                    break;
                case Coloring_Mode.EdgeColoring:
                    this.MeshDraw_EdgeCoding();
                    break;
                case Coloring_Mode.FaceColoring:
                    this.MeshDraw_FaceCoding();
                    break;
            }
            switch (contourMode)
            {
                case Contour_Mode.LineContour:
                    this.DrawContourLines();
                    break;
                case Contour_Mode.FloodedContour:
                    this.DrawFloodedContours();
                    break;
                case Contour_Mode.IsoSurface:
                    this.DrawIsoSurface();
                    break;
            }
        }
        void MeshDraw_Default()
        {
            Gl.glClearColor(0, 0, 0, 0);
            Gl.glClear(Gl.GL_COLOR_BUFFER_BIT | Gl.GL_DEPTH_BUFFER_BIT);
            Gl.glMatrixMode(Gl.GL_MODELVIEW);
            Gl.glLoadMatrixd(_Mesh.Transformation.Data);

            foreach (Zone z in _Mesh.Zones)
            {
                Gl.glColor3d(1, 1, 1);
                foreach (Face f in z.Faces)
                {
                    f.glTell(z);
                }
                Gl.glFlush();
            }
        }
        void MeshDraw_EdgeCoding()
        {
            Gl.glClearColor(0, 0, 0, 0);
            Gl.glClear(Gl.GL_COLOR_BUFFER_BIT | Gl.GL_DEPTH_BUFFER_BIT);
            Gl.glMatrixMode(Gl.GL_MODELVIEW);
            Gl.glLoadMatrixd(_Mesh.Transformation.Data);

            foreach (Zone z in _Mesh.Zones)
            {
                foreach (Face f in z.Faces)
                {
                    Gl.glBegin(Gl.GL_LINES);
                    foreach (uint e in f.Edges)
                    {
                        Edge edge = z.Edges[e];
                        NormColor color = CalculateEdgeColor(z, edge);
                        Gl.glColor3d(color.R, color.G, color.B);
                        z.Vertices[edge.Start].Position.glTell();
                        z.Vertices[edge.End].Position.glTell();
                    }
                    Gl.glEnd();
                    Gl.glFlush();
                }
            }
        }
        void MeshDraw_FaceCoding()
        {
            Gl.glClearColor(0, 0, 0, 0);
            Gl.glClear(Gl.GL_COLOR_BUFFER_BIT | Gl.GL_DEPTH_BUFFER_BIT);
            Gl.glMatrixMode(Gl.GL_MODELVIEW);
            Gl.glLoadMatrixd(_Mesh.Transformation.Data);

            foreach (Zone z in _Mesh.Zones)
            {
                foreach (Face f in z.Faces)
                {
                    FillFace(z, f);
                }
            }
        }

        public void CreateContourLines()
        {
            Contour.contourColors = new List<Color>();
            Contour.contourLines = VisualizationOperations.CalculateLineContours(_Mesh, Coding_DatatypeIndex, numContours);
        }

        public void CreateFloodedContours()
        {
            Contour.contourColors = new List<Color>();
            Contour.contourPolygons = VisualizationOperations.CalculateFloodedContours(_Mesh, Coding_DatatypeIndex, numContours);
        }
        public void CreateIsoSurface()
        {
            Contour.isoSurfacesColors = new List<Color>();
            Contour.isoSurfaces = VisualizationOperations.CalculateIsoSurface(_Mesh, Coding_DatatypeIndex, numContours);
        }
        void DrawContourLines()
        {
            List<Color> contourColors = Contour.contourColors;
            List<Point3[]> contourLines = Contour.contourLines;

            Gl.glBegin(Gl.GL_LINES);
            for (int i = 0; i < contourLines.Count; i++)
            {
                NormColor color = NormalizeColor(contourColors[i]);
                Gl.glColor3d(color.R, color.G, color.B);
                if (contourLines[i].Length <= 1) continue;
                contourLines[i][0].glTell();
                contourLines[i][1].glTell();
            }
            Gl.glEnd();
            Gl.glFlush();
        }

        void DrawFloodedContours()
        {
            List<Color> contourColors = Contour.contourColors;
            List<List<Point3[]>> contourPolygons = Contour.contourPolygons;

            for (int i = 0; i < contourPolygons.Count; i++)
            {
                NormColor color = NormalizeColor(contourColors[i]);
                for (int j = 0; j < contourPolygons[i].Count; j++)
                {
                    Gl.glColor3d(color.R, color.G, color.B);
                    Gl.glBegin(Gl.GL_POLYGON);
                    for (int k = 0; k < contourPolygons[i][j].Length; k++)
                    {
                        contourPolygons[i][j][k].glTell();
                    }
                    Gl.glEnd();
                    Gl.glFlush();
                }
            }
        }
        void DrawIsoSurface()
        {
            List<Color> isoSurfacesColors = Contour.isoSurfacesColors;
            List<Point3[]> isoSurfaces = Contour.isoSurfaces;

            Gl.glBegin(Gl.GL_TRIANGLES);
            for (int i = 0; i < isoSurfaces.Count; i++)
            {
                NormColor color = NormalizeColor(isoSurfacesColors[i]);
                Gl.glColor3d(color.R, color.G, color.B);
                for (int j = 0; j < isoSurfaces[i].Length; j += 3)
                {
                    isoSurfaces[i][j].glTell();
                    isoSurfaces[i][j + 1].glTell();
                    isoSurfaces[i][j + 2].glTell();
                }
            }
            Gl.glEnd();
            Gl.glFlush();
        }

        void FillFace(Zone owner, Face face)
        {
            NormColor color;
            switch (owner.ElementType)
            {
                case ElementType.Tetrahedron:
                case ElementType.Triangle:
                    color = CalculateFaceColor(owner, face, FaceType.Triangular);
                    Gl.glColor3d(color.R, color.G, color.B);
                    Gl.glBegin(Gl.GL_TRIANGLES);
                    owner.Vertices[face.Vertices[0]].Position.glTell();
                    owner.Vertices[face.Vertices[1]].Position.glTell();
                    owner.Vertices[face.Vertices[2]].Position.glTell();
                    Gl.glEnd();
                    break;

                case ElementType.IJKBrick:
                case ElementType.IJKQuad:
                case ElementType.FEBrick:
                case ElementType.FEQuad:
                    color = CalculateFaceColor(owner, face, FaceType.Quadrilateral);
                    Gl.glColor3d(color.R, color.G, color.B);
                    Gl.glBegin(Gl.GL_QUADS);
                    owner.Vertices[face.Vertices[0]].Position.glTell();
                    owner.Vertices[face.Vertices[1]].Position.glTell();
                    owner.Vertices[face.Vertices[2]].Position.glTell();
                    owner.Vertices[face.Vertices[3]].Position.glTell();
                    Gl.glEnd();
                    break;
            }
        }
        #endregion

        #region Transformation Functions
        public void Translate(Translation_Mode mode)
        {
            switch (mode)
            {
                case Translation_Mode.Up:
                    _Mesh.Transformation.Translate(0, transformationInfo.translationOffset, 0);
                    break;
                case Translation_Mode.Down:
                    _Mesh.Transformation.Translate(0, -transformationInfo.translationOffset, 0);
                    break;
                case Translation_Mode.Left:
                    _Mesh.Transformation.Translate(-transformationInfo.translationOffset, 0, 0);
                    break;
                case Translation_Mode.Right:
                    _Mesh.Transformation.Translate(transformationInfo.translationOffset, 0, 0);
                    break;
                case Translation_Mode.Forward:
                    _Mesh.Transformation.Translate(0, 0, transformationInfo.translationOffset);
                    break;
                case Translation_Mode.Backward:
                    _Mesh.Transformation.Translate(0, 0, -transformationInfo.translationOffset);
                    break;
            }
        }
        public void Scale(Scaling_Mode mode)
        {
            double Scalar;
            switch (mode)
            {
                case Scaling_Mode.ZoomIn:
                    Scalar = 1 + transformationInfo.scalingOffset;
                    _Mesh.Transformation.Scale(Scalar, Scalar, Scalar);
                    break;
                case Scaling_Mode.ZoomOut:
                    Scalar = 1 - transformationInfo.scalingOffset;
                    _Mesh.Transformation.Scale(Scalar, Scalar, Scalar);
                    break;
            }
        }
        public void RotationZ(Rotation_Mode mode)
        {
            switch (mode)
            {
                case Rotation_Mode.ClockWise:
                    _Mesh.Transformation.Multiply(Matrix.RotationZ(-transformationInfo.rotationOffset), Order.Prepend);
                    break;
                case Rotation_Mode.AntiClockWise:
                    _Mesh.Transformation.Multiply(Matrix.RotationZ(transformationInfo.rotationOffset), Order.Prepend);
                    break;
            }
        }
        public void RotationX(Rotation_Mode mode)
        {
            switch (mode)
            {
                case Rotation_Mode.ClockWise:
                    _Mesh.Transformation.Multiply(Matrix.RotationX(-transformationInfo.rotationOffset), Order.Append);
                    break;
                case Rotation_Mode.AntiClockWise:
                    _Mesh.Transformation.Multiply(Matrix.RotationX(transformationInfo.rotationOffset), Order.Append);
                    break;
            }
        }
        public void RotationY(Rotation_Mode mode)
        {
            switch (mode)
            {
                case Rotation_Mode.ClockWise:
                    _Mesh.Transformation.Multiply(Matrix.RotationY(-transformationInfo.rotationOffset), Order.Append);
                    break;
                case Rotation_Mode.AntiClockWise:
                    _Mesh.Transformation.Multiply(Matrix.RotationY(transformationInfo.rotationOffset), Order.Append);
                    break;
            }
        }
        #endregion

        #region Color Calculations
        NormColor CalculateEdgeColor(Zone owner, Edge edge)
        {
            float Average_value = (float)(owner.Vertices[edge.Start].Data[Coding_DatatypeIndex]
                                + owner.Vertices[edge.End].Data[Coding_DatatypeIndex]) / 2;
            double min = Mesh_Manager.dataTypeRange[Coding_DatatypeIndex].Key, max = Mesh_Manager.dataTypeRange[Coding_DatatypeIndex].Value;

            Color_Mapper.minValue = (float)min;
            Color_Mapper.maxValue = (float)max;
            Color C = Color_Mapper.ValueToColor(Average_value, mappingMode);
            NormColor color = NormalizeColor(C);
            return color;
        }

        NormColor CalculateFaceColor(Zone owner, Face face, FaceType faceType)
        {
            float averageValue = 0;
            switch (faceType)
            {
                case FaceType.Triangular:
                    averageValue += (float)owner.Vertices[face.Vertices[0]].Data[Coding_DatatypeIndex];
                    averageValue += (float)owner.Vertices[face.Vertices[1]].Data[Coding_DatatypeIndex];
                    averageValue += (float)owner.Vertices[face.Vertices[2]].Data[Coding_DatatypeIndex];
                    averageValue /= 3;
                    break;
                case FaceType.Quadrilateral:
                    averageValue += (float)owner.Vertices[face.Vertices[0]].Data[Coding_DatatypeIndex];
                    averageValue += (float)owner.Vertices[face.Vertices[1]].Data[Coding_DatatypeIndex];
                    averageValue += (float)owner.Vertices[face.Vertices[2]].Data[Coding_DatatypeIndex];
                    averageValue += (float)owner.Vertices[face.Vertices[3]].Data[Coding_DatatypeIndex];
                    averageValue /= 4;
                    break;
            }

            double min = Mesh_Manager.dataTypeRange[Coding_DatatypeIndex].Key, max = Mesh_Manager.dataTypeRange[Coding_DatatypeIndex].Value;
            Color_Mapper.minValue = (float)min;
            Color_Mapper.maxValue = (float)max;
            Color C = Color_Mapper.ValueToColor(averageValue, mappingMode);
            NormColor color = NormalizeColor(C);
            return color;
        }
        #endregion

        #region Helper Methods
        NormColor NormalizeColor(Color color)
        {
            NormColor normColor = new NormColor();
            normColor.R = (double)color.R / 255.0;
            normColor.G = (double)color.G / 255.0;
            normColor.B = (double)color.B / 255.0;
            return normColor;
        }
        #endregion
    }

    public static class Color_Mapper
    {
        public static float minValue;
        public static float maxValue;
        public static Color[] colors = { Color.Blue, Color.Green, Color.Yellow, Color.Red };
        public static Color ValueToColor(float value, Mapping_Mode mode)
        {
            Color resultColor = Color.White;
            switch (mode)
            {
                case Mapping_Mode.Discrete:
                    resultColor = ValueToColor_LookUpTable(value);
                    break;
                case Mapping_Mode.Continuous:
                    resultColor = ValueToColor_TransferFunction(value);
                    break;
            }
            return resultColor;
        }
        static Color ValueToColor_TransferFunction(float value)
        {
            int numberOfColors = colors.Length;
            float deltaS = (maxValue - minValue) / (numberOfColors - 1);
            float d_S = (value - minValue) / deltaS;
            int i1 = Math.Max((int)d_S, 0);
            int i2 = Math.Min(i1 + 1, numberOfColors - 1);
            float alpha = d_S - i1;

            int R = (int)(colors[i1].R + alpha * (colors[i2].R - colors[i1].R));
            int G = (int)(colors[i1].G + alpha * (colors[i2].G - colors[i1].G));
            int B = (int)(colors[i1].B + alpha * (colors[i2].B - colors[i1].B));
            return Color.FromArgb(R, G, B);

        }
        static Color ValueToColor_LookUpTable(float value)
        {
            int numberOfColors = colors.Length;
            float delta = maxValue - minValue;
            int index = (int)(numberOfColors * (value - minValue) / delta);

            index = Math.Min(index, numberOfColors - 1);
            return colors[index];
        }
    }
}