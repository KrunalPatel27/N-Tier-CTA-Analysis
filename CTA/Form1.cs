//
// // N-tier C# and SQL program to analyze CTA Ridership data. 
// 
// <<Krunal Patel>> 
// U. of Illinois, Chicago 
// CS341, Fall 2016 
// Homework 7 
// 

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;

namespace CTA
{

  public partial class Form1 : Form
  {
    private string BuildConnectionString()
    {
      string version = "MSSQLLocalDB";
      string filename = this.txtDatabaseFilename.Text;

      string connectionInfo = String.Format(@"Data Source=(LocalDB)\{0};AttachDbFilename={1};Integrated Security=True;", version, filename);

      return connectionInfo;
    }

    public Form1()
    {
      InitializeComponent();
    }

    private void Form1_Load(object sender, EventArgs e)
    {
      //
      // setup GUI:
      //
      this.lstStations.Items.Add("");
      this.lstStations.Items.Add("[ Use File>>Load to display L stations... ]");
      this.lstStations.Items.Add("");

      this.lstStations.ClearSelected();

      toolStripStatusLabel1.Text = string.Format("Number of stations:  0");

      // 
      // open-close connect to get SQL Server started:
      //
      SqlConnection db = null;

      try
      {
        db = new SqlConnection(BuildConnectionString());
        db.Open();
      }
      catch
      {
        //
        // ignore any exception that occurs, goal is just to startup
        //
      }
      finally
      {
        // close connection:
        if (db != null && db.State == ConnectionState.Open)
          db.Close();
      }
    }


    //
    // File>>Exit:
    //
    private void exitToolStripMenuItem1_Click(object sender, EventArgs e)
    {
      this.Close();
    }


    //
    // File>>Load Stations:
    //
    private void toolStripMenuItem2_Click(object sender, EventArgs e)
    {
      //
      // clear the UI of any current results:
      //
      ClearStationUI(true /*clear stations*/);

            //
            // now load the stations from the database:
            //
            try
            {
                BusinessTier.Business bizTier;

                bizTier = new BusinessTier.Business(this.txtDatabaseFilename.Text);

                var stations = bizTier.GetStations();

                foreach (var station in stations)  // display stations:         
                {
                    this.lstStations.Items.Add(station.Name);
                }
            }
            catch (Exception ex)
            {
                string msg = string.Format("Error: '{0}'.", ex.Message);
                MessageBox.Show(msg);
            }
        }


