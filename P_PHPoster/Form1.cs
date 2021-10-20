using Newtonsoft.Json;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using PinSharp;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using System.Windows.Forms; 

namespace PinterestToken
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            Control.CheckForIllegalCrossThreadCalls = false; 
        }
        string AccessToken; 
        private async void pintgetir()
        {
            if (access_token.Text == "")
            {
                MessageBox.Show("Access token boş olamaz.");
                return;
            }
            try
            {  
                AccessToken = access_token.Text;

                string boardgetir = "https://api.pinterest.com/v1/boards/"+textBox1.Text+"/pins/?access_token="
                    + AccessToken
                    + "&limit=100&fields=image";

                nextpage:

                WebClient wc = new WebClient();
                Stream oku = wc.OpenRead(boardgetir);
                StreamReader sr = new StreamReader(oku, Encoding.GetEncoding("windows-1254"));



                string gelen = sr.ReadToEnd();
                string HtmlKodum = gelen; 

                dynamic array = JsonConvert.DeserializeObject(HtmlKodum);

                //textBox1.Text = Convert.ToString(array);
                //boardname.Items.Clear();

                //boardid.Items.Clear(); 

                foreach (var item in array)
                {
                    try
                    {  
                        for (int i = 0; i < 1000; i++)
                        {
                            string width = array["data"][i]["image"]["original"]["width"].ToString();
                            string height = array["data"][i]["image"]["original"]["height"].ToString();

                            if (Convert.ToInt32(width)>400 & Convert.ToInt32(height)>400)
                            {
                                string name = array["data"][i]["image"]["original"]["url"].ToString();
                                string id = array["data"][i]["id"].ToString();

                                if (!resimlink.Items.Contains(name))
                                {
                                    resimlink.Items.Add(id + "|" + name);
                                }
                                else
                                {
                                }
                            }

                           
                        } 

                    }
                    catch (Exception )
                    {
                        boardgetir = array["page"]["next"].ToString();
                        goto nextpage;
                    }
                }
            }
            catch (Exception )
            {
                System.IO.File.WriteAllLines(@"C:\Users\ONUR\Desktop\test1.txt", resimlink.Items.Cast<string>().ToArray());

            }

        }

        private void Form1_Load(object sender, EventArgs e)
        { 
        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            Thread thread = new Thread(new ThreadStart(test));
            thread.Start();

            ////Thread thread = new Thread(new ThreadStart(pingönder));
            ////thread.Start();
        }
        void test()
        {
            

        }
        private async void pingönder()
        {
            boardname.Text = "";
            Thread thread = new Thread(new ThreadStart(BoardGetir));
            thread.Start();
            bekle:
            if (boardname.Text == "")
            {
                Thread.Sleep(100);
                goto bekle;
            }
            string boardad = boardname.Text.Split('/').Last();

            var client = new PinSharpClient(access_token.Text);

            for (int i = 0; i < resimlink.Items.Count; i++)
            {
                Random rastgele = new Random();
                string harfler = "ABCDEFGHIJKLMNOPRSsTUVYZabcCdefghiklmnoprstuvyz123456789";
                string kelime = "";
                for (int j = 0; j < 15; j++)
                {
                    kelime += harfler[rastgele.Next(harfler.Length)];
                }

                try
                {
                    postPinterest(access_token.Text, boardad, " ", resimlink.Items[i].ToString(), "https://deneme.com" + kelime);
                }
                catch (Exception)
                {
                }
            }
            MessageBox.Show("Pinler gönderilmiştir.");
        } 
        
        public string postPinterest(string access_token, string boardname, string note, string image_url, string linki)
        {

            string pinSharesEndPoint = "https://api.pinterest.com/v1/pins/?access_token={0}";

            var requestUrl = String.Format(pinSharesEndPoint, access_token);
            var message = new
            {
                board = boardname,
                note = note,
                image_url = image_url,
                link = linki
            };

            var requestJson = new JavaScriptSerializer().Serialize(message);
            var client = new WebClient();
            var requestHeaders = new NameValueCollection
                {
                    {"Content-Type", "application/json" },
                            {"x-li-format", "json" }

                };
            client.Headers.Add(requestHeaders);
            var responseJson = client.UploadString(requestUrl, "POST", requestJson);
            var response = new JavaScriptSerializer().Deserialize<Dictionary<string, object>>(responseJson);
            return Convert.ToString(response);
            //if (!Page.IsPostBack)
            //{
            //    postPinterest("AhM7TM2ZrbfGWNNDOCP-C2JhyvmkFfMfezm807FDVg1B8YApQQnrgDAAAyl7Rom6ofZAuC4AAAAA", "fredrayagi/grüne-küche", "Do not forget to stop by our site for more!", "https://i.pinimg.com/564x/2e/18/70/2e18700f6e98472b3986f4a4afda412a.jpg", "https://aesthetic-wallpaper.blogspot.com/");
            //}
        }
        void BoardGetir()
        {
            if (access_token.Text == "")
            {
                MessageBox.Show("Access token boş olamaz.");
                return;
            }
            AccessToken = access_token.Text;
            try
            {
                string boardgetir = "https://api.pinterest.com/v1/me/boards/?access_token="
                    + AccessToken
                    + "&fields=id%2Cname";

                WebClient wc = new WebClient();
                Stream oku = wc.OpenRead(boardgetir);
                StreamReader sr = new StreamReader(oku, Encoding.GetEncoding("windows-1254"));



                string gelen = sr.ReadToEnd();
                string HtmlKodum = gelen;
                HtmlKodum = (HtmlKodum).Replace(@"\u00f6", "ö");
                HtmlKodum = (HtmlKodum).Replace(@"\u00f", "ü");
                HtmlKodum = (HtmlKodum).Replace(@"\u00e4", "ä");
                HtmlKodum = (HtmlKodum).Replace(@"\u00df", "ß");
                HtmlKodum = (HtmlKodum).Replace(@"\u00c4", "Ä");


                dynamic array = JsonConvert.DeserializeObject(HtmlKodum);
                boardname.Items.Clear();
                foreach (var item in array)
                {
                    try
                    {
                        for (int i = 0; i < 10; i++)
                        {
                            string id = array["data"][i]["id"].ToString();
                            string name = array["data"][i]["name"].ToString();
                            boardname.Items.Add(name + "/" + id);
                        }
                    }
                    catch (Exception)
                    {
                    }
                }
                try
                {
                    boardname.SelectedIndex = 0;
                }
                catch (Exception)
                { 
                }
            }
            catch (Exception)
            {
            }
        }
        private void Getir_Click_1(object sender, EventArgs e)
        {
            Thread thread = new Thread(new ThreadStart(BoardGetir));
            thread.Start();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Thread thread = new Thread(new ThreadStart(pintgetir));
            thread.Start();
        }

        private void resimlink_DoubleClick(object sender, EventArgs e)
        {
            string url = resimlink.SelectedItem.ToString();
            System.Diagnostics.Process.Start(url);
        }

        private void button2_Click_1(object sender, EventArgs e)
        {
            Thread thread = new Thread(new ThreadStart(boardarama));
            thread.Start();
        }
        string boardgetir;

        void boardarama()
        {
            if (boardkelime.Text == "")
            {
                MessageBox.Show("Kelime girilmelidir.");
                return;
            }
            boards.Items.Clear();
            boards.Items.Add("Kelime ile ilgili boardlar çekiliyor.");

            var chromeDriverService = ChromeDriverService.CreateDefaultService();
            chromeDriverService.HideCommandPromptWindow = true;
            var options = new ChromeOptions();
            options.AddArgument("headless");
            options.AddUserProfilePreference("credentials_enable_service", false);
            options.AddUserProfilePreference("profile.password_manager_enabled", false);
            options.AddExcludedArgument("enable-automation");
            options.AddAdditionalCapability("useAutomationExtension", false);
            options.AddUserProfilePreference("profile.default_content_setting_values.images", 2);
            IWebDriver driver = new ChromeDriver(chromeDriverService, options);

            driver.Navigate().GoToUrl("https://tr.pinterest.com/search/boards/?q=test&rs=filter");
            driver.Manage().Cookies.DeleteAllCookies();



            OpenQA.Selenium.Cookie ck = new OpenQA.Selenium.Cookie("_pinterest_sess", "TWc9PSZ5L2hSNjBwa2VqS1VaTlRJZk5HdDRKbDJzNERlTFBpMlpuS284RkNoc2Q3YkJnNVdNNzNEelc0T3M5a3lpZkYvKzA4QmkyU1M2VjFhODBpbTd2d2ZKTUg5Vm9HMmZEL3VyZjVKR0VDaDdZZEYxSHdEUE5jOGZ6NE85dFY1TGFyRVB4Y3FiSFQ4a3ZRbFBHbUdheENJWGt5eU5maCtyaUNSeFBDYmM0UXAxZXd3NFhqT0JYcVVRekljU2huaktpNDlBdmNQNUtZKzVBUjJoT0xGYVdORkM0SHN5WkhPU0pYUWRITUlWaDlKRHBKR3VEdklrUFh6U2o0bUdjVU9YbnF2azJ6bWw2ZURwbzhZTnl3R0dMYVZjQjdCREtLZlEwdVBMRnMxRnZhTXZJdVFuK1B0QmRidmVrYmlHNklYbVU0TUZ2ZVFOMWRKdlJ2MG54NnY3ekNlYUQ4U0xmZnVpNml6ZWxQMkxFMDZIVWFrMWRwaHBKTVJsaHB2VjBjc3hpZVdoZ21pdTZjb3BXNWZXclJqMFdRa3UyZ2g2R0FDRE1HTmM2M29FR29aRU80VXR1ZzlNSGlhcHlzM0VxR251QTF1dGJWWFRkdVo4NkVPTHExSCt3dFhDV3dPWkNFMkFJazFLbWVBczQyeEdRbVR4WkNRZktjbnFvZWx1Zyt1YmZPNDZnWlFVSXA2NTBkb05MVFF5cEFCMkdVTnhvK05nS2prMjBjMVJHdjRhcGRic0NRZVI3R3E0dndZbFRzbmF3c0pPR0VuaVJvdmNwc1czSEoxN0VSMnpJYmczVHR6SGtadFc1M0N3SVpMb3dpRXpQZCtsSGVvZGlaRXRiVnpzeHFRSlV2cldVbUErSFAxWlhRajlydnJjemZpSy85YWtDeFpySWFGNFJvQWppdURtTjYvNVNodU1qTFFtblFZUDFkTmVESy8yWW1JUXR6QmhDQ05GWm1ySjl4bXJWTW5yc0ZVcjdub0ltZ3J1OHFjdHFBR0ZLS3QzWWpIRjU5YmF2OVhidWZLWGpGVFU2VFRQOWhza0Y1OU1aRVhxZ3pjZ1BsMXlURlFZT1l3WnF4ODMwRXR2L0pqb0gwb1I0ZFNzb2xEJi9rVk9iVEdXT1dpN3NlTTJPWFVJNC9hcmlPST0=");
            driver.Manage().Cookies.AddCookie(ck);


            driver.Navigate().GoToUrl("https://tr.pinterest.com/search/boards/?q="+boardkelime.Text+"&rs=filter");

            IJavaScriptExecutor js = (IJavaScriptExecutor)driver;
            js.ExecuteScript("window.scrollBy(0,22250)", ""); 
            Thread.Sleep(500); 
            js.ExecuteScript("window.scrollBy(0,22250)", "");
            Thread.Sleep(1500);
            js.ExecuteScript("window.scrollBy(0,22250)", ""); 
            Thread.Sleep(2000);

            boards.Items.Clear();

            ReadOnlyCollection<IWebElement> lists1 = driver.FindElements(By.ClassName("boardLinkWrapper"));

            try
            {
                for (int i = 0; i < 5000; i++)
                {
                    string link = lists1[i].GetAttribute("href").Replace("https://tr.pinterest.com/", ""); 
                    link = link.Substring(0, link.Length - 1);

                    boards.Items.Add(link);
                }
            }
            catch (Exception)
            { 
            }
            driver.Quit();

            label2.Text= "Çekilen board sayısı: " + boards.Items.Count.ToString() +" --> pinler çekilmeye başlandı.";

            if (access_token.Text == "")
            {
                MessageBox.Show("Access token boş olamaz.");
                return;
            }
            try
            {
                for (int j = 0; j < boards.Items.Count; j++)
                { 

                    AccessToken = access_token.Text;

                    boardgetir = "https://api.pinterest.com/v1/boards/" + boards.Items[j].ToString() + "/pins/?access_token="
                        + AccessToken
                        + "&limit=100&fields=image";

                    WebClient wc = new WebClient();
                    Stream oku = wc.OpenRead(boardgetir);
                    StreamReader sr = new StreamReader(oku, Encoding.GetEncoding("windows-1254"));



                    string gelen = sr.ReadToEnd();
                    string HtmlKodum = gelen;

                    dynamic array = JsonConvert.DeserializeObject(HtmlKodum);

                    foreach (var item in array)
                    {
                        try
                        {
                            for (int i = 0; i < 1000; i++)
                            {
                                if (resimlink.Items.Count >= Convert.ToInt32(kacpin.Text))
                                { 
                                    break;
                                }
                                string name = array["data"][i]["image"]["original"]["url"].ToString();


                                if (!resimlink.Items.Contains(name))
                                {
                                    resimlink.Items.Add(name);
                                    label3.Text = "Çekilen pin sayısı: "+resimlink.Items.Count.ToString(); 
                                }
                                else
                                {
                                }
                                
                            } 
                        }
                        catch (Exception)
                        {
                        }
                    }
                    if (resimlink.Items.Count > Convert.ToInt32(kacpin.Text))
                    {
                        break;
                    }
                }
            }
            catch (Exception)
            {

            }



        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            browserkapat();
        }
        void browserkapat()
        {
            try
            {
                Process[] p;
                p = Process.GetProcessesByName("chromedriver");
                if (p.Length > 0)
                {
                    foreach (Process process in p)
                    {
                        process.Kill();
                    }
                    Process[] p2;
                    p2 = Process.GetProcessesByName("reset"); if (p2.Length > 0)
                    {
                        foreach (Process process in p2)
                        {
                            process.Kill();
                        }
                    }
                }
                p = Process.GetProcessesByName("chrome");
                if (p.Length > 0)
                {
                    foreach (Process process in p)
                    {
                        process.Kill();
                    }
                    Process[] p2;
                    p2 = Process.GetProcessesByName("reset"); if (p2.Length > 0)
                    {
                        foreach (Process process in p2)
                        {
                            process.Kill();
                        }
                    }
                }
                p = Process.GetProcessesByName("PinterestToken");
                if (p.Length > 0)
                {
                    foreach (Process process in p)
                    {
                        process.Kill();
                    }
                    Process[] p3;
                    p3 = Process.GetProcessesByName("reset"); if (p3.Length > 0)
                    {
                        foreach (Process process in p3)
                        {
                            process.Kill();
                        }
                    }
                }
            }
            catch (Exception)
            {
            }
        }
        int[] sayılar;
        int randmboard;
        string boardgetirr; 
        private void button3_Click(object sender, EventArgs e)
        { 
            Thread th1 = new Thread(new ThreadStart(pinsap)); 

            th1.Start(); 
        }
        string yazilimdurumu; 
        void pinsap()
        {
            int hesapno; 
            for (int i = 0; i < Hesap.RowCount-1; i++)
            {
                hesapno = i;

                string sonpintarih = Hesap.Rows[hesapno].Cells[5].Value.ToString().Replace(Environment.NewLine, "");

                if (sonpintarih=="")
                {
                }
                else
                {
                    TimeSpan diff = DateTime.Now - Convert.ToDateTime(sonpintarih); 
                    if (diff.Days==0 && diff.Hours>=24)
                    {
                        goto tekrarboard;
                    }
                    else
                    {
                        goto bitti2;
                    }
                }
                
                

                tekrarboard:
                ArrayList aList = new ArrayList();
                aList.Clear(); 

                Random yeni = new Random();
                for (int j = 0; j < 8; j++)
                {
                    hesapno = i;
                    try
                    {
                        string access = Hesap.Rows[hesapno].Cells[2].Value.ToString().Replace(Environment.NewLine, "");


                        Random board = new Random();
                        int randmboardlar = board.Next(0, 3);
                        if (randmboardlar == 0)
                        {
                            randmboard = yeni.Next(0, boardlar.Items.Count);
                            boardgetirr = "https://api.pinterest.com/v1/boards/" + boardlar.Items[randmboard].ToString() + "/pins/?access_token=" + access.ToString() + "&limit=100&fields=image";
                        }
                        if (randmboardlar == 1)
                        {
                            randmboard = yeni.Next(0, boardlar2.Items.Count);
                            boardgetirr = "https://api.pinterest.com/v1/boards/" + boardlar2.Items[randmboard].ToString() + "/pins/?access_token=" + access.ToString() + "&limit=100&fields=image";
                        }
                        if (randmboardlar == 2)
                        {
                            randmboard = yeni.Next(0, boardlar3.Items.Count);
                            boardgetirr = "https://api.pinterest.com/v1/boards/" + boardlar3.Items[randmboard].ToString() + "/pins/?access_token=" + access.ToString() + "&limit=100&fields=image";
                        }

                        WebClient wc = new WebClient();
                        wc.UseDefaultCredentials = false;
                        wc.Credentials = CredentialCache.DefaultCredentials;

                        Stream oku = wc.OpenRead(boardgetirr);
                        StreamReader sr = new StreamReader(oku, Encoding.GetEncoding("windows-1254"));


                        string gelen = sr.ReadToEnd();
                        string HtmlKodum = gelen;

                        dynamic array = JsonConvert.DeserializeObject(HtmlKodum);


                        foreach (var item in array)
                        {
                            try
                            {
                                for (int k = 0; k < 1000; k++)
                                {
                                    string name = array["data"][k]["image"]["original"]["url"].ToString();


                                    if (!aList.Contains(name))
                                    {
                                        aList.Add(name);
                                    }

                                }
                            }
                            catch (Exception)
                            {
                            }
                        }
                    }
                    catch (Exception)
                    {
                    }
                }
                if (aList.Count < 32)
                {
                    goto tekrarboard;
                }


                id = "";

                try
                {
                    hesapno = i;
                    string access = Hesap.Rows[hesapno].Cells[2].Value.ToString().Replace(Environment.NewLine, "");

                    string boardgetir = "https://api.pinterest.com/v1/me/boards/?access_token="
                        + access
                        + "&fields=id%2Cname";


                    WebClient wc = new WebClient();
                    Stream oku = wc.OpenRead(boardgetir);
                    StreamReader sr = new StreamReader(oku, Encoding.GetEncoding("windows-1254"));



                    string gelen = sr.ReadToEnd();
                    string HtmlKodum = gelen;
                    dynamic array = JsonConvert.DeserializeObject(HtmlKodum);

                    foreach (var item in array)
                    {
                        try
                        {
                            id = array["data"][0]["id"].ToString();
                        }
                        catch (Exception)
                        {
                        }
                    }
                }
                catch (Exception)
                {
                }

                if (id == "")
                {
                    MessageBox.Show("Board yok kontrol ediniz.");
                }

                Random pin = new Random();
                int pinsayi = pin.Next(32, 32);
                try
                {
                    sayılar = new int[pinsayi];

                    Random rnd = new Random();
                    int sayi;

                    int sayac = 0;

                    while (sayac < pinsayi)
                    {
                        sayi = rnd.Next(0, aList.Count);
                        if (Array.IndexOf(sayılar, sayi) == -1)
                        {
                            sayılar[sayac] = sayi;
                            sayac++;
                        }
                    }
                }
                catch (Exception)
                {
                }

                for (int m = 0; m < pinsayi; m++)
                {
                    Random rastgele = new Random();
                    string harfler = "ABCDEFGHIJKLMNOPRSsTUVYZabcCdefghiklmnoprstuvyz123456789";
                    string kelime = "";
                    for (int j = 0; j < 25; j++)
                    {
                        kelime += harfler[rastgele.Next(harfler.Length)];
                    }


                    tekraretiket:
                    string aciklama = "";
                    Random etiket = new Random();
                    for (int y = 0; y < 8; y++)
                    {
                        int etikett = etiket.Next(0, aciklamaicinetiket.Items.Count);

                        aciklama += " #" + aciklamaicinetiket.Items[etikett].ToString();

                    }
                    if (aciklama.Length > 480)
                    {
                        goto tekraretiket;
                    }


                    try
                    {
                        hesapno = i;
                        string access = Hesap.Rows[hesapno].Cells[2].Value.ToString().Replace(Environment.NewLine, "");
                        string website = Hesap.Rows[hesapno].Cells[4].Value.ToString().Replace(Environment.NewLine, "");

                        try
                        {
                            postPinterest(access, id, aciklama, aList[sayılar[m]].ToString(), website +"/"+ kelime);
                        }
                        catch (Exception)
                        {
                            DataGridViewRow limit = Hesap.Rows[hesapno];
                            limit.Cells[3].Value = Convert.ToString(m + 1) + "/" + "32 (Limit)";
                            goto bitti;
                        }

                        DataGridViewRow newDataRoew = Hesap.Rows[hesapno];
                        newDataRoew.Cells[3].Value = Convert.ToString(m + 1) + "/" + "32";

                        int etikettt = etiket.Next(0, 500);
                        Thread.Sleep(etikettt);
                    }
                    catch (Exception)
                    {
                    } 
                }
                bitti:

                hesapno = i;
                DataGridViewRow newDataRoew2 = Hesap.Rows[hesapno];
                newDataRoew2.Cells[5].Value = DateTime.Now.ToString();


                string ID = Hesap.Rows[hesapno].Cells[0].Value.ToString();
                string KullaniciAdi = Hesap.Rows[hesapno].Cells[1].Value.ToString();
                string AccessToken = Hesap.Rows[hesapno].Cells[2].Value.ToString();
                string YazilimDurumu = Hesap.Rows[hesapno].Cells[3].Value.ToString();
                string Website = Hesap.Rows[hesapno].Cells[4].Value.ToString();
                string SonAtilanPin = Hesap.Rows[hesapno].Cells[5].Value.ToString();

                cmd = new SqlCommand();
                con.Open();
                cmd.Connection = con; 
                cmd.CommandText = "update PinYazilim set KullaniciAdi='" + KullaniciAdi + "',AccessToken='" + AccessToken + "' " +
                    ",YazilimDurumu='" + YazilimDurumu + "',Website='" + Website + "',SonAtilanPin='" + SonAtilanPin + "' where ID=" + ID + "";
                cmd.ExecuteNonQuery();
                con.Close();

                bitti2:
                Thread.Sleep(100);
            }
        }


        SqlConnection con;
        SqlDataAdapter da;
        SqlCommand cmd;
        DataSet ds;
        void gridgetir()
        {
            con = new SqlConnection("server=Onurerdemiroglu; Initial Catalog=YAZILIM;Integrated Security=SSPI");
            da = new SqlDataAdapter("Select * From PinYazilim", con);
            ds = new DataSet();
            con.Open();
            da.Fill(ds, "PinYazilim");
            Hesap.DataSource = ds.Tables["PinYazilim"];
            con.Close();
            try
            {
                Hesap.Columns[0].Width = 40;
                Hesap.Columns[1].Width = 150;
                Hesap.Columns[2].Width = 350;
                Hesap.Columns[3].Width = 145;
                Hesap.Columns[4].Width = 145;
                Hesap.Columns[5].Width = 145;
            }
            catch (Exception)
            { 
            }  
        }
        void griddoldur()
        {
         
            tekrar:
            try
            {
                cmd = new SqlCommand();
                con.Open();
                cmd.Connection = con;

                for (int i = 0; i < Hesap.RowCount; i++)
                {
                    string ID = Hesap.Rows[i].Cells[0].Value.ToString();
                    string KullaniciAdi = Hesap.Rows[i].Cells[1].Value.ToString();
                    string AccessToken = Hesap.Rows[i].Cells[2].Value.ToString();
                    string YazilimDurumu = Hesap.Rows[i].Cells[3].Value.ToString(); 
                    string Website = Hesap.Rows[i].Cells[4].Value.ToString();
                    string SonAtilanPin = Hesap.Rows[i].Cells[5].Value.ToString();

                    cmd.CommandText = "update PinYazilim set KullaniciAdi='" + KullaniciAdi + "',AccessToken='" + AccessToken + "' " +
                        ",YazilimDurumu='" + YazilimDurumu + "',Website='" + Website + "',SonAtilanPin='" + SonAtilanPin + "' where ID=" + ID + "";
                    cmd.ExecuteNonQuery();
                }

                con.Close();
                gridgetir();
            }
            catch (Exception)
            {
                con.Close();
                goto tekrar;
            }
           

        }
        private void Form1_Load_1(object sender, EventArgs e)
        {
            gridgetir();
            //Hesap.Columns.Item("AccessToken").Width = 40;

            kacpin.SelectedIndex = 179;
            boardlar.DataSource = File.ReadAllLines(Application.StartupPath + @"\Home-Decor Board.txt");
            boardlar2.DataSource = File.ReadAllLines(Application.StartupPath + @"\Nail.txt");
            boardlar3.DataSource = File.ReadAllLines(Application.StartupPath + @"\Garden diy.txt");
            hesapaccess.DataSource = File.ReadAllLines(Application.StartupPath + @"\access.txt");
        }
        string id;
         

        private void button5_Click(object sender, EventArgs e)
        {
            Thread thread = new Thread(new ThreadStart(pa));
            thread.Start(); 
        }
        
        void pa()
        {
            resimlink.Items.Clear();

            tekrarboard:
            for (int j = 0; j < 8; j++)
            {
                try
                {
                    Random yeni = new Random(); 
                    randmboard = yeni.Next(0, boardlar.Items.Count); 
                    boardgetirr = "https://api.pinterest.com/v1/boards/" + boardlar.Items[randmboard].ToString() + "/pins/?access_token="
                         + access_token.Text
                         + "&limit=100&fields=image";

                    WebClient wc = new WebClient();
                    Stream oku = wc.OpenRead(boardgetirr);
                    StreamReader sr = new StreamReader(oku, Encoding.GetEncoding("windows-1254"));


                    string gelen = sr.ReadToEnd();
                    string HtmlKodum = gelen;

                    dynamic array = JsonConvert.DeserializeObject(HtmlKodum);

                    foreach (var item in array)
                    {
                        try
                        {
                            for (int k = 0; k < 1000; k++)
                            {
                                string name = array["data"][k]["image"]["original"]["url"].ToString();
                                if (!resimlink.Items.Contains(name))
                                {
                                    resimlink.Items.Add(name);
                                }
                            }
                        }
                        catch (Exception)
                        {
                        }
                    }
                    }
                    catch (Exception)
                    {
                    }
                }
            if (resimlink.Items.Count < 180)
            {
                goto tekrarboard;
            }

            try
            {
                sayılar = new int[2];

                Random rnd = new Random();
                int sayi;

                int sayac = 0;

                while (sayac < 2)
                {
                    sayi = rnd.Next(0, resimlink.Items.Count);
                    if (Array.IndexOf(sayılar, sayi) == -1)
                    {
                        sayılar[sayac] = sayi;
                        sayac++;
                    }
                }
            }
            catch (Exception)
            {
            }


            try
            {
                WebClient webClient = new WebClient();
                webClient.DownloadFile(resimlink.Items[sayılar[0]].ToString(), "image.png");
                webClient.DownloadFile(resimlink.Items[sayılar[1]].ToString(), "image2.png");
                webClient.Dispose();
            }
            catch (Exception)
            {
            }
            try
            {
                String jpg1 = "image.png";
                String jpg2 = "image2.png";

                Image first = Image.FromFile(jpg1);
                Image second = Image.FromFile(jpg2);

                int outputImageWidth = first.Width > second.Width ? first.Width : second.Width;

                int outputImageHeight = first.Height + second.Height + 1;

                Bitmap outputImage = new Bitmap(outputImageWidth, outputImageHeight, System.Drawing.Imaging.PixelFormat.Format32bppArgb);

                using (Graphics graphics = Graphics.FromImage(outputImage))
                {
                    graphics.DrawImage(first, new Rectangle(new Point(), first.Size),
                        new Rectangle(new Point(), first.Size), GraphicsUnit.Pixel);
                    graphics.DrawImage(second, new Rectangle(new Point(0, first.Height + 1), second.Size),
                        new Rectangle(new Point(), second.Size), GraphicsUnit.Pixel);
                }
                first.Dispose();
                second.Dispose();
                outputImage.Save("image3.jpg");
            }
            catch (Exception)
            {
            }

            var chromeDriverService = ChromeDriverService.CreateDefaultService();
            chromeDriverService.HideCommandPromptWindow = true;
            var options = new ChromeOptions();
            //options.AddArgument("headless");
            options.AddUserProfilePreference("credentials_enable_service", false);
            options.AddUserProfilePreference("profile.password_manager_enabled", false);
            options.AddExcludedArgument("enable-automation");
            options.AddAdditionalCapability("useAutomationExtension", false);
            IWebDriver driver = new ChromeDriver(chromeDriverService, options);

            driver.Navigate().GoToUrl("https://tr.pinterest.com/");
            driver.Manage().Cookies.DeleteAllCookies();



            OpenQA.Selenium.Cookie ck = new OpenQA.Selenium.Cookie("_pinterest_sess", session.Text);
            driver.Manage().Cookies.AddCookie(ck);


            driver.Navigate().GoToUrl("https://tr.pinterest.com/pin-builder/");

            bekle:
            try
            {
                driver.FindElement(By.XPath("//textarea[@placeholder='Add your title']")).SendKeys("test");
                Thread.Sleep(3000);
                driver.FindElement(By.XPath("//textarea[@placeholder='Tell everyone what your Pin is about']")).SendKeys("test");
                Thread.Sleep(7000);


                var ElementToClick = driver.FindElement(By.Id("media-upload-input"));
                ElementToClick.SendKeys(Application.StartupPath + @"\image3.jpg");

                Thread.Sleep(4000);

                driver.FindElement(By.XPath("//button[@data-test-id='board-dropdown-select-button']")).Click();
                Thread.Sleep(2000);
                driver.FindElement(By.XPath("//div[@title='Wall Decors']")).Click();
                Thread.Sleep(2000);
                driver.FindElement(By.XPath("//button[@data-test-id='board-dropdown-save-button']")).Click();


            }
            catch (Exception)
            {
                Thread.Sleep(1000);
                goto bekle;
            }

            son:
            try
            {
                driver.FindElement(By.XPath("(//button[@type='button'])[8]"));
                Thread.Sleep(1000); 
            }
            catch (Exception)
            {
                Thread.Sleep(1000);
                goto son;
            }
            driver.Close();
        }
         
        void boardcekme()
        { 
            var chromeDriverService = ChromeDriverService.CreateDefaultService();
            chromeDriverService.HideCommandPromptWindow = true;
            var options = new ChromeOptions();
            options.AddArgument("headless");
            options.AddUserProfilePreference("credentials_enable_service", false);
            options.AddUserProfilePreference("profile.password_manager_enabled", false);
            options.AddExcludedArgument("enable-automation");
            options.AddAdditionalCapability("useAutomationExtension", false);
            options.AddUserProfilePreference("profile.default_content_setting_values.images", 2);
            IWebDriver driver = new ChromeDriver(chromeDriverService, options);

            driver.Navigate().GoToUrl("https://tr.pinterest.com/search/boards/?q=wallpaper&rs=filter");
            driver.Manage().Cookies.DeleteAllCookies();



            OpenQA.Selenium.Cookie ck = new OpenQA.Selenium.Cookie("_pinterest_sess", "TWc9PSZSYlZ0Y3hLRE9yMUlFZDdxMDBGL1JJYjFDdVdZTnlSUW9MTzBwWTcxZEpTamt6Y0pubE5oTzZkWDZmc0N1dWRjRWlwRUNXSlloSGhNRTBFL0FBMVFmSXNmSzhYVCtBQWlpdWk1NmRGVVJ1Sk1DNUhFOGh4MzErRDVIbENBclJHTVlFVXB6YXdpRVRucUJDRHI4ajVXb1A2L1VNYXgwT3VsZ3MydlZmMHpuemtTS1kzeSt3UVFNQUJuKzZzaGhKU2pobGZOL3diUS9vUVFMZFE3ZXNOVkpOZEgyL0xqamZxdWxCSkoyVkZTcWN5SVhCS1ZlL09VbkIyVkJtMWk5VEdVU1IzNTVHeWtSU3h6R2ZPVDlkQTV5UXpHRHRRRXB0YXpYbFFPaEh2NlVGOGVpZ0wzY1FTN0NybkpacEdieU1DYVAyNEYxRmsxbjNCVXhpcUc4TWFlQU0xaTJzUGFIYnYxV29KMnlmTFlLZ1orM0pZWUtETzNSejQ1bXRrQ294eHQwR1lCbXNsclVnYmpMc3ZyWXlwaEtCV1BTN2NBYjdHQ25sKzNlV3hndEhxaTdDMERzRE5iR0c3NmYxdFlJK3NzMm9DRG0vZ0YwWURqSmlDYnN4VlA2Z2ZvWGd3SjBkbG1UemIyWlAwKzhUWng3ZGlwYm42ZEhxNW51NXI5TkZMK1l0b1ZJdVU0dTVqN0tESnBmeFBrUGc9PSY1ZGNud3REUkVMemY5TWs3LzVKOEV3QzVzRTg9");
            driver.Manage().Cookies.AddCookie(ck);

            ReadOnlyCollection<IWebElement> lists1;

            for (int i = 0; i < 50; i++)
            {
                driver.Navigate().GoToUrl("https://tr.pinterest.com/search/boards/?q="+"BOARD"+"&rs=filter");

                IJavaScriptExecutor js = (IJavaScriptExecutor)driver;
                js.ExecuteScript("window.scrollBy(0,22250)", "");
                Thread.Sleep(500);
                js.ExecuteScript("window.scrollBy(0,22250)", "");
                Thread.Sleep(1500);
                js.ExecuteScript("window.scrollBy(0,22250)", "");
                Thread.Sleep(2000);
                lists1 = driver.FindElements(By.ClassName("boardLinkWrapper"));
                try
                {
                    for (int j = 0; j < 5000; j++)
                    {
                        string link = lists1[j].GetAttribute("href").Replace("https://tr.pinterest.com/", "");
                        link = link.Substring(0, link.Length - 1);

                        boards.Items.Add(link);
                    }
                }
                catch (Exception)
                {
                }
            }
            System.IO.File.WriteAllLines(@"C:\Users\ONUR\Desktop\test.txt", boards.Items.Cast<string>().ToArray());
            driver.Quit();

        }
        private void button4_Click_1(object sender, EventArgs e)
        {
            Thread thread = new Thread(new ThreadStart(boardcekme));
            thread.Start();
        }
         

        private void Hesap_CellMouseDoubleClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            string target = "http://www.pinterest.com/" + Hesap.CurrentRow.Cells[1].Value.ToString();
            System.Diagnostics.Process.Start(target);
        } 
    }
}
