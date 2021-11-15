// Forked from Wave Engine's develop (ce51ee3f557fbf2ebbba2c27ea5282e73e39ed2a):
// src/Tools/Editor/Evergine.Runner/Viewers/Common/CameraBehavior.cs
// https://dev.azure.com/Evergineteam/Wave.Engine/_git/Evergine?path=%2Fsrc%2FTools%2FEditor%2FEvergine.Runner%2FViewers%2FCommon%2FCameraBehavior.cs

// Copyright © 2019 Wave Engine S.L. All rights reserved. Use is subject to license terms.

using System;
using System.Diagnostics;
using System.Linq;
using Evergine.Framework;
using Evergine.Framework.Graphics;
using Evergine.Mathematics;

namespace DigitalTwin.Features.Orbit
{
    public abstract class OrbitBehavior : InputObserver
    {
        private const int OrbitSmoothTimeMilliseconds = 50;

        /// <summary>
        /// The camera to move.
        /// </summary>
        [BindComponent(false)]
        public Transform3D Transform = null;

        protected Vector2 currentPointerPosition;

        protected Vector2 lastPointerPosition;

        protected bool isOrbiting;

        private Transform3D targetTransform;

        private Quaternion initialOrientation;
        private Quaternion targetInitialOrientation;

        private float objectInitialAngleRadians;

        private float theta;

        private Quaternion objectOrbitSmoothDampDeriv;

        private Quaternion targetOrbitSmoothDampDeriv;

        public float OrbitFactor = 0.0025f;

        public Vector2 AngleFactor { get; set; } = Vector2.UnitX;

        /// <summary>
        /// Reset camera position.
        /// </summary>
        public void Reset()
        {
            this.targetTransform.LocalPosition = Vector3.Zero;
            this.targetTransform.LocalRotation = Vector3.Zero;
            this.Transform.LocalPosition = Vector3.Zero;
            this.Transform.LocalRotation = Vector3.Zero;

            this.theta = 0;
            this.isOrbiting = false;
        }

        public void ResetInertia()
        {
            this.targetTransform.LocalOrientation = this.targetInitialOrientation;
            this.Transform.LocalOrientation = this.initialOrientation;

            this.theta = this.targetTransform.LocalRotation.Y;
            this.isOrbiting = false;
        }

        /// <inheritdoc/>
        protected override bool OnAttached()
        {
            var child = this.Owner.ChildEntities.First();
            this.targetTransform = child.FindComponent<Transform3D>();
            this.targetInitialOrientation = this.targetTransform.LocalOrientation;

            this.initialOrientation = this.Transform.LocalOrientation;
            this.objectInitialAngleRadians = 0;

            this.theta = this.targetTransform.LocalRotation.Y * this.AngleFactor.X;

            return base.OnAttached();
        }

        /// <inheritdoc/>
        protected override void Update(TimeSpan gameTime)
        {
            if (this.TryGetPointerPosition(out Vector2 position))
            {
                this.lastPointerPosition = this.currentPointerPosition;
                this.currentPointerPosition = position;
            }
            else
            {
                return;
            }

            this.HandleOrbit();

            float elapsedMilliseconds = (float)gameTime.TotalMilliseconds;

            if (this.isOrbiting)
            {
                var orientation = Quaternion.CreateFromAxisAngle(Vector3.UnitY, this.theta);
                this.targetTransform.LocalOrientation = Quaternion.SmoothDamp(
                    this.targetTransform.LocalOrientation,
                    orientation,
                    ref this.objectOrbitSmoothDampDeriv,
                    OrbitSmoothTimeMilliseconds,
                    elapsedMilliseconds);

                if (this.Transform.LocalOrientation == orientation)
                {
                    this.isOrbiting = false;
                }
            }
        }

        protected abstract bool TryGetPointerPosition(out Vector2 position);

        protected virtual bool IsOrbitRequested() => false;

        protected Vector2 CalcDelta(Vector2 current, Vector2 last)
        {
            Vector2 delta;
            delta.X = -current.X + last.X;
            delta.Y = current.Y - last.Y;

            return delta;
        }

        private void HandleOrbit()
        {
            if (!this.IsOrbitRequested())
            {
                return;
            }

            Vector2 delta = this.CalcDelta(this.currentPointerPosition, this.lastPointerPosition);

            delta *= OrbitFactor;

            this.theta += delta.X;

            this.isOrbiting = true;
        }
    }
}
