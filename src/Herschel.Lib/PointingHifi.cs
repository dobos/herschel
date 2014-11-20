﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace Herschel.Lib
{
    public struct PointingHifi
    {
        public Instrument Instrument;
        public long ObsID;
        public HifiObsType ObsType;
        public double Ra;
        public double Dec;
        public double Pa;
        public double AV;
        public double SampleTime;
        public double CorrTime;
        public double mapwidth;
        public double maphigh;
        public double pattangle;
    }
}
