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

        List<string> destinations = new List<string>();

        public MainWindow() {
            InitializeComponent();
        }

        private void AddLocationButton_Click(object sender, RoutedEventArgs e) {
            string fromCity = FromCityTextBox.Text;
            string fromRegionCountry = FromRegionCountryTextBox.Text;
            string toCity = ToCityTextBox.Text;
            string toRegionCountry = ToRegionCountryTextBox.Text;

            string fromDestination = fromCity + ", " + fromRegionCountry;
            string toDestination = toCity + ", " + toRegionCountry;

            destinations.Add(fromDestination + " -> " + toDestination);

            string fullString = "";
            foreach (string destination in destinations) {
                fullString += (destination + "\n");
            }
            RoutesTextBlock.Text = fullString;

            
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

        private void PrintRoutesOnMapButton_Click(object sender, RoutedEventArgs e) {

            List<Route> routePaths = new List<Route>();
            
            foreach (string destination in destinations) {
                string fromDestination = destination.Split(" -> ")[0];
                string toDestination = destination.Split(" -> ")[1];
                Route routePath = getRouteFromUrl(String.Format(
                "https://dev.virtualearth.net/REST/V1/Routes/Driving?wp.0={0}&wp.1={1}&optmz=distance&routeAttributes=routePath&"
                + "key=DPkT2FfRTueyLqqZj3on~Q0nTGD7hmIXtB4ZPnGMdog~AllB5NgntcvtYNbdx0nHKeWTgDwwQjtoCYsKEdNJbULnLTHERmdJ31tK54P5NSKK",
                fromDestination,
                toDestination), fromDestination, toDestination);
                routePaths.Add(routePath);
            }

            foreach (Route route in routePaths) {
                MapPolyline routeLine = createMapPolyLine(route);
                drawLineOnMap(routeLine);
            }
            centerMapOnRouteStart(routePaths[0]);

            //// du kan skriva Paris, TX i textfältet för att speca vilket område
            //Route routePath = getRouteFromUrl(String.Format(
            //    "https://dev.virtualearth.net/REST/V1/Routes/Driving?wp.0={0}&wp.1={1}&optmz=distance&routeAttributes=routePath&"
            //    + "key=DPkT2FfRTueyLqqZj3on~Q0nTGD7hmIXtB4ZPnGMdog~AllB5NgntcvtYNbdx0nHKeWTgDwwQjtoCYsKEdNJbULnLTHERmdJ31tK54P5NSKK",
            //    fromDestination,
            //    toDestination), fromCity, toCity);

            //Route routePath2 = getRouteFromUrl(
            //    "https://dev.virtualearth.net/REST/V1/Routes/Driving?wp.0=stockholm&wp.1=uppsala&optmz=distance&routeAttributes=routePath&"
            //    + "key=DPkT2FfRTueyLqqZj3on~Q0nTGD7hmIXtB4ZPnGMdog~AllB5NgntcvtYNbdx0nHKeWTgDwwQjtoCYsKEdNJbULnLTHERmdJ31tK54P5NSKK", "stockholm", "uppsala");

            ////Route routePath = getRouteFromUrl(String.Format("http://dev.virtualearth.net/REST/v1/Routes/Driving?wayPoint.1={0}
            ////&countryRegion={1}&viaWaypoint.2={2}&optimize=distance&routeAttributes=routePath&countryRegion={3}
            ////&key=DPkT2FfRTueyLqqZj3on~Q0nTGD7hmIXtB4ZPnGMdog~AllB5NgntcvtYNbdx0nHKeWTgDwwQjtoCYsKEdNJbULnLTHERmdJ31tK54P5NSKK", fromCity, "Stockholm", toCity, "Austria"), fromCity, toCity);

            //MapPolyline routeLine = createMapPolyLine(routePath);
            //MapPolyline routeLine2 = createMapPolyLine(routePath2);


            //drawLineOnMap(routeLine);
            //drawLineOnMap(routeLine2);
            //centerMapOnRouteStart(routePath);
        }
    }
}