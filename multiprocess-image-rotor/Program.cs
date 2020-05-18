using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;

namespace multiprocess_image_rotor
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
            var startTimeGlobal = Stopwatch.StartNew();
            using (var queue = new QueueHandler(rp))
            {
                queue.InitQueue();
                string bmpPath;
                do
                {
                    bmpPath = queue.GetNextBmpPath();
                    if (bmpPath == null)
                        break;
                    using (var bitmap = new Bitmap(bmpPath))
                    {
                        var filename = Path.GetFileName(bmpPath);
                        var startTime = Stopwatch.StartNew();
                        Console.WriteLine($"{filename} is started to rotate");
                        bitmap.RotateFlip(RotateFlipType.Rotate270FlipNone);
                        bitmap.Save(rp.resultFolder + filename);
                        startTime.Stop();
                        var elapsedTime = startTime.Elapsed;
                        Console.WriteLine($"{filename} has finished to rotate for {elapsedTime.Milliseconds + elapsedTime.Seconds * 1000}ms");
                    }
                    
                } while (bmpPath != null);

                startTimeGlobal.Stop();
                var elapsedTimeGlobal = startTimeGlobal.Elapsed;
                Console.WriteLine($"Total time: {elapsedTimeGlobal.Seconds * 1000 + elapsedTimeGlobal.Milliseconds}ms");
            }

        }
    }
}