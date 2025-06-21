using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace CountCsvColLens
{
    internal class Program
    {
        static string GenerateRegex(string input)
        {
            input = input.Trim();
            if (string.IsNullOrWhiteSpace(input)) return "EMPTY";
            string output = Regex.Replace(input, "[a-zA-Z_, \\.]+", "TEXT");
            output = Regex.Replace(output, "[0-9]+", "NUM");
            return output;
        }

        static void Main(string[] args)
        {
            try
            {
                for (int i = 0; i < args.Length; i++)
                {
                    ProcessCsv(args[i]);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }

            Console.ReadLine();
        }

        private static void ProcessCsv(string file)
        {
            Console.WriteLine(file);

            Csv csv = new Csv();
            csv.Read(file);

            Dictionary<string, int>[] regexs = new Dictionary<string, int>[csv[0].Count];
            for (int i = 0; i < regexs.Length; i++)
            {
                regexs[i] = new Dictionary<string, int>();
            }

            int[] maxes = new int[csv[0].Count];

            for (int j = 0; j < maxes.Length; j++)
            {
                maxes[j] = 0;
            }
            for (int j = 1; j < csv.Count; j++)
            {
                for (int k = 0; k < csv[j].Count; k++)
                {
                    var r = GenerateRegex(csv[j][k]);
                    if (!regexs[k].ContainsKey(r))
                    {
                        regexs[k].Add(r, 1);
                    }
                    else
                    {
                        regexs[k][r]++;
                    }
                    if ((k < maxes.Length) && (maxes[k] < csv[j][k].Length))
                    {
                        maxes[k] = csv[j][k].Length;
                    }
                }
            }

            FileInfo fi = new FileInfo(file);
            string regexfile = $"{file}.rgx.txt";

            using (StreamWriter sw = new StreamWriter(regexfile))
            {
                for (int i = 0; i < csv[0].Count; i++)
                {
                    sw.WriteLine(csv[0][i]);
                    foreach (var reg in regexs[i])
                    {
                        sw.WriteLine($"{reg.Key} ({reg.Value})");
                    }
                    sw.WriteLine();
                }
            }

            for (int j = 0; j < maxes.Length; j++)
            {
                Console.WriteLine($"{csv[0][j]}   {maxes[j]}");
            }
            Console.WriteLine();
        }
    }
}
