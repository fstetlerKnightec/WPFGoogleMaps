using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyClassLibrary {
    public class Route {

        private string fromCity;
        private string toCity;
        private List<Coordinate> coordinates;

        public Route(string fromCity, string toCity, List<Coordinate> coordinates) {
            this.fromCity = fromCity;
            this.toCity = toCity;
            this.Coordinates = coordinates;
        }

        public string FromCity { get => fromCity; set => fromCity = value; }
        public string ToCity { get => toCity; set => toCity = value; }
        public List<Coordinate> Coordinates { get => coordinates; set => coordinates = value; }
    }
}
