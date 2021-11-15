using System;
using Evergine.Framework.Services;

namespace DigitalTwin
{
    public class TimeLapseService : UpdatableService
    {
        private TimeSpan elapsedTime;
        private SampleData sampleData;

        protected override bool OnAttached()
        {
            this.elapsedTime = TimeSpan.Zero;
            this.sampleData = new SampleData(100);
            return base.OnAttached();
        }

        public override void Update(TimeSpan gameTime)
        {
            this.elapsedTime += gameTime;

            if (this.elapsedTime.TotalSeconds > 0.5f)
            {
                this.elapsedTime = TimeSpan.Zero;

                var currentData = this.sampleData.Next();
                MyApplication.SunAngle = currentData.sunAngle;
                MyApplication.TrackerAngle = currentData.trackerAngle;
            }
        }
    }
}
