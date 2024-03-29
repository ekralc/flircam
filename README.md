# flircam

This is a resource for FiveM that provides a realistic FLIR camera system to be used on helicopters.

**This is an unfinished project, there's not much of use, but it might be useful to someone.**

## Planned Features

- A fully rotatable and zoomable camera can be attached to any helicopter in any position.
- Realistic Infrared imaging, it outputs a grayscale image instead of the blue/orange in the base game.
- Map data is used to determine a wide array of metrics, including the coordinates of the ground location the camera's pointing at.
- A synchronised spotlight that can be seen by other players, its position is interpolated and appears smooth.
- A realistic HUD drawn in the NUI, with support for your own department/organisation's name and logo.
- The terrain and AI population are properly rendered at the location the camera is pointing, even when far away. This allows you to fly at much higher altitudes than other scripts would.
- Images from the camera can be uploaded straight to a discord channel using webhooks (requires the screenshot-basic resource).
- Only one person can control the camera at once, camera movement is synchronised between clients.

## License

This is licensed under GPLv3, refer to LICENSE. 

This page provides an [easy to read summary](https://tldrlegal.com/license/gnu-lesser-general-public-license-v3-(lgpl-3)) of what this means. 

## Credits

- mraes, this project took a lot of inspiration from his [Heli Script](https://forum.cfx.re/t/release-heli-script/24094) resource.
