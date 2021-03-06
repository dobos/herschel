﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Herschel.Lib;

namespace Herschel.Loader
{
    public struct RawPointingPacs
    {
        public long ObsID;
        public long BBID;
        public long FineTime;
        public Instrument Instrument;
        public double Ra;
        public double RaError;
        public double Dec;
        public double DecError;
        public double Pa;
        public double PaError;
        public double AVX;
        public double AVXError;
        public double AVY;
        public double AVYError;
        public double AVZ;
        public double AVZError;
        public double AV;
        public long Utc;

        public bool IsAPosition;
        public bool IsBPosition;
        public bool IsOffPosition;
        public bool IsOnTarget;
        public int RasterLineNum;
        public int RasterColumnNum;
    }
}
