using Microsoft.Maps.MapControl.WPF;
using MyClassLibrary;
using Newtonsoft.Json;
using System.Net;
using System.Windows;
using System.Windows.Media;

namespace MyWPF {
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window {

        public MainWindow() {
            InitializeComponent();
        }

        private void GoToLocationButton_Click(object sender, RoutedEventArgs e) {
            string fromCity = LatituteBox.Text;
            string toCity = LongitureBox.Text;

            Route routePath = getRouteFromUrl(String.Format(
                "https://dev.virtualearth.net/REST/V1/Routes/Driving?wp.0={0}&wp.1={1}&optmz=distance&routeAttributes=routePath&key=DPkT2FfRTueyLqqZj3on~Q0nTGD7hmIXtB4ZPnGMdog~AllB5NgntcvtYNbdx0nHKeWTgDwwQjtoCYsKEdNJbULnLTHERmdJ31tK54P5NSKK",
                fromCity,
                toCity), fromCity, toCity);

            MapPolyline routeLine = createMapPolyLine(routePath);

            drawLineOnMap(routeLine);
            centerMapOnRouteStart(routePath);
        }

        public void drawLineOnMap(MapPolyline routeLine) {
            MapName.Children.Add(routeLine);
        }

        public void centerMapOnRouteStart(Route routePath) {
            MapName.Center = new Location(routePath.Coordinates[0].Latitude, routePath.Coordinates[0].Longitude);
        }

        public MapPolyline createMapPolyLine(Route routePath) {
            LocationCollection locationCollection = [];

            for (int i = 0; i < routePath.Coordinates.Count; i++) {
                locationCollection.Add(new Location(routePath.Coordinates[i].Latitude, routePath.Coordinates[i].Longitude));
            }

            MapPolyline routeLine = createMapPolyLine(locationCollection);

            return routeLine;
        }

        public MapPolyline createMapPolyLine(LocationCollection locationCollection) {
            return new MapPolyline() {
                Locations = locationCollection,
                Stroke = new SolidColorBrush(Colors.Blue),
                StrokeThickness = 5
            };
        }

        public Route getRouteFromUrl(string url, string fromCity, string toCity) {
            var json = new WebClient().DownloadString(url);

            dynamic jsonObj = JsonConvert.DeserializeObject<dynamic>(json);
            var coordinateList = jsonObj["resourceSets"][0]["resources"][0]["routePath"]["line"]["coordinates"];

            List<Coordinate> coordinates = new List<Coordinate>();

            for (int i = 0; i < coordinateList.Count; i++) {
                Coordinate coordinate = new Coordinate(
                    Convert.ToDouble(coordinateList[i][0].ToString()),
                    Convert.ToDouble(coordinateList[i][1].ToString()));
                coordinates.Add(coordinate);
            }

            Route route = new(fromCity, toCity, coordinates);

            return route;

        }
    }
}