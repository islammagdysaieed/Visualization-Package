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
    class Contour

    {
        public static List<Color> contourColors;
        public static List<Point3[]> contourLines;
        public static List<Color> isoSurfacesColors;
        public static List<Point3[]> isoSurfaces;

        public static List<List<Point3[]>> contourPolygons;
        public static bool isFlooded;

        public Contour()
        {
            isFlooded = false;
        }
    }
}
