using System;
using System.Collections.Generic;
using System.Text;
using Evergine.Common.Graphics;
using Evergine.Components.Graphics3D;
using Evergine.Framework;
using Evergine.Framework.Graphics;
using Evergine.Mathematics;

namespace DigitalTwin.Features.Colliders
{
    public abstract class SimpleCollider3D : Component
    {
        [BindComponent(source: BindComponentSource.Scene)]
        protected CollisionManager collisionManager;

        [BindComponent(isExactType: false, source: BindComponentSource.Children)]
        protected MeshComponent meshComponent;

        protected Transform3D transform;

        public abstract BoundingBox AABB { get; }

        public abstract BoundingOrientedBox BoundingBox { get; }
        
        public float DistanceFactor;

        public abstract bool Intersect(ref Ray ray);

        public abstract void DebugDraw(LineBatch3D lineBatch, Color color);

        protected override void OnActivated()
        {
            base.OnActivated();
            this.collisionManager.AddCollider(this);
        }

        protected override void Start()
        {
            base.Start();

            this.transform = this.meshComponent.Owner.FindComponent<Transform3D>();
        }

        protected override void OnDeactivated()
        {
            this.collisionManager.RemoveCollider(this);
            base.OnDeactivated();
        }
    }
}
