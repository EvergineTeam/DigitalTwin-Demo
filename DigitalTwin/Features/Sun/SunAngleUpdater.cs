using System;
using System.Collections.Generic;
using System.Text;
using Evergine.Components.WorkActions;
using Evergine.Framework;
using Evergine.Framework.Graphics;
using Evergine.Framework.Services;

namespace DigitalTwin.Features.Sun
{
    public class SunAngleUpdater : Component
    {
        [BindComponent]
        public Transform3D transform;

        private IWorkAction animation;

        protected override void OnActivated()
        {
            base.OnActivated();
            MyApplication.SunAngleUpdated += MyApplication_SunAngleUpdated;
        }

        protected override void OnDeactivated()
        {
            base.OnDeactivated();
            MyApplication.SunAngleUpdated -= MyApplication_SunAngleUpdated;
        }

        protected override void Start()
        {
            base.Start();
            UpdateAngle();
        }

        private void MyApplication_SunAngleUpdated(object sender, float e)
        {
            UpdateAngle();
        }

        private void UpdateAngle()
        {
            this.animation?.Cancel();
            var currentRotation = this.transform.LocalRotation;
            this.animation = new FloatAnimationWorkAction(this.Owner, currentRotation.Y, MyApplication.SunAngle, TimeSpan.FromSeconds(0.5), EaseFunction.None,
                (f) =>
                {
                    currentRotation.Y = f;
                    this.transform.LocalRotation = currentRotation;
                });
            this.animation.Run();
        }
    }
}
