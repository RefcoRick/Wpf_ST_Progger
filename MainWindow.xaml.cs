using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Diagnostics;
using System.IO;
using Wpf_ST_Progger.Properties;

namespace Wpf_ST_Progger
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private bool OKtoClose = true;
        private const string DefaultExeName = "STM32_Programmer_CLI.exe";
        private bool ExeLocated = false;
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            OKtoClose = true;
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            if (OKtoClose)
            {
                Close();
            }
            else
            {
                MessageBox.Show("Not yet OK to close");
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Debug.Print("DEBUG: Loaded: " + sender.ToString() + System.Environment.NewLine);
            /* attempt to locate the executable */

            /* see if there's a string in settings that will do */
            Debug.Print("Propery.Settings.Default.ExeLocation: " + Properties.Settings.Default.ExeLocation);
            if (Properties.Settings.Default.ExeLocation.Length == 0 )
            {
                Properties.Settings.Default.ExeLocation = FindProgrammerExe();
                Properties.Settings.Default.Save();
            }
            else
            {
                if (! File.Exists (Properties.Settings.Default.ExeLocation))
                {
                    MessageBox.Show("The stored location of the ST programmer is not valid" + Environment.NewLine +
                        "Please use the file menu to locate the executable manually");
                }
            }


        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Debug.Print("DEBUG: Closing event");

            if (!OKtoClose)
            {
                MessageBox.Show("Not yet OK to close");
                e.Cancel = true;
            }
        }

        private string FindProgrammerExe()
        {

            // http://www.blackwasp.co.uk/FolderRecursion.aspx

            List<string> found_files = new List<string>();

            foreach (string folder in Directory.GetDirectories(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles)))
            {
                //Debug.Print("Directory found: " + folder + Environment.NewLine);
                try // some directories in the ProgramFiles area are not readable and will throw.  Can't have that.
                {
                    found_files.AddRange(System.IO.Directory.GetFiles(folder, DefaultExeName, SearchOption.AllDirectories));
                }
                catch (UnauthorizedAccessException) { }

            }

            Debug.Print("DEBUG: FindProgrammerExe: Found {0} {1}" + Environment.NewLine, found_files.Count, found_files.Count == 1 ? @"entry" : @"entries");
            foreach (string single_entry in found_files)
            {
                Debug.Print("Found " + single_entry + Environment.NewLine);
            }

            // OK, with luck we've got a single executable to deal with. But maybe there are duplicates.
            if (found_files.Count == 0)
            {
                MessageBox.Show("Unable to locate the ST programmer exe \"" + DefaultExeName + "\"\nPlease use the file menu to locate the executable manually");
                return "";
            }
            if (found_files.Count > 1)
            {
                string messij = string.Format("{0} copies of the programmer executable (\"{1}\") were located", found_files.Count, DefaultExeName) + Environment.NewLine;
                messij += "Please use the file menu to locate the required executable manually";
                //int LocationCount = 1;
                //foreach (var thisone in found_files)
                //    messij += @"Location " + LocationCount++.ToString() + @": " + thisone + Environment.NewLine;
                MessageBox.Show(messij);
                return "";
            }

            return found_files.First();

        }

        private void MenuItem_Click_1(object sender, RoutedEventArgs e)
        {
            Window helpwindow = new HelpWindow();
            helpwindow.Show();
        }
    }
}
