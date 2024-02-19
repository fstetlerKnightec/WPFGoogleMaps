using IronXL;
using Microsoft.Maps.MapControl.WPF;
using MyClassLibrary;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Net;
using System.Text;
using System.Windows;
using System.Windows.Media;

namespace MyWPF {
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window {

        string APIKEY = "DPkT2FfRTueyLqqZj3on~Q0nTGD7hmIXtB4ZPnGMdog~AllB5NgntcvtYNbdx0nHKeWTgDwwQjtoCYsKEdNJbULnLTHERmdJ31tK54P5NSKK";

        List<string> DESTINATIONS = new List<string>();

        public MainWindow() {
            InitializeComponent();
        }

        protected void AddLocationButton_Click(object sender, RoutedEventArgs e) {
            string filePath = "resources\\Unilever_Spaltenindex_Eiger 8 Jahrestender.xlsx";
            WorkSheet sheet = GetDataFromExcelFileFromPath(filePath);

            //IronXL.Range fromCity = sheet["D1958"]; CAMPONA 1 dosnt work
            IronXL.Range fromCity = sheet["D1968:D1969"];
            IronXL.Range fromRegionCountry = sheet["E1968:E1969"];

            IronXL.Range toCity = sheet["D1985:D1986"];
            IronXL.Range toRegionCountry = sheet["E1985:E1986"];

            AddDestinationsToList(fromCity, fromRegionCountry, toCity, toRegionCountry);

            RoutesTextBlock.Text = GetDestinationStrings().ToString();
        }

        public StringBuilder GetDestinationStrings() {
            StringBuilder fullString = new StringBuilder();
            foreach (string destination in DESTINATIONS) {
                fullString.Append(destination + "\n");
            }
            return fullString;
        }

        public void AddDestinationsToList(IronXL.Range fromCity, IronXL.Range fromRegionCountry, IronXL.Range toCity, IronXL.Range toRegionCountry) {
            for (int i = 0; i < fromCity.RowCount; i++) {
                string fromDestination = fromCity.Rows[i].Value + ", " + fromRegionCountry.Rows[i].Value;
                string toDestination = toCity.Rows[i].Value + ", " + toRegionCountry.Rows[i].Value;

                DESTINATIONS.Add(fromDestination + " -> " + toDestination);
            }
        }

        protected void PrintRoutesOnMapButton_Click(object sender, RoutedEventArgs e) {
            List<Route> routePaths = GetRoutePaths();
            DrawAllLinesOnMap(routePaths);
            CenterMapOnRouteStart(routePaths[0]);
        }

        public List<Route> GetRoutePaths() {
            List<Route> routePaths = new List<Route>();

            foreach (string destination in DESTINATIONS) {
                string fromDestination = destination.Split(" -> ")[0];
                string toDestination = destination.Split(" -> ")[1];
                Route routePath = GetRouteFromUrl(String.Format(
                "https://dev.virtualearth.net/REST/V1/Routes/Driving?wp.0={0}&wp.1={1}&optmz=distance&routeAttributes=routePath&key=" + APIKEY,
                fromDestination,
                toDestination), fromDestination, toDestination);
                routePaths.Add(routePath);
            }

            return routePaths;
        }

        public void DrawAllLinesOnMap(List<Route> routePaths) {
            foreach (Route route in routePaths) {
                MapPolyline routeLine = CreateMapPolyLine(route);
                DrawLineOnMap(routeLine);
            }
        }

        public WorkSheet GetDataFromExcelFileFromPath(string filePath) {

            WorkBook workbook = WorkBook.Load(filePath);
            WorkSheet sheet = workbook.WorkSheets[7];

            return sheet;
        }

        public void DrawLineOnMap(MapPolyline routeLine) {
            MapName.Children.Add(routeLine);
        }

        public void CenterMapOnRouteStart(Route routePath) {
            MapName.Center = new Location(routePath.Coordinates[0].Latitude, routePath.Coordinates[0].Longitude);
        }

        public MapPolyline CreateMapPolyLine(Route routePath) {
            LocationCollection locationCollection = GetLocationCollection(routePath);
            return CreateMapPolyLine(locationCollection);
        }

        public LocationCollection GetLocationCollection(Route routePath) {
            LocationCollection locationCollection = [];
            for (int i = 0; i < routePath.Coordinates.Count; i++) {
                locationCollection.Add(new Location(routePath.Coordinates[i].Latitude, routePath.Coordinates[i].Longitude));
            }
            return locationCollection;
        }

        public MapPolyline CreateMapPolyLine(LocationCollection locationCollection) {
            return new MapPolyline() {
                Locations = locationCollection,
                Stroke = new SolidColorBrush(Colors.Blue),
                StrokeThickness = 5
            };
        }

        public Route GetRouteFromUrl(string url, string fromCity, string toCity) {
            JArray coordinateList = GetCoordinateList(url);
            List<Coordinate> coordinates = GetCoordinates(coordinateList);
            return new(fromCity, toCity, coordinates);
        }

        public JArray GetCoordinateList(string url) {
            string json = new WebClient().DownloadString(url);
            dynamic jsonObj = JsonConvert.DeserializeObject<dynamic>(json);
            return jsonObj["resourceSets"][0]["resources"][0]["routePath"]["line"]["coordinates"];
        }

        public List<Coordinate> GetCoordinates(JArray coordinateList) {
            return coordinateList.Select(c => new Coordinate(Convert.ToDouble(c[0].ToString()), Convert.ToDouble(c[1].ToString()))).ToList();
        }
    }
}