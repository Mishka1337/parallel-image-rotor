using System;
using System.Diagnostics;
using System.IO;

namespace multithread_image_rotor
{
    internal class Program
    {
        public static void Main(string[] args)
        {
            String dirname;
            if (args.Length == 0)
            {
                dirname = Directory.GetCurrentDirectory();
            }
            else
            {
                try
                {
                    var path = Path.GetFullPath(args[0]);
                    if (!Directory.Exists(path))
                    {
                        Console.WriteLine("Directory was not found");
                        return;
                    }

                    dirname = path;
                }
                catch (Exception e)
                {
                    Console.WriteLine("Invalid path to the directory");
                    return;
                }
            }
            
            var rp = new RotationPreparator(dirname);
            var bmpPaths = rp.ExtractBmpPaths();
            var ir = new ImagesRotor(rp.resultFolder,bmpPaths);
            var startTime = Stopwatch.StartNew();
            ir.RotateImages();
            startTime.Stop();
            var resultTime = startTime.Elapsed;
            Console.WriteLine($"Total Time: {resultTime.Seconds}s {resultTime.Milliseconds}ms");
            
            Console.ReadKey();
        }
    }
}