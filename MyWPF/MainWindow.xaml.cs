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
        private readonly string APIKEY = 
            "DPkT2FfRTueyLqqZj3on~Q0nTGD7hmIXtB4ZPnGMdog~AllB5NgntcvtYNbdx0nHKeWTgDwwQjtoCYsKEdNJbULnLTHERmdJ31tK54P5NSKK";

        private readonly List<string> DESTINATIONS = [];

        private WorkSheet SHEET;

        public MainWindow() {
            InitializeComponent();
        }

        protected void LoadExcelButton_Click(object sender, RoutedEventArgs e) {
            string filePath = "resources\\Unilever_Spaltenindex_Eiger 8 Jahrestender.xlsx";
            SHEET = GetDataFromExcelFileFromPath(filePath);
            ExcelLoadedTextBox.Text = "Loaded";
        }

        protected void AddLocationButton_Click(object sender, RoutedEventArgs e) {
            //IronXL.Range fromCity = sheet["D1958"]; CAMPONA 1 dosnt work
            //1965:1970 and 1982:1987
            IronXL.Range fromCity = SHEET["D1960:D1970"];
            IronXL.Range fromRegionCountry = SHEET["E1960:E1970"];

            IronXL.Range toCity = SHEET["D1977:D1987"];
            IronXL.Range toRegionCountry = SHEET["E1977:E1987"];

            AddDestinationsToList(fromCity, fromRegionCountry, toCity, toRegionCountry);

            RoutesTextBlock.Text = GetDestinationStrings().ToString();
        }

        protected void PrintRoutesOnMapButton_Click(object sender, RoutedEventArgs e) {
            List<Route> routePaths = GetRoutePaths();
            //if (routePaths.Count == 0) {
            //    return;
            //} 
            DrawAllLinesOnMap(routePaths);
            CenterMapOnRouteStart(routePaths[0]);
        }

        private List<Route> GetRoutePaths() {
            return DESTINATIONS.Select(d => 
            GetRouteFromUrl(String.Format(
                "https://dev.virtualearth.net/REST/V1/Routes/Driving?wp.0={0}&wp.1={1}&optmz=distance&routeAttributes=routePath&key=" + APIKEY, 
                d.Split(" -> ")[0], 
                d.Split(" -> ")[1])))
                .ToList();
        }

        private void AddDestinationsToList(IronXL.Range fromCity, IronXL.Range fromRegionCountry, IronXL.Range toCity, IronXL.Range toRegionCountry) {
            for (int i = 0; i < fromCity.RowCount; i++) {
                string fromDestination = fromCity.Rows[i].Value + ", " + fromRegionCountry.Rows[i].Value;
                string toDestination = toCity.Rows[i].Value + ", " + toRegionCountry.Rows[i].Value;
                DESTINATIONS.Add(fromDestination + " -> " + toDestination);
            }
        }

        private StringBuilder GetDestinationStrings() {
            StringBuilder fullString = new();
            DESTINATIONS.ForEach(d => fullString.AppendLine(d));
            return fullString;
        }

        private void DrawAllLinesOnMap(List<Route> routePaths) {
            routePaths.ForEach(rp => DrawLineOnMap(CreateMapPolyLine(rp)));
        }

        private WorkSheet GetDataFromExcelFileFromPath(string filePath) {
            WorkBook workbook = WorkBook.Load(filePath);
            return workbook.WorkSheets[7];
        }

        private void DrawLineOnMap(MapPolyline routeLine) {
            MapName.Children.Add(routeLine);
        }

        private void CenterMapOnRouteStart(Route routePath) {
            MapName.Center = new Location(routePath.Coordinates[0].Latitude, routePath.Coordinates[0].Longitude);
        }

        private MapPolyline CreateMapPolyLine(Route routePath) {
            LocationCollection locationCollection = GetLocationCollection(routePath);
            return CreateMapPolyLine(locationCollection);
        }

        private LocationCollection GetLocationCollection(Route routePath) {
            LocationCollection locationCollection = [];
            for (int i = 0; i < routePath.Coordinates.Count; i++) {
                locationCollection.Add(new Location(routePath.Coordinates[i].Latitude, routePath.Coordinates[i].Longitude));
            }
            return locationCollection;
        }

        private MapPolyline CreateMapPolyLine(LocationCollection locationCollection) {
            return new MapPolyline() {
                Locations = locationCollection,
                Stroke = new SolidColorBrush(Colors.Blue),
                StrokeThickness = 5
            };
        }

        private Route GetRouteFromUrl(string url) {
            JArray coordinateList = GetCoordinateList(url);
            List<Coordinate> coordinates = GetCoordinates(coordinateList);
            return new(coordinates);
        }

        //private Route GetRouteFromUrl(string url) {
        //    JArray coordinateList = GetCoordinateList(url);
        //    if (coordinateList == 404) {
        //        return emptylist;
        //    }
        //    List<Coordinate> coordinates = GetCoordinates(coordinateList);
        //    return new(coordinates);
        //}

        private JArray GetCoordinateList(string url) {
            string json = new WebClient().DownloadString(url);
            dynamic jsonObj = JsonConvert.DeserializeObject<dynamic>(json);
            return jsonObj["resourceSets"][0]["resources"][0]["routePath"]["line"]["coordinates"];
        }

        //private JArray GetCoordinateList(string url) {

        //    try {
        //        string json = new WebClient().DownloadString(url);
        //        dynamic jsonObj = JsonConvert.DeserializeObject<dynamic>(json);
        //        return jsonObj["resourceSets"][0]["resources"][0]["routePath"]["line"]["coordinates"];
        //    } catch (WebException we) {
        //        if (we.Status == WebExceptionStatus.) {
                    
        //        }
        //    }
        //}

        private List<Coordinate> GetCoordinates(JArray coordinateList) {
            return coordinateList.Select(c => new Coordinate(Convert.ToDouble(c[0].ToString()), Convert.ToDouble(c[1].ToString()))).ToList();
        }
    }
}