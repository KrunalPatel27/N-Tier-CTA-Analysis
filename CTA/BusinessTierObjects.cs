//
// BusinessTier objects:  these classes define the objects serving as data 
// transfer between UI and business tier.  These objects carry the data that
// is normally displayed in the presentation tier.  The classes defined here:
//
//    CTAStation
//    CTAStop
//
// NOTE: the presentation tier should not be creating instances of these objects,
// but instead calling the BusinessTier logic to obtain these objects.  You can 
// create instances of these objects if you want, but doing so has no impact on
// the underlying data store --- to change the data store, you have to call the
// BusinessTier logic.
//

using System;
using System.Collections.Generic;


namespace BusinessTier
{

  ///
  /// <summary>
  /// Info about one CTA station.
  /// </summary>
  /// 
  public class CTAStation
  {
    public int ID { get; private set; }
    public string Name { get; private set; }

    public CTAStation(int stationID, string stationName)
    {
      ID = stationID;
      Name = stationName;
    }
  }
  
  ///
  /// <summary>
  /// Info about one CTA stop.
  /// </summary>
  /// 
  public class CTAStop
  {
    public int ID { get; private set; }

    public string Name { get; private set; }

    public int StationID { get; private set; }

    public string Direction { get; private set; }

    public bool ADA { get; private set; }

    public double Latitude { get; private set; }
    public double Longitude { get; private set; }
    public List<string> LineColor { get; private set; }

        public CTAStop(int stopID, string stopName, int stationID, string direction, bool ada, double latitude, double longitude, List<string> lineColor)
    {
      ID = stopID;
      Name = stopName;
      StationID = stationID;
      Direction = direction;
      ADA = ada;
      Latitude = latitude;
      Longitude = longitude;
      LineColor = lineColor;
        }
  }

    ///
    /// <summary>
    /// Necessary Ridership info about one station.
    /// </summary>
    ///
    public class RidershipDetails
    {
        public int TotalRidership { get; private set; }
        public double AvgRidership { get; private set; }
        public double PercentRidership { get; private set; }
        public int WeekdayRidership { get; private set; }
        public int SaturdayRidership { get; private set; }
        public int SundayAndHolidayRidership { get; private set; }

        public RidershipDetails(int totalRidership, double averageRidership, double percentRidership, int weekdayRidership,
                                int saturdayRidership, int sundayAndHolidayRidership)
        {
            TotalRidership = totalRidership;
            AvgRidership = averageRidership;
            PercentRidership = percentRidership;
            WeekdayRidership = weekdayRidership;
            SaturdayRidership = saturdayRidership;
            SundayAndHolidayRidership = sundayAndHolidayRidership;
        }
    }

}//namespace
