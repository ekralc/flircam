using System;
using System.Threading.Tasks;
using CitizenFX.Core;

namespace flircam.Client
{
    public class ClientMain : BaseScript
    {
        public static ClientMain Instance { get; private set; }
        public FLIRCamera FLIRCamera;
        public ClientMain()
        {
            Instance = this;
            Debug.WriteLine("Hi from flircam.Client!");
        }

        public void AttachTickHandler(Func<Task> action)
        {
            try
            {
                Tick += action;
            } catch
            {
                Debug.WriteLine("Error registering tick handler");
            }
        }

        public void DetachTickHandler(Func<Task> action)
        {
            try
            {
                Tick -= action;
            }
            catch
            {
                Debug.WriteLine("Error unregistering tick handler");
            }
        }

        [Command("cam")]
        public void CreateCam()
        {
            var vehicle = Game.PlayerPed.CurrentVehicle;
            if (vehicle == null) return;

            if (FLIRCamera == null) FLIRCamera = new FLIRCamera(vehicle, new Vector3(0.0f, 2.9f, -0.9f));
            FLIRCamera.Enabled ^= true;
            AttachTickHandler(Controls);
            AttachTickHandler(RotationControls);
        }

        [Command("switchmode")]
        public void SwitchMode()
        {
            FLIRCamera.Mode = CameraMode.Night;
        }

        [Command("target")]
        public void Target()
        {
            FLIRCamera.GetTargetCoordinates();
        }

        public async Task Controls()
        {
            Game.DisableControlThisFrame(0, Control.VehicleSelectNextWeapon);
            if (!FLIRCamera.Enabled) return;
            if (Game.IsControlJustPressed(0, Control.CursorScrollUp))
            {
                FLIRCamera.FieldOfView -= 3.0f;
            }

            if (Game.IsControlJustPressed(0, Control.CursorScrollDown))
            {
                FLIRCamera.FieldOfView += 3.0f;
            }

            await Task.FromResult(0);
        }

        public async Task RotationControls()
        {
            var x = Game.GetDisabledControlNormal(0, Control.ScriptRightAxisX);
            var y = Game.GetDisabledControlNormal(0, Control.ScriptRightAxisY);

            if (y == 0.0f & x == 0.0f) return;
            var rot = FLIRCamera.Rotation;
            rot.Z -= (FLIRCamera.FieldOfView / 8.0f) * x;
            rot.X -= (FLIRCamera.FieldOfView / 8.0f) * y;
            FLIRCamera.Rotation = rot;

            await Task.FromResult(0);
        }

    }
}