using System.Net;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using Newtonsoft;
using Newtonsoft.Json;

namespace MyClassLibrary {
    public class MyClass {

        public static void Main(string[] args) {
            MyClass myClass = new MyClass();
            myClass.readJsonFromUrl(String.Format("https://dev.virtualearth.net/REST/V1/Routes/Driving?wp.0={0}&wp.1={1}&optmz=distance&routeAttributes=routePath&key=DPkT2FfRTueyLqqZj3on~Q0nTGD7hmIXtB4ZPnGMdog~AllB5NgntcvtYNbdx0nHKeWTgDwwQjtoCYsKEdNJbULnLTHERmdJ31tK54P5NSKK", "taby", "uppsala"));
           


        }
        public static string MyHelloMethod() {
            return "Hello world!!!! Hi team";
        }

        public void readJsonFromUrl(string url) {
            var json = new WebClient().DownloadString(url);
            var obj = JsonConvert.DeserializeObject(json);
            var f = JsonConvert.SerializeObject(obj, Formatting.Indented);
            Console.WriteLine(f);
            //AuthenticationResultCode authent = JsonConvert.DeserializeObject<AuthenticationResultCode>(json);

            //dynamic jsonObj = JsonConvert.DeserializeObject<dynamic>(json);
            //var coordinateList = jsonObj["resourceSets"][0]["resources"][0]["routePath"]["line"]["coordinates"];



            //List<Coordinate> coordinates = new List<Coordinate>();
 
            //for (int i = 0; i <  coordinateList.Count; i++) {
            //    Coordinate coordinate = new Coordinate(
            //        Convert.ToDouble(coordinateList[i][0].ToString()), 
            //        Convert.ToDouble(coordinateList[i][1].ToString()));
            //    coordinates.Add(coordinate);
            //}

            //Route route = new Route("Taby", "Uppsala", coordinates);

            //Console.WriteLine(route.Coordinates[0].Latitude);
            //Console.WriteLine(route.Coordinates[0].Longitude);

        }



    }
}
