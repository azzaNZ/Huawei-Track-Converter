﻿using System;
using System.Collections.Generic;
using System.Device.Location;
using System.Linq;

namespace Huawei_Track_Converter
{
    public class HuaweiParser
    {
        //altitude data embedded in the tp=lbs returns incorrect results 
        //altitude data contained in tp=alti is correct
        //when set to true the altitude data contained in tp=lbs will be ignored
        private bool ignoreAltitudeInLocationData = true;

        //list of data points extracted from Huawei file
        List<HuaweiDatumPoint> _data = new List<HuaweiDatumPoint>();
        public List<HuaweiDatumPoint> Data
        {
            get => _data;
            set => _data = value;
        }

        /// <summary>
        /// Total distance contained within parsed data
        /// </summary>
        public double TotalDistance
        {
            get { return _data.Sum(x => x.distance); }
        }
        /// <summary>
        /// Total climbing meters
        /// </summary>
        public double Ascent
        {
            get { return _data.Where(x => x.verticalDistance>0).Sum(x => x.verticalDistance); }
        }
        /// <summary>
        /// Total descending meters
        /// </summary>
        public double Descent
        {
            get { return Math.Abs(_data.Where(x => x.verticalDistance < 0).Sum(x => x.verticalDistance)); }
        }


        //path of file to attempt to parse as a huawei file
        public HuaweiParser(string path)
        {           
            int counter = 0;
            string line;

            using (System.IO.StreamReader file =
                new System.IO.StreamReader(path))
            {
                while ((line = file.ReadLine()) != null)
                {
                    try
                    {
                        long timeStamp = 0;
                        HuaweiDatumPoint dataPoint = null;
                        if (line.Length > 0)
                        {
                            counter++;

                            //break line into data items
                            string[] lineItemArray = line.Split(';');

                            //examine line type
                            switch (lineItemArray[0])
                            {
                                case "tp=lbs":
                                    //Location lines
                                    timeStamp = Convert.ToInt64(Convert.ToDouble(lineItemArray[5].Substring(2)));
                                    double latitude = Convert.ToDouble(lineItemArray[2].Substring(4));
                                    double longitude = Convert.ToDouble(lineItemArray[3].Substring(4));
                                    //non-reading will appear as 90,-80
                                    if (latitude != 90 && longitude != 80)
                                    {
                                        NormaliseTimeStamp(ref timeStamp);
                                        //get this timestamp from dataset
                                        dataPoint = GetDataPoint(ref _data, timeStamp);
                                        //add location data to dataPoint
                                        dataPoint.latitude = latitude;
                                        dataPoint.longitude = longitude;

                                        if (!ignoreAltitudeInLocationData)
                                            dataPoint.altitude = Convert.ToDouble(lineItemArray[4].Substring(4));
                                    }
                                    break;

                                case "tp=h-r":
                                    //heartrate lines
                                    timeStamp = Convert.ToInt64(Convert.ToDouble(lineItemArray[1].Substring(2)));
                                    NormaliseTimeStamp(ref timeStamp);
                                    dataPoint = GetDataPoint(ref _data, timeStamp);
                                    //add heart rate data to dataPoint
                                    int heartRate = Convert.ToInt32(lineItemArray[2].Substring(2));
                                    if (heartRate>0 && heartRate<255)
                                        dataPoint.heartRate = heartRate;
                                    break;
                                case "tp=s-r":
                                    //cadence
                                    timeStamp = Convert.ToInt64(Convert.ToDouble(lineItemArray[1].Substring(2)));
                                    NormaliseTimeStamp(ref timeStamp);
                                    dataPoint = GetDataPoint(ref _data, timeStamp);
                                    int cadence = Convert.ToInt32(lineItemArray[2].Substring(2));
                                    if (cadence > 0 && cadence < 255)
                                        dataPoint.cadence = cadence;
                                    break;
                                case "tp=rs":
                                    //average speed?
                                    break;
                                case "tp=p-m:":
                                    //unknown
                                    break;
                                case "tp=alti":
                                    //altitude
                                    timeStamp = Convert.ToInt64(Convert.ToDouble(lineItemArray[1].Substring(2)));
                                    NormaliseTimeStamp(ref timeStamp);
                                    dataPoint = GetDataPoint(ref _data, timeStamp);
                                    double altitude = Convert.ToDouble(lineItemArray[2].Substring(2));
                                    //don't use altitudes outside feasible range, or if we already have one for this data point (from location dataset)
                                    if (altitude>-1000 && altitude<10000 && dataPoint.altitude==0)
                                        dataPoint.altitude = altitude;
                                    break;
                                default:
                                    //unknown
                                    //throw new Exception($"Unknown line:{line}");
                                    break;
                            }
                        }

                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                        //throw;
                    }

                }

                //finished reading
                file.Close();

                //POST-PROCESSING
                //sort by date order
                _data.Sort();
                //fill in missing altitude data
                FillInAltitudeData(ref _data);
                //Calculate distances between points
                CalculateDistances(ref _data);
            }
        }

