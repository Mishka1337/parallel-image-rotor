using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace multithread_image_rotor
{
    public class ImagesRotor
    {
        private string resultFolder;
        private List<string> bmpPaths;
        
        public ImagesRotor(string resultFolder, List<string> bmpPaths)
        {
            this.resultFolder = resultFolder;
            this.bmpPaths = bmpPaths;
        }

        public void RotateImages()
        {
            Parallel.ForEach(bmpPaths, (bp) =>
            {
                var threadId = Thread.CurrentThread.ManagedThreadId;
                var startTime = Stopwatch.StartNew();
                var imageName = Path.GetFileName(bp);
                Console.WriteLine($"Thread #{threadId} is started rotating {imageName}");
                
                using (var bitmap = new Bitmap(bp))
                {
                    bitmap.RotateFlip(RotateFlipType.Rotate270FlipNone);
                    bitmap.Save(resultFolder + imageName);  
                }
                
                startTime.Stop();
                var resultTime = startTime.Elapsed;
                Console.WriteLine($"Thread #{threadId}: Image {imageName} has been rotated. {resultTime.Milliseconds + resultTime.Seconds * 1000}ms");
            });

        }
        


    }
}