    //
    // User has clicked on a station for more info:
    //
    private void lstStations_SelectedIndexChanged(object sender, EventArgs e)
    {
      // sometimes this event fires, but nothing is selected...
      if (this.lstStations.SelectedIndex < 0)   // so return now in this case:
        return; 
      
      //
      // clear GUI in case this fails:
      //
      ClearStationUI();
                BusinessTier.Business bizTier;
                bizTier = new BusinessTier.Business(this.txtDatabaseFilename.Text);
                var stations = bizTier.GetStations();
                var stationName = this.lstStations.SelectedItem.ToString();
                stationName.Replace("'", "''");
            try
            {
              
                foreach (var station in stations)  // display stations:         
                {
                    if (stationName.Equals(station.Name))
                    {
                        txtStationID.AppendText(string.Format("{0:n0}", station.ID));
                        var getStops = bizTier.GetStops(station.ID);
                        foreach (var stops in getStops)
                        {
                            this.lstStops.Items.Add(stops.Name);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                string msg = string.Format("Error: '{0}'.", ex.Message);
                MessageBox.Show(msg);
            }

            

            // Get the ridership info for a selected station
            try
            {
                int stationID = Convert.ToInt32(txtStationID.Text.Replace(",", ""));
                var ridershipInfo = bizTier.GetRidershipInfo(stationID);
                txtTotalRidership.AppendText(string.Format("{0:n0}", ridershipInfo.TotalRidership));
                txtAvgDailyRidership.AppendText(string.Format("{0:n0}/day", ridershipInfo.AvgRidership));
                txtPercentRidership.AppendText(string.Format("{0:0.00}%", ridershipInfo.PercentRidership));
                txtWeekdayRidership.AppendText(string.Format("{0:n0}", ridershipInfo.WeekdayRidership));
                txtSaturdayRidership.AppendText(string.Format("{0:n0}", ridershipInfo.SaturdayRidership));
                txtSundayHolidayRidership.AppendText(string.Format("{0:n0}", ridershipInfo.SundayAndHolidayRidership));
            }
            catch
            {

            }


           
        }

    private void ClearStationUI(bool clearStatations = false)
    {
      ClearStopUI();

      this.txtTotalRidership.Clear();
      this.txtTotalRidership.Refresh();

      this.txtAvgDailyRidership.Clear();
      this.txtAvgDailyRidership.Refresh();

      this.txtPercentRidership.Clear();
      this.txtPercentRidership.Refresh();

      this.txtStationID.Clear();
      this.txtStationID.Refresh();

      this.txtWeekdayRidership.Clear();
      this.txtWeekdayRidership.Refresh();
      this.txtSaturdayRidership.Clear();
      this.txtSaturdayRidership.Refresh();
      this.txtSundayHolidayRidership.Clear();
      this.txtSundayHolidayRidership.Refresh();

      this.lstStops.Items.Clear();
      this.lstStops.Refresh();

      if (clearStatations)
      {
        this.lstStations.Items.Clear();
        this.lstStations.Refresh();
      }
    }


    //
    // user has clicked on a stop for more info:
    //
    private void lstStops_SelectedIndexChanged(object sender, EventArgs e)
    {
      // sometimes this event fires, but nothing is selected...
      if (this.lstStops.SelectedIndex < 0)   // so return now in this case:
        return; 

      //
      // clear GUI in case this fails:
      //
      ClearStopUI();

            try
            {
                BusinessTier.Business bizTier;

                bizTier = new BusinessTier.Business(this.txtDatabaseFilename.Text);

                var stations = bizTier.GetStations();
                var stationName = this.lstStations.SelectedItem.ToString();
                stationName.Replace("'", "''");
                foreach (var station in stations)  // display stations:         
                {
                    if (stationName.Equals(station.Name))
                    {
                        var getStops = bizTier.GetStops(station.ID);
                        var selectedStop = this.lstStops.SelectedItem.ToString();
                        foreach (var stops in getStops)
                        {
                            if (selectedStop.Equals(stops.Name))
                            {
                                // handicap accessible?
                                bool accessible = stops.ADA;

                                if (accessible)
                                    this.txtAccessible.Text = "Yes";
                                else
                                    this.txtAccessible.Text = "No";
                                // direction of travel:
                                this.txtDirection.Text = stops.Direction.ToString();
                                //location of Stop
                                string msg1 = string.Format("({0:00.0000},{1:00.0000})", stops.Latitude.ToString(), stops.Longitude.ToString());
                                this.txtLocation.Text = msg1;
                                //all of the Line color
                                var lineColors = stops.LineColor;
                                foreach (var lineColor in lineColors)
                                {
                                    lstLines.Items.Add(lineColor);
                                }


                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                string msg = string.Format("Error: '{0}'.", ex.Message);
                MessageBox.Show(msg);
            }
   
        }

        private void ClearStopUI()
    {
      this.txtAccessible.Clear();
      this.txtAccessible.Refresh();

      this.txtDirection.Clear();
      this.txtDirection.Refresh();

      this.txtLocation.Clear();
      this.txtLocation.Refresh();

      this.lstLines.Items.Clear();
      this.lstLines.Refresh();
    }


    //
    // Top-10 stations in terms of ridership:
    //
    private void top10StationsByRidershipToolStripMenuItem_Click(object sender, EventArgs e)
    {
      //
      // clear the UI of any current results:
      //
      ClearStationUI(true /*clear stations*/);

            //
            // now load top-10 stations:
            //
            try
            {
                BusinessTier.Business bizTier;

                bizTier = new BusinessTier.Business(this.txtDatabaseFilename.Text);

                var stations = bizTier.GetTopStations(10);

                foreach (var station in stations)  // display stations:         
                {
                    this.lstStations.Items.Add(station.Name);
                }
            }
            catch (Exception ex)
            {
                string msg = string.Format("Error: '{0}'.", ex.Message);
                MessageBox.Show(msg);
            }
        }

        private void top10ToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }
    }//class
}//namespace
