using System.Collections.Generic;
using System.Linq;

namespace Potato
{
    internal class ControllerManager : IController
    {
        public ControllerManager() => Controllers = new List<IController>();
        public List<IController> Controllers { get; private set; }

        bool IController.ActivatePressed() => Controllers.Aggregate(false, (current, controller) => current | controller.ActivatePressed());

        float IController.DownHeld() => Controllers.Select((controller) => controller.DownHeld()).Max();

        bool IController.DownPressed() => Controllers.Aggregate(false, (current, controller) => current | controller.DownPressed());

        float IController.LeftHeld() => Controllers.Select((controller) => controller.LeftHeld()).Max();

        bool IController.LeftPressed() => Controllers.Aggregate(false, (current, controller) => current | controller.LeftPressed());

        float IController.RightHeld() => Controllers.Select((controller) => controller.RightHeld()).Max();

        bool IController.RightPressed() => Controllers.Aggregate(false, (current, controller) => current | controller.RightPressed());

        float IController.UpHeld() => Controllers.Select((controller) => controller.UpHeld()).Max();

        bool IController.UpPressed() => Controllers.Aggregate(false, (current, controller) => current | controller.UpPressed());
    }
}
