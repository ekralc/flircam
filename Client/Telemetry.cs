using CitizenFX.Core;
using CitizenFX.Core.Native;
using System;
using System.Threading.Tasks;

namespace flircam.Client
{
    /// <summary>
    /// Calculates and reports telemetry for a given <see cref="FLIRCamera"/>.
    /// </summary>
    public class Telemetry
    {
        /// <summary>
        /// The source camera for the telemetry.
        /// </summary>
        public FLIRCamera Camera { get; private set; }

        /// <summary>
        /// The velocity of the vehicle.
        /// </summary>
        public Vector3 Velocity { get { return Camera.Vehicle.Velocity; } }

        /// <summary>
        /// The speed of the vehicle in meters per second.
        /// </summary>
        public float Speed { get { return Velocity.Length(); } }

        /// <summary>
        /// The latitude in degrees, minutes and seconds
        /// </summary>
        public Vector3 Latitude { get; private set; }

        /// <summary>
        /// The latitude output as degrees, minutes and seconds
        /// </summary>
        public Vector3 Longitude { get; private set; }

        /// <summary>
        /// The height of the camera above mean sea level (Z=0) in meters.
        /// </summary>
        public float AMSL { get; private set; }

        /// <summary>
        /// The height of the camera measured with respect to the underlying ground surface in meters.
        /// </summary>
        public float AGL { get; private set; }

        /// <summary>
        /// The distance between the camera and the calculated ground point in meters.
        /// </summary>
        public float GroundDistance { get { return (GroundPosition - Camera.Vehicle.Position).Length(); } }

        /// <summary>
        /// The relative velocity of the coordinates of the calculated ground point in meters per second.
        /// </summary>
        public Vector3 GroundVelocity { get; private set; }

        /// <summary>
        /// The relative speed of the coordinates of the calculated ground point in meters per second.
        /// </summary>
        public float GroundSpeed { get { return GroundVelocity.Length(); } }

        /// <summary>
        /// The world coordinates of the calculated ground point.
        /// </summary>
        public Vector3 GroundPosition { get; private set; }

        /// <summary>
        /// The latitude of the ground <see cref="GroundPosition"/>.
        /// </summary>
        public Vector3 GroundLatitude { get; private set; }

        /// <summary>
        /// The longitude. of the ground <see cref="GroundPosition"/>
        /// </summary>
        public Vector3 GroundLongitude { get; private set; }

        private readonly double EARTH_RADIUS = 6.3781e6;

        public Telemetry(FLIRCamera camera)
        {
            Camera = camera;
            Debug.WriteLine($"Created new telemetry class from camera attached to vehicle {camera.Vehicle.Handle.ToString()}");
            ClientMain.Instance.AttachTickHandler(Tick);
            ClientMain.Instance.AttachTickHandler(DrawMarker);
        }

        public async Task Tick()
        {
            var ray = Camera.GetRaycastResult();
            await BaseScript.Delay(0);
            if (ray.DitHitEntity) {
                Debug.WriteLine("Hit an entity");
            }

            if (ray.DitHit)
            {
                // we hit the map!
                GroundPosition = ray.HitPosition;
            }
            await BaseScript.Delay(500);
        }

        public async Task DrawMarker()
        {
            API.DrawMarker(32, GroundPosition.X, GroundPosition.Y, GroundPosition.Z, 0.0f, 0.0f, 1.0f, 0.0f, 0.0f, 0.0f, 10.0f, 10.0f, 10.0f, 0, 255, 0, 255, true, true, 2, false, null, null, false);
            await Task.FromResult(0);
        }
    }
}
