//using Microsoft.Xna.Framework;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;

//namespace Potato
//{
//    internal class ControllerManager : IController
//    {
//        public ControllerManager() => Controllers = new List<IController>();
//        public List<TextInputEventArgs> KeysPressed { get; private set; } = null;
//        public bool CollectKeys { get; set; } = false;
//        public List<IController> Controllers { get; private set; }
        
//        public bool ActivatePressed() => Controllers.Aggregate(false, (current, controller) => current | controller.ActivatePressed());

//        public void ApplyDefaults() => Controllers.ForEach((x) => x.ApplyDefaults());

//        public float DownHeld() => Controllers.Select((controller) => controller.DownHeld()).Max();

//        public bool DownPressed() => Controllers.Aggregate(false, (current, controller) => current | controller.DownPressed());

//        public float LeftHeld() => Controllers.Select((controller) => controller.LeftHeld()).Max();

//        public bool LeftPressed() => Controllers.Aggregate(false, (current, controller) => current | controller.LeftPressed());

//        public float RightHeld() => Controllers.Select((controller) => controller.RightHeld()).Max();

//        public bool RightPressed() => Controllers.Aggregate(false, (current, controller) => current | controller.RightPressed());

//        public void Update(GameTime gameTime) => Controllers.ForEach((x) => x.Update(gameTime));

//        public float UpHeld() => Controllers.Select((controller) => controller.UpHeld()).Max();

//        public bool UpPressed() => Controllers.Aggregate(false, (current, controller) => current | controller.UpPressed());
//    }
//}
