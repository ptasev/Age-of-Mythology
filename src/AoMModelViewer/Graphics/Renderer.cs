using AoMEngineLibrary.Graphics.Brg;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace AoMModelViewer.Graphics
{
    public class Renderer : IDisposable
    {
        readonly Thread renderThread;
        readonly RenderTarget target;
        readonly BrgFile file;

        readonly Channel<(byte[],int,int)> channel;
        public ChannelReader<(byte[], int, int)> Channel => channel.Reader;

        public RenderTarget Target => target;

        object camLock;
        Vector3 camera;
        Vector3 camTarget;
        Vector3 cameraUp;

        float deltaTime;
        float lastFrame;
        public float FPS
        {
            get
            {
                lock (camLock) return 1 / deltaTime;
            }
        }

        public Renderer(BrgFile file)
        {
            target = new RenderTarget(400, 400);
            this.file = file;
            channel = System.Threading.Channels.Channel.CreateBounded<(byte[], int, int)>(new BoundedChannelOptions(2)
            {
                FullMode = BoundedChannelFullMode.DropOldest,
                SingleWriter = true
            });

            camLock = new object();
            camera = new Vector3(0, 0, 10);
            camTarget = new Vector3(0, 0, -1);
            cameraUp = new Vector3(0, 1, 0);

            run = 1;
            renderThread = new Thread(Run);
            renderThread.Start();
        }

        private ValueTask PublishAsync((byte[], int, int) item)
        {
            async Task AsyncSlowPath((byte[], int, int) thing)
            {
                await channel.Writer.WriteAsync(thing);
            }

            return channel.Writer.TryWrite(item) ? default : new ValueTask(AsyncSlowPath(item));
        }

        private async void Run()
        {
            int pixCount = target.Width * target.Height;
            float[] zbuffer = new float[target.Width * target.Height];

            var lightDir = new Vector3(0, 0, -1);

            Matrix4x4 viewport = Matrix4x4.Identity;
            viewport.M41 = (0 / 8) + target.Width / 2.0f;
            viewport.M42 = (0 / 8) + target.Height / 2.0f;
            viewport.M43 = 1.0f;
            viewport.M11 = target.Width / 2.0f;
            viewport.M22 = target.Height / 2.0f;
            viewport.M33 = 0;

            Color black = new Color(255, 0, 0, 0);
            Stopwatch sw = new Stopwatch();
            sw.Start();
            while (run == 1)
            {
                Vector3 camera;
                Vector3 camTarget;
                lock (camLock)
                {
                    float currentFrame = (float)sw.Elapsed.TotalSeconds;
                    deltaTime = currentFrame - lastFrame;
                    lastFrame = currentFrame;

                    camera = this.camera;
                    camTarget = this.camTarget;
                }

                Matrix4x4 modelView = Matrix4x4.CreateLookAt(camera, camera + camTarget, cameraUp);
                Matrix4x4 proj = Matrix4x4.Identity;
                proj.M34 = 1.0f / (camera - camTarget).Length();
                //proj = Matrix4x4.CreatePerspective(target.Width, target.Height, 0.1f, 20.0f);
                proj = Matrix4x4.CreatePerspectiveFieldOfView(45f * (float)Math.PI / 180.0f, target.Width / target.Height, 0.1f, 20.0f);

                for (int i = 0; i < pixCount; ++i) zbuffer[i] = float.MaxValue;
                for (int i = 0; i < target.Height; ++i)
                {
                    for (int j = 0; j < target.Width; ++j)
                    {
                        target.Set(j, i, ref black);
                    }
                }

                BrgMesh mesh = file.Meshes[0];
                mesh.Vertices.ForEach(x => x.X = -x.X);
                mesh.Normals.ForEach(x => x.X = -x.X);
                foreach (var face in mesh.Faces)
                {
                    var temp = face.B;
                    face.B = face.C;
                    face.C = temp;
                }

                //mesh = new BrgMesh(file);
                //mesh.Vertices = new List<Vector3>();
                //mesh.Vertices.Add(new Vector3(-3, -4, 0));
                //mesh.Vertices.Add(new Vector3(6, -8, 0));
                //mesh.Vertices.Add(new Vector3(0, 5, 0));
                //mesh.Vertices.Add(new Vector3(-3, -4, -10));
                //mesh.Vertices.Add(new Vector3(6, -8, -10));
                //mesh.Vertices.Add(new Vector3(0, 5, -10));
                //mesh.Normals.Add(new Vector3(0, 0, 1));
                //mesh.Normals.Add(new Vector3(0, 0, 1));
                //mesh.Normals.Add(new Vector3(0, 0, 1));
                //mesh.Normals.Add(new Vector3(0, 0, 1));
                //mesh.Normals.Add(new Vector3(0, 0, 1));
                //mesh.Normals.Add(new Vector3(0, 0, 1));
                //mesh.TextureCoordinates.Add(new Vector3(0.5f, 0.5f, 1));
                //mesh.TextureCoordinates.Add(new Vector3(0.5f, 0.5f, 1));
                //mesh.TextureCoordinates.Add(new Vector3(0.5f, 0.5f, 1));
                //mesh.TextureCoordinates.Add(new Vector3(0.75f, 0.75f, 1));
                //mesh.TextureCoordinates.Add(new Vector3(0.75f, 0.75f, 1));
                //mesh.TextureCoordinates.Add(new Vector3(0.75f, 0.75f, 1));
                //var face = new AoMEngineLibrary.Graphics.Model.Face();
                //face.Indices.Add(0); face.Indices.Add(1); face.Indices.Add(2);
                //mesh.Faces.Add(face);
                //face = new AoMEngineLibrary.Graphics.Model.Face();
                //face.Indices.Add(3); face.Indices.Add(4); face.Indices.Add(5);
                //mesh.Faces.Add(face);

                Shader shader = new Shader(mesh.Vertices, mesh.Normals, mesh.TextureCoordinates, mesh.Faces, lightDir, modelView, proj, viewport);
                for (int i = 0; i < mesh.Faces.Count; i++)
                {
                    Vector4[] screenCoords = new Vector4[3];
                    byte clipMask = 0;
                    for (int j = 0; j < 3; j++)
                    {
                        // Vertex shader
                        Vector4 glPos = shader.Vertex(i, j);

                        // Determine whether this vertex is outside of viewing volume
                        bool clipVertex = !(glPos.X >= -glPos.W && glPos.X <= glPos.W);
                        clipVertex |= !(glPos.Y >= -glPos.W && glPos.Y <= glPos.W);
                        clipVertex |= !(glPos.Z >= 0 && glPos.Z <= glPos.W);
                        if (clipVertex)
                            clipMask |= (byte)(1 << j);

                        // Perspective Divide
                        glPos.W = 1 / glPos.W;
                        glPos.X *= glPos.W;
                        glPos.Y *= glPos.W;
                        glPos.Z *= glPos.W;

                        // Viewport transform
                        glPos.X = (glPos.X * viewport.M11 + viewport.M41);
                        glPos.Y = (glPos.Y * viewport.M22 + viewport.M42);
                        screenCoords[j] = glPos;
                    }

                    // TODO: Clip triangles that are partially out of view
                    Triangle(target, screenCoords, shader, zbuffer);
                }

                await PublishAsync((target.GetRawData(), target.Width, target.Height));
            }

            channel.Writer.Complete();
        }

        private int run = 0;
        private void Stop()
        {
            Interlocked.Exchange(ref run, 0);
        }

        public void MoveCameraForward(float factor)
        {
            lock (camLock)
            {
                camera += factor * camTarget;
            }
        }

        public void MoveCameraBackward(float factor)
        {
            lock (camLock)
            {
                camera -= factor * camTarget;
            }
        }

        public void MoveCameraLeft(float factor)
        {
            lock (camLock)
            {
                camera -= factor * Vector3.Normalize(Vector3.Cross(camTarget, cameraUp));
            }
        }

        public void MoveCameraRight(float factor)
        {
            lock (camLock)
            {
                camera += factor * Vector3.Normalize(Vector3.Cross(camTarget, cameraUp));
            }
        }

        public void SetCameraTarget(Vector3 target)
        {
            lock (camLock)
            {
                camTarget = -Vector3.Normalize(target);
            }
        }

        void Triangle(RenderTarget target, Vector4[] pts, Shader shader, Span<float> zBuffer)
        {
            Vec2i clamp = new Vec2i(target.Width - 1, target.Height - 1);
            Vec2i bboxmin = new Vec2i(target.Width - 1, target.Height - 1);
            Vec2i bboxmax = new Vec2i(0, 0);
            for (int i = 0; i < 3; i++)
            {
                bboxmin.X = Math.Min(bboxmin.X, (int)pts[i].X);
                bboxmax.X = Math.Max(bboxmax.X, (int)pts[i].X);
                bboxmin.Y = Math.Min(bboxmin.Y, (int)pts[i].Y);
                bboxmax.Y = Math.Max(bboxmax.Y, (int)pts[i].Y);
            }
            // Clamp the bounding box
            bboxmin.X = Math.Max(0, bboxmin.X);
            bboxmax.X = Math.Min(clamp.X, bboxmax.X);
            bboxmin.Y = Math.Max(0, bboxmin.Y);
            bboxmax.Y = Math.Min(clamp.Y, bboxmax.Y);

            // Determine triangle area
            float area = Edge(ref pts[0], ref pts[1], ref pts[2]);
            if (Math.Abs(area) < 0.01f)
                return;

            // Barycentric coord steps for window coords
            Vector3 bcxStep = new Vector3(
                pts[2].Y - pts[1].Y,
                pts[0].Y - pts[2].Y,
                pts[1].Y - pts[0].Y);
            Vector3 bcyStep = new Vector3(
                pts[1].X - pts[2].X,
                pts[2].X - pts[0].X,
                pts[0].X - pts[1].X);

            // Barycentric coords at min row
            Vector4 min = new Vector4(bboxmin.X + 0.5f, bboxmin.Y + 0.5f, 0, 0);
            Vector3 bcRow = new Vector3(
                Edge(ref pts[1], ref pts[2], ref min),
                Edge(ref pts[2], ref pts[0], ref min),
                Edge(ref pts[0], ref pts[1], ref min));

            // Vector one over z (stored in w component)
            Vector3 oneOverW = new Vector3(pts[0].W, pts[1].W, pts[2].W);

            Vector2 P = new Vector2();
            Vector3 bcc;
            for (int y = bboxmin.Y; y <= bboxmax.Y; y++)
            {
                bcc = bcRow;

                for (int x = bboxmin.X; x <= bboxmax.X; x++)
                {
                    P.X = x + 0.5f; P.Y = y + 0.5f;

                    // TODO: Check if on edge, and top-left edge, skip if not

                    Vector3 c = bcc / area;
                    if (c.X >= 0 && c.Y >= 0 && c.Z >= 0)
                    {
                        // interpolate inverse depth linearly
                        float ndcZInterpolated = Vector3.Dot(c, new Vector3(pts[0].Z, pts[1].Z, pts[2].Z));
                        float oneOverWInterpolated = Vector3.Dot(c, oneOverW);

                        // Clip fragment to near/far planes (assuming NDC Z range is [0,1])
                        if (ndcZInterpolated >= 0 && ndcZInterpolated <= 1)
                        {
                            // Convert barycentric to be perspective correct
                            float z = 1 / oneOverWInterpolated;
                            Vector3 perpsectiveBarycentric = z * c * oneOverW;

                            int zIndex = (x + y * target.Width);
                            if (z < zBuffer[zIndex] && !shader.Fragment(perpsectiveBarycentric, out Color color))
                            {
                                zBuffer[zIndex] = z;
                                target.Set(x, y, ref color);
                            }
                        }
                    }

                    bcc += bcxStep;
                }

                bcRow += bcyStep;
            }
        }

        private static float Edge(ref Vector4 a, ref Vector4 b, ref Vector4 c)
        {
            return (c.X - a.X) * (b.Y - a.Y) - (c.Y - a.Y) * (b.X - a.X);
        }

        public void Dispose()
        {
            Stop();
        }
    }
}
