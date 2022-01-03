using Microsoft.Win32;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Paper_Downloader
{
    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public string versionresponse;
        public string buildresponse;
        public dynamic apires1;
        public WebClient client = new WebClient();
        public MainWindow()
        {
            
            AppDomain.CurrentDomain.AssemblyResolve += (sender, args) =>
            {
                string resourceName = new AssemblyName(args.Name).Name + ".dll";
                string resource = Array.Find(this.GetType().Assembly.GetManifestResourceNames(), element => element.EndsWith(resourceName));

                using (var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(resource))
                {
                    Byte[] assemblyData = new Byte[stream.Length];
                    stream.Read(assemblyData, 0, assemblyData.Length);
                    return Assembly.Load(assemblyData);
                }
            };
            InitializeComponent();
            foo();
            downButt.IsEnabled = false;
            buildCB.IsEnabled = false;
            foreach (string version in apires1.versions)
            {
                versionsCB.Items.Add(version.ToString());
            }
        }

        public void foo()
        {
            versionresponse = client.DownloadString("https://papermc.io/api/v2/projects/paper/");
            apires1 = JsonConvert.DeserializeObject<dynamic>(versionresponse);
        }

        private void versionsCB_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (buildCB.Items.Count > 0)
            {
                buildCB.Items.Clear();
            }
            if (versionsCB.SelectedItem == null) { buildCB.IsEnabled = false; } else { buildCB.IsEnabled = true; }
            buildresponse = client.DownloadString("https://papermc.io/api/v2/projects/paper/versions/" + versionsCB.SelectedItem.ToString());
            var apires2 = JsonConvert.DeserializeObject<dynamic>(buildresponse);
            foreach (int build in apires2.builds)
            {
                buildCB.Items.Add(build.ToString());
            }
            downButt.IsEnabled = false;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            string url = $"https://papermc.io/api/v2/projects/paper/versions/{versionsCB.SelectedItem.ToString()}/builds/{buildCB.SelectedItem.ToString()}/downloads/paper-{versionsCB.SelectedItem.ToString()}-{buildCB.SelectedItem.ToString()}.jar";
            SaveFileDialog saveFileDialog1 = new SaveFileDialog();
            saveFileDialog1.FileName = $"paper-{versionsCB.SelectedItem.ToString()}-{buildCB.SelectedItem.ToString()}.jar";
            saveFileDialog1.Filter = "Java Executable (*.jar)|*.jar";
            saveFileDialog1.Title = "Save the Paper Jar File";
            saveFileDialog1.ShowDialog();
            client.DownloadFileCompleted += new AsyncCompletedEventHandler(Completed);
            client.DownloadProgressChanged += new DownloadProgressChangedEventHandler(ProgressChanged);
            client.DownloadFileAsync(new Uri(url), saveFileDialog1.FileName);

        }

        // The event that will fire whenever the progress of the WebClient is changed
        private void ProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            progbar.Value = e.ProgressPercentage;

            lbdown.Content = string.Format("Downloaded: {0} MB / {1} MB",
                (e.BytesReceived / 1024d / 1024d).ToString("0.00"),
                (e.TotalBytesToReceive / 1024d / 1024d).ToString("0.00"));
        }

        private void Completed(object sender, AsyncCompletedEventArgs e)
        {
            if (e.Cancelled == true)
            {
                MessageBox.Show("Download has been canceled.");
            }
            else
            {
                MessageBox.Show("Download completed!");
            }
        }

        private void buildCB_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (buildCB.SelectedItem == null)
            {
                downButt.IsEnabled = false;
            } else
            {
                downButt.IsEnabled = true;
            }
        }
    }
}
