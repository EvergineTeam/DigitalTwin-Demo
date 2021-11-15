using System;
using System.Collections.Generic;
using System.Text;
using Evergine.Components.WorkActions;
using Evergine.Framework;
using Evergine.Framework.Graphics;
using Evergine.Framework.Services;
using Evergine.Mathematics;

namespace DigitalTwin.Features.SolarPanel
{
    public class TrackerAngleUpdater : Component
    {
        [BindComponent]
        public Transform3D transform;

        private IWorkAction animation;

        protected override void OnActivated()
        {
            base.OnActivated();
            MyApplication.TrackerAngleUpdated += MyApplication_TrackerAngleUpdated;
        }

        protected override void OnDeactivated()
        {
            base.OnDeactivated();
            MyApplication.TrackerAngleUpdated -= MyApplication_TrackerAngleUpdated;
        }

        protected override void Start()
        {
            base.Start();
            UpdateAngle();
        }

        private void MyApplication_TrackerAngleUpdated(object sender, float e)
        {
            UpdateAngle();
        }

        private void UpdateAngle()
        {
            this.animation?.Cancel();
            var currentRotation = this.transform.LocalRotation;
            this.animation = new FloatAnimationWorkAction(this.Owner, currentRotation.X, MyApplication.TrackerAngle, TimeSpan.FromSeconds(0.5), EaseFunction.None,
                (f) =>
                {
                    currentRotation.X = f;
                    this.transform.LocalRotation = currentRotation;
                });
            this.animation.Run();
        }
    }
}
