﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Jhu.Spherical;

namespace Herschel.Lib
{
    public class DetectorHifiMap : Detector
    {
        public double Width { get; set; }
        public double Height { get; set; }

        public override Cartesian[] GetDefaultCorners()
        {
            double a = Width / 2.0 / 60.0;
            double b = Height / 2.0 / 60.0;

            return new Cartesian[]
                {
                    new Cartesian(a, b),
                    new Cartesian(-a, b),
                    new Cartesian(-a, -b),
                    new Cartesian(a, -b)
                };
        }

        /// <summary>
        /// Returns the footprint of the map
        /// </summary>
        /// <returns></returns>
        public override Region GetFootprint(Cartesian pointing, double pa, double aperture)
        {
            return GetFootprintRectangle(GetDefaultCorners(), pointing, pa);
        }
    }
}
