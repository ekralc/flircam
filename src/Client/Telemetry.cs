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
        /// The velocity of the host vehicle.
        /// </summary>
        public Vector3 Velocity { get { return Camera.Vehicle.Velocity; } }

        /// <summary>
        /// The speed of the host vehicle in meters per second.
        /// </summary>
        public float Speed { get { return Velocity.Length(); } }

        /// <summary>
        /// The position of the host vehicle.
        /// </summary>
        public Vector3 Position { get; private set; }

        /// <summary>
        /// The latitude and longitude (X and Y respectively) of host vehicle's <see cref="Position"/> in degrees.
        /// </summary>
        public Vector2 LatitudeLongitude { get { return ConvertToLatitudeAndLongitude(Position); } }

        /// <summary>
        /// Above mean sea level.
        /// The height of the camera above mean sea level (Z=0) in meters.
        /// </summary>
        public float AMSL { get; private set; }

        /// <summary>
        /// Above ground level.
        /// The height of the camera measured with respect to the underlying ground surface in meters.
        /// </summary>
        public float AGL { get; private set; }

        /// <summary>
        /// The distance between the camera and the calculated ground point in meters.
        /// </summary>
        public float GroundDistance { get { return (GroundPosition - Camera.Vehicle.Position).Length(); } }

        /// <summary>
        /// The velocity of the coordinates of the calculated ground point in meters per second.
        /// </summary>
        public Vector3 GroundVelocity { get; private set; }

        /// <summary>
        /// The speed of the coordinates of the calculated ground point in meters per second.
        /// </summary>
        public float GroundSpeed { get { return GroundVelocity.Length(); } }

        /// <summary>
        /// The world coordinates of the calculated ground point.
        /// </summary>
        public Vector3 GroundPosition { get; private set; }

        /// <summary>
        /// The latitude and longitude (X and Y respectively) of the <see cref="GroundPosition"/> in degrees.
        /// </summary>
        public Vector2 GroundLatitudeLongitude { get { return ConvertToLatitudeAndLongitude(GroundPosition); } }

        private readonly double EARTH_RADIUS = 6.3781e6;
        private Vector2 ZERO_POINT = new Vector2(34.0f, 118.0f);
        private int GroundPosUpdated = 0;

        public Telemetry(FLIRCamera camera)
        {
            Camera = camera;
            Debug.WriteLine($"Created new telemetry class from camera attached to vehicle {camera.Vehicle.Handle.ToString()}");
            ClientMain.Instance.AttachTickHandler(Tick);
            ClientMain.Instance.AttachTickHandler(DrawMarker);
        }

        public async Task Tick()
        {
            Position = Camera.Vehicle.Position;
            var ray = Camera.GetRaycastResult();
            await BaseScript.Delay(250);
            if (!ray.DitHit) return;

            var time = API.GetGameTimer();
            var diff = (time - GroundPosUpdated) / 1000.0f; // time difference in seconds
            var initialPos = GroundPosition;
            var pos = ray.HitPosition;

            if (ray.DitHitEntity) {
                pos.Z = World.GetGroundHeight(pos);
            }

            GroundPosition = pos;
            GroundVelocity = (GroundPosition - initialPos) / diff;
            GroundPosUpdated = API.GetGameTimer();
            Debug.WriteLine(GroundLatitudeLongitude.ToString());
        }

        public async Task DrawMarker()
        {
            API.DrawMarker(32, GroundPosition.X, GroundPosition.Y, GroundPosition.Z, 0.0f, 0.0f, 1.0f, 0.0f, 0.0f, 0.0f, 10.0f, 10.0f, 10.0f, 0, 255, 0, 255, true, true, 2, false, null, null, false);
            await Task.FromResult(0);
        }

        /// <summary>
        /// Converts given world coordinates to latitude (N) and longitude (W) in degrees.
        /// </summary>
        /// <param name="worldCoords"></param>
        /// <returns></returns>
        private Vector2 ConvertToLatitudeAndLongitude(Vector3 worldCoords) {
            var pos = new Vector2(worldCoords.Y, -worldCoords.X);
            var offset = ((float)(180.0f/(Math.PI * EARTH_RADIUS)) * pos);
            return ZERO_POINT + offset;
        }
    }
}
