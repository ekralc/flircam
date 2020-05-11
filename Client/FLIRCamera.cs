using CitizenFX.Core;
using CitizenFX.Core.Native;
using System;
using System.Threading.Tasks;

namespace flircam.Client
{
    /// <summary>
    /// Handles the creation and display of the camera to the player.
    /// </summary>
    public class FLIRCamera : IDisposable
    {
        /// <summary>
        /// The script camera
        /// </summary>
        private readonly Camera camera;
        private bool enabled = false;
        private CameraMode cameraMode = CameraMode.Normal;
        private float zoom = 1.0f;

        /// <summary>
        /// Gets or sets whether the camera is being viewed by the player
        /// </summary>
        public bool Enabled
        {
            get { return enabled; }
            set
            {
                if (value) Enable(); else Disable();
                enabled = value;
            }
        }

        /// <summary>
        /// Gets or sets the mode of the camera
        /// </summary>
        public CameraMode Mode {
            get { return cameraMode; } 
            set {
                cameraMode = value;
                SwitchMode(cameraMode);
            }
        }

        /// <summary>
        /// Gets or sets the camera's field of view, minimum 1.0f and maximum 130.0f.
        /// </summary>
        public float FieldOfView
        {
            get { return zoom; }
            set
            {
                zoom = value > 130 ? 130.0f : (value < 1 ? 1.0f : value);
            }
        }

        /// <summary>
        /// Gets or sets the direction of the camera.
        /// </summary>
        public Vector3 Direction
        {
            get { return camera.Direction;  }
            set { camera.Direction = value;  }
        }

        /// <summary>
        /// Gets or sets the rotation of the camera.
        /// </summary>
        public Vector3 Rotation
        {
            get { return camera.Rotation; }
            set {
                // Requires a hardcoded limit else the camera will glitch out beyond these
                if (value.X < -89.5f) value.X = -89.5f;
                if (value.X > 15.0f) value.X = 15.0f;
                camera.Rotation = value; 
            }
        }

        /// <summary>
        /// Gets the vehicle to which the camera is attached.
        /// </summary>
        public Vehicle Vehicle
        {
            get; private set;
        }

        public FLIRCamera(Vehicle vehicle, Vector3 offset)
        {
            Vehicle = vehicle;
            camera = new Camera(API.CreateCam("DEFAULT_SCRIPTED_FLY_CAMERA", true));
            camera.AttachTo(vehicle, offset);
            FieldOfView = camera.FieldOfView;
            ClientMain.Instance.AttachTickHandler(ZoomTick);
        }

        /// <summary>
        /// Starts rendering the camera.
        /// </summary>
        private void Enable()
        {
            if (Enabled) return;
            World.RenderingCamera = camera;
            API.SetTimecycleModifier("heliGunCam");
            API.SetTimecycleModifierStrength(0.1f);
            SwitchMode(Mode);
            ClientMain.Instance.AttachTickHandler(CameraTick);
        }

        /// <summary>
        /// Stops rendering the camera
        /// </summary>
        private void Disable()
        {
            if (!Enabled) return;
            World.RenderingCamera = null;
            API.ClearTimecycleModifier();
            API.SetNightvision(false);
            API.SetSeethrough(false);
            ClientMain.Instance.DetachTickHandler(CameraTick);
        }

        /// <summary>
        /// Calculates the target coordinates.
        /// </summary>
        /// <remarks>This uses a raycast which is expensive.</remarks>
        /// <returns>Coordinates of the location on the ground where the camera's pointing. Returns 0,0,0 if none found.</returns>
        public Vector3 GetTargetCoordinates()
        {
            var endCoords = new Vector3();
            var pos = camera.Position;
            var forward = camera.ForwardVector * 5000.0f;
            // We use the flag '1' here to intersect with the map.
            var handle = API.CastRayPointToPoint(pos.X, pos.Y, pos.Z, forward.X, forward.Y, forward.Z, 1, Vehicle.Handle, 0);
            var result = new RaycastResult(handle);
            Debug.WriteLine(result.HitPosition.ToString());
            return endCoords;
 
        }

        /// <summary>
        /// Sets up the display for a given camera mode
        /// </summary>
        /// <param name="mode">The mode to switch to.</param>
        private void SwitchMode(CameraMode mode)
        {
            if (!Enabled) return;
            API.SetNightvision(false);
            API.SetSeethrough(false);
            switch (mode)
            {
                case CameraMode.Normal:
                    Console.WriteLine("test");
                    break;
                case CameraMode.Infrared:
                    API.SetSeethrough(true);
                    break;
                case CameraMode.Night:
                    API.SetNightvision(true);
                    break;
            }
            Audio.PlaySoundFrontend("SELECT", "HUD_FRONTEND_DEFAULT_SOUNDSET");
        }

        #region ticks
        /// <summary>
        /// Various natives that need to be called whilst viewing the camera.
        /// </summary>
        private async Task CameraTick()
        {
            API.HideAreaAndVehicleNameThisFrame();
            API.HideHudAndRadarThisFrame();
            API.HideHelpTextThisFrame();
            await Task.FromResult(0);
        }

        /// <summary>
        /// Ensures smooth zooming
        /// </summary>
        private async Task ZoomTick()
        {
            // Smooth camera zooming
            while (camera.FieldOfView != FieldOfView)
            {
                var difference = FieldOfView - camera.FieldOfView;
                if (Math.Abs(difference) < 0.01f)
                {
                    camera.FieldOfView = FieldOfView;
                    break;
                }
                camera.FieldOfView += difference * 0.05f;
                await BaseScript.Delay(0);
            }
            await Task.FromResult(0);
        }
        #endregion

        /// <summary>
        /// Safely gets rid of the camera
        /// </summary>
        public void Dispose()
        {
            Enabled = false;
            camera.Detach();
            camera.Delete();
        }
    }

    public enum CameraMode
    {
        Normal, Night, Infrared
    }
}
