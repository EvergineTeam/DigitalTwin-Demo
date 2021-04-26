using System.Collections.Generic;
using System.Diagnostics;
using WaveEngine.Common.Graphics;
using WaveEngine.Framework;
using WaveEngine.Framework.Graphics;
using WaveEngine.Framework.Services;
using WaveEngine.Mathematics;
using WaveEngine.OpenGL;
using WaveEngine.Web;
using WebAssembly;

namespace DigitalTwin.Web
{
    public class Program
    {
        private static readonly Dictionary<string, WebSurface> appCanvas = new Dictionary<string, WebSurface>();

        public static void Main(string canvasId)
        {
            // Hack for AOT dll dependencies
            var cp = new WaveEngine.Components.Graphics3D.Spinner();

            // Create app
            var application = new MyApplication();

            MyApplication.EntitySelected += MyApplication_EntitySelected;
            MyApplication.TrackerAngleUpdated += MyApplication_TrackerAngleUpdated;

            // Create Services
            var windowsSystem = new WebWindowsSystem();
            application.Container.RegisterInstance(windowsSystem);

            var document = (JSObject)Runtime.GetGlobalObject("document");
            var canvas = (JSObject)document.Invoke("getElementById", canvasId);
            var surface = (WebSurface)windowsSystem.CreateSurface(canvas);
            appCanvas[canvasId] = surface;
            ConfigureGraphicsContext(application, surface);

            // Audio is currently unsupported
            //var xaudio = new WaveEngine.XAudio2.XAudioDevice();
            //application.Container.RegisterInstance(xaudio);

            Stopwatch clockTimer = Stopwatch.StartNew();
            windowsSystem.Run(
                () =>
                {
                    application.Initialize();
                    Runtime.InvokeJS("WaveEngine.init();");
                },
                () =>
                {
                    var gameTime = clockTimer.Elapsed;
                    clockTimer.Restart();

                    application.UpdateFrame(gameTime);
                    application.DrawFrame(gameTime);
                });
        }

        private static void MyApplication_EntitySelected(object sender, string e)
        {
            Runtime.InvokeJS($"WaveEngine.onEvent(\"{e}\");");
        }

        private static void MyApplication_TrackerAngleUpdated(object sender, float angle)
        {
            var angleAsDegrees = MathHelper.ToDegrees(angle);
            Runtime.InvokeJS($"WaveEngine.onTrackerAngleUpdated({angleAsDegrees});");
        }

        public static void UpdateCanvasSize(string canvasId)
        {
            if (appCanvas.TryGetValue(canvasId, out var surface))
            {
                surface.RefreshSize();
            }
        }

        private static void ConfigureGraphicsContext(Application application, Surface surface)
        {
            // Enabled web canvas antialias (MSAA)
            Runtime.InvokeJS("EGL.contextAttributes.antialias = true;");
            Runtime.InvokeJS("EGL.contextAttributes.preserveDrawingBuffer = true;");

            GraphicsContext graphicsContext = new GLGraphicsContext(GraphicsBackend.WebGL2);
            graphicsContext.CreateDevice();
            SwapChainDescription swapChainDescription = new SwapChainDescription()
            {
                SurfaceInfo = surface.SurfaceInfo,
                Width = surface.Width,
                Height = surface.Height,
                ColorTargetFormat = PixelFormat.R8G8B8A8_UNorm,
                ColorTargetFlags = TextureFlags.RenderTarget | TextureFlags.ShaderResource,
                DepthStencilTargetFormat = PixelFormat.D24_UNorm_S8_UInt,
                DepthStencilTargetFlags = TextureFlags.DepthStencil,
                SampleCount = TextureSampleCount.None,
                IsWindowed = true,
                RefreshRate = 60
            };
            var swapChain = graphicsContext.CreateSwapChain(swapChainDescription);
            swapChain.VerticalSync = true;

            var graphicsPresenter = application.Container.Resolve<GraphicsPresenter>();
            var firstDisplay = new Display(surface, swapChain);
            graphicsPresenter.AddDisplay("DefaultDisplay", firstDisplay);

            application.Container.RegisterInstance(graphicsContext);
        }

        public static void UpdateData(JSObject command)
        {
            var value1 = command.GetObjectProperty("TrackerPosition").ToString();
            var value2 = command.GetObjectProperty("SolarPosition").ToString();
            float.TryParse(value2, out float sunAngle);
            float.TryParse(value1, out float trackerAngle);

            MyApplication.SunAngle = MathHelper.ToRadians(sunAngle);
            MyApplication.TrackerAngle = MathHelper.ToRadians(trackerAngle);
        }
    }
}
