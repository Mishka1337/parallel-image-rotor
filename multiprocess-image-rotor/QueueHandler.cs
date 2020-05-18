using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.MemoryMappedFiles;
using System.Threading;

namespace multiprocess_image_rotor
{
    public class QueueHandler : IDisposable
    {
        private RotationPreparator rp;
        private MemoryMappedFile interProcessFile;
        private IEnumerable<string> allBmpPaths;
        
        public QueueHandler(RotationPreparator rp)
        {
            this.rp = rp;
        }

        public void InitQueue()
        {
            var processName = Process.GetCurrentProcess().ProcessName;
            var mutex = new Mutex(true,"pathList");
            if (Process.GetProcessesByName(processName).Length == 1)
            {
                allBmpPaths = rp.ExtractBmpPaths();
                var stringAllBmpPaths = string.Join("\n", allBmpPaths);
                var stringLength = stringAllBmpPaths.Length;
                interProcessFile = MemoryMappedFile
                        .CreateNew("bmps",stringLength * 2, MemoryMappedFileAccess.ReadWrite);
                using (var stream = interProcessFile.CreateViewStream())
                {
                    var writer = new BinaryWriter(stream);
                    writer.Write(stringAllBmpPaths);
                }
                mutex.ReleaseMutex();
                return;
            }
            interProcessFile = MemoryMappedFile.OpenExisting("bmps");
        }

        public void Dispose()
        {
            interProcessFile.Dispose();
        }
        
        public string GetNextBmpPath()
        {
            String allBmps;
            String returningValue;
            Mutex mutex;
            try
            {
                mutex = Mutex.OpenExisting("pathList");
                mutex.WaitOne();
            }
            catch (AbandonedMutexException e)
            {
                return null;
            }
            using (var stream = interProcessFile.CreateViewStream())
            {
                BinaryReader reader = new BinaryReader(stream);
                allBmps = reader.ReadString();
                if (allBmps == "")
                {
                    return null;
                }

                if (allBmps.IndexOf('\n') > 0)
                {
                    returningValue = allBmps.Substring(0, allBmps.IndexOf('\n'));
                    allBmps = allBmps.Remove(0, allBmps.IndexOf('\n') + 1);
                }
                else
                {
                    returningValue = allBmps;
                    allBmps = "";
                }

                stream.Seek(0, SeekOrigin.Begin);
                BinaryWriter writer = new BinaryWriter(stream);
                writer.Write(allBmps);
            }

            mutex.ReleaseMutex();
            return returningValue;
        }
    }   
    
}