using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using GMap.NET.WindowsForms;
using GMap.NET;
using GMap.NET.MapProviders;
using GMap.NET.WindowsForms.Markers;
using GMap.NET.WindowsForms.ToolTips;
using System.Data.SqlClient;
using System.Data.Sql;

namespace client_yeni
{
    public partial class Form1 : Form
    {
        com.basaktelemetri.Service1 servis = new com.basaktelemetri.Service1(); 

        public GMapOverlay overlayOne;
        public RoutingProvider rp;

        public GMapMarker[] m = new GMapMarker[1];
        List<Track_LocationInf_new> list_local;
        
        public double longitude = 0;
        public double latitude = 0;
        bool sim = false;
        public string zaman;
        public bool sim_icin_flag = false;
        public bool flga_for_single = true;
        public bool tekli_sorgu = false;

        public double speed;
        public string direction, yon = "sabit";
        public string durum, drm;
        public string kontak, kntk;
        public string[] plakalar_ = new string[0];
        public double direct;
        public string port_num = "";
        public double dist = 0;
        public int zoomValue = 9;

        bool paralel_simi_flag = false;
        bool paralel_sorgu_flag = false;

        public bool draging;
        public int mouseX, mouseY;

        public PointLatLng points;
        public List<PointLatLng> lokasyonlar = new List<PointLatLng>();        

        public AracTakip_YakinDataContext data_yakin = new AracTakip_YakinDataContext();

        public Form1()
        {
            CheckForIllegalCrossThreadCalls = false;
            InitializeComponent();           
        }

        public void map_initialize()
        {
            overlayOne.Markers.Clear();
            mapexplr.Overlays.Add(overlayOne);
        }

        public void refresh()
        {
            comboBox1.Items.Clear();
            int size = 0;
            Array.Resize(ref plakalar_, 0);
            var plakalars = servis.plakalar(port_num);
            foreach (var plakalar in plakalars)
            {
                comboBox1.Items.Add(plakalar.Plaka);
                Array.Resize(ref plakalar_, plakalar_.Length + 1);
                Array.Resize(ref m, m.Length + 1);
                plakalar_[size] = plakalar.Plaka.Trim();
                size = size + 1;
            }
        }

        public void loginsorgu()
        {
            try
            {
                string loginn = servis.login(textBox1.Text, textBox2.Text, textBox3.Text);
                if (loginn != "hata")
                {
                    port_num = loginn;
                    panel1.Visible = false;
                    sim = true;
                    izlemeToolStripMenuItem.Enabled = true;
                    araçlarToolStripMenuItem.Enabled = true;
                    zoomToolStripMenuItem.Enabled = true;
                    haritalarToolStripMenuItem.Enabled = true;
                    refresh();
                }
                else
                {
                    label4.Visible = true;
                    sim = false;
                    label4.Text = "Hatalı Giriş";
                }
            }
            catch { }
        }

