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
            if (args.Length != 3)
            {
                Console.WriteLine(Strings.UsageMessage);
                return;
            }

            if (args[0] == "-u")
            {
                using (FileStream fStream = File.OpenRead(args[1]))
                {
                    if (!Directory.Exists(args[2])) Directory.CreateDirectory(args[2]);
                    TXG txg = new TXG(fStream, args[2]);
                    Console.WriteLine(string.Format(Strings.UnpackedMessage, args[1], args[2]));
                }
            }

            else if (args[0] == "-p")
            {
                if (!Directory.Exists(args[1]))
                {
                    Console.WriteLine(Strings.UsageMessage);
                    return;
                }

                using (FileStream fStream = File.OpenWrite(args[2]))
                {
                    string[] files = Directory.EnumerateFiles(args[1], "*.tpl")
                                              .AsEnumerable()
                                              .OrderBy(item => item, new NaturalSortComparer<string>())
                                               .ToArray();
                    TXG txg = new TXG(fStream, files);
                    Console.Write(string.Format(Strings.PackedMessage, args[1], args[2]));
                }
            }

            else
            {
                Console.Write(Strings.UsageMessage);
                return;
            }
        }
    }
}
