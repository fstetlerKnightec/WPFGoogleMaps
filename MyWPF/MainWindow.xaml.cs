using IronXL;
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

        string apiKey = "DPkT2FfRTueyLqqZj3on~Q0nTGD7hmIXtB4ZPnGMdog~AllB5NgntcvtYNbdx0nHKeWTgDwwQjtoCYsKEdNJbULnLTHERmdJ31tK54P5NSKK";

        List<string> destinations = new List<string>();

        public MainWindow() {
            InitializeComponent();
        }

        protected void AddLocationButton_Click(object sender, RoutedEventArgs e) {
            //string fromCity = FromCityTextBox.Text;
            //string fromRegionCountry = FromRegionCountryTextBox.Text;
            //string toCity = ToCityTextBox.Text;
            //string toRegionCountry = ToRegionCountryTextBox.Text;
            string filePath = "resources\\Unilever_Spaltenindex_Eiger 8 Jahrestender.xlsx";
            WorkSheet sheet = getDataFromExcelFileFromPath(filePath);

            IronXL.Range fromCity = sheet["D1968:D1969"];
            IronXL.Range fromRegionCountry = sheet["E1968:E1969"];

            IronXL.Range toCity = sheet["D1985:D1986"];
            IronXL.Range toRegionCountry = sheet["E1985:E1986"];

            for (int i = 0; i < fromCity.RowCount; i++) {
                string fromDestination = fromCity.Rows[i].Value + ", " + fromRegionCountry.Rows[i].Value;
                string toDestination = toCity.Rows[i].Value + ", " + toRegionCountry.Rows[i].Value;

                destinations.Add(fromDestination + " -> " + toDestination);
            }


            string fullString = "";
            foreach (string destination in destinations) {
                fullString += destination + "\n";
            }
            RoutesTextBlock.Text = fullString;
        }

        protected void PrintRoutesOnMapButton_Click(object sender, RoutedEventArgs e) {
            List<Route> routePaths = new List<Route>();

            foreach (string destination in destinations) {
                string fromDestination = destination.Split(" -> ")[0];
                string toDestination = destination.Split(" -> ")[1];
                Route routePath = getRouteFromUrl(String.Format(
                "https://dev.virtualearth.net/REST/V1/Routes/Driving?wp.0={0}&wp.1={1}&optmz=distance&routeAttributes=routePath&key=" + apiKey,
                fromDestination,
                toDestination), fromDestination, toDestination);
                routePaths.Add(routePath);
            }

            foreach (Route route in routePaths) {
                MapPolyline routeLine = createMapPolyLine(route);
                drawLineOnMap(routeLine);
            }
            centerMapOnRouteStart(routePaths[0]);
        }

        public WorkSheet getDataFromExcelFileFromPath(string filePath) {

            WorkBook workbook = WorkBook.Load(filePath);
            WorkSheet sheet = workbook.WorkSheets[7];

            return sheet;
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