        /// <summary>
        /// Fill in missing altitude data
        /// </summary>
        /// <param name="data"></param>
        private void FillInAltitudeData(ref List<HuaweiDatumPoint> data)
        {
            //fill in missing altitude data
            double currentAltitude = 0;
            //look for first available altitude
            if (data.FirstOrDefault(x => x.altitude != 0) != null)
                currentAltitude = data.FirstOrDefault(x => x.altitude != 0).altitude;             //start with first altitude found
            foreach (HuaweiDatumPoint point in data)
            {
                if (point.altitude != 0)
                    currentAltitude = point.altitude;               //set current altitude
                else if (point.HasPosition && currentAltitude != 0)
                    point.altitude = currentAltitude;               //if we have a position then set altitude
            }
        }

        /// <summary>
        /// Calculate distances between points
        /// </summary>
        /// <param name="data"></param>
        private void CalculateDistances(ref List<HuaweiDatumPoint> data)
        {
            HuaweiDatumPoint currentPoint = null;
            int index = 0;      //keep track which point we are on

            //used to hold list of points that are obviously wrong and should be removed.
            //Can't do it inside enumeration, so collecting point to remove later
            //Should probably combined with the "no position" 90,-80 points
            List<HuaweiDatumPoint> removePoints = new List<HuaweiDatumPoint>();
            double climbing = 0;

            foreach (HuaweiDatumPoint point in data)
            {
                index++;
                if (point.HasPosition)
                {
                    //work out distance from previous position to here
                    if (currentPoint != null)
                    {
                        //calculate distance
                        point.distance = currentPoint.position.GetDistanceTo(point.position); //distance in meters
                        point.speed = point.distance/(point.time- currentPoint.time)*(3600/1000);        //speed in km/h

                        //calculate vertical distance (climbing or descending)
                        point.verticalDistance = point.altitude - currentPoint.position.Altitude;
                        if (point.verticalDistance > 0)
                            climbing += point.verticalDistance;
                    }
                    
                    //check for obvious incorrect location
                    //I've picked an arbitrary speed, 100km/h
                    //I've seen instances in deep bush of the watch throwing in the odd rogue lat/long point
                    if (point.speed > 100)
                        removePoints.Add(point);
                    else
                        //update current location with this point's position
                        currentPoint = point;
                }
            }

            foreach (HuaweiDatumPoint point in removePoints)
            {
                data.Remove(point);
            }
        }

        /// <summary>
        /// Returns a searched for timestamp if found.  If not, creates a new entry in dataset for this timestamp
        /// </summary>
        /// <param name="data"></param>
        /// <param name="timeStamp"></param>
        /// <returns></returns>
        private HuaweiDatumPoint GetDataPoint(ref List<HuaweiDatumPoint> data, long timeStamp)
        {
            HuaweiDatumPoint dataPoint = data.FirstOrDefault(x => x.time == timeStamp);
            if (dataPoint == null)
            {
                //not found - create new data point for this time
                dataPoint = new HuaweiDatumPoint
                {
                    time = timeStamp,
                };
                data.Add(dataPoint);
            }

            return dataPoint;
        }

        /// <summary>
        ///Timestamps taken from different devices can have different values.Most common are seconds
        ///    (i.e.t= 1.543646826E9) or microseconds(i.e.t= 1.55173212E12).
        ///This method implements a generic normalization function that transform all values to valid
        ///    unix timestamps(integer with 10 digits).
        /// </summary>
        private void NormaliseTimeStamp(ref long timestamp)
        {
            int oom = Convert.ToInt32(Math.Log10(timestamp));
            if (oom != 9)
            {
                double divisor = 0;
                if (oom > 9)
                    divisor = Math.Pow(10, oom - 9);
                else
                    divisor = Math.Pow(0.1, 9 - oom);
                timestamp = Convert.ToInt64(timestamp / divisor);
            }
        }

    }

    /// <summary>
    /// Class for holding set of data 
    /// </summary>
    public class HuaweiDatumPoint: IComparable
    {
        //normalised time of datum.  Note that is used as an identifier for adding separately tracked data such as altitude and cadence
        private long _time;
        public long time
        {
            get => _time;
            set
            {
                if (value == 0)
                    Console.WriteLine("0");

                _time = value;
                utcTime = DateTimeOffset.FromUnixTimeSeconds(value).DateTime;
            }
        }                
        public DateTime utcTime { get; private set; }           //time evaluated as utc

        private double _latitude;
        public double latitude
        {
            get => _latitude;
            set
            {
                _latitude = value;
                position = new GeoCoordinate(latitude, longitude, altitude);
            }

        }            

        private double _longitude;
        public double longitude
        {
            get => _longitude;
            set
            {
                _longitude = value;
                position = new GeoCoordinate(latitude, longitude, altitude);
            }
        }

        private double _altitude;
        public double altitude
        {
            get => _altitude;
            set
            {
                _altitude = value;
                position = new GeoCoordinate(latitude, longitude, altitude);
            }
        }

        /// <summary>
        /// returns whether we have a position in this data point
        /// </summary>
        public bool HasPosition => latitude != 0 || longitude != 0;

        public GeoCoordinate position { get; private set; }     //lat/long as coordinate
        public double distance { get; set; }            //distance covered since last position
        public double verticalDistance { get; set; }    //climbing if positive, descending if negative
        public double speed { get; set; }               //speed in km/h
        public int heartRate { get; set; }              //heartRate
        public int cadence { get; set; }                //cadence

        /// <summary>
        /// Sort based on recorded time
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public int CompareTo(object obj)
        {
            try
            {
                return time.CompareTo(((HuaweiDatumPoint)obj).time);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
            
        }
    }
}
