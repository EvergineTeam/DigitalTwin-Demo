using System;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using WaveEngine.Common.Attributes;
using WaveEngine.Common.Input;
using WaveEngine.Components.Graphics3D;
using WaveEngine.Framework;
using WaveEngine.Framework.Graphics;
using WaveEngine.Mathematics;

namespace DigitalTwin.Features.Colliders
{
    public class CollisionManager : Component
    {
        private List<SimpleCollider3D> colliders = new List<SimpleCollider3D>();
        private Camera3D camera;

        [WaveIgnore]
        public List<SimpleCollider3D> Colliders => this.colliders;

        public void AddCollider(SimpleCollider3D collider)
        {
            this.colliders.Add(collider);
        }

        public void RemoveCollider(SimpleCollider3D collider)
        {
            this.colliders.Remove(collider);
        }

        public Entity Test(ref Vector2 screenPosition)
        {
            this.camera.CalculateRay(ref screenPosition, out var ray);
            return this.Test(ref ray);
        }

        public Entity Test(ref Ray ray)
        {
            var camera = this.Managers.RenderManager.ActiveCamera3D;

            float minDistanceSqr = float.MaxValue;

            Entity nearestEntity = null;

            foreach (var collider in this.colliders)
            {
                if (collider.Intersect(ref ray))
                {
                    float distanceSqr = Vector3.DistanceSquared(collider.AABB.Center, camera.Position);
                    distanceSqr *= collider.DistanceFactor;

                    if (distanceSqr < minDistanceSqr)
                    {
                        minDistanceSqr = distanceSqr;
                        nearestEntity = collider.Owner;
                    }                    
                }
            }

            return nearestEntity;
        }

        protected override void Start()
        {
            base.Start();
            this.camera = this.Managers.RenderManager.ActiveCamera3D;
        }

        protected override void OnDetach()
        {
            base.OnDetach();
            this.camera = null;
        }
    }
}
