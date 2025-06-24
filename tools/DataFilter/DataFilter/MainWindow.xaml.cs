using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
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
            t = new System.Timers.Timer();
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

        System.Timers.Timer t;

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

        private void LoadSet_Click(object sender, RoutedEventArgs e)
        {
            
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Multiselect = true;
            if (ofd.ShowDialog() == true)
            {
                if (ofd.FileNames.Length != 4)
                {
                    MessageBox.Show("Please select a full set (4) of menu csv files.");
                }
                else
                {
                    ThreadPool.QueueUserWorkItem((state) =>
                    {
                        StringBuilder sb = new StringBuilder();
                        Csv menu = new Csv();
                        Csv dish = new Csv();
                        Csv menuitem = new Csv();
                        Csv menupage = new Csv();

                        foreach (var item in ofd.FileNames)
                        {
                            if (item.Contains("MenuPage"))
                            {
                                menupage.Read(item);
                            }
                            else if (item.Contains("MenuItem"))
                            {
                                menuitem.Read(item);
                            }
                            else if (item.Contains("Menu"))
                            {
                                menu.Read(item);
                            }
                            else if (item.Contains("Dish"))
                            {
                                dish.Read(item);
                            }
                        }

                        HashSet<int> menuids = new HashSet<int>();
                        HashSet<int> dishids = new HashSet<int>();
                        HashSet<int> itemids = new HashSet<int>();
                        HashSet<int> pageids = new HashSet<int>();

                        var arr = new[]
                        {
                        new
                        {
                            csv = menu,
                            ids = menuids,
                            name = "menu"
                        },
                        new
                        {
                            csv = dish,
                            ids = dishids,
                            name = "dish"
                        },
                        new
                        {
                            csv = menuitem,
                            ids = itemids,
                            name = "menuitem"
                        },
                        new
                        {
                            csv = menupage,
                            ids = pageids,
                            name = "menupage"
                        }
                        };

                        FileInfo fi = new FileInfo(ofd.FileNames[0]);


                        using (StreamWriter sw = new StreamWriter($"{fi.DirectoryName}\\integrityissues.txt"))
                        {

                            foreach (var item in arr)
                            {
                                sw.WriteLine($"Reading ids from {item.name}");
                                int idcol = item.csv[0].IndexOf("id");
                                for (int i = 1; i < item.csv.Count; i++)
                                {
                                    int id = int.Parse(item.csv[i][idcol]);

                                    if (item.ids.Contains(id))
                                    {
                                        sw.WriteLine($"Duplicate id {id} in {item.name}");
                                    }
                                    else
                                    {
                                        item.ids.Add(id);
                                    }
                                }
                                sw.WriteLine();
                            }

                            foreach (var item in arr)
                            {
                                sw.WriteLine($"Checking ids in {item.name}");
                                int dishcol = item.csv[0].IndexOf("dish_id");
                                int menucol = item.csv[0].IndexOf("menu_id");
                                int pagecol = item.csv[0].IndexOf("menu_page_id");
                                //int itemcol = item.csv[0].IndexOf(""); NA

                                if (dishcol >= 0)
                                {
                                    for (int i = 1; i < item.csv.Count; i++)
                                    {
                                        if (int.TryParse(item.csv[i][dishcol], out int id))
                                        {
                                            if (!dishids.Contains(id))
                                            {
                                                sw.WriteLine($"Error:{item.name} references dish_id {id} on line {i} does not exist.");
                                            }
                                        }

                                    }
                                }
                                if (menucol >= 0)
                                {
                                    for (int i = 1; i < item.csv.Count; i++)
                                    {
                                        if (int.TryParse(item.csv[i][menucol], out int id))
                                        {
                                            if (!menuids.Contains(id))
                                            {
                                                sw.WriteLine($"Error:{item.name} references menu_id {id} on line {i} does not exist.");
                                            }
                                        }
                                    }
                                }
                                if (pagecol >= 0)
                                {
                                    for (int i = 1; i < item.csv.Count; i++)
                                    {
                                        if (int.TryParse(item.csv[i][pagecol], out int id))
                                        {
                                            if (!pageids.Contains(id))
                                            {
                                                sw.WriteLine($"Error:{item.name} references menu_page_id {id} on line {i} does not exist.");
                                            }
                                        }
                                    }
                                }
                                item.csv.Clear(); // free up some memory
                                sw.WriteLine();
                            }
                            Dispatcher.BeginInvoke((Action)(() =>
                            {
                                IntegrityIssues.Text = "Complete";
                            }));
                        }
                    });

                    
                }
            }
        }
    }
}
