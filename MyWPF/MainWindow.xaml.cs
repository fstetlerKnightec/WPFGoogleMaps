using Microsoft.Maps.MapControl.WPF;
using System.ComponentModel;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Text.Json.Nodes;
using System.Windows;
using Newtonsoft.Json;
using System.Text.Json.Serialization;
using Newtonsoft;
using MyClassLibrary;

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

            readJsonFromUrl(String.Format("https://dev.virtualearth.net/REST/V1/Routes/Driving?wp.0={0}&wp.1={1}&optmz=distance&routeAttributes=routePath&key=DPkT2FfRTueyLqqZj3on~Q0nTGD7hmIXtB4ZPnGMdog~AllB5NgntcvtYNbdx0nHKeWTgDwwQjtoCYsKEdNJbULnLTHERmdJ31tK54P5NSKK", "taby", "uppsala"));

        }

        public Route readJsonFromUrl(string url) {
            var json = new WebClient().DownloadString(url);
            //var obj = JsonConvert.DeserializeObject(json);
            //var f = JsonConvert.SerializeObject(obj, Formatting.Indented);
            //AuthenticationResultCode authent = JsonConvert.DeserializeObject<AuthenticationResultCode>(json);

            dynamic jsonObj = JsonConvert.DeserializeObject<dynamic>(json);
            var coordinateList = jsonObj["resourceSets"][0]["resources"][0]["routePath"]["line"]["coordinates"];

            List<Coordinate> coordinates = new List<Coordinate>();

            for (int i = 0; i < coordinateList.Count; i++) {
                Coordinate coordinate = new Coordinate(
                    Convert.ToDouble(coordinateList[i][0].ToString()),
                    Convert.ToDouble(coordinateList[i][1].ToString()));
                coordinates.Add(coordinate);
            }

            Route route = new Route("Taby", "Uppsala", coordinates);

            return route;

        }
    }
}