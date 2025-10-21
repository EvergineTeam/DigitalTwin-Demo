using Android.Content.PM;
using Android.Views;
using Evergine.Common.Graphics;
using Evergine.Common.Helpers;
using Evergine.Framework.Services;
using Evergine.Vulkan;
using System.Diagnostics;
using Display = Evergine.Framework.Graphics.Display;
using Surface = Evergine.Common.Graphics.Surface;

namespace DigitalTwin.Android
{
    [Activity(Label = "@string/app_name",
        ConfigurationChanges = ConfigChanges.KeyboardHidden | ConfigChanges.Orientation,
        ScreenOrientation = ScreenOrientation.Sensor,
        LaunchMode = LaunchMode.SingleTask)]
    public class MainActivity : global::Android.App.Activity
    {
        private SwapChain? swapChain;
        private Evergine.Android.AndroidSurface? surface;

        protected override void OnCreate(Bundle? savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Set fullscreen surface
            this.RequestWindowFeature(WindowFeatures.NoTitle);
            this.Window!.AddFlags(WindowManagerFlags.Fullscreen);

            // Set Main layout
            this.SetContentView(Resource.Layout.Main);

            // Create app
            var application = new MyApplication();

            // Create Services
            var windowsSystem = new global::Evergine.Android.AndroidWindowsSystem(this);
            application.Container.RegisterInstance(windowsSystem);
            this.surface = windowsSystem.CreateSurface(0, 0) as global::Evergine.Android.AndroidSurface;
            this.surface!.OnSurfaceInfoChanged += this.Surface_OnSurfaceInfoChanged;
            this.surface!.Closing += this.Surface_OnClosing;
            this.surface!.OnScreenSizeChanged += this.Surface_OnScreenSizeChanged;

            var view = this.FindViewById<RelativeLayout>(Resource.Id.evergineContainer);
            view!.AddView(surface.NativeSurface);

            // Creates XAudio device
            var xaudio = new global::Evergine.OpenAL.ALAudioDevice();
            application.Container.RegisterInstance(xaudio);

            Stopwatch clockTimer = Stopwatch.StartNew();
            windowsSystem.Run(
            () =>
            {
                ConfigureGraphicsContext(application, surface);
                application.Initialize();
            },
            () =>
            {
                var gameTime = clockTimer.Elapsed;
                clockTimer.Restart();

                application.UpdateFrame(gameTime);
                application.DrawFrame(gameTime);
            });

        }

        private void Surface_OnSurfaceInfoChanged(object? sender, SurfaceInfo surfaceInfo)
        {
            this.swapChain!.RefreshSurfaceInfo(surfaceInfo);
            this.swapChain!.ResizeSwapChain(this.surface!.Width, this.surface!.Height);
            this.surface!.OnScreenSizeChanged -= this.Surface_OnScreenSizeChanged;
            this.surface!.OnScreenSizeChanged += this.Surface_OnScreenSizeChanged;
        }

        private void Surface_OnClosing(object? sender, EventArgs e)
        {
            this.surface!.OnScreenSizeChanged -= this.Surface_OnScreenSizeChanged;
        }

        private void Surface_OnScreenSizeChanged(object? sender, SizeEventArgs e)
        {
            this.swapChain?.ResizeSwapChain(e.Width, e.Height);
        }

        private void ConfigureGraphicsContext(MyApplication application, Surface surface)
        {
            GraphicsContext graphicsContext = new VKGraphicsContext();
            graphicsContext.CreateDevice();
            var swapChainDescription = new SwapChainDescription()
            {
                SurfaceInfo = surface.SurfaceInfo,
                Width = surface.Width,
                Height = surface.Height,
                ColorTargetFormat = PixelFormat.R8G8B8A8_UNorm_SRgb,
                ColorTargetFlags = TextureFlags.RenderTarget | TextureFlags.ShaderResource,
                DepthStencilTargetFormat = PixelFormat.D24_UNorm_S8_UInt,
                DepthStencilTargetFlags = TextureFlags.DepthStencil,
                SampleCount = TextureSampleCount.None,
                IsWindowed = true,
                RefreshRate = 60
            };
            
            this.swapChain = graphicsContext.CreateSwapChain(swapChainDescription);
            this.swapChain.VerticalSync = true;

            var graphicsPresenter = application.Container.Resolve<GraphicsPresenter>();
            var firstDisplay = new Display(surface, this.swapChain);
            graphicsPresenter.AddDisplay("DefaultDisplay", firstDisplay);

            application.Container.RegisterInstance(graphicsContext);
        }
    }
}

