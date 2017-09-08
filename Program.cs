using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TXG2TPL
{
    class Program
    {
        static void Main(string[] args)
        {
            string usage = "Usage:\nTXG2TPL -p <Folder> <Name>.txg\nTXG2TPL -u <Name>";
            if (args.Length == 3)
            {
                using (FileStream fStream = File.OpenWrite(args[2]))
                {
                    TXG txg = new TXG(fStream, Directory.GetFiles(args[1]));
                }
            }

            else if (args.Length == 2)
            {
                using (FileStream fStream = File.OpenRead(args[1]))
                {
                    TXG txg = new TXG(fStream);
                }
            }

            else
            {
                Console.WriteLine(usage);

            }
        }
    }
}
