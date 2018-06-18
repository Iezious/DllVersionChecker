using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace DLLVersionCheck
{
    public class AssemblyVersionReader : MarshalByRefObject
    {
        public  string[][] ReadFolder(string folder)
        {
            Console.WriteLine($"Execution for {folder}");
            
            var res = new List<string[]>();

            foreach (var filename in Directory.GetFiles(folder))
            {
                switch (Path.GetExtension(filename))
                {
                    case ".dll":
                    case ".exe":
                        try
                        {
                            var assembly = Assembly.LoadFrom(filename);
                            var name = assembly.GetName();

                            res.Add(new[] {name.Name, name.Version.ToString()});
                                
                            Console.Write("[");
                            WriteInColor(" OK ", ConsoleColor.DarkGreen);
                            Console.WriteLine($"] {assembly.GetName().Name}");
                        }
                        catch
                        {
                            Console.Write("[");
                            WriteInColor("SKIP", ConsoleColor.Yellow);
                            Console.WriteLine($"] {filename}");
                        }
                            
                        break;
                }
            }

            Console.WriteLine();
            return res.ToArray();
        }
        
        
        static void WriteInColor(string text, ConsoleColor color)
        {
            var c = Console.ForegroundColor;
            Console.ForegroundColor = color;
            Console.Write(text);
            Console.ForegroundColor = c;
        }
    }
}