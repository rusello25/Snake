using TestSnake.Presentation.Console;

namespace TestSnake.Presentation.Rendering
{
    public static class MenuRenderer
    {
        public static void RenderMenu(List<MenuRow> rows, int frameWidth, int _, string wall, string corner)
        {
            // Calcola dimensioni menu
            int menuWidth = (frameWidth + 2) * 2;
            int menuHeight = rows.Count + 2; // +2 per bordo superiore/inferiore
            int padLeftAuto = Math.Max(0, (System.Console.WindowWidth - menuWidth) / 2);
            int padTop = Math.Max(0, (System.Console.WindowHeight - menuHeight) / 2);

            // Padding verticale
            for (int i = 0; i < padTop; i++) System.Console.WriteLine();

            // Riga superiore
            System.Console.Write(new string(' ', padLeftAuto));
            System.Console.ForegroundColor = ConsoleColor.DarkYellow;
            System.Console.Write(corner);
            for (int i = 0; i < frameWidth; i++) System.Console.Write(wall);
            System.Console.Write(corner);
            System.Console.ResetColor();
            System.Console.WriteLine();

            // Righe di contenuto
            foreach (var row in rows)
            {
                System.Console.Write(new string(' ', padLeftAuto));
                System.Console.ForegroundColor = ConsoleColor.DarkYellow;
                System.Console.Write(wall);
                System.Console.ResetColor();

                // Calcolo padding per centrare il contenuto
                int contentLen = GetVisualLength(row.Content);
                int totalPad = frameWidth * 2 - contentLen;
                int padLeftContent = totalPad / 2;
                int padRightContent = totalPad - padLeftContent;

                System.Console.Write(new string(' ', padLeftContent));
                if (row.Color.HasValue)
                    System.Console.ForegroundColor = row.Color.Value;
                System.Console.Write(row.Content);
                System.Console.ResetColor();
                System.Console.Write(new string(' ', padRightContent));

                System.Console.ForegroundColor = ConsoleColor.DarkYellow;
                System.Console.Write(wall);
                System.Console.ResetColor();
                System.Console.WriteLine();
            }

            // Riga inferiore
            System.Console.Write(new string(' ', padLeftAuto));
            System.Console.ForegroundColor = ConsoleColor.DarkYellow;
            System.Console.Write(corner);
            for (int i = 0; i < frameWidth; i++) System.Console.Write(wall);
            System.Console.Write(corner);
            System.Console.ResetColor();
            System.Console.WriteLine();
        }

        private static int GetVisualLength(string s)
        {
            int len = 0;
            foreach (var c in s)
            {
                if (char.IsSurrogate(c) || char.GetUnicodeCategory(c) == System.Globalization.UnicodeCategory.OtherSymbol)
                    len += 1;
                else
                    len++;
            }
            return len;
        }
    }
}