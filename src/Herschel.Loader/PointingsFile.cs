﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Data;
using System.Data.SqlClient;
using Herschel.Lib;

namespace Herschel.Loader
{
    abstract class PointingsFile
    {
        private PointingObservationType observationType;

        public PointingObservationType ObservationType
        {
            get { return observationType; }
            set { observationType = value; }
        }

        protected abstract bool Parse(string[] parts, out RawPointing pointing);

        protected void Write(RawPointing p, TextWriter writer)
        {
            if (p.FineTime < 1000000000000000 || p.FineTime > 2000000000000000)
            {
                throw new Exception("Suspicious value of fine time.");
            }

            writer.Write("{0} ", (byte)p.Instrument);
            writer.Write("{0} ", p.ObsID);
            writer.Write("{0} ", p.BBID);
            writer.Write("{0} ", (sbyte)p.ObsType);
            writer.Write("{0} ", p.FineTime);
            writer.Write("{0} ", p.Ra);
            writer.Write("{0} ", p.Dec);
            writer.Write("{0} ", p.Pa);
            writer.Write("{0} ", p.AV);
            writer.Write("{0} ", p.Aperture);
            writer.Write("{0} ", p.Width);
            writer.Write("{0} ", p.Height);
            writer.Write("{0} ", p.IsAPosition ? 1 : 0);
            writer.Write("{0} ", p.IsBPosition ? 1 : 0);
            writer.Write("{0} ", p.IsOffPosition ? 1 : 0);
            writer.Write("{0} ", p.IsOnTarget ? 1 : 0);
            writer.Write("{0} ", p.RasterLineNum);
            writer.Write("{0} ", p.RasterColumnNum);
            writer.Write("{0}", p.RasterAngle);

            writer.WriteLine();
        }
        
        protected IEnumerable<RawPointing> ReadPointingsFile(string filename)
        {
            // Open file
            using (var infile = new StreamReader(filename))
            {
                // Skip four lines
                for (int i = 0; i < 4; i++)
                {
                    infile.ReadLine();
                }

                string line;
                while ((line = infile.ReadLine()) != null)
                {
                    RawPointing p;
                    if (Parse(line.Split(' '), out p))
                    {
                        yield return p;
                    }
                }
            }
        }

        /// <summary>
        /// Reads Herschel pointing file and writes bulk-insert ready file
        /// </summary>
        /// <param name="inputFile"></param>
        /// <param name="outputFile"></param>
        public virtual void ConvertPointingsFile(string inputFile, string outputFile, bool append)
        {
            append &= File.Exists(outputFile);

            using (var outfile = new StreamWriter(outputFile, append))
            {
                foreach (var p in ReadPointingsFile(inputFile))
                {
                    Write(p, outfile);
                }
            }
        }

    }
}
