using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyClassLibrary {
    public class Route {

        private List<Coordinate> coordinates;

        public Route(List<Coordinate> coordinates) {
            this.Coordinates = coordinates;
        }

        public List<Coordinate> Coordinates { get => coordinates; set => coordinates = value; }
    }
}
