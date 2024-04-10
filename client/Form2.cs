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
using Excel = Microsoft.Office.Interop.Excel;
using Microsoft.Office.Interop.Excel;

namespace client_yeni
{
    public partial class Form2 : Form
    {
        com.basaktelemetri.Service1 servis = new com.basaktelemetri.Service1();

        string[] s = new string[1];
        string[] p = new string[1];

        string slinkp;

        bool sorgula_1 = false;
        bool sorgula_2_1 = false;
        bool sorgula_2_2 = false;
        bool pre_sorgula_flag = false;
        bool yedekle_flag = false;
        bool tekrarlari_temizle = false;

        bool rich_flag = false;
        bool rich_flag_2 = false;
        bool kucuk_lin_flag = false;
        bool flag_for_var_yok = false;

        double[] hiz_array = new double[1];
        decimal[] mesafe_array = new decimal[1];
        DateTime[] zaman_array = new DateTime[1];
        int[] kac_array = new int[1];

        bool ortalama_hiz = false;
        bool max_hiz = false;
        bool mesafe_flag = false;

        public string[] asa = new string[1];
        public string[] plak = new string[1];

        string s_link;

        double max = 0, old, latitude, longitude;
        double direct;
        string plakam = "";
        string kontakN = "";
        string durum = "";
        DateTime nerede;
        double hizz;


        GMapRoute[] r = new GMapRoute[1];

        DateTime d1;
        double[] dist = new double[1];
        string zaman;

        int GlobalCNTforRich = 0;
        int GlobalCNTforTimes = 0;


        List<Track_LocationInf_new> list_yakin;
        int richIcicn = 0;

        string[] globalplaka = new string[1];
        string[] globalS = new string[1];

        bool gunluk_hiz_ort = false;
        bool gunluk_hiz_yuk = false;
        bool gunluk_mesafe = false;

        bool aylik_hiz_ort = false;
        bool aylik_hiz_yuk = false;
        bool aylik_mesafe = false;

        bool yillik_hiz_ort = false;
        bool yillik_hiz_yuk = false;
        bool yillik_mesafe = false;

        public PointLatLng loc;

        public PointLatLng[] m = new PointLatLng[1];

        public AracTakip_YakinDataContext data_yakin = new AracTakip_YakinDataContext();
       

        LinkLabel[] lnklbl1 = new LinkLabel[1];

        public Form2()
        {
            CheckForIllegalCrossThreadCalls = false;
            InitializeComponent();
        }

        public void refresh()
        {
            try
            {
                for (int i = 0; i < (this.Owner as Form1).plakalar_.Length; i++)
                {
                    comboBox2.Items.Add((this.Owner as Form1).plakalar_[i]);
                    comboBox4.Items.Add((this.Owner as Form1).plakalar_[i]);
                    comboBox5.Items.Add((this.Owner as Form1).plakalar_[i]);
                }
            }
            catch { }
        }

        public void sorgu_plakali()
        {
            double progrescnt = 0;
            try
            {
                max = 0;
                richTextBox1.AppendText("Veriler çekiliyor lütfen bekleyiniz...");
                richTextBox1.AppendText("\n\0");
                var plakalars = servis.plakalar_FORM2(comboBox2.Text, dateTimePicker1.Value, dateTimePicker2.Value);

                double kacTane = plakalars.Count();
                richTextBox1.AppendText("Şu anda " + kacTane.ToString() + " veri işlenmektedir lütfen bekleyiniz...");
                richTextBox1.AppendText("\n\0");
                foreach (var plakalar in plakalars)
                {
                    progrescnt++;
                    double per = (progrescnt / kacTane) * 100;
                    backgroundWorker1.ReportProgress(Convert.ToInt32(per));
                    old = float.Parse(((plakalar.Speed).ToString()).Replace(".", ","));
                    if (max < old)
                    {
                        max = old;
                        d1 = plakalar.DateTime;
                    }
                }
                max = max * 1.852;
                if (d1.Year != 0001)
                {
                    richTextBox1.AppendText("En yüksek hız  " + (max).ToString().Trim() + " KM/h ile " + d1 + " zamanında  yapılmıştır" + "\n");
                }
                else
                {
                    richTextBox1.AppendText("Seçtiğiniz tarih aralığında kayıt bulunamamıştır.");
                }

            }
            catch { MessageBox.Show("veriler çekilirken hata oluştu, lütfen tekrar deneyiniz \n"); }
        }
        public void temizle()
        {
            try
            {
                richTextBox1.Controls.Remove(lnklbl1[0]);
                for (int i = 0; i < lnklbl1.Length - 1; i++)
                {
                    richTextBox1.Controls.Remove(lnklbl1[i]);

                }
            }
            catch { }
        }

        public void sorgu_hiz_kucuk()
        {
            Array.Resize(ref s, 1);
            Array.Resize(ref p, 1);
            int cnt = 0;
            try
            {
                for (int i = 0; i < (this.Owner as Form1).plakalar_.Length; i++)
                {
                    max = 0;
                    var track = servis.sorgu_hiz_kucuk_FORM2((this.Owner as Form1).plakalar_[i], dateTimePicker1.Value, dateTimePicker2.Value, Convert.ToDouble(textBox1.Text) / 1.852);
                    if (track[0] != "0000.000")
                    {
                        double per = ((Convert.ToDouble(i + 1) / Convert.ToDouble((this.Owner as Form1).plakalar_.Length)) * 100);
                        backgroundWorker1.ReportProgress(Convert.ToInt32(per));

                        int point_ind_lat = track[3].IndexOf(".");
                        string lat_pos_W = track[3].Substring(point_ind_lat - 2);
                        lat_pos_W = lat_pos_W.Replace(".", ",");
                        double lat_pos = Convert.ToDouble(lat_pos_W) / 60;
                        double lat_pos_2 = Convert.ToDouble(track[3].Substring(0, point_ind_lat - 2));
                        latitude = lat_pos_2 + lat_pos;

                        int point_ind_lon = track[5].IndexOf(".");
                        string lon_pos_W = track[5].Substring(point_ind_lon - 2);
                        lon_pos_W = lon_pos_W.Replace(".", ",");
                        double lon_pos = Convert.ToDouble(lon_pos_W) / 60;
                        double lon_pos_2 = Convert.ToDouble(track[5].Substring(0, point_ind_lon - 2));
                        longitude = lon_pos_2 + lon_pos;

                        loc.Lat = latitude;
                        loc.Lng = longitude;

                        s[cnt] = track[2].Trim() + " plakalı araç, " + track[11] + " tarihinde saattaki hızı " + ((Convert.ToDouble((track[7]).Replace(".", ","))) * 1.852).ToString() + " KmH dir.";
                        p[cnt] = track[2].Trim();

                        Array.Resize(ref s, s.Length + 1);
                        Array.Resize(ref p, p.Length + 1);
                        cnt++;
                        richIcicn = cnt;
                        GlobalCNTforTimes = cnt * 20 / richTextBox1.Size.Height;
                    }
                }
                kucuk_lin_flag = true;
            }
            catch { MessageBox.Show("hata oluştu, lütfen tekrar deneyiniz!\n"); }
            finally
            {
                Array.Resize(ref s, cnt);
                Array.Resize(ref p, cnt);
            }
            if (cnt == 0)
            {
                flag_for_var_yok = true;
            }
        }

