using System;
using System.Collections.Generic;
using System.Data;
using System.Device.Location;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Huawei_Track_Converter
{
    public class HuaweiParser
    {
        List<HuaweiDatumPoint> data = new List<HuaweiDatumPoint>();
        public List<HuaweiDatumPoint> Data
        {
            get => data;
            set => data = value;
        }

        public double TotalDistance
        {
            get { return data.Sum(x => x.distance); }
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
                                        dataPoint = GetDataPoint(ref data, timeStamp);
                                        //add location data to dataPoint
                                        dataPoint.latitude = latitude;
                                        dataPoint.longitude = longitude;
                                        dataPoint.altitude = Convert.ToDouble(lineItemArray[4].Substring(4));
                                    }
                                    break;

                                case "tp=h-r":
                                    //heartrate lines
                                    timeStamp = Convert.ToInt64(Convert.ToDouble(lineItemArray[1].Substring(2)));
                                    NormaliseTimeStamp(ref timeStamp);
                                    dataPoint = GetDataPoint(ref data, timeStamp);
                                    //add heart rate data to dataPoint
                                    int heartRate = Convert.ToInt32(lineItemArray[2].Substring(2));
                                    if (heartRate>0 && heartRate<255)
                                        dataPoint.heartRate = heartRate;
                                    break;
                                case "tp=s-r":
                                    //cadence
                                    timeStamp = Convert.ToInt64(Convert.ToDouble(lineItemArray[1].Substring(2)));
                                    NormaliseTimeStamp(ref timeStamp);
                                    dataPoint = GetDataPoint(ref data, timeStamp);
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
                                    dataPoint = GetDataPoint(ref data, timeStamp);
                                    double altitude = Convert.ToDouble(lineItemArray[2].Substring(2));
                                    if (altitude>-1000 && altitude<10000)
                                        dataPoint.altitude = altitude;
                                    break;
                                    //default:
                                    //unknown
                                    //throw new Exception($"Unknown line:{line}");
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
                data.Sort();
                //fill in missing altitude data
                FillInAltitudeData(ref data);
                //Calculate distances between points
                CalculateDistances(ref data);
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
                    currentAltitude = point.altitude;
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
            //fill in missing altitude data
            GeoCoordinate currentLocation = null;
            foreach (HuaweiDatumPoint point in data)
            {
                if (point.HasPosition)
                {
                    //work out distance from previous position to here
                    if (currentLocation != null)
                        point.distance = currentLocation.GetDistanceTo(point.position);  //distance in meters

                    //update current location with this point's position
                    currentLocation = point.position;
                }
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
        private Int64 _time;
        public Int64 time
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
