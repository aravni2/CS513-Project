using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace DataFilter
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            t = new Timer();
            t.AutoReset = false;
            t.Elapsed += T_Elapsed;
            t.Interval = 800;
        }

        void Update()
        {
            if (cols == null) return;

            bool unique = Unique.IsChecked == true;
            string search = FilterString.Text.ToLower();

            vals.Clear();

            if (unique)
            {
                Dictionary<string, string> set = new Dictionary<string, string>();
                for (int i = 1; i < csv.Count; i++)
                {
                    if (csv[i][colindex].ToLower().Contains(search))
                    {
                        vals.Clear();
                        bool first = true;
                        foreach (var index in cols)
                        {
                            if (!first)
                            {
                                vals.Append(" |<>| ");
                            }
                            first = false;
                            vals.Append(csv[i][index]);
                        }

                        set[csv[i][colindex]] = vals.ToString();
                    }
                }

                vals.Clear();
                vals.AppendLine($"{set.Count} Total Unique Values");
                foreach (var str in set)
                {
                    vals.AppendLine(str.Value);
                }
            }
            else
            {
                for (int i = 1; i < csv.Count; i++)
                {
                    if (csv[i][colindex].ToLower().Contains(search))
                    {
                        StringBuilder sb = new StringBuilder();
                        bool first = true;
                        foreach (var index in cols)
                        {
                            if (!first)
                            {
                                sb.Append(" |<>| ");
                            }
                            first = false;
                            sb.Append(csv[i][index]);
                        }
                        vals.AppendLine(sb.ToString());//csv[i][colindex]);
                    }
                }
            }

            Values.Text = vals.ToString();
        }

        private void T_Elapsed(object sender, ElapsedEventArgs e)
        {
            Dispatcher.BeginInvoke((Action)(() => Update()), null);
        }

        Timer t;

        Csv csv = new Csv();
        string selcolumn = null;
        int colindex = 0;

        int[] cols;

        private void LoadCsv_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            if (ofd.ShowDialog() == true)
            {
                csv.Read(ofd.FileName);
                ColumnList.ItemsSource = csv[0];
            }
        }

        StringBuilder vals = new StringBuilder();
        private void FilterString_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (selcolumn != null)
            {
                t.Stop();
                t.Start();
            }
        }



        private void ColumnList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            selcolumn = ColumnList.SelectedValue as string;
            if (selcolumn != null)
            {
                colindex = csv[0].IndexOf(selcolumn);
            }

            if (ColumnList.SelectedItems.Count > 1)
            {
                cols = new int[ColumnList.SelectedItems.Count];
                int ind = 0;
                foreach (var item in ColumnList.SelectedItems)
                {
                    cols[ind++] = csv[0].IndexOf(item as string);
                }
            }
            else
            {
                cols = new int[] { colindex };
            }
            t.Stop();
            t.Start();
        }

        private void Unique_Click(object sender, RoutedEventArgs e)
        {
            t.Stop();
            t.Start();
        }
    }
}