        public void sorgu_hiz_buyuk()
        {
            Array.Resize(ref s, 1);
            Array.Resize(ref p, 1);
            int cnt = 0;
            try
            {
                for (int i = 0; i < (this.Owner as Form1).plakalar_.Length; i++)
                {
                    max = 0;
                    var track = servis.sorgu_hiz_buyuk_FORM2((this.Owner as Form1).plakalar_[i], dateTimePicker1.Value, dateTimePicker2.Value, Convert.ToDouble(textBox1.Text) / 1.852);
                    if (track[0] != "0000.000")
                    {
                        double per = ((Convert.ToDouble(i + 1) / Convert.ToDouble((this.Owner as Form1).plakalar_.Length)) * 100);
                        backgroundWorker1.ReportProgress(Convert.ToInt32(per));

                        int point_ind_lat = track[3].IndexOf(".");
                        string lat_pos_W = track[3].Substring(point_ind_lat - 2);
                        lat_pos_W = lat_pos_W.Replace(".", ",");
                        double lat_pos = Convert.ToDouble(lat_pos_W) / 60;
                        double lat_pos_2 = Convert.ToDouble(track[3].Substring(0, point_ind_lat - 2));
                        latitude = lat_pos_2 + lat_pos;

                        int point_ind_lon = track[5].IndexOf(".");
                        string lon_pos_W = track[5].Substring(point_ind_lon - 2);
                        lon_pos_W = lon_pos_W.Replace(".", ",");
                        double lon_pos = Convert.ToDouble(lon_pos_W) / 60;
                        double lon_pos_2 = Convert.ToDouble(track[5].Substring(0, point_ind_lon - 2));
                        longitude = lon_pos_2 + lon_pos;

                        loc.Lat = latitude;
                        loc.Lng = longitude;
                        s[cnt] = track[2].Trim() + " plakalı araç, " + track[11] + " tarihinde saattaki hızı " + ((Convert.ToDouble((track[7]).Replace(".", ","))) * 1.852).ToString() + " KmH dir.";
                        p[cnt] = track[2].Trim();
                        Array.Resize(ref s, s.Length + 1);
                        Array.Resize(ref p, p.Length + 1);
                        cnt++;
                        richIcicn = cnt;
                        GlobalCNTforTimes = cnt * 20 / richTextBox1.Size.Height;
                    }
                }
                kucuk_lin_flag = true;
            }
            catch { MessageBox.Show("hata oluştu, lütfen tekrar deneyiniz!"); }
            finally
            {
                Array.Resize(ref s, cnt);
                Array.Resize(ref p, cnt);
            }
            if (cnt == 0)
            {
                flag_for_var_yok = true;
            }
        }

        public void sorgu_yer(bool durum)//web servisten çekilecek
        {
            bool flag_for_linklabel = false;
            MapRoute route;
            RoutingProvider rp;
            int cnt = 0;
            int cnnt = 0;
            Array.Resize(ref m, 1);
            Array.Resize(ref dist, 1);
            Array.Resize(ref s, 1);
            Array.Resize(ref p, 1);

            rp = ((this.Owner as Form1).mapexplr).MapProvider as RoutingProvider;
            if (rp == null)
            {
                rp = GMapProviders.GoogleMap;
            }

            (this.Owner as Form1).mapexplr.SetCurrentPositionByKeywords(textBox2.Text);

            try
            {
                for (int i = 0; i < (this.Owner as Form1).plakalar_.Length; i++)
                {
                    double per = ((Convert.ToDouble(i + 1) / Convert.ToDouble((this.Owner as Form1).plakalar_.Length)) * 100);
                    backgroundWorker1.ReportProgress(Convert.ToInt32(per));

                    var track = servis.sorgu_yer_FORM2((this.Owner as Form1).plakalar_[i]);


                    int point_ind_lat = track[3].IndexOf(".");
                    string lat_pos_W = track[3].Substring(point_ind_lat - 2);
                    lat_pos_W = lat_pos_W.Replace(".", ",");
                    double lat_pos = Convert.ToDouble(lat_pos_W) / 60;
                    double lat_pos_2 = Convert.ToDouble(track[3].Substring(0, point_ind_lat - 2));
                    latitude = lat_pos_2 + lat_pos;

                    int point_ind_lon = track[5].IndexOf(".");
                    string lon_pos_W = track[5].Substring(point_ind_lon - 2);
                    lon_pos_W = lon_pos_W.Replace(".", ",");
                    double lon_pos = Convert.ToDouble(lon_pos_W) / 60;
                    double lon_pos_2 = Convert.ToDouble(track[5].Substring(0, point_ind_lon - 2));
                    longitude = lon_pos_2 + lon_pos;

                    zaman = track[11];
                    m[i].Lat = latitude;
                    m[i].Lng = longitude;

                    Array.Resize(ref m, m.Length + 1);
                    Array.Resize(ref dist, dist.Length + 1);

                    route = rp.GetRouteBetweenPoints(m[i], (this.Owner as Form1).mapexplr.Position, radioButton5.Checked, false, 7);

                    if (route != null)
                    {
                        if (track[0] != "0000.000")
                        {
                            dist[i] = route.Distance;
                            if (durum == true)
                            {
                                if (track[9].Trim() == "0")
                                {
                                    s[cnt] = track[11] + " Son tarihli " + track[2].Trim() + " plakalı aracın, aradığınız noktaya uzaklığı " + Math.Round(dist[i], 2).ToString() + " Km";

                                    p[cnt] = track[2].Trim();
                                    loc = m[i];
                                    cnt++;
                                    cnnt++;
                                    Array.Resize(ref s, s.Length + 1);
                                    Array.Resize(ref p, p.Length + 1);
                                }
                            }

                            else if (durum == false)
                            {
                                s[cnt] = track[11] + " Son tarihli " + track[2].Trim() + " plakalı aracın, aradığınız noktaya uzaklığı " + Math.Round(dist[i], 2).ToString() + " Km";

                                p[cnt] = track[2].Trim();
                                loc = m[i];
                                cnt++;
                                cnnt++;
                                Array.Resize(ref s, s.Length + 1);
                                Array.Resize(ref p, p.Length + 1);
                            }

                            flag_for_linklabel = true;
                        }
                    }
                    else
                    {
                        flag_for_linklabel = false;
                        temizle();
                        richTextBox1.AppendText("ulaşılamaz bir adres girdiniz");
                    }
                }
            }

            catch
            {
                (this.Owner as Form1).mapexplr.SetCurrentPositionByKeywords("turkey");
                (this.Owner as Form1).mapexplr.Zoom = 5;
                MessageBox.Show("Hata oluştu, sayfa kapatılacak, lütfen tekrar deneyiniz");//sayfa kapatılacak, lütfen tekrar deneyiniz! "+e.ToString());
                this.Close();
            }

            finally
            {
                Array.Resize(ref s, cnnt);
                Array.Resize(ref p, cnnt);
            }
            double hold;
            string hold2;

            for (int i = 0; i < s.Length - 1; i++)
            {
                for (int j = 0; j < s.Length - 1 - i; j++)
                {
                    if (dist[j] > dist[j + 1])
                    {
                        hold = dist[j];
                        dist[j] = dist[j + 1];
                        dist[j + 1] = hold;

                        hold2 = s[j];
                        s[j] = s[j + 1];
                        s[j + 1] = hold2;

                        hold2 = p[j];
                        p[j] = p[j + 1];
                        p[j + 1] = hold2;
                    }
                }
            }
            GlobalCNTforTimes = s.Length * 20 / richTextBox1.Size.Height + 1;
            if (flag_for_linklabel)
            {
                rich_flag = true;
            }
        }

        public void rich_link()
        {
            for (int i = 0; i < s.Length; i++)
            {
                richIcicn = s.Length;
                lnklbl1[i] = new LinkLabel();
                lnklbl1[i].Width = richTextBox1.Width;
                plak[i] = p[i];
                lnklbl1[i].Text = s[i];
                this.lnklbl1[i].LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.lnklbl1_Clicked);
                lnklbl1[i].Links.Add(0, s[i].Length);
                richTextBox1.Controls.Add(lnklbl1[i]);
                lnklbl1[i].Location = new System.Drawing.Point(0, (i) * 20);

                asa[i] = s[i];
                Array.Resize(ref lnklbl1, lnklbl1.Length + 1);
                Array.Resize(ref plak, plak.Length + 1);
                Array.Resize(ref asa, asa.Length + 1);
            }

            double mesele = richTextBox1.Size.Height / (20 * (s.Length + 1));
            if (Math.Floor(mesele) == 0)
            {
                panel1.Visible = true;
                panel2.Visible = true;
            }
            else
            {
                panel1.Visible = false;
                panel2.Visible = false;
            }
            flag_for_var_yok = false;
        }

