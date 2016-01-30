using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Management;
using System.ServiceProcess;
using System.Windows;
using System.Windows.Controls;
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

        // Memory used percentage.
        public double memVal { get; set; }
        private int totalMem;
        private int availMem;

        public MainWindow()
        {
            InitializeComponent();

            memVal = 0.0;
            totalMem = 0;
            availMem = 0;

            initServiceList();
            updateSysInfo();

            // Update memory statistics every second.
            DispatcherTimer dTimer = new DispatcherTimer();
            dTimer.Tick += new EventHandler(updateSysInfo_perTick);
            dTimer.Interval = new TimeSpan(0, 0, 0, 0, 100);
            dTimer.Start();
        }

        // Get system information
        private void updateSysInfo()
        {
            ObjectQuery oq = new ObjectQuery("SELECT * FROM Win32_OperatingSystem");
            ManagementObjectSearcher mos = new ManagementObjectSearcher(oq);
            ManagementObjectCollection moc = mos.Get();

            ManagementClass mc = new ManagementClass("Win32_Processor");
            ManagementObjectCollection moc2 = mc.GetInstances();

            ManagementObjectSearcher mos3 = new ManagementObjectSearcher("SELECT * FROM Win32_VideoController");
            ManagementObjectCollection moc3 = mos3.Get();

            try
            {
                foreach (ManagementObject mo in moc)
                {
                    // Total memory
                    int memSize = int.Parse(mo["TotalVisibleMemorySize"].ToString()) / 1024;
                    lblMemTotalVal.Content = memSize.ToString() + " MB";
                    totalMem = memSize;

                    // Device name
                    lblSysNameVal.Content = Environment.MachineName;

                    // Operating system
                    lblOSVer1.Content = mo["Caption"].ToString();
                    lblOSVer2.Content = Environment.OSVersion.Version;
                }
            }
            catch (Exception ex)
            {
                // Exception
            }

            try
            {
                foreach(ManagementObject mo2 in moc2)
                {
                    // CPU info
                    lblCPUInfoVal.Content = mo2.Properties["Name"].Value.ToString();
                    lblCoresVal.Content = mo2.Properties["NumberOfCores"].Value.ToString();
                    lblThreadsVal.Content = Environment.ProcessorCount;
                }
            }
            catch (Exception ex)
            {
                // Exception
            }

            try
            {
                foreach(ManagementObject mo3 in moc3)
                {
                    // GPU info
                    lblGPUInfoVal.Content = mo3.Properties["Name"].Value.ToString();
                    lblGPUInfoVer.Content = mo3.Properties["DriverVersion"].Value.ToString();
                }
            }
            catch (Exception ex)
            {

            }
        }

        // Get system information - once every tick
        private void updateSysInfo_perTick(object sender, EventArgs e)
        {
            PerformanceCounter ramInfo;   
            
            try
            {
                ramInfo = new PerformanceCounter("Memory", "Available MBytes");
                lblMemAvailVal.Content = ramInfo.NextValue() + " MB";
                availMem = (int)ramInfo.NextValue();

                if ((availMem > 0) && (totalMem > 0))
                {
                    memVal = ((double)(totalMem-availMem) / totalMem) * 100;
                    lblMemPct.Content = ((int)memVal).ToString();
                }
            }
            catch (Exception ex)
            {

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
