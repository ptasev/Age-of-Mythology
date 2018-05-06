using AoMEngineLibrary.Graphics.Brg;
using HelixToolkit.Wpf;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace AoMModelViewer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            string[] args = Environment.GetCommandLineArgs();
            BrgFile file;

            if (args.Length > 1 && false)
            {
                file = new BrgFile(File.Open(args[1], FileMode.Open, FileAccess.Read, FileShare.Read));
                file.WriteJson(File.Open(args[1] + ".json", FileMode.Create, FileAccess.Write, FileShare.Read));
                if (args.Length > 2 && args[2] == "-s")
                {
                    Application.Current.Shutdown();
                }
            }
            else
            {
                //archer e slinger_attacka.brg
                file = new BrgFile(File.Open(@"C:\Games\Steam\steamapps\common\Age of Mythology\models\version2.0\infantry g hoplite head standard.brg", FileMode.Open, FileAccess.Read, FileShare.Write));
                file.WriteJson(File.Open("infantry g hoplite head standard.brg.json.brg", FileMode.Create, FileAccess.Write, FileShare.Read));
                var grnFile = new AoMEngineLibrary.Graphics.Grn.GrnFile();
                grnFile.Read(File.Open(@"C:\Games\Steam\steamapps\common\Age of Mythology\models\version2.0\ajax.grn", FileMode.Open, FileAccess.Read, FileShare.Write));
            }

            //BrgFile t = new BrgFile();
            //t.ReadJson(File.Open("hi.brg", FileMode.Open, FileAccess.Read, FileShare.Read));
            //t.WriteJson(File.Open("hi2.brg", FileMode.Create, FileAccess.Write, FileShare.Read));
            
            List<Point3D> positions = new List<Point3D>(file.Meshes[0].Vertices.Count);
            List<int> triangleIndices = new List<int>(file.Meshes[0].Faces.Count * 3);

            for (int i = 0; i < file.Meshes[0].Vertices.Count; ++i)
            {
                positions.Add(new Point3D(
                    file.Meshes[0].Vertices[i].X,
                    file.Meshes[0].Vertices[i].Y,
                    file.Meshes[0].Vertices[i].Z));
            }

            for (int i = 0; i < file.Meshes[0].Faces.Count; ++i)
            {
                triangleIndices.Add(file.Meshes[0].Faces[i].Indices[0]);
                triangleIndices.Add(file.Meshes[0].Faces[i].Indices[1]);
                triangleIndices.Add(file.Meshes[0].Faces[i].Indices[2]);
            }

            Mesh3D mesh = new Mesh3D(positions, triangleIndices);
            MeshVisual3D meshVis = new MeshVisual3D();
            meshVis.Mesh = mesh;

            ModelVisual3D modVis = new ModelVisual3D();
            GeometryModel3D geomod = new GeometryModel3D();
            MeshGeometry3D meshgeo = new MeshGeometry3D();

            //modVis.Transform = new MatrixTransform3D(new Matrix3D(-1, 0, 0, 0, 0, 0, -1, 0, 0, 1, 0, 0, 0, 0, 0, 1));
            //modVis.Transform = new MatrixTransform3D(new Matrix3D(-0, -0, -1, 0, -1, -0, -0, 0, 0, 1, 0, 0, 0, 0, 0, 1));
            modVis.Transform = new MatrixTransform3D(new Matrix3D(0, -1, 0, 0, 0, 0, 1, 0, 1, 0, 0, 0, 0, 0, 0, 1));
            meshgeo.Positions = new Point3DCollection(positions);
            meshgeo.TriangleIndices = new Int32Collection(triangleIndices);
            geomod.Geometry = meshgeo;
            modVis.Content = geomod;
            MaterialGroup matGroup = new MaterialGroup();
            matGroup.Children.Add(new DiffuseMaterial(new SolidColorBrush(Color.FromArgb(255, 0xFF, 0x00, 0xFF))));
            geomod.Material = matGroup;

            //viewport3d.Children.Add(meshVis);
            viewport3d.Children.Add(modVis);

            file = new BrgFile(File.Open(@"C:\Games\Steam\steamapps\common\Age of Mythology\models\version2.0\animal aurochs_attacka.brg", FileMode.Open, FileAccess.Read));
            glTFLoader.Schema.Gltf gltf;
            using (var stream = File.Open("dataBuffer.bin", FileMode.Create, FileAccess.Write, FileShare.Read))
            {
                GltfFormatter frmt = new GltfFormatter();
                gltf = frmt.FromBrg(file, stream);
            }
            glTFLoader.Interface.SaveModel(gltf, "brgGltf.gltf");
        }
    }
}