        public void istatistik()
        {
            double hiz = 0;
            int kac = 0;
            int day_pre = 0;
            int month_pre = 0;
            int year_pre = 0;
            int j = 0;

            ortalama_hiz = false;
            max_hiz = false;
            mesafe_flag = false;

            Array.Resize(ref mesafe_array, 1);
            Array.Resize(ref hiz_array, 1);
            Array.Resize(ref zaman_array, 1);
            Array.Resize(ref kac_array, 1);

            Array.Clear(mesafe_array, 0, mesafe_array.Length);
            Array.Clear(hiz_array, 0, hiz_array.Length);
            Array.Clear(zaman_array, 0, zaman_array.Length);
            Array.Clear(kac_array, 0, kac_array.Length);


            DateTime time_pre = DateTime.Now;

            string plaka = "";
            int ind = 0;
            double hiz_pre = 0;
            decimal mesafe = 0;
            decimal an = 0;

            try
            {
                if (!checkBox6.Checked)//web servisten çekilecek
                {
                    richTextBox1.AppendText("Veriler çekiliyor lütfen bekleyin...");
                    richTextBox1.AppendText("\n\0");
                    richTextBox1.AppendText("\n\0");
                    var plakalars = servis.istatistik_FORM2(comboBox4.Text, dateTimePicker3.Value, dateTimePicker4.Value);

                    day_pre = plakalars.First().DateTime.Day;
                    month_pre = plakalars.First().DateTime.Month;
                    year_pre = plakalars.First().DateTime.Year;
                    time_pre = plakalars.First().DateTime;
                    plaka = plakalars.First().Plaka;
                    hiz_pre = Convert.ToDouble(plakalars.First().Speed.Replace(".", ","));
                    int num = plakalars.Count();

                    richTextBox1.AppendText("Veriler işleniyor lütfen bekleyin...");
                    richTextBox1.AppendText("\n\0");
                    richTextBox1.AppendText("\n\0");

                    foreach (var item in plakalars)
                    {
                        if (gunluk_hiz_ort)
                        {
                            hiz = hiz + (Convert.ToDouble((item.Speed).Replace(".", ",")));
                            kac++;
                            ind++;
                            if (day_pre.ToString().Trim() != item.DateTime.Day.ToString().Trim())
                            {
                                hiz_array[j] = hiz;
                                kac_array[j] = kac;
                                zaman_array[j] = time_pre.Date;
                                hiz = 0;
                                kac = 0;
                                j++;
                                Array.Resize(ref hiz_array, hiz_array.Length + 1);
                                Array.Resize(ref kac_array, kac_array.Length + 1);
                                Array.Resize(ref zaman_array, zaman_array.Length + 1);
                                ortalama_hiz = true;
                            }
                            if (ind == num)
                            {

                                hiz_array[j] = hiz;
                                kac_array[j] = kac;
                                zaman_array[j] = time_pre.Date;
                                hiz = 0;
                                kac = 0;
                                j = 0;
                            }
                            day_pre = item.DateTime.Day;
                            time_pre = item.DateTime;
                        }
                        else if (gunluk_hiz_yuk)
                        {
                            ind++;
                            old = float.Parse(((item.Speed).ToString()).Replace(".", ","));
                            if (max < old)
                            {
                                max = old;
                            }

                            if (day_pre.ToString().Trim() != item.DateTime.Day.ToString().Trim())
                            {
                                hiz_array[j] = max;
                                zaman_array[j] = time_pre.Date;
                                max = 0;
                                hiz = 0;
                                j++;
                                Array.Resize(ref hiz_array, hiz_array.Length + 1);
                                Array.Resize(ref zaman_array, zaman_array.Length + 1);
                                max_hiz = true;
                            }

                            if (ind == num)
                            {
                                hiz_array[j] = max;
                                zaman_array[j] = time_pre.Date;
                                hiz = 0;
                                j = 0;
                                max = 0;
                            }
                            day_pre = item.DateTime.Day;
                            time_pre = item.DateTime;
                        }

                        else if (gunluk_mesafe)
                        {
                            ind++;
                            if (ind < num - 1)
                            {
                                an = plakalars[ind + 1].DateTime.Second - plakalars[ind].DateTime.Second;
                            }
                            if (an < 0)
                            {
                                an = 60 + an;
                            }
                            if (an == 0)
                            {
                                an = 60;
                            }
                            if (ind < num - 1)
                            {
                                mesafe = mesafe + ((((Convert.ToDecimal(plakalars[ind + 1].Speed.Replace(".", ",")) + Convert.ToDecimal(plakalars[ind].Speed.Replace(".", ","))) / 2) / 3600) * an) * Convert.ToDecimal(1.852);
                            }

                            if (day_pre.ToString().Trim() != item.DateTime.Day.ToString().Trim())
                            {
                                mesafe_array[j] = mesafe;
                                zaman_array[j] = time_pre.Date;
                                mesafe = 0;
                                j++;
                                Array.Resize(ref mesafe_array, mesafe_array.Length + 1);
                                Array.Resize(ref zaman_array, zaman_array.Length + 1);
                                mesafe_flag = true;
                            }
                            if (ind == num)
                            {
                                mesafe_array[j] = mesafe;
                                zaman_array[j] = time_pre.Date;
                                hiz = 0;
                                kac = 0;
                                j = 0;
                            }
                            day_pre = item.DateTime.Day;
                            time_pre = item.DateTime;
                        }
                        /////// AYLIK BAŞLANGIÇ ////////
                        else if (aylik_hiz_ort)
                        {
                            hiz = hiz + (Convert.ToDouble((item.Speed).Replace(".", ",")));
                            kac++;
                            ind++;
                            if (month_pre.ToString().Trim() != item.DateTime.Month.ToString().Trim())
                            {
                                hiz_array[j] = hiz;
                                kac_array[j] = kac;
                                zaman_array[j] = time_pre.Date;
                                hiz = 0;
                                kac = 0;
                                j++;
                                Array.Resize(ref hiz_array, hiz_array.Length + 1);
                                Array.Resize(ref kac_array, kac_array.Length + 1);
                                Array.Resize(ref zaman_array, zaman_array.Length + 1);
                                ortalama_hiz = true;
                            }
                            if (ind == num)
                            {
                                hiz_array[j] = hiz;
                                kac_array[j] = kac;
                                zaman_array[j] = time_pre.Date;
                                hiz = 0;
                                kac = 0;
                                j = 0;
                            }
                            month_pre = item.DateTime.Month;
                            time_pre = item.DateTime;
                        }
                        else if (aylik_hiz_yuk)
                        {
                            ind++;
                            old = float.Parse(((item.Speed).ToString()).Replace(".", ","));
                            if (max < old)
                            {
                                max = old;
                            }

                            if (month_pre.ToString().Trim() != item.DateTime.Month.ToString().Trim())
                            {
                                hiz_array[j] = max;
                                zaman_array[j] = time_pre.Date;
                                max = 0;
                                hiz = 0;
                                j++;
                                Array.Resize(ref hiz_array, hiz_array.Length + 1);
                                Array.Resize(ref zaman_array, zaman_array.Length + 1);
                                max_hiz = true;
                            }

                            if (ind == num)
                            {
                                hiz_array[j] = max;
                                zaman_array[j] = time_pre.Date;
                                hiz = 0;
                                j = 0;
                                max = 0;
                            }
                            month_pre = item.DateTime.Month;
                            time_pre = item.DateTime;
                        }

                        else if (aylik_mesafe)
                        {
                            ind++;
                            if (ind < num - 1)
                            {
                                an = plakalars[ind + 1].DateTime.Second - plakalars[ind].DateTime.Second;
                            }
                            if (an < 0)
                            {
                                an = 60 + an;
                            }
                            if (an == 0)
                            {
                                an = 60;
                            }
                            if (ind < num - 1)
                            {
                                mesafe = mesafe + ((((Convert.ToDecimal(plakalars[ind + 1].Speed.Replace(".", ",")) + Convert.ToDecimal(plakalars[ind].Speed.Replace(".", ","))) / 2) / 3600) * an) * Convert.ToDecimal(1.852);
                            }

                            if (month_pre.ToString().Trim() != item.DateTime.Month.ToString().Trim())
                            {
                                mesafe_array[j] = mesafe;
                                zaman_array[j] = time_pre.Date;
                                mesafe = 0;
                                j++;
                                Array.Resize(ref mesafe_array, mesafe_array.Length + 1);
                                Array.Resize(ref zaman_array, zaman_array.Length + 1);
                                mesafe_flag = true;
                            }
                            if (ind == num)
                            {
                                mesafe_array[j] = mesafe;
                                zaman_array[j] = time_pre.Date;
                                hiz = 0;
                                kac = 0;
                                j = 0;
                            }
                            month_pre = item.DateTime.Month;
                            time_pre = item.DateTime;
                        }
                        ///////// YILLIK BAŞLANGIÇ //////////
                        else if (yillik_hiz_ort)
                        {
                            hiz = hiz + (Convert.ToDouble((item.Speed).Replace(".", ",")));
                            kac++;
                            ind++;
                            if (year_pre.ToString().Trim() != item.DateTime.Year.ToString().Trim())
                            {
                                hiz_array[j] = hiz;
                                kac_array[j] = kac;
                                zaman_array[j] = time_pre.Date;
                                hiz = 0;
                                kac = 0;
                                j++;
                                Array.Resize(ref hiz_array, hiz_array.Length + 1);
                                Array.Resize(ref kac_array, kac_array.Length + 1);
                                Array.Resize(ref zaman_array, zaman_array.Length + 1);
                                ortalama_hiz = true;
                            }
                            if (ind == num)
                            {
                                hiz_array[j] = hiz;
                                kac_array[j] = kac;
                                zaman_array[j] = time_pre.Date;
                                hiz = 0;
                                kac = 0;
                                j = 0;
                            }
                            year_pre = item.DateTime.Year;
                            time_pre = item.DateTime;
                        }
                        else if (yillik_hiz_yuk)
                        {
                            ind++;
                            old = float.Parse(((item.Speed).ToString()).Replace(".", ","));
                            if (max < old)
                            {
                                max = old;
                            }

                            if (year_pre.ToString().Trim() != item.DateTime.Year.ToString().Trim())
                            {
                                hiz_array[j] = max;
                                zaman_array[j] = time_pre.Date;
                                max = 0;
                                hiz = 0;
                                j++;
                                Array.Resize(ref hiz_array, hiz_array.Length + 1);
                                Array.Resize(ref zaman_array, zaman_array.Length + 1);
                                max_hiz = true;
                            }

                            if (ind == num)
                            {
                                hiz_array[j] = max;
                                zaman_array[j] = time_pre.Date;
                                hiz = 0;
                                j = 0;
                                max = 0;
                            }
                            year_pre = item.DateTime.Year;
                            time_pre = item.DateTime;
                        }

                        else if (yillik_mesafe)
                        {
                            ind++;
                            if (ind < num - 1)
                            {
                                an = plakalars[ind + 1].DateTime.Second - plakalars[ind].DateTime.Second;
                            }
                            if (an < 0)
                            {
                                an = 60 + an;
                            }
                            if (an == 0)
                            {
                                an = 60;
                            }
                            if (ind < num - 1)
                            {
                                mesafe = mesafe + ((((Convert.ToDecimal(plakalars[ind + 1].Speed.Replace(".", ",")) + Convert.ToDecimal(plakalars[ind].Speed.Replace(".", ","))) / 2) / 3600) * an) * Convert.ToDecimal(1.852);
                            }

                            if (year_pre.ToString().Trim() != item.DateTime.Year.ToString().Trim())
                            {
                                mesafe_array[j] = mesafe;
                                zaman_array[j] = time_pre.Date;
                                mesafe = 0;
                                j++;
                                Array.Resize(ref mesafe_array, mesafe_array.Length + 1);
                                Array.Resize(ref zaman_array, zaman_array.Length + 1);
                                mesafe_flag = true;
                            }
                            if (ind == num)
                            {
                                mesafe_array[j] = mesafe;
                                zaman_array[j] = time_pre.Date;
                                hiz = 0;
                                kac = 0;
                                j = 0;
                            }
                            year_pre = item.DateTime.Year;
                            time_pre = item.DateTime;
                        }
                    }
                    for (int ii = 0; ii < 1000; ii++)
                    {
                        double per = (Convert.ToDouble(ii) / Convert.ToDouble(1000)) * 90;
                        backgroundWorker1.ReportProgress(Convert.ToInt32(per));
                        System.Threading.Thread.Sleep(3);
                    }
                    backgroundWorker1.ReportProgress(100);
                    System.Threading.Thread.Sleep(1000);
                }
                else////burası yerel olduğundan web servisten çekilmeyecek
                {
                    richTextBox1.AppendText("Veriler çekiliyor lütfen bekleyin...");
                    richTextBox1.AppendText("\n\0");
                    richTextBox1.AppendText("\n\0");

                    var plakalars = (from a in data_yakin.Track_LocationInf_news
                                     where a.Plaka == comboBox4.Text && dateTimePicker3.Value <= a.DateTime && dateTimePicker4.Value >= a.DateTime
                                     orderby a.DateTime descending
                                     select a);

                    day_pre = plakalars.First().DateTime.Day;
                    month_pre = plakalars.First().DateTime.Month;
                    year_pre = plakalars.First().DateTime.Year;
                    time_pre = plakalars.First().DateTime;
                    plaka = plakalars.First().Plaka;
                    hiz_pre = Convert.ToDouble(plakalars.First().Speed.Replace(".", ","));
                    int num = plakalars.Count();
                    list_yakin = plakalars.ToList();

                    richTextBox1.AppendText("Veriler işleniyor lütfen bekleyin...");
                    richTextBox1.AppendText("\n\0");
                    richTextBox1.AppendText("\n\0");

                    foreach (var item in plakalars)
                    {
                        //i++;
                        //double per = ((Convert.ToDouble(i) / Convert.ToDouble(plakalars.Count()) * 100));
                        //backgroundWorker1.ReportProgress(Convert.ToInt32(per));

                        if (gunluk_hiz_ort)
                        {
                            hiz = hiz + (Convert.ToDouble((item.Speed).Replace(".", ",")));
                            kac++;
                            ind++;
                            if (day_pre.ToString().Trim() != item.DateTime.Day.ToString().Trim())
                            {
                                hiz_array[j] = hiz;
                                kac_array[j] = kac;
                                zaman_array[j] = time_pre.Date;
                                hiz = 0;
                                kac = 0;
                                j++;
                                Array.Resize(ref hiz_array, hiz_array.Length + 1);
                                Array.Resize(ref kac_array, kac_array.Length + 1);
                                Array.Resize(ref zaman_array, zaman_array.Length + 1);
                                ortalama_hiz = true;
                            }
                            if (ind == num)
                            {

                                hiz_array[j] = hiz;
                                kac_array[j] = kac;
                                zaman_array[j] = time_pre.Date;
                                hiz = 0;
                                kac = 0;
                                j = 0;
                            }
                            day_pre = item.DateTime.Day;
                            time_pre = item.DateTime;
                        }
                        else if (gunluk_hiz_yuk)
                        {
                            ind++;
                            old = float.Parse(((item.Speed).ToString()).Replace(".", ","));
                            if (max < old)
                            {
                                max = old;
                            }

                            if (day_pre.ToString().Trim() != item.DateTime.Day.ToString().Trim())
                            {
                                hiz_array[j] = max;
                                zaman_array[j] = time_pre.Date;
                                max = 0;
                                hiz = 0;
                                j++;
                                Array.Resize(ref hiz_array, hiz_array.Length + 1);
                                Array.Resize(ref zaman_array, zaman_array.Length + 1);
                                max_hiz = true;
                            }

                            if (ind == num)
                            {
                                hiz_array[j] = max;
                                zaman_array[j] = time_pre.Date;
                                hiz = 0;
                                j = 0;
                                max = 0;
                            }
                            day_pre = item.DateTime.Day;
                            time_pre = item.DateTime;
                        }

                        else if (gunluk_mesafe)
                        {
                            ind++;
                            if (ind < num - 1)
                            {
                                an = list_yakin[ind + 1].DateTime.Second - list_yakin[ind].DateTime.Second;
                            }
                            if (an < 0)
                            {
                                an = 60 + an;
                            }
                            if (an == 0)
                            {
                                an = 60;
                            }
                            if (ind < num - 1)
                            {
                                mesafe = mesafe + ((((Convert.ToDecimal(list_yakin[ind + 1].Speed.Replace(".", ",")) + Convert.ToDecimal(list_yakin[ind].Speed.Replace(".", ","))) / 2) / 3600) * an) * Convert.ToDecimal(1.852);
                            }

                            if (day_pre.ToString().Trim() != item.DateTime.Day.ToString().Trim())
                            {
                                mesafe_array[j] = mesafe;
                                zaman_array[j] = time_pre.Date;
                                mesafe = 0;
                                j++;
                                Array.Resize(ref mesafe_array, mesafe_array.Length + 1);
                                Array.Resize(ref zaman_array, zaman_array.Length + 1);
                                mesafe_flag = true;
                            }
                            if (ind == num)
                            {
                                mesafe_array[j] = mesafe;
                                zaman_array[j] = time_pre.Date;
                                hiz = 0;
                                kac = 0;
                                j = 0;
                            }
                            day_pre = item.DateTime.Day;
                            time_pre = item.DateTime;
                        }
                        /////// AYLIK BAŞLANGIÇ ////////
                        else if (aylik_hiz_ort)
                        {
                            hiz = hiz + (Convert.ToDouble((item.Speed).Replace(".", ",")));
                            kac++;
                            ind++;
                            if (month_pre != item.DateTime.Month)
                            {
                                hiz_array[j] = hiz;
                                kac_array[j] = kac;
                                zaman_array[j] = time_pre.Date;
                                hiz = 0;
                                kac = 0;
                                j++;
                                Array.Resize(ref hiz_array, hiz_array.Length + 1);
                                Array.Resize(ref kac_array, kac_array.Length + 1);
                                Array.Resize(ref zaman_array, zaman_array.Length + 1);
                                ortalama_hiz = true;
                            }
                            if (ind == num)
                            {
                                hiz_array[j] = hiz;
                                kac_array[j] = kac;
                                zaman_array[j] = time_pre.Date;
                                hiz = 0;
                                kac = 0;
                                j = 0;
                            }
                            month_pre = item.DateTime.Month;
                            time_pre = item.DateTime;
                        }
                        else if (aylik_hiz_yuk)
                        {
                            ind++;
                            old = float.Parse(((item.Speed).ToString()).Replace(".", ","));
                            if (max < old)
                            {
                                max = old;
                            }

                            if (month_pre.ToString().Trim() != item.DateTime.Month.ToString().Trim())
                            {
                                hiz_array[j] = max;
                                zaman_array[j] = time_pre.Date;
                                max = 0;
                                hiz = 0;
                                j++;
                                Array.Resize(ref hiz_array, hiz_array.Length + 1);
                                Array.Resize(ref zaman_array, zaman_array.Length + 1);
                                max_hiz = true;
                            }

                            if (ind == num)
                            {
                                hiz_array[j] = max;
                                zaman_array[j] = time_pre.Date;
                                hiz = 0;
                                j = 0;
                                max = 0;
                            }
                            month_pre = item.DateTime.Month;
                            time_pre = item.DateTime;
                        }

                        else if (aylik_mesafe)
                        {
                            ind++;
                            if (ind < num - 1)
                            {
                                an = list_yakin[ind + 1].DateTime.Second - list_yakin[ind].DateTime.Second;
                            }
                            if (an < 0)
                            {
                                an = 60 + an;
                            }
                            if (an == 0)
                            {
                                an = 60;
                            }
                            if (ind < num - 1)
                            {
                                mesafe = mesafe + ((((Convert.ToDecimal(list_yakin[ind + 1].Speed.Replace(".", ",")) + Convert.ToDecimal(list_yakin[ind].Speed.Replace(".", ","))) / 2) / 3600) * an) * Convert.ToDecimal(1.852);
                            }

                            if (month_pre.ToString().Trim() != item.DateTime.Month.ToString().Trim())
                            {
                                mesafe_array[j] = mesafe;
                                zaman_array[j] = time_pre.Date;
                                mesafe = 0;
                                j++;
                                Array.Resize(ref mesafe_array, mesafe_array.Length + 1);
                                Array.Resize(ref zaman_array, zaman_array.Length + 1);
                                mesafe_flag = true;
                            }
                            if (ind == num)
                            {
                                mesafe_array[j] = mesafe;
                                zaman_array[j] = time_pre.Date;
                                hiz = 0;
                                kac = 0;
                                j = 0;
                            }
                            month_pre = item.DateTime.Month;
                            time_pre = item.DateTime;
                        }
                        ///////// YILLIK BAŞLANGIÇ //////////
                        else if (yillik_hiz_ort)
                        {
                            hiz = hiz + (Convert.ToDouble((item.Speed).Replace(".", ",")));
                            kac++;
                            ind++;
                            if (year_pre.ToString().Trim() != item.DateTime.Year.ToString().Trim())
                            {
                                hiz_array[j] = hiz;
                                kac_array[j] = kac;
                                zaman_array[j] = time_pre.Date;
                                hiz = 0;
                                kac = 0;
                                j++;
                                Array.Resize(ref hiz_array, hiz_array.Length + 1);
                                Array.Resize(ref kac_array, kac_array.Length + 1);
                                Array.Resize(ref zaman_array, zaman_array.Length + 1);
                                ortalama_hiz = true;
                            }
                            if (ind == num)
                            {
                                hiz_array[j] = hiz;
                                kac_array[j] = kac;
                                zaman_array[j] = time_pre.Date;
                                hiz = 0;
                                kac = 0;
                                j = 0;
                            }
                            year_pre = item.DateTime.Year;
                            time_pre = item.DateTime;
                        }
                        else if (yillik_hiz_yuk)
                        {
                            ind++;
                            old = float.Parse(((item.Speed).ToString()).Replace(".", ","));
                            if (max < old)
                            {
                                max = old;
                            }

                            if (year_pre.ToString().Trim() != item.DateTime.Year.ToString().Trim())
                            {
                                hiz_array[j] = max;
                                zaman_array[j] = time_pre.Date;
                                max = 0;
                                hiz = 0;
                                j++;
                                Array.Resize(ref hiz_array, hiz_array.Length + 1);
                                Array.Resize(ref zaman_array, zaman_array.Length + 1);
                                max_hiz = true;
                            }

                            if (ind == num)
                            {
                                hiz_array[j] = max;
                                zaman_array[j] = time_pre.Date;
                                hiz = 0;
                                j = 0;
                                max = 0;
                            }
                            year_pre = item.DateTime.Year;
                            time_pre = item.DateTime;
                        }

                        else if (yillik_mesafe)
                        {
                            ind++;
                            if (ind < num - 1)
                            {
                                an = list_yakin[ind + 1].DateTime.Second - list_yakin[ind].DateTime.Second;
                            }
                            if (an < 0)
                            {
                                an = 60 + an;
                            }
                            if (an == 0)
                            {
                                an = 60;
                            }
                            if (ind < num - 1)
                            {
                                mesafe = mesafe + ((((Convert.ToDecimal(list_yakin[ind + 1].Speed.Replace(".", ",")) + Convert.ToDecimal(list_yakin[ind].Speed.Replace(".", ","))) / 2) / 3600) * an) * Convert.ToDecimal(1.852);
                            }

                            if (year_pre.ToString().Trim() != item.DateTime.Year.ToString().Trim())
                            {
                                mesafe_array[j] = mesafe;
                                zaman_array[j] = time_pre.Date;
                                mesafe = 0;
                                j++;
                                Array.Resize(ref mesafe_array, mesafe_array.Length + 1);
                                Array.Resize(ref zaman_array, zaman_array.Length + 1);
                                mesafe_flag = true;
                            }
                            if (ind == num)
                            {
                                mesafe_array[j] = mesafe;
                                zaman_array[j] = time_pre.Date;
                                hiz = 0;
                                kac = 0;
                                j = 0;
                            }
                            year_pre = item.DateTime.Year;
                            time_pre = item.DateTime;
                        }
                    }
                    for (int ii = 0; ii < 1000; ii++)
                    {
                        double per = (Convert.ToDouble(ii) / Convert.ToDouble(1000)) * 90;
                        backgroundWorker1.ReportProgress(Convert.ToInt32(per));
                        System.Threading.Thread.Sleep(3);
                    }                    
                    backgroundWorker1.ReportProgress(100);
                    System.Threading.Thread.Sleep(1000);
                }
            }
            catch { MessageBox.Show("Hata oluştu, lütfen tekrar deneyiniz!"); }

            if (ortalama_hiz)
            {
                for (int i = 0; i < hiz_array.Length; i++)
                {
                    richTextBox1.AppendText(plaka.Trim() + "  " + zaman_array[i].ToString().Trim().Substring(0, 10) + "  " + Math.Round((hiz_array[i] / kac_array[i]) * 1.852, 2).ToString().Trim() + "\n\r");
                }
            }

            if (max_hiz)
            {
                for (int i = 0; i < hiz_array.Length; i++)
                {
                    richTextBox1.AppendText(plaka.Trim() + "  " + zaman_array[i].ToString().Trim().Substring(0, 10) + "  " + Math.Round((hiz_array[i] * 1.852), 2).ToString().Trim() + "\n\r");
                }
            }

            if (mesafe_flag)
            {
                for (int i = 0; i < mesafe_array.Length; i++)
                {
                    richTextBox1.AppendText((plaka.Trim() + "  " + zaman_array[i].ToString().Trim().Substring(0, 10) + "  " + Math.Round(mesafe_array[i], 2)).ToString().Trim() + "\n\r");
                }
            }

        }


