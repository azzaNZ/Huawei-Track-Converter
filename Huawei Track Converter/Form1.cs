using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Device.Location;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using GMap.NET;
using GMap.NET.MapProviders;
using GMap.NET.WindowsForms;
using GMap.NET.WindowsForms.Markers;

namespace Huawei_Track_Converter
{
    public partial class Form1 : Form
    {
        private string folder = "";
        private HuaweiParser hp = null;         

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            //load up starting folder for development purposes
            if (Debugger.IsAttached)
            {
                folder = @"C:\temp\examples";
                LoadFolder(folder);
            }

            //basic map config
            try
            {
                gMapControl1.MapProvider = GMapProviders.GoogleMap;
                gMapControl1.Position = new PointLatLng(-43.1961334816182, 172);
                gMapControl1.MinZoom = 0;
                gMapControl1.MaxZoom = 24;
                gMapControl1.Zoom = 9;
            }
            catch
            { }
        }

        /// <summary>
        /// Go get files for provided pathx`
        /// </summary>
        /// <param name="path"></param>
        private void LoadFolder(string path)
        {
            try
            {
                listBoxFiles.Items.Clear();
                listBoxFiles.BeginUpdate();

                System.IO.DirectoryInfo dir = new System.IO.DirectoryInfo(path);

                // For each file in the c:\ directory, create a ListViewItem
                // and set the icon to the icon extracted from the file.
                foreach (System.IO.FileInfo file in dir.GetFiles())
                {
                    listBoxFiles.Items.Add(file.Name);
                }
                listBoxFiles.EndUpdate();
            }
            catch
            { }

        }

        private void button1_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog folderPicker = new FolderBrowserDialog();
            if (folderPicker.ShowDialog() == DialogResult.OK)
            {
                folder = folderPicker.SelectedPath;
                LoadFolder(folderPicker.SelectedPath);

            }
        }

        private void LoadHuaweiDataOntoMap(HuaweiParser hp)
        {
            try
            {
                //create layer to store route
                GMapRoute line_layer = new GMapRoute("single_line") { Stroke = new Pen(Brushes.Blue, 4) };
                GMapOverlay line_overlay = new GMapOverlay("route");

                gMapControl1.Overlays.Clear();              //remove any previous routes on map
                line_overlay.Routes.Add(line_layer);
                gMapControl1.Overlays.Add(line_overlay);

                //add route points
                foreach (HuaweiDatumPoint point in hp.Data)
                {
                    //start of section ignored, as track sometimes ends with 'rogue' start of section
                    if (point.HasPosition  && !point.startOfSection)
                    {
                        line_layer.Points.Add(new PointLatLng(point.latitude, point.longitude));
                    }
                }

                //To force the draw, you need to update the route
                gMapControl1.UpdateRouteLocalPosition(line_layer);

                //zoom to route
                gMapControl1.ZoomAndCenterRoute(line_layer);

                //mark start pos
                GeoCoordinate startPos = hp.Data.Where(x => x.HasPosition).FirstOrDefault().position;
                PointLatLng startMarker = new PointLatLng(startPos.Latitude,startPos.Longitude);
                GMapMarker marker = new GMarkerGoogle(startMarker, GMarkerGoogleType.green_big_go);
                line_overlay.Markers.Add(marker);

                //mark end pos
                GeoCoordinate endPos = hp.Data.Where(x => x.HasPosition && !x.startOfSection).LastOrDefault().position;
                PointLatLng endMarker = new PointLatLng(endPos.Latitude, endPos.Longitude);
                marker = new GMarkerGoogle(endMarker, GMarkerGoogleType.red_big_stop);
                line_overlay.Markers.Add(marker);

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                if (listBoxFiles.SelectedItems.Count > 0)
                {
                    //get path of huawei file to process
                    string path = $@"{folder}\{listBoxFiles.SelectedItems[0]}";
                    //go do work
                    hp = new HuaweiParser(path);

                    //interpret results
                    lblDistance.Text = $@"Distance: {Convert.ToInt32(hp.TotalDistance).ToString()}m";
                    lblAscent.Text = $@"Ascent: {Convert.ToInt32(hp.Ascent).ToString()}m";
                    lblDescent.Text = $@"Descent: {Convert.ToInt32(hp.Descent).ToString()}m";

                    //duration
                    TimeSpan time = TimeSpan.FromSeconds(hp.Duration);
                    lblDuration.Text = $"Duration: {time:hh\\:mm\\:ss}";

                    //show track on map
                    LoadHuaweiDataOntoMap(hp);
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
                MessageBox.Show(exception.Message);
            }
        }

        private void btnExport_Click(object sender, EventArgs e)
        {
            try
            {
                string exportPath = $@"{folder}\{listBoxFiles.Text}";

                if (listBoxFiles.Text == "")
                    throw new Exception("Please select file to export");

                if (hp == null || hp.Data.Count == 0)
                    throw new Exception("No data to export");

                if (listBoxActivity.Visible && listBoxActivity.Text == "")
                    throw new Exception("Please select Activity type");

                switch (listBoxExportFormat.Text)
                {
                    case "GPX":
                        exportPath += ".gpx";
                        hp.ExportToGPX(exportPath, listBoxFiles.Text);
                        MessageBox.Show($@"Exported to {exportPath}");
                        break;
                    case "TCX":
                        exportPath += ".tcx";
                        hp.ExportToTCX(exportPath, listBoxActivity.Text);
                        MessageBox.Show($@"Exported to {exportPath}");
                        break;
                    default:
                        throw new Exception ("Please select format to export to");
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Could not export", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
        }

        private void listBoxExportFormat_SelectedIndexChanged(object sender, EventArgs e)
        {
            listBoxActivity.Visible = listBoxExportFormat.Text == "TCX";
            lblActivityType.Visible = listBoxActivity.Visible;
        }
    }
}