        public void sorgu(int length)// veri tabanından araç ile bilgilerin çekildiği sorgu
        {
             try
            {
                overlayOne.Markers.Clear();
                for (int i = 0; i < length; i++)
                {
                    var plakalarss = servis.sorgu(plakalar_[i]);

                    if (plakalarss[0] != "" && plakalarss[0] != "0000.000")
                    {
                        latitude = Convert.ToDouble(plakalarss[0]);
                        longitude = Convert.ToDouble(plakalarss[1]);

                        drm = plakalarss[3];
                        kontak = plakalarss[4];
                        yon = plakalarss[5];
                        zaman = plakalarss[6];
                        kntk = plakalarss[4];

                        if (drm == "normal")
                        {
                            m[i] = new GMap.NET.WindowsForms.Markers.GMapMarkerGoogleGreen(new PointLatLng(latitude, longitude));
                        }
                        if (drm == "acil")
                        {
                            m[i] = new GMap.NET.WindowsForms.Markers.GMapMarkerGoogleRed(new PointLatLng(latitude, longitude));
                        }

                        m[i].ToolTipMode = MarkerTooltipMode.Always;
                        if (flga_for_single)
                        {
                            m[i].ToolTipText = "Plaka: " + plakalar_[i] + "\nHIZ: " + speed.ToString() + " Kmh" + "\nYön: " + yon + "\nDurum: " + drm + "\nKontak: " + kntk + "\nZaman: " + zaman;
                            mapexplr.Position = m[i].Position;
                        }
                        else
                        {
                            m[i].ToolTipText = "Plaka: " + plakalar_[i] + "\nZaman: " + zaman;
                        }
                        overlayOne.Markers.Add(m[i]);
                    }
                }            
            }
             catch{ MessageBox.Show("Hata oluştu, lütfen tekrar deneyiniz!");}
        }
        public void nerelerdeydim(double Lat, double Lon, double direct, string plakam, DateTime zaman, double speed, string durum,string kontakN)
        {
            string yonN = "Sabit", drmN = "", kntkN = "";
            try
            {
                overlayOne.Markers.Clear();

                if (direct > 67 && direct < 112)
                {
                    yonN = "Doğu";
                }
                if (direct > 247 && direct < 293)//OK
                {
                    yonN = "Batı";
                }
                if (direct > 337 && direct < 21)
                {
                    yonN = "Kuzey";
                }
                if (direct > 157 && direct < 202)
                {
                    yonN = "Güney";
                }
                if (direct > 23 && direct < 67)
                {
                    yonN = "Kuzey-Doğu";
                }
                if (direct > 293 && direct < 337)
                {
                    yonN = "Kuzey-Batı";
                }
                if (direct > 112 && direct < 157)
                {
                    yonN = "Güney-Doğu";
                }
                if (direct > 202 && direct < 247)
                {
                    yonN = "Güney-Batı";
                }

                if (durum == "0")
                {
                    drmN = "Normal";
                }
                if (durum == "1")
                {
                    drmN = "ACİL DURUM!";
                }
                if (kontakN == "1")
                {
                    kntkN = "Açık";
                }
                if (kontakN == "0")
                {
                    kntkN = "Kapalı";
                }

                if (durum == "0")
                {
                    m[0] = new GMap.NET.WindowsForms.Markers.GMapMarkerGoogleGreen(new PointLatLng(Lat, Lon));
                }
                if (durum == "1")
                {
                    m[0] = new GMap.NET.WindowsForms.Markers.GMapMarkerGoogleRed(new PointLatLng(Lat, Lon));
                }

                m[0].ToolTipMode = MarkerTooltipMode.Always;

                m[0].ToolTipText = "Plaka: " + plakam /*+ "\nlat:" + latitude.ToString() + "\nlon:" + longitude.ToString()*/ + "\nHIZ: " + speed.ToString() + " Kmh" + "\nYön: " + yonN + "\nDurum: " + drmN + "\nKontak: " + kntkN + "\nZaman: " + zaman;
                mapexplr.Position = m[0].Position;
                mapexplr.Zoom = 13;

                overlayOne.Markers.Add(m[0]);
                mapexplr.Overlays.Add(overlayOne);
            }
            catch { }
        }

        
        private void Form1_Load(object sender, EventArgs e)
        {
            
            try
            {
                #if !MONO   // mono doesn't handle it, so we 'lost' provider list ;]
                    comboBox2.ValueMember = "Name";
                    comboBox2.DataSource = GMapProviders.List;
                    comboBox2.SelectedItem = mapexplr.MapProvider;
                #endif

                    

                mapexplr.MinZoom = 3;
                mapexplr.MaxZoom = 17;
                mapexplr.Zoom = zoomValue;
                mapexplr.Manager.Mode = AccessMode.ServerAndCache;

                overlayOne = new GMapOverlay(mapexplr, "OverlayOne");

                mapexplr.DragButton = MouseButtons.Left;
                mapexplr.IgnoreMarkerOnMouseWheel = true;

                mapexplr.MapProvider = comboBox2.Items[1] as GMapProvider;
                comboBox2.Text = comboBox2.Items[1].ToString();

                map_initialize();
                dateTimePicker2.Value = DateTime.Now;
                dateTimePicker1.Value = DateTime.Now;
            }
            catch { }
        }

        private void mapexplr_Load(object sender, EventArgs e)
        {
            System.Threading.Thread.Sleep(1);
        }
              