        public void neredeydim()
        {
            try
            {
                if (!checkBox5.Checked)//web servisten çekilecek
                {
                    var plakalars = servis.neredeydim_FORM2(comboBox5.SelectedItem as string, dateTimePicker5.Value);
                    backgroundWorker1.ReportProgress(100);

                    direct = Convert.ToDouble(plakalars[8]);
                    plakam = plakalars[2].Trim();
                    nerede = Convert.ToDateTime(plakalars[11]);
                    hizz = (Convert.ToDouble((plakalars[7]).Replace(".", ","))) * 1.852;
                    durum = plakalars[9].Trim();
                    kontakN = plakalars[10].Trim();

                    int point_ind_lat = plakalars[3].IndexOf(".");
                    string lat_pos_W = plakalars[3].Substring(point_ind_lat - 2);
                    lat_pos_W = lat_pos_W.Replace(".", ",");
                    double lat_pos = Convert.ToDouble(lat_pos_W) / 60;
                    double lat_pos_2 = Convert.ToDouble(plakalars[3].Substring(0, point_ind_lat - 2));
                    latitude = lat_pos_2 + lat_pos;

                    int point_ind_lon = plakalars[5].IndexOf(".");
                    string lon_pos_W = plakalars[5].Substring(point_ind_lon - 2);
                    lon_pos_W = lon_pos_W.Replace(".", ",");
                    double lon_pos = Convert.ToDouble(lon_pos_W) / 60;
                    double lon_pos_2 = Convert.ToDouble(plakalars[5].Substring(0, point_ind_lon - 2));
                    longitude = lon_pos_2 + lon_pos;
                    s_link = plakalars[2].Trim() + " plakalı aracını " + plakalars[11] + " tarihinde bulunduğu konunum ve hız bilgisi için tıklayınız.";
                    slinkp = plakalars[2].Trim(); ;
                    rich_flag_2 = true;
                }

                else//burası yerel olduğundan web servisten çekilmeyecek
                {
                    var plakalars = (from a in data_yakin.Track_LocationInf_news
                                     where a.Plaka == comboBox5.SelectedItem as string && a.DateTime < dateTimePicker5.Value
                                     orderby a.LocationInf descending
                                     select a).First();

                    direct = Convert.ToDouble(plakalars.Course);
                    plakam = plakalars.Plaka.Trim();
                    nerede = plakalars.DateTime;
                    hizz = (Convert.ToDouble((plakalars.Speed).Replace(".", ","))) * 1.852;
                    durum = plakalars.EmergencyCall.Trim();
                    kontakN = plakalars.Contact.Trim();

                    int point_ind_lat = plakalars.Latitude.IndexOf(".");
                    string lat_pos_W = plakalars.Latitude.Substring(point_ind_lat - 2);
                    lat_pos_W = lat_pos_W.Replace(".", ",");
                    double lat_pos = Convert.ToDouble(lat_pos_W) / 60;
                    double lat_pos_2 = Convert.ToDouble(plakalars.Latitude.Substring(0, point_ind_lat - 2));
                    latitude = lat_pos_2 + lat_pos;

                    int point_ind_lon = plakalars.Longitude.IndexOf(".");
                    string lon_pos_W = plakalars.Longitude.Substring(point_ind_lon - 2);
                    lon_pos_W = lon_pos_W.Replace(".", ",");
                    double lon_pos = Convert.ToDouble(lon_pos_W) / 60;
                    double lon_pos_2 = Convert.ToDouble(plakalars.Longitude.Substring(0, point_ind_lon - 2));
                    longitude = lon_pos_2 + lon_pos;
                    s_link = plakalars.Plaka.Trim() + " plakalı aracını " + plakalars.DateTime + " tarihinde bulunduğu konunum ve hız bilgisi için tıklayınız.";
                    slinkp = plakalars.Plaka.Trim();
                    rich_flag_2 = true;

                }
            }
            catch { }
        }

