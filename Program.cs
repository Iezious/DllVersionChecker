using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace DLLVersionCheck
{
    class Program
    {
        static void Main(string[] args)
        {
            var domains = new List<string[][]>();
            
            foreach (var path in args)
            {
                var appDomain = AppDomain.CreateDomain(path);
                
                dynamic reader = appDomain.CreateInstanceAndUnwrap(Assembly.GetExecutingAssembly().FullName, "DLLVersionCheck.AssemblyVersionReader", true, BindingFlags.Default,
                    null, null, null, null);

                var data = reader.ReadFolder(path) as string[][];
                
                domains.Add(data);
            }

            var parsed = domains.Select(FetchNames).ToArray();
            var names = parsed.SelectMany(di => di.Keys).Distinct().OrderBy(v => v).ToArray();
            
            Console.WriteLine("=========================================================================================================================");
            foreach (var name in names)
            {
                Console.Write($"{name, -60}");
                string baseVersion = null;

                foreach (var collection in parsed)
                {
                    if (collection.TryGetValue(name, out var version))
                    {
                        if (baseVersion == null)
                            baseVersion = version;

                        if (version != baseVersion)
                        {
                            WriteInColor($"{version,20}", ConsoleColor.Red);
                        }
                        else
                        {
                            Console.Write($"{version,20}");
                        }
                    }
                    else
                    {
                        Console.Write("{0,20}", "NONE");
                    }
                }
                
                Console.WriteLine();
            }
        }

        private static Dictionary<string, string> FetchNames(string[][] domain)
        {
            return domain.ToDictionary(pair => pair[0], pair => pair[1]);
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