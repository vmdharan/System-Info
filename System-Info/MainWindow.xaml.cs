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

            // Update memory statistics every tick.
            DispatcherTimer dTimer = new DispatcherTimer();
            dTimer.Tick += new EventHandler(updateSysInfo_perTick);
            dTimer.Interval = new TimeSpan(0, 0, 0, 0, 1000);
            dTimer.Start();
        }

        // Get system information
        private void updateSysInfo()
        {
            // Query information about the operating system.
            ObjectQuery oq = new ObjectQuery("SELECT * FROM Win32_OperatingSystem");
            ManagementObjectSearcher mos = new ManagementObjectSearcher(oq);
            ManagementObjectCollection moc = mos.Get();

            // Query information about the processor.
            ManagementClass mc = new ManagementClass("Win32_Processor");
            ManagementObjectCollection moc2 = mc.GetInstances();

            // Query information about the video card.
            ManagementObjectSearcher mos3 = new ManagementObjectSearcher("SELECT * FROM Win32_VideoController");
            ManagementObjectCollection moc3 = mos3.Get();

            try
            {
                foreach (ManagementObject mo in moc)
                {
                    // Total memory - in Megabytes
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
                    // CPU info - Name, physical cores and logical cores.
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

        // Get system information - once every tick.
        private void updateSysInfo_perTick(object sender, EventArgs e)
        {
            PerformanceCounter ramInfo;
            PerformanceCounter uptime;   
            
            // Get information on the available system memory.
            try
            {
                ramInfo = new PerformanceCounter("Memory", "Available MBytes");
                lblMemAvailVal.Content = ramInfo.NextValue() + " MB";
                availMem = (int)ramInfo.NextValue();

                // Update the memory usage percentage.
                if ((availMem > 0) && (totalMem > 0))
                {
                    memVal = ((double)(totalMem-availMem) / totalMem) * 100;
                    lblMemPct.Content = ((int)memVal).ToString();
                }
            }
            catch (Exception ex)
            {

            }

            // Get information on the system uptime.
            try
            {
                uptime = new PerformanceCounter("System", "System Up Time");
                uptime.NextValue();
                lblSysUptimeVal.Content = TimeSpan.FromSeconds((int)uptime.NextValue()).ToString();
            }
            catch (Exception ex)
            {

            }
        }


        // Initialise service list.
        private void initServiceList()
        {
            // Query the ServiceController to get the list of services.
            services = ServiceController.GetServices();

            // Create a list to hold the name and status for each service.
            List<servList> sl = new List<servList>();
            
            // Loop through the services and add them to the list.
            for (int i = 0; i < services.Length; i++)
            {
                sl.Add(new servList()
                {
                    serviceName = services[i].DisplayName.ToString(),
                    serviceStatus = services[i].Status.ToString()
                });
            }

            // Assign the list of services to the list view in the 'Services' tab.
            lvServices.ItemsSource = sl;
        }

        // When an index is selected in the list box, get information 
        // about the service selected.
        private void listView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                // Set the service name field
                lblSNVal.Content = services[lvServices.SelectedIndex].ServiceName.ToString();

                // Set the service status field
                lblSSVal.Content = services[lvServices.SelectedIndex].Status.ToString();

                ManagementPath mp;
                mp = new ManagementPath(string.Format("Win32_Service.Name='{0}'",
                    services[lvServices.SelectedIndex].ServiceName));

                ManagementObject mo = new ManagementObject(mp);
                tbServDesc.Text = mo["Description"].ToString();

                // Query the process ID for a service.
                uint pid = 0;
                string ServicePIDQuery = string.Format(
                    "SELECT ProcessId FROM Win32_Service WHERE Name='{0}'",
                services[lvServices.SelectedIndex].ServiceName.ToString());
                ManagementObjectSearcher searcher =
                    new ManagementObjectSearcher(ServicePIDQuery);

                foreach (ManagementObject obj in searcher.Get())
                {
                    try
                    {
                        // Set the process ID field for a service.
                        pid = (uint)obj["ProcessId"];
                        lblServPIDInfo.Content = (pid > 0 ? pid.ToString() : "");
                    }
                    catch (Exception ex)
                    {

                    }
                }
            }
            catch (Exception ex)
            {

            }
        }

        // Define the class for holding the service name and status, for displaying 
        // them in the list view.
        protected class servList
        {
            public string serviceName { get; set; }
            public string serviceStatus { get; set; }
        }
    }
}
