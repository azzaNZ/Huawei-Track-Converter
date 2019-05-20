using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
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

                line_overlay.Routes.Add(line_layer);
                gMapControl1.Overlays.Add(line_overlay);

                //simply add the points you want

                foreach (HuaweiDatumPoint point in hp.Data)
                {
                    if (point.HasPosition)
                    {
                        line_layer.Points.Add(new PointLatLng(point.latitude, point.longitude));
                    }
                }
                //line_layer.Points.Add(new PointLatLng(lat, lon));
                //line_layer.Points.Add(new PointLatLng(lat2, lon2));

                //Note that if you are using the MouseEventArgs you need to use local coordinates and convert them:
                //line_layer.Points.Add(gMapControl1.FromLocalToLatLng(e.X, e.Y));

                //To force the draw, you need to update the route
                gMapControl1.UpdateRouteLocalPosition(line_layer);

                //zoom to route
                gMapControl1.ZoomAndCenterRoute(line_layer);
                //you can even add markers at the end of the lines by adding markers to the same layer:
                //GMapMarker marker_ = new GMarkerCross(p);
                //line_overlay.Markers.Add(marker_);

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
                    string path = $@"{folder}\{listBoxFiles.SelectedItems[0]}";
                    HuaweiParser hp = new HuaweiParser(path);
                    lblDistance.Text = $"Distance: {Convert.ToInt32(hp.TotalDistance).ToString()}m";
                    lblAscent.Text = $"Ascent: {Convert.ToInt32(hp.Ascent).ToString()}m";
                    lblDescent.Text = $"Descent: {Convert.ToInt32(hp.Descent).ToString()}m";
                    LoadHuaweiDataOntoMap(hp);
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
                MessageBox.Show(exception.Message);
            }
        }
    }
}