        private void sorguSayfasıToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (sim)
            {
                timer1.Enabled = false;
                refresh();
                Form2 For2 = new Form2();
                For2.Owner = this;
                For2.Show();
            }
            else
                MessageBox.Show("Lütfen Giriş Yapınız");
        }

        private void button1_Click(object sender, EventArgs e)
        {

        }

        private void button1_Click_1(object sender, EventArgs e)
        {            
            
        }

        private void comboBox2_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                draging = true;
                mouseX = -e.X;
                mouseY = -e.Y;
                int clipleft = PointToClient(MousePosition).X - comboBox2.Location.X;
                int cliptop = PointToClient(MousePosition).Y - comboBox2.Location.Y;
                int clipwidth = ClientSize.Width - (comboBox2.Width - clipleft);
                int clipheight = ClientSize.Height - (comboBox2.Height - cliptop);
                Cursor.Clip = RectangleToScreen(new Rectangle(clipleft, cliptop, clipwidth, clipheight));
                comboBox2.Invalidate();
            }
        }

        private void comboBox2_MouseMove(object sender, MouseEventArgs e)
        {
            if (draging)
            {
                Point MPosition = new Point();
                MPosition = PointToClient(MousePosition);
                MPosition.Offset(mouseX, mouseY);
                comboBox2.Location = MPosition;
            }
        }

        private void comboBox2_MouseUp(object sender, MouseEventArgs e)
        {
            if (draging)
            {
                draging = false;
                Capture = false;
                Cursor.Clip = new Rectangle(this.Location, this.Size);
                comboBox2.Invalidate();
            }
        }

        private void comboBox1_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                draging = true;
                mouseX = -e.X;
                mouseY = -e.Y;
                int clipleft = PointToClient(MousePosition).X - comboBox1.Location.X;
                int cliptop = PointToClient(MousePosition).Y - comboBox1.Location.Y;
                int clipwidth = ClientSize.Width - (comboBox1.Width - clipleft);
                int clipheight = ClientSize.Height - (comboBox1.Height - cliptop);
                Cursor.Clip = RectangleToScreen(new Rectangle(clipleft, cliptop, clipwidth, clipheight));
                comboBox1.Invalidate();
            }
        }

        private void comboBox1_MouseMove(object sender, MouseEventArgs e)
        {
            if (draging)
            {
                Point MPosition = new Point();
                MPosition = PointToClient(MousePosition);
                MPosition.Offset(mouseX, mouseY);
                comboBox1.Location = MPosition;
            }
        }

        private void comboBox1_MouseUp(object sender, MouseEventArgs e)
        {
            if (draging)
            {
                draging = false;
                Capture = false;
                Cursor.Clip = new Rectangle(this.Location, this.Size);
                comboBox1.Invalidate();
            }
        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                overlayOne.Markers.Clear();
                mapexplr.MapProvider = comboBox2.SelectedItem as GMapProvider;
                map_initialize();
            }
            catch { }
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            lokasyonlar.Clear();
            overlayOne.Markers.Clear();
            overlayOne.Routes.Clear();            
            timer1.Enabled = true;
            sim_icin_flag = true;
            flga_for_single = true;
            tekli_sorgu = true;

            plakalar_[0] = comboBox1.SelectedItem.ToString();
            plakalar_[1] = "\0";        
        }

        private void comboBox1_SelectedValueChanged(object sender, EventArgs e)
        {
            
        }

        private void comboBox1_Click(object sender, EventArgs e)
        {

        }

        private void similasyonToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void similasyonToolStripMenuItem_Click_1(object sender, EventArgs e)
        {
            if (sim && panel2.Visible == false)
            {
                panel2.Visible = true;
                similasyonToolStripMenuItem.Checked = true;
            }
            else if (sim && panel2.Visible == true)
            {
                panel2.Visible = false;
                similasyonToolStripMenuItem.Checked = false;
            }
            else
            {
                MessageBox.Show("Lütfen giriş yapınız.");
                panel2.Visible = false;
                similasyonToolStripMenuItem.Checked = false;
            }
        }

        private void panel2_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                draging = true;
                mouseX = -e.X;
                mouseY = -e.Y;
                int clipleft = PointToClient(MousePosition).X - panel2.Location.X;
                int cliptop = PointToClient(MousePosition).Y - panel2.Location.Y;
                int clipwidth = ClientSize.Width - (panel2.Width - clipleft);
                int clipheight = ClientSize.Height - (panel2.Height - cliptop);
                Cursor.Clip = RectangleToScreen(new Rectangle(clipleft, cliptop, clipwidth, clipheight));
                panel2.Invalidate();
            }
        }

        private void panel2_MouseMove(object sender, MouseEventArgs e)
        {
            if (draging)
            {
                Point MPosition = new Point();
                MPosition = PointToClient(MousePosition);
                MPosition.Offset(mouseX, mouseY);
                panel2.Location = MPosition;
            }
        }

        private void panel2_MouseUp(object sender, MouseEventArgs e)
        {
            if (draging)
            {
                draging = false;
                Capture = false;
                Cursor.Clip = new Rectangle(this.Location, this.Size);
                panel2.Invalidate();
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            paralel_simi_flag = true;
            timer1.Enabled = false;
            button2.Enabled = false;
            panel5.Visible = true;
            backgroundWorker1.RunWorkerAsync();
            //pre_sim();
        }

        public void pre_sim()
        {
            if (sim_icin_flag)
            {
                if (comboBox3.Text != "Çözünürlük")
                {
                    overlayOne.Routes.Clear();
                    MapRoute route;
                    similasyon();
                    rp = mapexplr.MapProvider as RoutingProvider;

                    if (rp == null)
                    {
                        rp = GMapProviders.GoogleMap; // use google if provider does notimplement routing
                    }

                    for (int i = 0; i < lokasyonlar.Count - 1; i++)
                    {
                        double per = ((Convert.ToDouble(i) / Convert.ToDouble(lokasyonlar.Count)) * 100);
                        backgroundWorker1.ReportProgress(Convert.ToInt32(per));

                        route = rp.GetRouteBetweenPoints(lokasyonlar[i], lokasyonlar[i + 1], false, false, 7);
                        if (route != null && route.Distance != 0 )//&& route.Distance < 0.65)
                        {
                            // add route
                            GMapRoute r = new GMapRoute(route.Points, route.Name);
                            overlayOne.Routes.Add(r);
                        }
                    }
                    mapexplr.Overlays.Add(overlayOne);
                    lokasyonlar.Clear();
                    overlayOne.Markers.Clear();
                }
                else
                {
                    MessageBox.Show("Lütfen çözünürlük değerini belirleyiniz.");
                    button2.Enabled = true;
                }
            }
            else
            {
                MessageBox.Show("Lütfen GÖRÜNÜM > ARAÇLAR menüsünden araç seçiniz");
                button2.Enabled = true;
            }
        }

        public void similasyon()
        {
            int ind = 0;
            int num = 0;
            try
            {
                lokasyonlar.Clear();
                mapexplr.Overlays.Clear();
                string selceted = (comboBox1.Items[comboBox1.SelectedIndex].ToString().Trim());
                if (!checkBox1.Checked)
                {
                    var plakalars = servis.smilasyon_atlamali(selceted, dateTimePicker2.Value, dateTimePicker1.Value, Convert.ToInt32(comboBox3.SelectedItem));
                    
                    num = plakalars.Length;
                    if (plakalars.Count() != 0)
                    {
                        for (ind = 0; ind < num; ind = ind + 1)
                        {
                            int point_ind_lat = plakalars[ind].Latitude.IndexOf(".");

                            if (point_ind_lat != -1)
                            {
                                string lat_pos_W = plakalars[ind].Latitude.Substring(point_ind_lat - 2);
                                lat_pos_W = lat_pos_W.Replace(".", ",");
                                double lat_pos = Convert.ToDouble(lat_pos_W) / 60;
                                double lat_pos_2 = Convert.ToDouble(plakalars[ind].Latitude.Substring(0, point_ind_lat - 2));
                                latitude = lat_pos_2 + lat_pos;
                            }

                            int point_ind_lon = plakalars[ind].Longitude.IndexOf(".");
                            if (point_ind_lon != -1)
                            {
                                string lon_pos_W = plakalars[ind].Longitude.Substring(point_ind_lon - 2);
                                lon_pos_W = lon_pos_W.Replace(".", ",");
                                double lon_pos = Convert.ToDouble(lon_pos_W) / 60;
                                double lon_pos_2 = Convert.ToDouble(plakalars[ind].Longitude.Substring(0, point_ind_lon - 2));
                                longitude = lon_pos_2 + lon_pos;
                            }
                            if (point_ind_lon != -1 && point_ind_lat != -1 && Convert.ToDouble(plakalars[ind].Speed) >= 5)
                            {
                                points.Lat = latitude;
                                points.Lng = longitude;
                                lokasyonlar.Add(points);
                            }
                        }
                    }

                    else
                    {
                        MessageBox.Show(dateTimePicker1.Value.ToString().Trim() + "-" + dateTimePicker2.Value.ToString().Trim() + " tarihleri arasında " + comboBox1.SelectedItem.ToString().Trim() + " plakalı araç için kayıt bulunamadı");
                        button2.Enabled = true;
                    }
                }

                else
                {
                    var plakalars = (from a in data_yakin.Track_LocationInf_news
                                     where a.Plaka == selceted && a.DateTime <= dateTimePicker2.Value && a.DateTime >= dateTimePicker1.Value
                                     select a);
                    list_local = plakalars.ToList();


                    num = list_local.Count;
                    if (plakalars.Count() != 0)
                    {
                        foreach (var plakalar in plakalars)
                        {
                            ind++;
                            int point_ind_lat = plakalar.Latitude.IndexOf(".");
                            if (point_ind_lat != -1)
                            {
                                string lat_pos_W = plakalar.Latitude.Substring(point_ind_lat - 2);
                                lat_pos_W = lat_pos_W.Replace(".", ",");
                                double lat_pos = Convert.ToDouble(lat_pos_W) / 60;
                                double lat_pos_2 = Convert.ToDouble(plakalar.Latitude.Substring(0, point_ind_lat - 2));
                                latitude = lat_pos_2 + lat_pos;
                            }

                            int point_ind_lon = plakalar.Longitude.IndexOf(".");
                            if (point_ind_lon != -1)
                            {
                                string lon_pos_W = plakalar.Longitude.Substring(point_ind_lon - 2);
                                lon_pos_W = lon_pos_W.Replace(".", ",");
                                double lon_pos = Convert.ToDouble(lon_pos_W) / 60;
                                double lon_pos_2 = Convert.ToDouble(plakalar.Longitude.Substring(0, point_ind_lon - 2));
                                longitude = lon_pos_2 + lon_pos;
                            }
                            if (point_ind_lon != -1 && point_ind_lat != -1 && Convert.ToDouble(plakalar.Speed) >= 5)
                            {
                                points.Lat = latitude;
                                points.Lng = longitude;
                                lokasyonlar.Add(points);
                            }
                        }
                    }
                    else
                    {
                        MessageBox.Show(dateTimePicker1.Value.ToString().Trim() + "-" + dateTimePicker2.Value.ToString().Trim() + " tarihleri arasında " + comboBox1.SelectedItem.ToString().Trim() + " plakalı araç için kayıt bulunamadı");
                        button2.Enabled = true;
                    }
                }
            }
            catch 
            { 
                MessageBox.Show("veriler çekilirken internet bağlantısından kaynaklanan hata oluştu. Lütfen tekrar deneyiniz.");
                button2.Enabled = true;
            }
            finally { }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            try
            {
                paralel_sorgu_flag = true;
                backgroundWorker1.RunWorkerAsync();
            }
            catch {}           
        } 

        private void zoomToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (panel4.Visible == true)
            {
                panel4.Visible = false;
            }
            else
            {
                panel4.Visible = true;
            }
        }

        private void panel3_MouseDown(object sender, MouseEventArgs e)
        {
            
        }

        private void panel3_MouseMove(object sender, MouseEventArgs e)
        {
            
        }

        private void panel3_MouseUp(object sender, MouseEventArgs e)
        {
           
        }

        private void araçlarToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (comboBox1.Visible == true)
                comboBox1.Visible = false;
            else
                comboBox1.Visible = true;
        }

        private void haritalarToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (comboBox2.Visible == true)
                comboBox2.Visible = false;
            else
                comboBox2.Visible = true;
        }

        private void mapexplr_Scroll(object sender, ScrollEventArgs e)
        {
            System.Threading.Thread.Sleep(20);
        }

        private void mapexplr_OnMapTypeChanged(GMapProvider type)
        {
            try
            {
                overlayOne.Markers.Clear();
                GC.SuppressFinalize(mapexplr);
                GC.SuppressFinalize(overlayOne);
            }
            catch { }
        }

        private void mapexplr_OnMapZoomChanged()
        {
            try
            {
                zoomValue = Convert.ToInt32(mapexplr.Zoom);
            }
            catch { }
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            
        }

        private void çıkışToolStripMenuItem_Click(object sender, EventArgs e)
        {
            System.Environment.Exit(1);
        }

        private void button3_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                draging = true;
                mouseX = -e.X;
                mouseY = -e.Y;
                int clipleft = PointToClient(MousePosition).X - panel4.Location.X;
                int cliptop = PointToClient(MousePosition).Y - panel4.Location.Y;
                int clipwidth = ClientSize.Width - (panel4.Width - clipleft);
                int clipheight = ClientSize.Height - (panel4.Height - cliptop);
                Cursor.Clip = RectangleToScreen(new Rectangle(clipleft, cliptop, clipwidth, clipheight));
                panel4.Invalidate();
            }
        }

        private void button4_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                draging = true;
                mouseX = -e.X;
                mouseY = -e.Y;
                int clipleft = PointToClient(MousePosition).X - panel4.Location.X;
                int cliptop = PointToClient(MousePosition).Y - panel4.Location.Y;
                int clipwidth = ClientSize.Width - (panel4.Width - clipleft);
                int clipheight = ClientSize.Height - (panel4.Height - cliptop);
                Cursor.Clip = RectangleToScreen(new Rectangle(clipleft, cliptop, clipwidth, clipheight));
                panel4.Invalidate();
            }
        }

        private void button3_MouseMove(object sender, MouseEventArgs e)
        {
            if (draging)
            {
                Point MPosition = new Point();
                MPosition = PointToClient(MousePosition);
                MPosition.Offset(mouseX, mouseY);
                panel4.Location = MPosition;
            }
        }

        private void button4_MouseMove(object sender, MouseEventArgs e)
        {
            if (draging)
            {
                Point MPosition = new Point();
                MPosition = PointToClient(MousePosition);
                MPosition.Offset(mouseX, mouseY);
                panel4.Location = MPosition;
            }
        }

        private void button3_MouseUp(object sender, MouseEventArgs e)
        {
            if (draging)
            {
                draging = false;
                Capture = false;
                Cursor.Clip = new Rectangle(this.Location, this.Size);
                panel4.Invalidate();
            }
        }

        private void button4_MouseUp(object sender, MouseEventArgs e)
        {
            if (draging)
            {
                draging = false;
                Capture = false;
                Cursor.Clip = new Rectangle(this.Location, this.Size);
                panel4.Invalidate();
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (zoomValue < 17)
            {
                zoomValue++;
                mapexplr.Zoom = zoomValue;
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            if (zoomValue > 5)
            {
                zoomValue--;
                mapexplr.Zoom = zoomValue;
            }
        }

        private void button1_Click_4(object sender, EventArgs e)
        {
            loginsorgu();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            
        }

        private void mapexplr_Click(object sender, EventArgs e)
        {
            
        }

        private void button6_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                draging = true;
                mouseX = -e.X;
                mouseY = -e.Y;
                int clipleft = PointToClient(MousePosition).X - panel3.Location.X;
                int cliptop = PointToClient(MousePosition).Y - panel3.Location.Y;
                int clipwidth = ClientSize.Width - (panel3.Width - clipleft);
                int clipheight = ClientSize.Height - (panel3.Height - cliptop);
                Cursor.Clip = RectangleToScreen(new Rectangle(clipleft, cliptop, clipwidth, clipheight));
                panel3.Invalidate();
            }
        }

        private void button6_MouseMove(object sender, MouseEventArgs e)
        {
            if (draging)
            {
                Point MPosition = new Point();
                MPosition = PointToClient(MousePosition);
                MPosition.Offset(mouseX, mouseY);
                panel3.Location = MPosition;
            }
        }

        private void button6_MouseUp(object sender, MouseEventArgs e)
        {
            if (draging)
            {
                draging = false;
                Capture = false;
                Cursor.Clip = new Rectangle(this.Location, this.Size);
                panel3.Invalidate();
            }
        }

        private void button5_MouseUp(object sender, MouseEventArgs e)
        {
            if (draging)
            {
                draging = false;
                Capture = false;
                Cursor.Clip = new Rectangle(this.Location, this.Size);
                panel3.Invalidate();
            }
        }

        private void button5_MouseMove(object sender, MouseEventArgs e)
        {
            if (draging)
            {
                Point MPosition = new Point();
                MPosition = PointToClient(MousePosition);
                MPosition.Offset(mouseX, mouseY);
                panel3.Location = MPosition;
            }
        }

        private void button5_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                draging = true;
                mouseX = -e.X;
                mouseY = -e.Y;
                int clipleft = PointToClient(MousePosition).X - panel3.Location.X;
                int cliptop = PointToClient(MousePosition).Y - panel3.Location.Y;
                int clipwidth = ClientSize.Width - (panel3.Width - clipleft);
                int clipheight = ClientSize.Height - (panel3.Height - cliptop);
                Cursor.Clip = RectangleToScreen(new Rectangle(clipleft, cliptop, clipwidth, clipheight));
                panel3.Invalidate();
            }
        }

        private void izlemeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (panel3.Visible == true)
                panel3.Visible = false;
            else
                panel3.Visible = true;
        }

        private void button6_Click(object sender, EventArgs e)
        {
            tekli_sorgu = false;
            refresh();
            flga_for_single = false;
            timer1.Enabled = true;
        }

        private void button5_Click_1(object sender, EventArgs e)
        {
            timer1.Enabled = false;
        }

    
        private void Form1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == '-')
            {
                if (zoomValue > 5)
                {
                    zoomValue--;
                    mapexplr.Zoom = zoomValue;
                }
            }

            if (e.KeyChar == '+')
            {
                if (zoomValue < 17)
                {
                    zoomValue++;
                    mapexplr.Zoom = zoomValue;
                }
            }
        }

        private void mapexplr_KeyPress(object sender, KeyPressEventArgs e)
        {
            System.Threading.Thread.Sleep(20);
            if (e.KeyChar == '-')
            {
                if (zoomValue > 5)
                {
                    zoomValue--;
                    mapexplr.Zoom = zoomValue;
                }
            }

            if (e.KeyChar == '+')
            {
                if (zoomValue < 17)
                {
                    zoomValue++;
                    mapexplr.Zoom = zoomValue;
                }
            }
        }

        private void button1_Click_2(object sender, EventArgs e)
        {
            DBolustur();
            tabloolustur();
            data_yakin.Connection.ConnectionString = ("Server=.\\SQLEXPRESS;Integrated security=SSPI;database=master");
            data_yakin.Connection.Open();
            loginsorgu();
        }

        private void groupBox1_Enter(object sender, EventArgs e)
        {

        }

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            backgroundWorker1.ReportProgress(0);
            if (paralel_simi_flag)
            {
                paralel_simi_flag = false;
                pre_sim();
            }
            else if (paralel_sorgu_flag)
            {
                paralel_sorgu_flag = false;

                if (tekli_sorgu)
                {
                    sorgu(1);
                }
                else
                    sorgu(plakalar_.Length);
            }
            
        }

        private void backgroundWorker1_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            progressBar1.Value = e.ProgressPercentage;
        }

        private void backgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {            
            panel5.Visible = false;
            button2.Enabled = true;
        }

        public void printscreen()
        {
            Bitmap Foto;
            Graphics graf;
            Rectangle rec;
            rec = new Rectangle();
            rec = Screen.GetWorkingArea(this);


            Foto = new Bitmap(rec.Width, rec.Height, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            graf = Graphics.FromImage(Foto);
            graf.CopyFromScreen(rec.X, rec.Y, 0, 0, rec.Size, CopyPixelOperation.SourceCopy);

            SaveFileDialog save = new SaveFileDialog();

            if (save.ShowDialog()==DialogResult.OK)
            {
                Foto.Save(save.FileName);
            }
        }

        private void button7_Click(object sender, EventArgs e)
        {
            printscreen();
        }

        private void mapexplr_MouseDown(object sender, MouseEventArgs e)
        {
            System.Threading.Thread.Sleep(20);
        }

        private void mapexplr_OnMapDrag()
        {
            System.Threading.Thread.Sleep(20);
        }

        private void mapexplr_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            System.Threading.Thread.Sleep(20);
        }

        private void mapexplr_Move(object sender, EventArgs e)
        {
            System.Threading.Thread.Sleep(20);
        }

        private void mapexplr_MouseClick(object sender, MouseEventArgs e)
        {
            System.Threading.Thread.Sleep(20);
        }

        private void mapexplr_SizeChanged(object sender, EventArgs e)
        {
            System.Threading.Thread.Sleep(20);
        }

        private void mapexplr_RegionChanged(object sender, EventArgs e)
        {
            System.Threading.Thread.Sleep(20);
        }

        private void mapexplr_Resize(object sender, EventArgs e)
        {
            System.Threading.Thread.Sleep(20);
        }

        private void mapexplr_DragDrop(object sender, DragEventArgs e)
        {
            System.Threading.Thread.Sleep(20);
        }

        private void mapexplr_ClientSizeChanged(object sender, EventArgs e)
        {
            System.Threading.Thread.Sleep(20);
        }

        private void mapexplr_DragEnter(object sender, DragEventArgs e)
        {
            System.Threading.Thread.Sleep(20);
        }

        private void mapexplr_DragLeave(object sender, EventArgs e)
        {
            System.Threading.Thread.Sleep(20);
        }

        private void mapexplr_DragOver(object sender, DragEventArgs e)
        {
            System.Threading.Thread.Sleep(20);
        }

        void DBolustur()
        {
            String str;
            SqlConnection myConn = new SqlConnection("Server=.\\SQLEXPRESS;Integrated security=SSPI");

            str = "if not exists(select * from sys.databases where name = 'master') begin " +
            "CREATE DATABASE master ON PRIMARY " +
                "(NAME = MyDatabase_Data, " +
                "FILENAME = 'C:\\MyDatabaseData.mdf', " +
                "SIZE = 3MB, MAXSIZE = 10MB, FILEGROWTH = 10%) " +
                "LOG ON (NAME = MyDatabase_Log, " +
                "FILENAME = 'C:\\MyDatabaseLog.ldf', " +
                "SIZE = 1MB, " +
                "MAXSIZE = 5MB, " +
                "FILEGROWTH = 10%) end";

            SqlCommand myCommand = new SqlCommand(str, myConn);
            try
            {
                myConn.Open();
                myCommand.ExecuteNonQuery();
                //MessageBox.Show("DataBase is Created Successfully", "MyProgram", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch //(System.Exception ex)
            {
                //MessageBox.Show(ex.ToString(), "MyProgram", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            finally
            {
                if (myConn.State == ConnectionState.Open)
                {
                    myConn.Close();
                }
            }

        }

        void tabloolustur()
        {
            String str;
            SqlConnection myConn = new SqlConnection("Server=.\\SQLEXPRESS;Integrated security=SSPI;database=master");

            str = "if not exists(select * from sys.tables where name ='Track_LocationInf_new') begin create table Track_LocationInf_new(LocationInf int identity(1,1) primary key ,PortNumber nchar(6) not null,Plaka nchar(20) not null,Latitude  nchar(20) not null,LatPos nchar(2),Longitude nchar(20) not null,LonPos nchar(2), Speed nchar(10) ,Course nchar(15),EmergencyCall nchar(2) not null, Contact nchar(2) not null, DateTime datetime not null)end";

            SqlCommand myCommand = new SqlCommand(str, myConn);
            try
            {
                myConn.Open();
                myCommand.ExecuteNonQuery();
                //MessageBox.Show("DataBase is Created Successfully", "MyProgram", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
           catch  //(System.Exception ex)
            {
                //MessageBox.Show(ex.ToString(), "MyProgram", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            finally
            {
                if (myConn.State == ConnectionState.Open)
                {
                    myConn.Close();
                }
            }
        }

        private void talimatlarToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }            
    }
}
