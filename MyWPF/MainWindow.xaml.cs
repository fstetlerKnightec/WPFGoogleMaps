﻿using IronXL;
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
        private readonly string GOOGLE_APIKEY =
            "AIzaSyBM_ZnbQ_ahHUBjSieaaMOgP7_eSK7gFvw";

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
            //IronXL.Range fromCity = SHEET["D1960:D1970"];
            //IronXL.Range fromRegionCountry = SHEET["E1960:E1970"];

            //IronXL.Range toCity = SHEET["D1977:D1987"];
            //IronXL.Range toRegionCountry = SHEET["E1977:E1987"];

            string fromCity = "Uppsala";
            string fromRegionCountry = "Sverige";
            string toCity = "Stockholm";
            string toRegionCountry = "Sverige";
            AddDestinationsToList(fromCity, fromRegionCountry, toCity, toRegionCountry);

            RoutesTextBlock.Text = GetDestinationStrings().ToString();
        }

        protected void PrintRoutesOnMapButton_Click(object sender, RoutedEventArgs e) {
            List<Route> routePaths = GetRoutePaths();
            DrawAllLinesOnMap(routePaths);
            CenterMapOnRouteStart(routePaths[0]);
        }

        private List<Route> GetRoutePaths() {

            return DESTINATIONS.Select(d =>
            GetRouteFromUrl(String.Format(
                "https://maps.googleapis.com/maps/api/directions/json?origin={0}&destination={1}&key=" + GOOGLE_APIKEY,
                d.Split(" -> ")[0],
                d.Split(" -> ")[1])))
                .ToList();
        }

        private void AddDestinationsToList(string fromCity, string fromRegionCountry, string toCity, string toRegionCountry) {
            string fromDestination = fromCity + ", " + fromRegionCountry;
            string toDestination = toCity + ", " + toRegionCountry;
            DESTINATIONS.Add(fromDestination + " -> " + toDestination);
        }

        //private void AddDestinationsToList(IronXL.Range fromCity, IronXL.Range fromRegionCountry, IronXL.Range toCity, IronXL.Range toRegionCountry) {
        //    for (int i = 0; i < fromCity.RowCount; i++) {
        //        string fromDestination = fromCity.Rows[i].Value + ", " + fromRegionCountry.Rows[i].Value;
        //        string toDestination = toCity.Rows[i].Value + ", " + toRegionCountry.Rows[i].Value;
        //        DESTINATIONS.Add(fromDestination + " -> " + toDestination);
        //    }
        //}

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
            JArray coordinateList = GetCoordinateListGoogle(url);
            List<Coordinate> coordinates = GetCoordinatesGoogle(coordinateList);
            return new(coordinates);
        }

        private JArray GetCoordinateListGoogle(string url) {
            JArray jArray = new JArray();
            string json = new WebClient().DownloadString(url);
            dynamic jsonObj = JsonConvert.DeserializeObject<dynamic>(json);
            for (int i = 0; i < jsonObj["routes"][0]["legs"][0]["steps"].Count; i++) {
                string polyline = jsonObj["routes"][0]["legs"][0]["steps"][i]["polyline"]["points"];
                for (int j = 0; j < DecodePolyline(polyline).Count; j++) {
                    JArray array1 = new JArray();
                    array1.Add(DecodePolyline(polyline)[j].Item1);
                    array1.Add(DecodePolyline(polyline)[j].Item2);
                    jArray.Add(array1);
                }
            }
            return jArray;

        }

        private List<Coordinate> GetCoordinatesGoogle(JArray coordinateList) {
            return coordinateList.Select(c => new Coordinate(Convert.ToDouble((c[0]).ToString()), Convert.ToDouble(c[1].ToString()))).ToList();
        }

        private static List<(double, double)> DecodePolyline(string polylinePoints) {
            List<(double, double)> coordinates = new List<(double, double)>();
            int index = 0;
            int latitude = 0;
            int longitude = 0;

            while (index < polylinePoints.Length) {
                int shift = 0;
                int result = 0;
                int b;
                do {
                    b = polylinePoints[index++] - 63;
                    result |= (b & 0x1f) << shift;
                    shift += 5;
                } while (b >= 0x20);
                int dlat = ((result & 1) != 0 ? ~(result >> 1) : (result >> 1));
                latitude += dlat;

                shift = 0;
                result = 0;
                do {
                    b = polylinePoints[index++] - 63;
                    result |= (b & 0x1f) << shift;
                    shift += 5;
                } while (b >= 0x20);
                int dlng = ((result & 1) != 0 ? ~(result >> 1) : (result >> 1));
                longitude += dlng;

                double lat = latitude / 1e5;
                double lng = longitude / 1e5;

                coordinates.Add((lat, lng));
            }

            return coordinates;
        }
    }
}