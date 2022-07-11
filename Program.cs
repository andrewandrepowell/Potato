using System;

namespace Potato
{
    public static class Program
    {
        [STAThread]
        static void Main()
        {
            using (Potato game = new Potato())
                game.Run();
        }
    }
}
