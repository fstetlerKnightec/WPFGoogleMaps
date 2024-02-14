using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyClassLibrary {
    public class Coordinate {

        private double latitude;
        private double longitude;

        public Coordinate(double latitude, double longitude) {
            this.Latitude = latitude;
            this.Longitude = longitude;
        }

        public double Latitude { get => latitude; set => latitude = value; }
        public double Longitude { get => longitude; set => longitude = value; }
    }
}
