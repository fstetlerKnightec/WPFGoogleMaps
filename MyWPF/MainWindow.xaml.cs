using Microsoft.Maps.MapControl.WPF;
using System.ComponentModel;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Text.Json.Nodes;
using System.Windows;
using Newtonsoft.Json;

namespace MyWPF {
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window {

        public MainWindow() {
            InitializeComponent();
        }

        private void GoToLocationButton_Click(object sender, RoutedEventArgs e) {
            double latitude = Convert.ToDouble(LatituteBox.Text);
            double longitude = Convert.ToDouble(LongitureBox.Text);
            MapName.Center = new Location(latitude, longitude);


        }
    }
}