using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Management;
using System.ServiceProcess;
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
using System.Windows.Threading;

namespace System_Info
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        // Array to hold services.
        ServiceController[] services;

        public MainWindow()
        {
            InitializeComponent();

            initServiceList();

            // Update memory statistics every second.
            DispatcherTimer dTimer = new DispatcherTimer();
            dTimer.Tick += new EventHandler(updateSysInfo);
            dTimer.Interval = new TimeSpan(0, 0, 1);
            dTimer.Start();
        }

        // Get system information
        private void updateSysInfo(object sender, EventArgs e)
        {
            PerformanceCounter ramInfo;
            ObjectQuery oq = new ObjectQuery("SELECT * FROM Win32_OperatingSystem");
            ManagementObjectSearcher mos = new ManagementObjectSearcher(oq);
            ManagementObjectCollection moc = mos.Get();
            
            try
            {

                foreach (ManagementObject mo in moc)
                {
                    int memSize = int.Parse(mo["TotalVisibleMemorySize"].ToString()) / 1024;
                    lblMemTotalVal.Content = memSize.ToString() + " MB";
                }

                ramInfo = new PerformanceCounter("Memory", "Available MBytes");
                lblMemAvailVal.Content = ramInfo.NextValue() + " MB";
            }
            catch (Exception ex)
            {

            }

            var wmiObject = new ManagementObjectSearcher("select * from Win32_OperatingSystem");

            var memoryValues = wmiObject.Get().Cast<ManagementObject>().Select(mo => new {
                FreePhysicalMemory = Double.Parse(mo["FreePhysicalMemory"].ToString()),
                TotalVisibleMemorySize = Double.Parse(mo["TotalVisibleMemorySize"].ToString())
            }).FirstOrDefault();

            if (memoryValues != null)
            {
                var percent = ((memoryValues.TotalVisibleMemorySize - memoryValues.FreePhysicalMemory) / memoryValues.TotalVisibleMemorySize) * 100;
            }
        }

        // Initialise service list.
        private void initServiceList()
        {
            services = ServiceController.GetServices();
            List<servList> sl = new List<servList>();

            for (int i = 0; i < services.Length; i++)
            {
                sl.Add(new servList()
                {
                    serviceName = services[i].DisplayName.ToString(),
                    serviceStatus = services[i].Status.ToString()
                });
            }

            lvServices.ItemsSource = sl;
        }

        // When an index is selected in the list box, get information 
        // about the service selected.
        private void listView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                lblSNVal.Content = services[lvServices.SelectedIndex].ServiceName.ToString();
                lblSSVal.Content = services[lvServices.SelectedIndex].Status.ToString();

                ManagementPath mp;
                mp = new ManagementPath(string.Format("Win32_Service.Name='{0}'",
                    services[lvServices.SelectedIndex].ServiceName));

                ManagementObject mo = new ManagementObject(mp);
                tbServDesc.Text = mo["Description"].ToString();
            }
            catch (Exception ex)
            {

            }
        }

        protected class servList
        {
            public string serviceName { get; set; }
            public string serviceStatus { get; set; }
        }
    }
}
