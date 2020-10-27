using AoMEngineLibrary.Graphics;
using AoMEngineLibrary.Graphics.Brg;
using AoMEngineLibrary.Graphics.Ddt;
using AoMEngineLibrary.Graphics.Grn;
using AoMModelViewer.Graphics;
using HelixToolkit.Wpf;
using SharpGLTF.Schema2;
using SixLabors.ImageSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
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
        BrgFile file;
        Renderer? r;
        BitmapSource? bitmap;
        CancellationTokenSource cts;

        public MainWindow()
        {
            InitializeComponent();
            cts = new CancellationTokenSource();

            // Image convert
            {
                var ddt = new DdtFile();
                string fname = "animal bear map";
                //using (var fs = File.Open(@$"C:\Games\Age of Mythology\textures\t1extracted\{fname}.ddt", FileMode.Open, FileAccess.Read, FileShare.Read))
                using (var fs = File.Open(@$"C:\Games\Steam\steamapps\common\Age of Mythology\textures\{fname}.ddt", FileMode.Open, FileAccess.Read, FileShare.Read))
                {
                    ddt.Read(fs);
                }
                var imgConv = new DdtImageConverter();
                var imgs = imgConv.Convert(ddt);
                imgs[0][0].SaveAsPng($"{fname}.png");
            }
            return;

            {
                using (var fs = File.Open(@"C:\c\Argos\ptasev\Argos\current source\models\version2.0\sfx e locust path_walk.brg", FileMode.Open, FileAccess.Read, FileShare.Read))
                {
                    BrgFile f = new BrgFile(fs);
                    new BrgGltfConverter().Convert(f, new TextureManager(string.Empty)).SaveGLTF("./brgLocustWalk.gltf");
                }

                using (var fs = File.Open(@"C:\c\Argos\ptasev\Argos\current source\models\version2.0\sfx e locust path_idle.brg", FileMode.Open, FileAccess.Read, FileShare.Read))
                {
                    BrgFile f = new BrgFile(fs);
                    new BrgGltfConverter().Convert(f, new TextureManager(string.Empty)).SaveGLTF("./brgLocustIdle.gltf");
                }
            }

            string[] args = Environment.GetCommandLineArgs();

            if (args.Length > 1 && false)
            {
                file = new BrgFile(File.Open(args[1], FileMode.Open, FileAccess.Read, FileShare.Read));
                //file.WriteJson(File.Open(args[1] + ".json", FileMode.Create, FileAccess.Write, FileShare.Read));
                if (args.Length > 2 && args[2] == "-s")
                {
                    System.Windows.Application.Current.Shutdown();
                }
            }
            else
            {
                //archer e slinger_attacka.brg
                file = new BrgFile(File.Open(@"C:\Games\Steam\steamapps\common\Age of Mythology\models\version2.0\infantry g hoplite head standard.brg", FileMode.Open, FileAccess.Read, FileShare.Write));
                //file.WriteJson(File.Open("infantry g hoplite head standard.brg.json.brg", FileMode.Create, FileAccess.Write, FileShare.Read));
                var grnFile = new AoMEngineLibrary.Graphics.Grn.GrnFile();
                grnFile.Read(File.Open(@"C:\Games\Steam\steamapps\common\Age of Mythology\models\version2.0\ajax.grn", FileMode.Open, FileAccess.Read, FileShare.Write));
            }

            // bla
            //var ttt = ModelRoot.Load("grnGltf.gltf");

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
            matGroup.Children.Add(new DiffuseMaterial(new SolidColorBrush(System.Windows.Media.Color.FromArgb(255, 0xFF, 0x00, 0xFF))));
            geomod.Material = matGroup;

            //viewport3d.Children.Add(meshVis);
            //viewport3d.Children.Add(modVis);

            using (var fs = File.Open(@"C:\Games\Steam\steamapps\common\Age of Mythology\models\version1.0\ajax.grn", FileMode.Open, FileAccess.Read, FileShare.Read))
            using (var fs2 = File.Open(@"C:\Games\Steam\steamapps\common\Age of Mythology\models\version1.0\ajax_walka.grn", FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                GrnFile grn = new GrnFile();
                grn.Read(fs);
                GrnFile grnAnim = new GrnFile();
                grnAnim.Read(fs2);
                grn.SetAnimation(grnAnim);
                new GrnGltfConverter().Convert(grn).SaveGLTF("./grnGltf.gltf");
                var newGrn = new GltfGrnConverter().Convert(ModelRoot.Load("./grnGltf.gltf"));
                new GrnGltfConverter().Convert(newGrn).SaveGLTF("./grnGltf2.gltf");
            }

            //file = new BrgFile(File.Open(@"C:\Games\Steam\steamapps\common\Age of Mythology\models\version2.0\animal aurochs_attacka.brg", FileMode.Open, FileAccess.Read));
            //file = new BrgFile(File.Open(@"C:\Games\Steam\steamapps\common\Age of Mythology\models\version2.0\infantry g hoplite head bronze.brg", FileMode.Open, FileAccess.Read));
            file = new BrgFile(File.Open(@"C:\Games\Steam\steamapps\common\Age of Mythology\models\version2.0\infantry g hoplite_walka.brg", FileMode.Open, FileAccess.Read));
            glTFLoader.Schema.Gltf gltf;
            //using (var stream = File.Open("dataBuffer.bin", FileMode.Create, FileAccess.Write, FileShare.Read))
            //{
            //    GltfFormatter frmt = new GltfFormatter();
            //    gltf = frmt.FromBrg(file, stream);
            //}
            //glTFLoader.Interface.SaveModel(gltf, "brgGltf.gltf");
            new BrgGltfConverter().Convert(file, new TextureManager(string.Empty)).SaveGLTF("./brgGltfConv.gltf");

            var gltfModel = ModelRoot.Load("blendergltf/blendergltf.gltf");
            //var gltfModel = ModelRoot.Load("brgGltf.gltf");
            BrgFile gltfBrg = new GltfBrgConverter().Convert(gltfModel, new GltfBrgParameters());
            using (var stream = File.Open("dataBuffer3Recreated.bin", FileMode.Create, FileAccess.Write, FileShare.Read))
            {
                GltfFormatter frmt = new GltfFormatter();
                gltf = frmt.FromBrg(gltfBrg, stream);
            }
            glTFLoader.Interface.SaveModel(gltf, "brgGltf3Recreated.gltf");

            this.Loaded += MainWindow_Loaded;
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            cts.Cancel();
        }

        private async void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            using (r = new Renderer(file))
            {
                try
                {
                    while (await r.Channel.WaitToReadAsync(cts.Token))
                    {
                        if (r.Channel.TryRead(out (byte[],int,int) item))
                        {
                            fpsLabel.Content = r.FPS.ToString("000");
                            bitmap = RenderTarget.CreateBitmapSource(item.Item1, item.Item2, item.Item3);
                            fimg.Width = bitmap.Width;
                            fimg.Height = bitmap.Height;
                            fimg.Source = bitmap;
                        }
                    }
                }
                catch
                {
                }
            }
        }

        private void fimg_KeyDown(object sender, KeyEventArgs e)
        {
            if (r == null) return;

            if (e.Key == Key.W)
            {
                r.MoveCameraForward(1);
            }
            else if (e.Key == Key.S)
            {
                r.MoveCameraBackward(1);
            }
            else if (e.Key == Key.A)
            {
                r.MoveCameraLeft(1);
            }
            else if (e.Key == Key.D)
            {
                r.MoveCameraRight(1);
            }
        }

        bool firstMouse = true;
        double lastX;
        double lastY;
        double yaw = 0;
        double pitch;
        private void Window_MouseMove(object sender, MouseEventArgs e)
        {
            if (r == null) return;

            var pos = e.GetPosition(fimg);
            if (firstMouse)
            {
                lastX = fimg.Width / 2;
                lastY = fimg.Height / 2;
                firstMouse = false;
            }

            double xoffset = lastX - pos.X;
            double yoffset = lastY - pos.Y;
            lastX = pos.X;
            lastY = pos.Y;

            double sensitivity = 0.20;
            xoffset *= sensitivity;
            yoffset *= sensitivity;

            yaw += xoffset;
            pitch += yoffset;

            pitch = Math.Clamp(pitch, -89, 89);

            double yrad = yaw * Math.PI / 180.0;
            double prad = pitch * Math.PI / 180.0;

            var yawMat = Matrix4x4.CreateRotationY((float)yrad);
            var pitchMat = Matrix4x4.CreateRotationX((float)prad);
            Vector3 direction = Vector3.Transform(Vector3.Transform(new Vector3(0, 0, 1), pitchMat), yawMat);

            //Vector3 direction = new Vector3(
            //    (float)(Math.Cos(yrad)),
            //    (float)0,
            //    (float)(Math.Sin(yrad)));
            r.SetCameraTarget(direction);
        }
    }

    internal struct Vec2i
    {
        public int X;
        public int Y;

        public Vec2i(int x, int y)
        {
            X = x; Y = y;
        }
    }

    public struct Color
    {
        readonly byte a, r, g, b;
        public uint color;

        public Color(byte a, byte r, byte g, byte b)
        {
            this.a = a;
            this.r = r;
            this.g = g;
            this.b = b;
            this.color = (uint)((a << 24) | (r << 16) | (g << 8) | b);
        }
    }
}
