using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace DataFilter
{
    public class CsvLine : List<string> { }

    public class Csv : List<CsvLine>
    {
        public Csv(char delimiter = ',', char quote = '"')
        {
            Delimiter = delimiter;
            Quote = quote;
        }

        private char _Delimiter = ',';
        public char Delimiter
        {
            get { return _Delimiter; }
            set
            {
                _Delimiter = value;
            }
        }

        private string quote = "\"";
        private string tripleQuote = "\"\"\"";
        private char _Quote = '"';
        public char Quote
        {
            get { return _Quote; }
            set
            {
                _Quote = value;
                quote = new string(value, 1);
                tripleQuote = new string(value, 3);
            }
        }

        public void Clear()
        {
            lines.Clear();
        }

        protected List<CsvLine> lines
        {
            get { return this; }
        }

        public void Write(string filename)
        {
            StreamWriter sw = new StreamWriter(filename);

            foreach (CsvLine line in lines)
            {
                bool first = true;
                foreach (string cell in line)
                {
                    if (!first)
                    {
                        sw.Write(_Delimiter);
                    }
                    sw.Write(Format(cell));
                    first = false;
                }
                sw.WriteLine();
            }
        }

        protected string Format(string cell)
        {
            string output = cell.Replace(quote, tripleQuote);
            return _Quote + output + _Quote;
        }

        public void Read(string filename, bool useSetLength = true)
        {
            StreamReader sr = new StreamReader(filename);
            string file = sr.ReadToEnd();
            sr.Close();

            lines.Clear();
            lines.Add(new CsvLine());
            int i = lines.Count - 1;

            StringBuilder sb = new StringBuilder();

            bool inquote = false;
            bool trim = true;
            bool ignore = false;

            int x = 0;
            int end = file.Length;

            int max = 0;
            while (x < end)
            {
                if (file[x] == _Quote)
                {
                    if (inquote)
                    {
                        inquote = false;
                        if ((x < file.Length - 1) &&
                            (file[x + 1] == _Quote))
                        {
                            x++;
                            sb.Append(_Quote);
                            inquote = true;
                        }
                        // Ignore all characters outside of a quoted
                        // section until the next delimiter/newline
                        if (!inquote)
                        {
                            ignore = true;
                        }
                    }
                    else
                    {
                        sb.Remove(0, sb.Length);
                        inquote = true;
                        trim = false;
                    }
                }
                else if (file[x] == _Delimiter)
                {
                    if (inquote)
                    {
                        sb.Append(_Delimiter);
                    }
                    else
                    {
                        // Add value to array and clear
                        string value = sb.ToString();
                        if (trim)
                        {
                            value = value.Trim();
                        }
                        lines[i].Add(value);
                        sb.Remove(0, sb.Length);

                        // reset ignore and trim
                        ignore = false;
                        trim = true;
                    }
                }
                else if ((file[x] == '\n') ||
                         (file[x] == '\r'))
                {
                    if (inquote)
                    {
                        sb.Append(file[x]);
                    }
                    else
                    {
                        // Handle \r\n style line endings
                        if ((file[x] == '\r') &&
                            (x < end - 1) &&
                            (file[x + 1] == '\n'))
                        {
                            x++;
                        }

                        // Add value to array and clear
                        string value = sb.ToString();
                        if (trim)
                        {
                            value = value.Trim();
                        }
                        lines[i].Add(value);
                        sb.Remove(0, sb.Length);

                        max = Math.Max(lines[lines.Count - 1].Count, max);
                        // Add a new line to the array
                        lines.Add(new CsvLine());
                        i++;

                        // reset ignore and trim
                        ignore = false;
                        trim = true;
                    }
                }
                else
                {
                    if (!ignore)
                    {
                        sb.Append(file[x]);
                    }
                }

                x++;
            }
            if (lines[lines.Count - 1].Count == 0)
            {
                lines.RemoveAt(lines.Count - 1);
            }

            if (useSetLength)
            {
                for (int y = 0; y < lines.Count; y++)
                {
                    while (lines[y].Count < max)
                    {
                        lines[y].Add(quote + quote);
                    }
                }
            }
        }
    }
}
