using System;
using System.IO;

namespace NestedRectangleDrawer
{
    class Program
    {
        const string Format = "jpeg";

        static void Main(string[] args)
        {
            var drawer = new NestedRectangleDrawer();
            var imageBytes = drawer.GetImageBytes(3660, 2440, Format);

            SaveToHomeFolder(imageBytes);

            Console.WriteLine("Image created!");
        }

        static void SaveToHomeFolder(byte[] bytes)
        {
            var directory = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + "/NestedRectangleDrawer/";
            Directory.CreateDirectory(directory);

            var fileName = Guid.NewGuid() + "." + Format.ToLower();

            File.WriteAllBytes(directory + fileName, bytes);
        }
    }
}