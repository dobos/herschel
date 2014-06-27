﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ComponentModel;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Configuration;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Net;
using System.Globalization;
using Herschel.Lib;
using Jhu.Spherical;

namespace Herschel.Ws.Api
{
    [ServiceContract]
    [Description("This service queries the observation database and returns the footprints in various formats.")]
    public interface ISearch
    {
        [OperationContract]
        [DynamicResponseFormat]
        [Description("Finds observations by J2000 equatorial coordinates.")]
        [WebGet(BodyStyle = WebMessageBodyStyle.Wrapped,
            UriTemplate = "Observations?findby=eq&inst={inst}&ra={ra}&dec={dec}&start={start}&end={end}")]
        IEnumerable<Observation> FindObservationEq(string inst, double ra, double dec, long start, long end);

        [OperationContract]
        [DynamicResponseFormat]
        [Description("Finds observations by an intersecting region.")]
        [WebGet(UriTemplate = "Observations?findby=intersect&inst={inst}&region={region}&start={start}&end={end}")]
        IEnumerable<Observation> FindObservationIntersect(string inst, string region, long start, long end);

        [OperationContract]
        [WebGet(UriTemplate = "Observations/{obsID}")]
        [DynamicResponseFormat]
        [Description("Returns the details of a single observation by obsID.")]
        Observation GetObservation(string obsID);

        [OperationContract]
        [WebGet(UriTemplate = "Observations/{obsID}/Footprint")]
        [Description("Returns the footprint of an observation.")]
        string GetObservationFootprint(string obsID);

        [OperationContract]
        [WebGet(UriTemplate = "Observations/{obsID}/Footprint/Outline")]
        [Description("Returns the outline of the footprint of an observation.")]
        string GetObservationOutline(string obsID);

        [OperationContract]
        [WebGet(UriTemplate = "Observations/{obsID}/Footprint/Outline/Points")]
        [Description("Returns the arc endpoints of the outline of the footprint of an observation.")]
        string GetObservationOutlinePoints(string obsID);

        [OperationContract]
        [WebGet(UriTemplate = "Observations/{obsID}/Footprint/Outline/Reduced?limit={limit}")]
        [Description("Returns the reduced outline of the footprint of an observation.")]
        string GetObservationOutlineReduced(string obsID, double limit);

        [OperationContract]
        [WebGet(UriTemplate = "Observations/{obsID}/Footprint/Outline/Reduced/Points?limit={limit}")]
        [Description("Returns the arc endpoints of the reduced outline of the footprint of an observation.")]
        string GetObservationOutlineReducedPoints(string obsID, double limit);

        [OperationContract]
        [WebGet(UriTemplate = "Observations/{obsID}/Footprint/ConvexHull")]
        [Description("Returns the convex hull of the footprint of an observation.")]
        string GetObservationConvexHull(string obsID);

        [OperationContract]
        [WebGet(UriTemplate = "Observations/{obsID}/Footprint/ConvexHull/Outline")]
        [Description("Returns the outline of the convex hull of the footprint of an observation.")]
        string GetObservationConvexHullOutline(string obsID);

        [OperationContract]
        [WebGet(UriTemplate = "Observations/{obsID}/Footprint/ConvexHull/Outline/Points")]
        [Description("Returns the arc endpoints of the outline of the convex hull of the footprint of an observation.")]
        string GetObservationConvexHullOutlinePoints(string obsID);
    }

    public class Footprint : ISearch
    {
        #region Private utility functions

        private Observation GetObservation(long obsID)
        {
            var s = new ObservationSearch();
            var obs = s.Get(obsID);

            if (obs == null)
            {
                ThrowNotFoundException();
            }

            return obs;
        }

        private void ThrowNotFoundException()
        {
            throw new WebFaultException<string>("Observation not found", HttpStatusCode.NotFound);
        }

        private string FormatOutlinePoints(Outline outline)
        {
            var sb = new StringBuilder();

            foreach (var loop in outline.LoopList)
            {
                int q = 0;
                foreach (var arc in loop.ArcList)
                {
                    if (q == 0)
                    {
                        sb.AppendFormat(CultureInfo.InvariantCulture, "{0:R} {1:R}", arc.Point1.RA, arc.Point1.Dec);
                        sb.AppendLine();
                    }

                    sb.AppendFormat(CultureInfo.InvariantCulture, "{0:R} {1:R}", arc.Point2.RA, arc.Point2.Dec);
                    sb.AppendLine();

                    q++;
                }

                sb.AppendLine();
            }

            return sb.ToString();
        }

        #endregion
        #region Interface implementation

        public IEnumerable<Observation> FindObservationEq(string inst, double ra, double dec, long start, long end)
        {
            var s = new ObservationSearch()
            {
                Instrument = Instrument.Pacs,
                Point = new Jhu.Spherical.Cartesian(ra, dec),
            };

            return s.FindEq();
        }

        public IEnumerable<Observation> FindObservationIntersect(string inst, string region, long start, long end)
        {
            return null;
        }

        public Observation GetObservation(string obsID)
        {
            var obs = GetObservation(long.Parse(obsID));
            return obs;
        }

        public string GetObservationFootprint(string obsID)
        {
            var obs = GetObservation(long.Parse(obsID));
            return obs.Region.ToString();
        }

        public string GetObservationOutline(string obsID)
        {
            var obs = GetObservation(long.Parse(obsID));
            return obs.Region.Outline.ToString();
        }

        public string GetObservationOutlinePoints(string obsID)
        {
            var obs = GetObservation(long.Parse(obsID));
            return FormatOutlinePoints(obs.Region.Outline);
        }

        public string GetObservationOutlineReduced(string obsID, double limit)
        {
            var obs = GetObservation(long.Parse(obsID));
            obs.Region.Outline.Reduce(limit);
            return obs.Region.Outline.ToString();
        }

        public string GetObservationOutlineReducedPoints(string obsID, double limit)
        {
            var obs = GetObservation(long.Parse(obsID));
            obs.Region.Outline.Reduce(limit);
            return FormatOutlinePoints(obs.Region.Outline);
        }

        public string GetObservationConvexHull(string obsID)
        {
            var obs = GetObservation(long.Parse(obsID));
            return obs.Region.Outline.GetConvexHull().ToString();
        }

        public string GetObservationConvexHullOutline(string obsID)
        {
            var obs = GetObservation(long.Parse(obsID));

            var chull = obs.Region.Outline.GetConvexHull();
            chull.Simplify();

            return chull.Outline.ToString();
        }

        public string GetObservationConvexHullOutlinePoints(string obsID)
        {
            var obs = GetObservation(long.Parse(obsID));

            var chull = obs.Region.Outline.GetConvexHull();
            chull.Simplify();

            return FormatOutlinePoints(chull.Outline);
        }

        #endregion
    }
}