        public void slink()
        {
            lnklbl1[0] = new LinkLabel();
            lnklbl1[0].Width = richTextBox1.Width;
            plak[0] = slinkp;
            lnklbl1[0].Text = s_link;
            this.lnklbl1[0].LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.lnklbl_Clicked);
            lnklbl1[0].Links.Add(0, s_link.Length);
            richTextBox1.Controls.Add(lnklbl1[0]);
            lnklbl1[0].Location = new System.Drawing.Point(0, (0 + 0) * 20);

            asa[0] = s_link;
        }

        public void yedekle_DB()
        {
            try
            {
                double per = 0;
                richTextBox1.AppendText(dateTimePicker6.Value.ToString() + " " + dateTimePicker7.Value.ToString() + " tarihleri arasında ki veriler yerel ilgisayarınız aktarılıyor, lütfen bekleyiniz!\n");
                var uzak = servis.yedekle_FORM2(dateTimePicker6.Value, dateTimePicker7.Value);
                foreach (var item in uzak)
                {
                    Track_LocationInf_new ac = new Track_LocationInf_new();

                    ac.PortNumber = item.PortNumber.Trim();
                    ac.Plaka = item.Plaka.Trim();
                    ac.Latitude = item.Latitude.Trim();
                    ac.LatPos = item.LatPos.Trim();
                    ac.Longitude = item.Longitude.Trim();
                    ac.LonPos = item.LonPos.Trim();
                    ac.Speed = item.Speed.Trim();
                    ac.Course = item.Course.Trim();
                    ac.EmergencyCall = item.EmergencyCall.Trim();
                    ac.Contact = item.Contact.Trim();
                    ac.DateTime = item.DateTime;
                    data_yakin.Track_LocationInf_news.InsertOnSubmit(ac);
                    
                }
                richTextBox1.AppendText("Veriler kaydediliyor, lütfen bekleyiniz.\n");
                for (int i = 0; i < 10000; i++)
                {
                    per = (Convert.ToDouble(i) / Convert.ToDouble(10000)) * 90;
                    backgroundWorker1.ReportProgress(Convert.ToInt32(per));
                    System.Threading.Thread.Sleep(3);
                }
                data_yakin.SubmitChanges();
                backgroundWorker1.ReportProgress(100);
                System.Threading.Thread.Sleep(1000);
                richTextBox1.AppendText("Aktarma işlemi başarıyla tamamlandı.\n");
            }
            catch
            {
                richTextBox1.AppendText("Hata oluştu lütfen tekrar deneyiniz \n");
            }
        }

        public void ExcelleAktar()
        {
            Excel.Application ExcelApp = new Excel.Application();
            ExcelApp.Workbooks.Add();
            ExcelApp.Visible = true;
            ExcelApp.Worksheets[1].Activate();
            string nereye = "C";

            ExcelApp.Cells[1, 1].value = "Plaka";
            ExcelApp.Cells[1, 2].value = "Tarih";
            ExcelApp.Cells[1, 3].value = "Değer";

            if (ortalama_hiz)
            {
                ortalama_hiz = false;
                for (int i = 2; i < hiz_array.Length + 2; i++)
                {
                    ExcelApp.Cells[i, 1].value = comboBox4.SelectedItem;
                    ExcelApp.Cells[i, 2].value = zaman_array[i - 2].ToString().Trim().Substring(0, 10);
                    ExcelApp.Cells[i, 3].value = Convert.ToDouble(Math.Round((hiz_array[i - 2] / kac_array[i - 2]) * 1.852, 2).ToString().Trim());
                }
                nereye = nereye + (hiz_array.Length + 1).ToString();
            }

            if (max_hiz)
            {
                max_hiz = false;
                for (int i = 2; i < hiz_array.Length + 2; i++)
                {
                    ExcelApp.Cells[i, 1].value = comboBox4.SelectedItem;
                    ExcelApp.Cells[i, 2].value = zaman_array[i - 2].ToString().Trim().Substring(0, 10);
                    ExcelApp.Cells[i, 3].value = Convert.ToDouble(Math.Round((hiz_array[i - 2] * 1.852), 2).ToString().Trim());
                }
                nereye = nereye + (hiz_array.Length + 1).ToString();
            }

            if (mesafe_flag)
            {
                mesafe_flag = false;
                for (int i = 2; i < mesafe_array.Length + 2; i++)
                {
                    ExcelApp.Cells[i, 1].value = comboBox4.SelectedItem;
                    ExcelApp.Cells[i, 2].value = zaman_array[i - 2].ToString().Trim().Substring(0, 10);
                    ExcelApp.Cells[i, 3].value = Convert.ToDouble((Math.Round(mesafe_array[i - 2], 2)).ToString().Trim());
                }
                nereye = nereye + (mesafe_array.Length + 1).ToString();
            }


            if (checkBox3.Checked)
            {
                Excel.Worksheet xlWorkSheet = (Excel.Worksheet)ExcelApp.ActiveSheet;
                Excel.ChartObjects xlCharts = (Excel.ChartObjects)xlWorkSheet.ChartObjects();
                Excel.ChartObject Chart = (Excel.ChartObject)xlCharts.Add(200, 10, 90 * (mesafe_array.Length + 1), 45 * (mesafe_array.Length + 1));
                Excel.Chart chartPage = Chart.Chart;
                Excel.Range chartRange;
                chartRange = xlWorkSheet.get_Range("B2", nereye);
                chartPage.SetSourceData(chartRange);

                chartPage.ChartType = Excel.XlChartType.xl3DColumnClustered;
                chartPage.HasDataTable = true;
            }
            button1.Enabled = true;
            button2.Enabled = true;
            button3.Enabled = true;
            button4.Enabled = true;
            button10.Enabled = true;
            button11.Enabled = true;
            button12.Enabled = true;
        }
        public void Tekrarlari_Temizle()
        {
            double per = 0;
            richTextBox1.AppendText("Tekrarlarlanmış veriler siliniyor, lütfen bekleyiniz...\n");
            try
            {
                var sil = (from a in data_yakin.Track_LocationInf_news
                           select a);
                var tut = (from a in data_yakin.Track_LocationInf_news
                           select new { a.PortNumber, a.Plaka, a.Latitude, a.LatPos, a.Longitude, a.LonPos, a.Speed, a.Course, a.EmergencyCall, a.Contact, a.DateTime }).Distinct();

                data_yakin.Track_LocationInf_news.DeleteAllOnSubmit(sil);
                richTextBox1.AppendText("Veriler işleniyor, lütfen bekleyiniz...\n");

                foreach (var item in tut)
                {
                    Track_LocationInf_new ac = new Track_LocationInf_new();

                    ac.PortNumber = item.PortNumber;
                    ac.Plaka = item.Plaka;
                    ac.Latitude = item.Latitude;
                    ac.LatPos = item.LatPos;
                    ac.Longitude = item.Longitude;
                    ac.LonPos = item.LonPos;
                    ac.Speed = item.Speed;
                    ac.Course = item.Course;
                    ac.EmergencyCall = item.EmergencyCall;
                    ac.Contact = item.Contact;
                    ac.DateTime = item.DateTime;

                    data_yakin.Track_LocationInf_news.InsertOnSubmit(ac);
                }
                for (int i = 0; i < 10000; i++)
                {
                    per = (Convert.ToDouble(i) / Convert.ToDouble(10000)) * 90;
                    backgroundWorker1.ReportProgress(Convert.ToInt32(per));
                    System.Threading.Thread.Sleep(3);
                }
                data_yakin.SubmitChanges();
                backgroundWorker1.ReportProgress(100);
                System.Threading.Thread.Sleep(1000);
                richTextBox1.AppendText("Tekrarlarlanmış veriler silindi...\n");
            }
            catch
            {
                richTextBox1.AppendText("Hata oluştu, lütfen tekrar deneyiniz.\n");
            }
        }

        private void lnklbl1_Clicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            for (int i = 0; i < asa.Length; i++)
            {
                if (sender.ToString().Substring(38, (sender.ToString()).Length - 38) == asa[i])
                {
                    (this.Owner as Form1).lokasyonlar.Clear();
                    (this.Owner as Form1).overlayOne.Markers.Clear();
                    (this.Owner as Form1).overlayOne.Routes.Clear();
                    (this.Owner as Form1).timer1.Enabled = true;
                    (this.Owner as Form1).sim_icin_flag = true;
                    (this.Owner as Form1).tekli_sorgu = true;

                    (this.Owner as Form1).plakalar_[0] = plak[i];
                    (this.Owner as Form1).plakalar_[1] = "\0";
                    (this.Owner as Form1).mapexplr.Position = loc;
                    this.Close();
                }
            }
        }

        private void lnklbl_Clicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            for (int i = 0; i < asa.Length; i++)
            {
                if (sender.ToString().Substring(38, (sender.ToString()).Length - 38) == asa[i])
                {
                    (this.Owner as Form1).lokasyonlar.Clear();
                    (this.Owner as Form1).overlayOne.Markers.Clear();
                    (this.Owner as Form1).overlayOne.Routes.Clear();
                    (this.Owner as Form1).nerelerdeydim(latitude, longitude, direct, plakam, nerede, hizz, durum, kontakN);
                    this.Close();
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            flag_for_var_yok = false;
            temizle();
            richTextBox1.Clear();
            button1.Enabled = false;
            button2.Enabled = false;
            button3.Enabled = false;
            button4.Enabled = false;
            button10.Enabled = false;
            button11.Enabled = false;
            button12.Enabled = false;
            progressBar1.Visible = true;
            sorgula_1 = true;
            backgroundWorker1.RunWorkerAsync();
        }

        public void pre_ilk_tab()
        {
            panel2.Visible = false;
            button7.Enabled = true;
            button8.Enabled = true;
            GlobalCNTforRich = 0;
            richTextBox1.Clear();
            if (checkBox1.Checked)
            {
                if ((string)"En yüksek hiz" == (string)(comboBox1.SelectedItem))
                {
                    sorgu_plakali();
                }
                else
                {
                    MessageBox.Show("Plaka ile yalnızca En Yüksek Hız sorgu yapılabilir");
                    comboBox1.Text = comboBox1.Items[2].ToString();
                    sorgu_plakali();
                }

            }

            else if (comboBox1.Text == "KmH 'den küçük")
            {
                sorgu_hiz_kucuk();
                if (flag_for_var_yok)
                {
                    richTextBox1.AppendText("Seçtiğiniz tarih aralığı için kayıt bulunamamıştır. ");
                }
            }

            else if (comboBox1.Text == "KmH 'den büyük")
            {
                sorgu_hiz_buyuk();
                if (flag_for_var_yok)
                {
                    richTextBox1.AppendText("Seçtiğiniz tarih aralığı için kayıt bulunamamıştır. ");
                }
            }

            else if (comboBox1.Text == "En yüksek hız" && !checkBox1.Checked)
            {
                richTextBox1.AppendText("Plakalı sorgu ile geçerlidir! Lütfen *Plaka ile?* kutucuğunu işaretleyip, plaka seçiniz. ");
            }
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {

        }

        private void Form2_Load(object sender, EventArgs e)
        {
            servis.Timeout = 100000000;
            refresh();
            data_yakin.Connection.ConnectionString = ("Server=.\\SQLEXPRESS;Integrated security=SSPI;database=master");
            data_yakin.Connection.Open();
        }

        private void Form2_FormClosing(object sender, FormClosingEventArgs e)
        {

        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox1.Checked)
            {
                comboBox2.Enabled = true;
                comboBox1.Text = comboBox1.Items[2].ToString();
                textBox1.Enabled = false;
            }
            else
            {
                comboBox2.Enabled = false;
                comboBox1.Text = "Sorgu seç";
                textBox1.Enabled = true;
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            flag_for_var_yok = false;
            temizle();
            richTextBox1.Clear();
            button1.Enabled = false;
            button2.Enabled = false;
            button3.Enabled = false;
            button4.Enabled = false;
            button10.Enabled = false;
            button11.Enabled = false;
            button12.Enabled = false;
            progressBar1.Visible = true;
            sorgula_2_1 = true;
            backgroundWorker1.RunWorkerAsync();
        }

        public void pre_konum_tab2()
        {
            panel1.Visible = false;
            button5.Enabled = true;
            button6.Enabled = true;
            GlobalCNTforRich = 0;
            if (comboBox3.Text == "Uygun")
            {
                sorgu_yer(true);
            }
            else if (comboBox3.Text == "Hepsi")
            {
                sorgu_yer(false);
            }
            else
            {
                MessageBox.Show("lütfen bir sorgu türü seçiniz");
            }
        }

        private void tabPage3_Click(object sender, EventArgs e)
        {

        }

        private void richTextBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton1.Checked)
            {
                groupBox3.Enabled = true;
            }
            else
            {
                groupBox3.Enabled = false;
                radioButton3.Checked = false;
                radioButton4.Checked = false;
            }
        }

        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton1.Checked)
            {
                groupBox3.Enabled = true;
            }
            else
            {
                groupBox3.Enabled = false;
                radioButton3.Checked = false;
                radioButton4.Checked = false;
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            richTextBox1.Clear();
            temizle();
            button1.Enabled = false;
            button2.Enabled = false;
            button3.Enabled = false;
            button4.Enabled = false;
            button10.Enabled = false;
            button11.Enabled = false;
            button12.Enabled = false;
            progressBar1.Visible = true;
            pre_sorgula_flag = true;
            backgroundWorker1.RunWorkerAsync();
        }

        void pre_sorgula()
        {
            if (comboBox10.Text == "Sıralama Seçeneği")
            {
                MessageBox.Show("Lütfen Sıralama Seçeneğini Belirleyiniz!");
            }
            if (comboBox10.Text == "Günlük")
            {
                if (radioButton1.Checked)
                {
                    if (radioButton3.Checked)
                    {
                        gunluk_hiz_ort = true;
                        istatistik();
                    }
                    if (radioButton4.Checked)
                    {
                        gunluk_hiz_yuk = true;
                        istatistik();
                    }
                }
                if (radioButton2.Checked)
                {
                    gunluk_mesafe = true;
                    istatistik();
                }
            }

            if (comboBox10.Text == "Aylık")
            {
                if (radioButton1.Checked)
                {
                    if (radioButton3.Checked)
                    {
                        aylik_hiz_ort = true;
                        istatistik();
                    }
                    if (radioButton4.Checked)
                    {
                        aylik_hiz_yuk = true;
                        istatistik();
                    }
                }
                if (radioButton2.Checked)
                {
                    aylik_mesafe = true;
                    istatistik();
                }
            }

            if (comboBox10.Text == "Yıllık")
            {
                if (radioButton1.Checked)
                {
                    if (radioButton3.Checked)
                    {
                        yillik_hiz_ort = true;
                        istatistik();
                    }
                    if (radioButton4.Checked)
                    {
                        yillik_hiz_yuk = true;
                        istatistik();
                    }
                }
                if (radioButton2.Checked)
                {
                    yillik_mesafe = true;
                    istatistik();
                }
            }

            gunluk_hiz_ort = false;
            gunluk_hiz_yuk = false;
            gunluk_mesafe = false;

            aylik_hiz_ort = false;
            aylik_hiz_yuk = false;
            aylik_mesafe = false;

            yillik_hiz_ort = false;
            yillik_hiz_yuk = false;
            yillik_mesafe = false;
        }

        private void button4_Click(object sender, EventArgs e)
        {
            richTextBox1.Clear();
            temizle();
            button1.Enabled = false;
            button2.Enabled = false;
            button3.Enabled = false;
            button4.Enabled = false;
            button10.Enabled = false;
            button11.Enabled = false;
            button12.Enabled = false;
            progressBar1.Visible = true;
            sorgula_2_2 = true;
            backgroundWorker1.RunWorkerAsync();
        }

        private void button6_Click(object sender, EventArgs e)
        {
            button5.Enabled = true;
            GlobalCNTforRich++;
            double mesele = richTextBox1.Size.Height / (20);
            int kac = Convert.ToInt32(Math.Round(mesele)) * GlobalCNTforRich;
            temizle();
            for (int i = kac; i < richIcicn; i++)
            {
                lnklbl1[i].Location = new System.Drawing.Point(0, (i - kac) * 20);
                richTextBox1.Controls.Add(lnklbl1[i]);
            }
            if (GlobalCNTforTimes == GlobalCNTforRich)
            {
                button6.Enabled = false;
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            button6.Enabled = true;
            GlobalCNTforRich--;
            double mesele = richTextBox1.Size.Height / (20);
            int kac = Convert.ToInt32(Math.Round(mesele)) * GlobalCNTforRich;
            temizle();
            for (int i = kac; i < richIcicn; i++)
            {
                lnklbl1[i].Location = new System.Drawing.Point(0, (i - kac) * 20);
                richTextBox1.Controls.Add(lnklbl1[i]);
            }
            if (0 == GlobalCNTforRich)
            {
                button5.Enabled = false;
            }
        }

        private void button8_Click(object sender, EventArgs e)
        {
            button7.Enabled = true;
            GlobalCNTforRich++;
            double mesele = richTextBox1.Size.Height / (20);
            int kac = Convert.ToInt32(Math.Round(mesele)) * GlobalCNTforRich;
            temizle();
            for (int i = kac; i < richIcicn; i++)
            {
                lnklbl1[i].Location = new System.Drawing.Point(0, (i - kac) * 20);
                richTextBox1.Controls.Add(lnklbl1[i]);
            }
            if (GlobalCNTforTimes == GlobalCNTforRich)
            {
                button8.Enabled = false;
            }
        }

        private void button7_Click(object sender, EventArgs e)
        {
            button8.Enabled = true;
            GlobalCNTforRich--;
            double mesele = richTextBox1.Size.Height / (20);
            int kac = Convert.ToInt32(Math.Round(mesele)) * GlobalCNTforRich;
            temizle();
            for (int i = kac; i < richIcicn; i++)
            {
                lnklbl1[i].Location = new System.Drawing.Point(0, (i - kac) * 20);
                richTextBox1.Controls.Add(lnklbl1[i]);
            }
            if (0 == GlobalCNTforRich)
            {
                button7.Enabled = false;
            }
        }

        private void groupBox4_Enter(object sender, EventArgs e)
        {

        }

        private void button10_Click(object sender, EventArgs e)
        {
            button1.Enabled = false;
            button2.Enabled = false;
            button3.Enabled = false;
            button4.Enabled = false;
            button10.Enabled = false;
            button11.Enabled = false;
            button12.Enabled = false;
            ExcelleAktar();
        }

        private void button11_Click(object sender, EventArgs e)
        {
            richTextBox1.Clear();
            temizle();
            progressBar1.Visible = true;
            button1.Enabled = false;
            button2.Enabled = false;
            button3.Enabled = false;
            button4.Enabled = false;
            button10.Enabled = false;
            button11.Enabled = false;
            button12.Enabled = false;
            yedekle_flag = true;
            backgroundWorker1.RunWorkerAsync();
        }

        private void button12_Click(object sender, EventArgs e)
        {
            richTextBox1.Clear();
            temizle();
            progressBar1.Visible = true;
            button1.Enabled = false;
            button2.Enabled = false;
            button3.Enabled = false;
            button4.Enabled = false;
            button10.Enabled = false;
            button11.Enabled = false;
            button12.Enabled = false;
            tekrarlari_temizle = true;
            backgroundWorker1.RunWorkerAsync();
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            backgroundWorker1.ReportProgress(0);
            if (sorgula_1)
            {
                sorgula_1 = false;
                pre_ilk_tab();
            }

            if (sorgula_2_1)
            {
                sorgula_2_1 = false;
                pre_konum_tab2();
            }

            if (sorgula_2_2)
            {
                sorgula_2_2 = false;
                neredeydim();
            }

            if (pre_sorgula_flag)
            {
                pre_sorgula_flag = false;
                pre_sorgula();
            }
            if (yedekle_flag)
            {
                yedekle_flag = false;
                yedekle_DB();
            }
            if (tekrarlari_temizle)
            {
                tekrarlari_temizle = false;
                Tekrarlari_Temizle();
            }

        }

        private void backgroundWorker1_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            progressBar1.Value = e.ProgressPercentage;
        }

        private void backgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            System.Threading.Thread.Sleep(1000);
            progressBar1.Visible = false;

            button1.Enabled = true;
            button2.Enabled = true;
            button3.Enabled = true;
            button4.Enabled = true;
            button10.Enabled = true;
            button11.Enabled = true;
            button12.Enabled = true;

            if (rich_flag)
            {
                rich_link();
                rich_flag = false;
            }

            if (rich_flag_2)
            {
                slink();
                rich_flag_2 = false;
            }

            if (kucuk_lin_flag)
            {
                rich_link();
                kucuk_lin_flag = false;
            }

        }

        private void groupBox1_Enter(object sender, EventArgs e)
        {

        }
    }
}
        