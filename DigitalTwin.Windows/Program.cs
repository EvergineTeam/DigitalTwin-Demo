using System;
using System.Diagnostics;
using WaveEngine.Common.Graphics;
using WaveEngine.Framework;
using WaveEngine.Framework.Graphics;
using WaveEngine.Framework.Services;
using WaveEngine.Mathematics;

namespace DigitalTwin.Windows
{
    class Program
    {
        static void Main(string[] args)
        {
            // Create app
            MyApplication application = new MyApplication();

            // Create Services
            uint width = 1280;
            uint height = 720;
            WindowsSystem windowsSystem = new WaveEngine.Forms.FormsWindowsSystem();
            application.Container.RegisterInstance(windowsSystem);
            var window = windowsSystem.CreateWindow("DigitalTwin - DX11", width, height);

            ConfigureGraphicsContext(application, window);
			
			// Creates XAudio device
            var xaudio = new WaveEngine.XAudio2.XAudioDevice();
            application.Container.RegisterInstance(xaudio);

            uint dataCount = 100;
            float[] sunAngles = new float[dataCount];
            float[] trackerAngles = new float[dataCount];
            uint index = 0;

            //Fill angles
            int start = -90;
            int end = 90;
            for (int i = 0; i < dataCount; i++)
            {
                int value1 = (int)WaveEngine.Mathematics.MathHelper.Lerp(start, end, (float)i / (float)dataCount);
                sunAngles[i] = MathHelper.ToRadians(value1);
                int value2 = (int)WaveEngine.Mathematics.MathHelper.Lerp(start + 35, end - 35, (float)i / (float)dataCount);
                trackerAngles[i] = MathHelper.ToRadians(value2);
            }

            Stopwatch clockTimer = Stopwatch.StartNew();
            TimeSpan updateTimer = TimeSpan.Zero;

            windowsSystem.Run(
            () =>
            {
                application.Initialize();
            },
            () =>
            {
                var gameTime = clockTimer.Elapsed;
                updateTimer += gameTime;
                clockTimer.Restart();

                if (updateTimer.TotalSeconds > 0.5f)
                {
                    updateTimer = TimeSpan.Zero;
                    index = (index + 1) % (uint)sunAngles.Length;
                    MyApplication.SunAngle = sunAngles[index];
                    MyApplication.TrackerAngle = trackerAngles[index];
                }
                
                application.UpdateFrame(gameTime);
                application.DrawFrame(gameTime);
            });
        }

        private static void ConfigureGraphicsContext(Application application, Window window)
        {
            GraphicsContext graphicsContext = new WaveEngine.DirectX11.DX11GraphicsContext();
            graphicsContext.CreateDevice();
            SwapChainDescription swapChainDescription = new SwapChainDescription()
            {
                SurfaceInfo = window.SurfaceInfo,
                Width = window.Width,
                Height = window.Height,
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
            var firstDisplay = new Display(window, swapChain);
            graphicsPresenter.AddDisplay("DefaultDisplay", firstDisplay);

            application.Container.RegisterInstance(graphicsContext);
        }
    }
}
