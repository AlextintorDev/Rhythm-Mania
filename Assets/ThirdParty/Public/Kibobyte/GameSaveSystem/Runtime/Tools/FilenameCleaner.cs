using System;
using System.IO;

namespace GameSaveSystem.Tools
{
    public class FilenameCleaner
    {
        private static readonly char[] InvalidChars = Path.GetInvalidFileNameChars();

        public static string Clean(string savename)
        {
            if (string.IsNullOrWhiteSpace(savename))
                throw new ArgumentException("Save name cannot be null or empty.", nameof(savename));

            foreach (char c in InvalidChars)
                savename = savename.Replace(c, '_');

            savename = savename.Replace('/', '_');
            savename = savename.Replace('\\', '_');

            while (savename.Contains(".."))
                savename = savename.Replace("..", "");

            return savename.Trim();
        }
    }
}
