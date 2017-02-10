//
// BusinessTier:  business logic, acting as interface between UI and data store.
//

using System;
using System.Collections.Generic;
using System.Data;


namespace BusinessTier
{

  //
  // Business:
  //
  public class Business
  {
    //
    // Fields:
    //
    private string _DBFile;
    private DataAccessTier.Data dataTier;


    ///
    /// <summary>
    /// Constructs a new instance of the business tier.  The format
    /// of the filename should be either |DataDirectory|\filename.mdf,
    /// or a complete Windows pathname.
    /// </summary>
    /// <param name="DatabaseFilename">Name of database file</param>
    /// 
    public Business(string DatabaseFilename)
    {
      _DBFile = DatabaseFilename;

      dataTier = new DataAccessTier.Data(DatabaseFilename);
    }


    ///
    /// <summary>
    ///  Opens and closes a connection to the database, e.g. to
    ///  startup the server and make sure all is well.
    /// </summary>
    /// <returns>true if successful, false if not</returns>
    /// 
    public bool TestConnection()
    {
      return dataTier.OpenCloseConnection();
    }


    ///
    /// <summary>
    /// Returns all the CTA Stations, ordered by name.
    /// </summary>
    /// <returns>Read-only list of CTAStation objects</returns>
    /// 
    public IReadOnlyList<CTAStation> GetStations()
    {
      List<CTAStation> stations = new List<CTAStation>();

      try
      {
                string sql  = string.Format(@"
                                            SELECT Name, StationID
                                            FROM Stations
                                            ORDER BY Name ASC;");
                DataSet result = dataTier.ExecuteNonScalarQuery(sql);

                foreach (DataRow row in result.Tables["table"].Rows)
                {
                    stations.Add(new CTAStation(Convert.ToInt32(row["StationID"]), Convert.ToString(row["Name"])) );
                }

            }
      catch (Exception ex)
      {
        string msg = string.Format("Error in Business.GetStations: '{0}'", ex.Message);
        throw new ApplicationException(msg);
      }

      return stations;
    }


    ///
    /// <summary>
    /// Returns the CTA Stops associated with a given station,
    /// ordered by name.
    /// </summary>
    /// <returns>Read-only list of CTAStop objects</returns>
    ///
    public IReadOnlyList<CTAStop> GetStops(int stationID)
    {
      List<CTAStop> stops = new List<CTAStop>();

      try
      {
                string sql = string.Format(@"
                                    SELECT Name, StopID, StationID, Direction, ADA, Latitude, Longitude
                                    FROM Stops 
                                    WHERE StationID = {0} 
                                    ORDER BY Name ASC;"
                                    , stationID);
                DataSet result = dataTier.ExecuteNonScalarQuery(sql);
                foreach (DataRow row in result.Tables["table"].Rows)
                {
                    List<string> lineColors = GetLineColor(Convert.ToInt32(row["StopID"]));
                    stops.Add(new CTAStop(Convert.ToInt32(row["StopID"]), Convert.ToString(row["Name"]), Convert.ToInt32(row["StationID"]), 
                        Convert.ToString(row["Direction"]), Convert.ToBoolean(row["ADA"]), Convert.ToDouble(row["Latitude"]), Convert.ToDouble(row["Longitude"]), lineColors));
                }
            }
      catch (Exception ex)
      {
        string msg = string.Format("Error in Business.GetStops: '{0}'", ex.Message);
        throw new ApplicationException(msg);
      }

      return stops;
    }


    ///
    /// <summary>
    /// Returns the top N CTA Stations by ridership, 
    /// ordered by name.
    /// </summary>
    /// <returns>Read-only list of CTAStation objects</returns>
    /// 
    public IReadOnlyList<CTAStation> GetTopStations(int N)
    {
      if (N < 1)
        throw new ArgumentException("GetTopStations: N must be positive");

      List<CTAStation> stations = new List<CTAStation>();

      try
      {
                string sql = string.Format(@"
                                        SELECT TOP {0} Stations.StationID, Stations.Name FROM Stations
                                        INNER JOIN Riderships
                                        ON Stations.StationID = Riderships.StationID
                                        GROUP BY Stations.StationID, Stations.Name
                                        ORDER BY SUM(Riderships.DailyTotal) DESC, Stations.Name;
                                        ", N);
                var result = this.dataTier.ExecuteNonScalarQuery(sql);
                foreach (DataRow row in result.Tables[0].Rows)
                {
                    stations.Add(new CTAStation(Convert.ToInt32(row["stationID"]), Convert.ToString(row["Name"])));
                }


            }
      catch (Exception ex)
      {
        string msg = string.Format("Error in Business.GetTopStations: '{0}'", ex.Message);
        throw new ApplicationException(msg);
      }

      return stations;
    }
        

        //
        // Business:most code upto here given by prof.   

            ///Ridership object holds data for several output textbox
            /// <summary>
            /// Returns Ridership details for a station
            /// </summary>
            /// <returns>RidershipDetails Object
            /// /returns>
            /// 
            public RidershipDetails GetRidershipInfo(int stationID)
            {
                try
                {
                    int totalRidership = GetTotalRidership(stationID);
                    double averageRidership = GetAverageRidership(stationID);
                    double percentRidership = GetPercentRidership(stationID);
                    Console.WriteLine(percentRidership);
                    int weekdayRidership = GetWeekdayRidership(stationID);
                    int saturdayRidership = GetSaturdayRidership(stationID);
                    int sundayAndHolidayRidership = GetSundayAndHolidayRidership(stationID);
                    RidershipDetails riderInfo = new RidershipDetails(totalRidership, averageRidership, percentRidership,
                                                    weekdayRidership, saturdayRidership, sundayAndHolidayRidership);
                    return riderInfo;
                }
                catch (Exception ex)
                {
                    string msg = string.Format("Error in Business.GetRidershipInfo: '{0}'", ex.Message);
                    throw new ApplicationException(msg);
                }

            }

        ///Helper Function to GetRidershipInfo
        /// <summary>
        /// Returns the total ridership of given station
        /// </summary>
        /// <returns>Integer containing the total ridership of a station</returns>
        ///     
        private int GetTotalRidership(int stationID)
            {
                try
                {
                    string sql = string.Format(@"
                                        SELECT Sum(DailyTotal) 
                                        FROM Riderships
                                        WHERE Riderships.StationID = {0};
                                        ", stationID);
                    var result = Convert.ToInt32(this.dataTier.ExecuteScalarQuery(sql));
                    return result;
                }
                catch (Exception ex)
                {
                    string msg = string.Format("Error in Business.GetTotalRidership: '{0}'", ex.Message);
                    throw new ApplicationException(msg);
                }
            }

        ///Helper Function to GetRidershipInfo
        /// <summary>
        /// Returns the average ridership of given station
        /// </summary>
        /// <returns>Integer containing the average ridership of a station</returns>
        ///     
        private double GetAverageRidership(int stationID)
            {
                try
                {
                    string sql = string.Format(@"
                                        SELECT Avg(DailyTotal) 
                                        FROM Riderships
                                        WHERE Riderships.StationID = {0};
                                        ", stationID);
                    var result = Convert.ToDouble(this.dataTier.ExecuteScalarQuery(sql));
                    return result;
                }
                catch (Exception ex)
                {
                    string msg = string.Format("Error in Business.GetAverageRidership: '{0}'", ex.Message);
                    throw new ApplicationException(msg);
                }
            }

        ///Helper Function to GetRidershipInfo
        /// <summary>
        /// Returns the % ridership of given station
        /// </summary>
        /// <returns>Integer containing the % ridership of a station</returns>
        ///     
        private double GetPercentRidership(int stationID)
            {
                try
                {
                    string sql = string.Format(@"
                                        SELECT (sum2/sum1 * 100) AS percnt
                                        FROM
                                        (SELECT SUM(CAST((DailyTotal) AS FLOAT)) AS sum1 FROM Riderships) AS T1
                                        ,
                                        (SELECT SUM(DailyTotal) AS sum2 FROM Riderships
                                        WHERE Riderships.StationID = {0}) AS T2;
                                        ", stationID);
                    var result = Convert.ToDouble(this.dataTier.ExecuteScalarQuery(sql));
                    return result;
                }
                catch (Exception ex)
                {
                    string msg = string.Format("Error in Business.GetPercentRidership: '{0}'", ex.Message);
                    throw new ApplicationException(msg);
                }
            }

        ///Helper Function to GetRidershipInfo
        /// <summary>
        /// Returns the total ridership of a given station during weekdays
        /// </summary>
        /// <returns>Integer containing the total ridership of a station during weekdays</returns>
        ///     
        private int GetWeekdayRidership(int stationID)
            {
                try
                {
                    string sql = string.Format(@"
                                        SELECT SUM(DailyTotal) 
                                        FROM Riderships
                                        Where Riderships.StationID = {0}
                                        AND Riderships.TypeOfDay = 'W';
                                        ", stationID);
                    var result = Convert.ToInt32(this.dataTier.ExecuteScalarQuery(sql));
                    return result;
                }
                catch (Exception ex)
                {
                    string msg = string.Format("Error in Business.GetWeekdayRidership: '{0}'", ex.Message);
                    throw new ApplicationException(msg);
                }
            }

        ///Helper Function to GetRidershipInfo
        /// <summary>
        /// Returns the total ridership of a given station during Saturdays
        /// </summary>
        /// <returns>Integer containing the total ridership of a station during saturdays</returns>
        ///     
        private int GetSaturdayRidership(int stationID)
            {
                try
                {
                    string sql = string.Format(@"
                                        SELECT SUM(DailyTotal) 
                                        FROM Riderships
                                        WHERE Riderships.StationID = {0}
                                        AND Riderships.TypeOfDay = 'A';
                                        ", stationID);
                    var result = Convert.ToInt32(this.dataTier.ExecuteScalarQuery(sql));
                    return result;
                }
                catch (Exception ex)
                {
                    string msg = string.Format("Error in Business.GetSaturdayRidership: '{0}'", ex.Message);
                    throw new ApplicationException(msg);
                }
            }

        ///Helper Function to GetRidershipInfo
        /// <summary>
        /// Returns the total ridership of a given station during Sundays and holidays
        /// </summary>
        /// <returns>Integer containing the total ridership of a station during sundays and holidays</returns>
        ///     
        private int GetSundayAndHolidayRidership(int stationID)
            {
                try
                {
                    string sql = string.Format(@"
                                        SELECT SUM(DailyTotal) 
                                        FROM Riderships
                                        WHERE Riderships.StationID = {0}
                                        AND Riderships.TypeOfDay = 'U';
                                        ", stationID);
                    var result = Convert.ToInt32(this.dataTier.ExecuteScalarQuery(sql));
                    return result;
                }
                catch (Exception ex)
                {
                    string msg = string.Format("Error in Business.GetSundayAndHolidayRidership: '{0}'", ex.Message);
                    throw new ApplicationException(msg);
                }
            }

        ///Helper Function to GetRidershipInfo
        /// <summary>
        /// Returns the colors of lines for a stop
        /// </summary>
        /// <returns>List<string> containing the colors of lines for a stop</returns>
        ///     
        private List<string> GetLineColor(int stopID)
            {
                try
                {
                    string sql = string.Format(@"
                                        Select Lines.Color as Color from Lines
                                        Inner Join StopDetails
                                        On Lines.LineID = StopDetails.LineID
                                        Inner Join Stops
                                        On StopDetails.StopID = Stops.StopID
                                        and Stops.StopID = {0};
                                ", stopID);
                    var result = this.dataTier.ExecuteNonScalarQuery(sql);
                    List<string> lineColors = new List<string>();
                    foreach (DataRow dr in result.Tables[0].Rows)
                    {
                        lineColors.Add(Convert.ToString(dr["Color"]));
                    }
                    return lineColors;
                }
                catch (Exception ex)
                {
                    string msg = string.Format("Error in Business.GetLineColor: '{0}'", ex.Message);
                    throw new ApplicationException(msg);
                }
            }

}//class
}//namespace
