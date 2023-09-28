using System;
using Evergine.Framework;
using Evergine.Framework.Services;
using Evergine.Framework.Threading;
using Evergine.Platform;

namespace DigitalTwin
{
    public partial class MyApplication : Application
    {
        private static float sunAngle;
        private static float trackerAngle;

        public static event EventHandler<float> SunAngleUpdated;
        public static event EventHandler<float> TrackerAngleUpdated;

        public static event EventHandler<string> EntitySelected;

        public static float SunAngle
        {
            get => sunAngle;
            set
            {
                sunAngle = value;
                SunAngleUpdated?.Invoke(null, value);
            }
        }

        public static float TrackerAngle
        {
            get => trackerAngle;
            set
            {
                trackerAngle = value;
                TrackerAngleUpdated?.Invoke(null, value);
            }
        }

        public static void FireEntitySelected(string entityName)
        {
            EntitySelected?.Invoke(null, entityName);
        }

        public MyApplication()
        {
            this.Container.Register<Clock>();
            this.Container.Register<TimerFactory>();
            this.Container.Register<Evergine.Framework.Services.Random>();
            this.Container.Register<ErrorHandler>();
            this.Container.Register<ScreenContextManager>();
            this.Container.Register<GraphicsPresenter>();
            this.Container.Register<AssetsDirectory>();
            this.Container.Register<AssetsService>();
            this.Container.Register<ForegroundTaskSchedulerService>();            
            this.Container.Register<WorkActionScheduler>();
            this.Container.RegisterInstance(new TimeLapseService());
        }

        public override void Initialize()
        {
            base.Initialize();

            // Get ScreenContextManager
            var screenContextManager = this.Container.Resolve<ScreenContextManager>();
            var assetsService = this.Container.Resolve<AssetsService>();

            // Navigate to scene
            var scene = assetsService.Load<MyScene>(EvergineContent.Scenes.MyScene_wescene);
            ScreenContext screenContext = new ScreenContext(scene);
            screenContextManager.To(screenContext);
        }
    }
}
