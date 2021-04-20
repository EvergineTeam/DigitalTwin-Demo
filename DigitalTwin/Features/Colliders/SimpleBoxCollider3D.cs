using System;
using System.Collections.Generic;
using System.Text;
using WaveEngine.Common.Graphics;
using WaveEngine.Components.Graphics3D;
using WaveEngine.Framework;
using WaveEngine.Framework.Graphics;
using WaveEngine.Mathematics;

namespace DigitalTwin.Features.Colliders
{
    public class SimpleBoxCollider3D : SimpleCollider3D
    {
        private bool hasValidBBox;
        public BoundingOrientedBox boundingBox;
        private BoundingBox rawBBox;
        private BoundingBox aabb;

        public override BoundingBox AABB => this.aabb;

        public override BoundingOrientedBox BoundingBox => this.boundingBox;

        public override bool Intersect(ref Ray ray)
        {
            if (this.hasValidBBox)
            {
                var result = this.boundingBox.Intersects(ref ray);
                return result.HasValue;
            }

            return false;
        }

        public override void DebugDraw(LineBatch3D lineBatch, Color color)
        {
            lineBatch.DrawBoundingOrientedBox(this.boundingBox, color);
        }

        protected override void Start()
        {
            base.Start();
            this.RegisterTransformEvents();
            this.RefreshBoundingBox();
        }

        protected override void OnDetach()
        {
            this.UnregisterTransformEvents();
            base.OnDetach();
        }

        private void RegisterTransformEvents()
        {
            this.transform.TransformChanged += this.Transform_TransformChanged;
            this.meshComponent.ChangedMesh += this.MeshComponent_ChangedMesh;
        }

        private void UnregisterTransformEvents()
        {
            this.transform.TransformChanged -= this.Transform_TransformChanged;
            this.meshComponent.ChangedMesh -= this.MeshComponent_ChangedMesh;
        }

        private void RefreshBoundingBox()
        {
            this.boundingBox = default;
            this.hasValidBBox = this.meshComponent.BoundingBox.HasValue;
            if (this.hasValidBBox)
            {
                this.rawBBox = this.meshComponent.BoundingBox.Value;
                this.aabb = WaveEngine.Mathematics.BoundingBox.Transform(this.rawBBox, this.transform.WorldTransform);

                this.boundingBox.HalfExtent = this.rawBBox.HalfExtent * this.transform.Scale;
                this.boundingBox.Center = this.transform.Position;
                this.boundingBox.Orientation = this.transform.Orientation;
            }
        }

        private void Transform_TransformChanged(object sender, EventArgs e)
        {
            this.boundingBox.Center = this.transform.Position;
            this.boundingBox.Orientation = this.transform.Orientation;
            this.boundingBox.HalfExtent = this.rawBBox.HalfExtent * this.transform.Scale;
            this.aabb = WaveEngine.Mathematics.BoundingBox.Transform(this.rawBBox, this.transform.WorldTransform);
        }

        private void MeshComponent_ChangedMesh(object sender, MeshContainer e)
        {
            this.RefreshBoundingBox();
        }
    }
}
