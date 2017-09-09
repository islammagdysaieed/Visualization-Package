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
    public partial class Form1 : Form
    {
        public string fileName;
        Mesh_Manager meshManager;

        public Form1()
        {
            InitializeComponent();
            InitGraphics();
        }

        void InitGraphics()
        {
            Mode_2D();
        }

        void Mode_2D()
        {
            simpleOpenGlControl1.InitializeContexts();
            simpleOpenGlControl1.Paint += new PaintEventHandler(simpleOpenGlControl1_Paint);
            int height = simpleOpenGlControl1.Height;
            int width = simpleOpenGlControl1.Width;
            Gl.glViewport(0, 0, width, height);
            Gl.glMatrixMode(Gl.GL_PROJECTION);
            Gl.glLoadIdentity();
            Glu.gluOrtho2D(-100, 100, -100, 100);

            isosurfaceButton.Enabled = false;
        }

        void Mode_3D()
        {
            simpleOpenGlControl1.InitializeContexts();
            simpleOpenGlControl1.Paint += new PaintEventHandler(simpleOpenGlControl1_Paint);
            int height = simpleOpenGlControl1.Height;
            int width = simpleOpenGlControl1.Width;
            Gl.glViewport(0, 0, width, height);
            Gl.glMatrixMode(Gl.GL_PROJECTION);
            Gl.glLoadIdentity();
            Glu.gluPerspective(45.0f, (double)width / (double)height, 0.01f, 500.0f);
            Gl.glEnable(Gl.GL_DEPTH_TEST);
            Gl.glDepthFunc(Gl.GL_LESS);

            isosurfaceButton.Enabled = true;
        }

        private void loadFileButton_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog1 = new OpenFileDialog();
            DialogResult result = openFileDialog1.ShowDialog(); // Show the dialog.
            if (result == DialogResult.OK)
            {
                fileName = openFileDialog1.FileName;
                meshManager = new Mesh_Manager(@fileName);
                simpleOpenGlControl1.Invalidate();
            }
            //loading paramters
            LoadDataTypesToListBox();
            LoadMappingModesToComboBox();
            num_Contours.Clear();
            if (!yAxisRadio.Checked && !xAxisRadio.Checked)
            {
                zAxisRadio.Checked = true;
            }
            if (threeDRadioBtn.Checked == true)
            {
                for (int i = 0; i < 100; i++)
                    meshManager.Translate(Translation_Mode.Backward);
            }
        }

        private void simpleOpenGlControl1_Paint(object sender, PaintEventArgs e)
        {
            if (meshManager != null)
            {
                meshManager.MeshDraw();
            }
        }

        private void simpleOpenGlControl1_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.W:
                    meshManager.Translate(Translation_Mode.Up);
                    break;
                case Keys.S:
                    meshManager.Translate(Translation_Mode.Down);
                    break;
                case Keys.A:
                    meshManager.Translate(Translation_Mode.Left);
                    break;
                case Keys.D:
                    meshManager.Translate(Translation_Mode.Right);
                    break;
                case Keys.D1:
                    meshManager.Translate(Translation_Mode.Forward);
                    break;
                case Keys.D2:
                    meshManager.Translate(Translation_Mode.Backward);
                    break;
                ///rotation
                case Keys.Y:
                    meshManager.Scale(Scaling_Mode.ZoomIn);
                    break;
                case Keys.H:
                    meshManager.Scale(Scaling_Mode.ZoomOut);
                    break;
                case Keys.G:
                    if (zAxisRadio.Checked)
                        meshManager.RotationZ(Rotation_Mode.AntiClockWise);
                    else if (yAxisRadio.Checked)
                        meshManager.RotationY(Rotation_Mode.AntiClockWise);
                    else if (xAxisRadio.Checked)
                        meshManager.RotationX(Rotation_Mode.AntiClockWise);
                    break;
                case Keys.J:
                    if (zAxisRadio.Checked)
                        meshManager.RotationZ(Rotation_Mode.ClockWise);
                    else if (yAxisRadio.Checked)
                        meshManager.RotationY(Rotation_Mode.ClockWise);
                    else if (xAxisRadio.Checked)
                        meshManager.RotationX(Rotation_Mode.ClockWise);
                    break;
            }
            simpleOpenGlControl1.Refresh();
        }

        private void mappingMode_combo_SelectedIndexChanged(object sender, EventArgs e)
        {
            meshManager.mappingMode = (Mapping_Mode)mappingMode_combo.SelectedItem;
            simpleOpenGlControl1.Refresh();
            colorsPanel.Refresh();
        }

        //loading functions
        void LoadDataTypesToListBox()
        {
            if (meshManager != null)
            {
                dataName_ListBox.Items.Clear();
                int numOfValues = meshManager._Mesh.VarToIndex.Count;
                foreach (string str in meshManager._Mesh.VarToIndex.Keys)
                {
                    dataName_ListBox.Items.Add(str);
                }
                if (numOfValues > 0)
                {
                    dataName_ListBox.SelectedIndex = 0;
                    meshManager.codingDatatype = (string)dataName_ListBox.SelectedItem;
                    UpdateMinMaxValues();
                }
            }
        }

        void LoadMappingModesToComboBox()
        {
            mappingMode_combo.Items.Clear();
            mappingMode_combo.Items.Add(Mapping_Mode.Continuous);
            mappingMode_combo.Items.Add(Mapping_Mode.Discrete);
            mappingMode_combo.SelectedItem = mappingMode_combo.Items[0];
        }

        private void colorsPanel_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = colorsPanel.CreateGraphics();
            int colorArrayLength = Color_Mapper.colors.Length;
            //////////////////////////////////////////      

            if (meshManager != null && meshManager.mappingMode == Mapping_Mode.Discrete)
            {
                for (int i = 0; i < colorArrayLength; i++)
                {
                    g.FillRectangle(
                        new SolidBrush(Color_Mapper.colors[i]),
                        (i * colorsPanel.Width) / (colorArrayLength),
                        0.0f,
                        colorsPanel.Width / (colorArrayLength),
                        colorsPanel.Height);
                }
                return;
            }
            for (int i = 0; i < colorArrayLength - 1; i++)
            {
                LinearGradientBrush b = new LinearGradientBrush(
                    new Rectangle(0, 0, colorsPanel.Width / (colorArrayLength - 1), colorsPanel.Height),
                    Color_Mapper.colors[i],
                    Color_Mapper.colors[i + 1],
                    LinearGradientMode.Horizontal
                    );

                g.FillRectangle(
                    b,
                    (i * colorsPanel.Width) / (colorArrayLength - 1) + 1,
                    0.0f,
                    colorsPanel.Width / (colorArrayLength - 1),
                    colorsPanel.Height);
            }
        }

        //if mode selected before loading the mesh
        void CheckSelectedColorMode()
        {
            if (defaultColoringRadio.Checked)
            {
                meshManager.coloringMode = Coloring_Mode.Default;
            }
            else if (edgeColoringRadio.Checked)
            {
                meshManager.coloringMode = Coloring_Mode.EdgeColoring;
            }
            else if (faceColoringRadio.Checked)
            {
                meshManager.coloringMode = Coloring_Mode.FaceColoring;
            }
            simpleOpenGlControl1.Refresh();
        }

        private void defaultColoringRadio_CheckedChanged(object sender, EventArgs e)
        {
            if (meshManager != null)
            {
                if (defaultColoringRadio.Checked)
                {
                    meshManager.coloringMode = Coloring_Mode.Default;
                }
            }
            simpleOpenGlControl1.Refresh();
        }

        private void faceColoringRadio_CheckedChanged(object sender, EventArgs e)
        {
            if (meshManager != null)
            {
                if (faceColoringRadio.Checked)
                {
                    meshManager.coloringMode = Coloring_Mode.FaceColoring;
                }
            }
            simpleOpenGlControl1.Refresh();
        }

        private void edgeColoringRadio_CheckedChanged(object sender, EventArgs e)
        {
            if (meshManager != null)
            {
                if (edgeColoringRadio.Checked)
                {
                    meshManager.coloringMode = Coloring_Mode.EdgeColoring;
                }
            }
            simpleOpenGlControl1.Refresh();
        }

        private void dataName_ListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (meshManager != null)
            {
                meshManager.codingDatatype = (string)dataName_ListBox.SelectedItem;
                UpdateMinMaxValues();
                meshManager.contourMode = Contour_Mode.None;
                noContourRadioBtn.Checked = true;
                simpleOpenGlControl1.Refresh();
            }
        }

        void UpdateMinMaxValues()
        {
            double min = 0, max = 0;
            meshManager._Mesh.GetMinMaxValues(meshManager.Coding_DatatypeIndex, out min, out max);
            minTextBox.Text = min.ToString();
            maxTextBox.Text = max.ToString();
        }

        private void num_Contours_TextChanged(object sender, EventArgs e)
        {
            noContourRadioBtn.Checked = true;
            floodedContourRadioBtn.Checked = false;
            lineContourRadioBtn.Checked = false;
            Contour.isFlooded = false;
        }

        private void noContourRadioBtn_CheckedChanged(object sender, EventArgs e)
        {
            if (meshManager != null)
            {
                if (noContourRadioBtn.Checked)
                {
                    Contour.isFlooded = false;
                    meshManager.contourMode = Contour_Mode.None;
                }
            }
            simpleOpenGlControl1.Refresh();
        }

        private void lineContourRadioBtn_CheckedChanged(object sender, EventArgs e)
        {
            if (meshManager != null && num_Contours.Text.Length != 0)
            {
                if (lineContourRadioBtn.Checked)
                {
                    Contour.isFlooded = false;
                    meshManager.contourMode = Contour_Mode.LineContour;
                    meshManager.numContours = Convert.ToInt32(num_Contours.Text);
                    meshManager.CreateContourLines();
                }
            }
            simpleOpenGlControl1.Refresh();
        }

        private void floodedContourRadioBtn_CheckedChanged(object sender, EventArgs e)
        {
            if (meshManager != null && num_Contours.Text.Length != 0)
            {
                if (floodedContourRadioBtn.Checked)
                {
                    Contour.isFlooded = true;
                    meshManager.contourMode = Contour_Mode.FloodedContour;
                    meshManager.numContours = Convert.ToInt32(num_Contours.Text);
                    meshManager.CreateFloodedContours();
                }
                else
                {
                    Contour.isFlooded = false;
                }
            }
            simpleOpenGlControl1.Refresh();
        }

        private void isosurfaceButton_CheckedChanged(object sender, EventArgs e)
        {
            if (meshManager != null && num_Contours.Text.Length != 0)
            {
                if (isosurfaceButton.Checked)
                {
                    Contour.isFlooded = false;
                    meshManager.contourMode = Contour_Mode.IsoSurface;
                    meshManager.numContours = Convert.ToInt32(num_Contours.Text);
                    meshManager.CreateIsoSurface();
                }
            }
            simpleOpenGlControl1.Refresh();
        }

        private void threeDRadioBtn_CheckedChanged(object sender, EventArgs e)
        {
            if (threeDRadioBtn.Checked == true) Mode_3D();
            else Mode_2D();
        }
    }
}
