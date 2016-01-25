using System;
using System.Collections.Generic;
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
        }

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
                //lblSNVal.Text = services[lvServices.SelectedIndices[0]].ServiceName.ToString();
                //lblSSVal.Text = services[lvServices.SelectedIndices[0]].Status.ToString();

                ManagementPath mp;
                //mp = new ManagementPath(string.Format("Win32_Service.Name='{0}'",
                //services[lvServices.SelectedIndices[0]].ServiceName));

                //ManagementObject mo = new ManagementObject(mp);
                //rtbServDesc.Text = mo["Description"].ToString();
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
