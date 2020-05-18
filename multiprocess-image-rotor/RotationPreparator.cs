using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace multiprocess_image_rotor
{
    public class RotationPreparator
    {
        private string dirpath;
        public string resultFolder;

        public RotationPreparator(string dirpath)
        {
            this.dirpath = dirpath;
            this.resultFolder = dirpath + "/results/";
        }

        public IEnumerable<string> ExtractBmpPaths()
        {
            if (!Directory.Exists(resultFolder))
            {
                Directory.CreateDirectory(resultFolder);
            }
            var filePaths = Directory.GetFiles(dirpath);
            var bmpPaths = filePaths.Where((fp) => fp.EndsWith(".bmp"));
            return bmpPaths;
        }
        
        
    }
}