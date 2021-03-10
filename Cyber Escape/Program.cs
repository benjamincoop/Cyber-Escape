using System;

namespace Cyber_Escape
{
    public static class Program
    {
        [STAThread]
        static void Main()
        {
            using (var game = new CyberEscape())
                game.Run();
        }
    }
}
