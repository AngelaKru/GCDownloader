using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Data;
using System.Data.Common;
using System.Data.Sql;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using System.Windows.Forms;

using Microsoft.Win32;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace GCDownloader
{
    public partial class GCDownloader : Form
    {
        //private CookieContainer _cookieJar;
        private string _username;
        //private string _folder;
        private string _password;
        private int _redirectCount;

        private static string BASE_URL = "https://sso.garmin.com/sso/signin";
        //private static string GAUTH = "https://connect.garmin.com/gauth/hostname";
        //private static string GAUTH = "https://connect.garmin.com/modern/auth/hostname";
        private static string SSO = "https://sso.garmin.com/sso";
        private static string CSS = "https://static.garmincdn.com/com.garmin.connect/ui/css/gauth-custom-v1.2-min.css";
        //private static string REDIRECT = "https://connect.garmin.com/post-auth/login";
        private static string REDIRECT = "https://connect.garmin.com/modern/";
        //private static string ACTIVITIES = "http://connect.garmin.com/proxy/activity-search-service-1.2/json/activities?start={0}&limit={1}";
        private static string ACTIVITIES = "https://connect.garmin.com/modern/proxy/activitylist-service/activities/search/activities?start={0}&limit={1}";
        private static string WELLNESS = "https://connect.garmin.com/modern/proxy/userstats-service/wellness/daily/{0}?fromDate={1}&untilDate={2}";
        private static string DAILYSUMMARY = "https://connect.garmin.com/modern/proxy/wellness-service/wellness/dailySummaryChart/{0}?date={1}";

        private static string TCX = "https://connect.garmin.com/modern/proxy/download-service/export/tcx/activity/{0}";
        private static string GPX = "https://connect.garmin.com/modern/proxy/download-service/export/gpx/activity/{0}";
        private static string ActivityDetail = "https://connect.garmin.com/modern/activity/{0}";

        private static string ActivityTypesURL = "https://connect.garmin.com/modern/proxy/activity-service/activity/activityTypes";
        private static string EventTypesURL = "https://connect.garmin.com/modern/proxy/activity-service/activity/eventTypes";

        private int ActivityBatchStart = 0;
        private int ActivityBatchSize = 10;
        private string DownloadFolder = "";
        private LoginStatus loginStatus = LoginStatus.LoggedOut;

        private GCActivityTypes activityTypes;
        private GCEventTypes eventTypes;

        enum ActivityFormat { TCX, GPX };
        enum LoginStatus { LoggedIn, LoggedOut };

        public string Username { get { return _username; } set { _username = value; } }

        public string Password { get { return _password; } set { _password = value; } }

        public GCActivityTypes ActivityTypes { get { return activityTypes; } set { activityTypes = value; } }
        public GCEventTypes EventTypes { get { return eventTypes; } set { eventTypes = value; } }

        private List<GCActivity> ActivityCache { get; set; }

        private List<GCDailySummary> DailySummaryCache { get; set; }

        #region From the interwebs: https://github.com/bergziege/GarminToolboxNet

        private const string ClientId = "GCDownloader";
        private CookieContainer Cookies = new CookieContainer();

        public bool SignIn(string userName, string password)
        {
            try
            {
                SetStatus("Logging into GarminConnect...");
                HttpWebResponse signInResponse = PostLogInRequest(userName, password);
                string ticketUrl = GetServiceTicketUrl(signInResponse);
                SetStatus("Login successful");
                signInResponse.Close();
                return ProcessTicket(ticketUrl);
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Error signing in. {0}", ex.Message);
                SetStatus(String.Format("Login failed: {0}", ex.Message));
            }

            return false;
        }

        private HttpWebResponse PostLogInRequest(string userName, string password)
        {
            HttpWebRequest request = HttpUtils.CreateRequest(GetLogInUrl(), Cookies);
            request.Headers.Add("origin", "https://sso.garmin.com");
            request.WriteFormData(BuildLogInFormData(userName, password));
            return (HttpWebResponse)request.GetResponse();
        }

        private static string GetLogInUrl()
        {
            var qs = HttpUtils.CreateQueryString();
            //qs.Add("service", "https://connect.garmin.com/post-auth/login");
            qs.Add("service", REDIRECT);
            qs.Add("clientId", ClientId);
            return "https://sso.garmin.com/sso/signin?" + qs;
        }

        private static NameValueCollection BuildLogInFormData(string userName, string password)
        {
            var data = HttpUtils.CreateQueryString();
            data.Add("username", userName);
            data.Add("password", password);
            data.Add("embed", "true");
            data.Add("_eventId", "submit");
            return data;
        }

        private bool ProcessTicket(string ticketUrl)
        {
            HttpWebRequest request = HttpUtils.CreateRequest(ticketUrl, Cookies);
            request.Headers.Add("origin", "https://sso.garmin.com");
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            if (response.StatusCode != HttpStatusCode.OK)
                throw new Exception("Invalid ticket URL.");

            //bool retVal = IsDashboardUri(response.ResponseUri);
            response.Close();
            return true;
        }

        private static bool IsDashboardUri(Uri uri)
        {
            if (uri != null && uri.LocalPath == "/dashboard")
                return uri.Host == "connect.garmin.com" && uri.LocalPath == "/dashboard";
            else
                return uri.Host == "connect.garmin.com" && uri.LocalPath == "/modern/";
        }

        private string GetServiceTicketUrl(HttpWebResponse signInResponse)
        {
            var content = signInResponse.GetResponseAsString();
            return ParseServiceTicketUrl(content);
        }

        private static string ParseServiceTicketUrl(string content)
        {
            // var response_url                 = 'http://connect.garmin.com/post-auth/login?ticket=ST-XXXXXX-XXXXXXXXXXXXXXXXXXXX-cas';
            content = content.Replace("\\", "");
            content = content.Replace("\"", "'");
            var re = new Regex(@"response_url\s*=\s*'(?<url>[^']*)'");
            var m = re.Match(content);
            if (!m.Success)
                throw new Exception("Servcie ticket URL not found.");

            return m.Groups["url"].Value;
        }

        public void SignOut()
        {
            HttpWebRequest request = HttpUtils.CreateRequest("https://sso.garmin.com/sso/logout?service=http%3A%2F%2Fconnect.garmin.com%2F", Cookies);
            request.Headers.Add("origin", "https://sso.garmin.com");
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            response.Close();
            loginStatus = LoginStatus.LoggedOut;
        }

        #endregion

        #region From the interwebs: http://www.seirer.net/blog/2014/6/10/solved-how-to-open-a-url-in-the-default-browser-in-csharp

        public static string GetDefaultBrowserPath()
        {
            string urlAssociation = @"Software\Microsoft\Windows\Shell\Associations\UrlAssociations\http";
            string browserPathKey = @"$BROWSER$\shell\open\command";

            RegistryKey userChoiceKey = null;
            string browserPath = "";

            try
            {
                //Read default browser path from userChoiceLKey
                userChoiceKey = Registry.CurrentUser.OpenSubKey(urlAssociation + @"\UserChoice", false);

                //If user choice was not found, try machine default
                if (userChoiceKey == null)
                {
                    //Read default browser path from Win XP registry key
                    var browserKey = Registry.ClassesRoot.OpenSubKey(@"HTTP\shell\open\command", false);

                    //If browser path wasn’t found, try Win Vista (and newer) registry key
                    if (browserKey == null)
                    {
                        browserKey =
                        Registry.CurrentUser.OpenSubKey(
                        urlAssociation, false);
                    }
                    var path = CleanifyBrowserPath(browserKey.GetValue(null) as string);
                    browserKey.Close();
                    return path;
                }
                else
                {
                    // user defined browser choice was found
                    string progId = (userChoiceKey.GetValue("ProgId").ToString());
                    userChoiceKey.Close();

                    // now look up the path of the executable
                    string concreteBrowserKey = browserPathKey.Replace("$BROWSER$", progId);
                    var kp = Registry.ClassesRoot.OpenSubKey(concreteBrowserKey, false);
                    browserPath = CleanifyBrowserPath(kp.GetValue(null) as string);
                    kp.Close();
                    return browserPath;
                }
            }
            catch (Exception ex)
            {
                return "";
            }
        }

        private static string CleanifyBrowserPath(string p)
        {
            string[] url = p.Split('"');
            string clean = url[1];
            return clean;
        }

        #endregion

        #region Form Controls

        #region Login

        public GCDownloader()
        {
            InitializeComponent();
            //_cookieJar = new CookieContainer();
            Cookies = new CookieContainer();

            toolStripGCDownloader.SendToBack();
            toolStripGCDownloader.Visible = false;
            panelLogin.BringToFront();
            if (txtUsername.Text.Length > 0) txtPassword.Focus();
            else txtUsername.Focus();
        }

        private void GCDownloader_Load(object sender, EventArgs e)
        {
            toolStripGCDownloader.SendToBack();
            SetStatus(message: "Welcome to GCDownloader. Please login to continue.");
            numBatchSize.Value = ActivityBatchSize;
            DoRegCheck();
            if (txtUsername.Text.Length > 0) txtPassword.Focus();
            else txtUsername.Focus();
            try
            {
                this.Text = string.Format("GarminConnect Downloader v{0}", System.Deployment.Application.ApplicationDeployment.CurrentDeployment.CurrentVersion);
            }
            catch
            {
                this.Text = string.Format("GarminConnect Downloader v{0}", Application.ProductVersion);
            }
        }

        private void txtUsername_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter)
            {
                txtPassword.Focus();
            }
        }

        private void txtPassword_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter)
            {
                GCAuthenticate();
            }
        }

        private void btnLogin_Click(object sender, EventArgs e)
        {
            statusMessage.Text = "Attempting to login...";
            GCAuthenticate();
        }

        private void GCDownloader_FormClosing(object sender, FormClosingEventArgs e)
        {
            DoRegCheck(Username, Password);
        }

        #endregion

        #region Activities

        private void btnPrevious_Click(object sender, EventArgs e)
        {
            ActivityBatchStart -= ActivityBatchSize;
            if (ActivityBatchStart < 0) ActivityBatchStart = 0;
            RefreshList();
        }

        private void btnNext_Click(object sender, EventArgs e)
        {
            ActivityBatchStart += ActivityBatchSize;
            RefreshList();
        }

        private void numBatchSize_ValueChanged(object sender, EventArgs e)
        {
            ActivityBatchSize = Decimal.ToInt32(numBatchSize.Value);
            RefreshList();
        }

        private void mniSetDownloadFolder_Click(object sender, EventArgs e)
        {
            using (FolderBrowserDialog fbd = new FolderBrowserDialog())
            {
                if (fbd.ShowDialog(this) == DialogResult.OK) DownloadFolder = fbd.SelectedPath;
            }
        }

        private void mniSelectAll_Click(object sender, EventArgs e)
        {
            if (mniSelectAll.Text == "Select &All")
            {
                for (int i = 0; i < lstActivities.Items.Count; i++) lstActivities.SetSelected(i, true);
                mniSelectAll.Text = "Unselect &All";
            }
            else
            {
                for (int i = 0; i < lstActivities.Items.Count; i++) lstActivities.SetSelected(i, false);
                mniSelectAll.Text = "Select &All";
            }
        }

        private void mniDownloadSelected_Click(object sender, EventArgs e)
        {
            List<GCActivity> activityList = new List<GCActivity>();
            foreach (object li in lstActivities.SelectedItems)
            {
                activityList.Add((GCActivity)li);
            }

            DownloadActivities(activityList);
        }

        private void lstActivities_DoubleClick(object sender, EventArgs e)
        {
            List<GCActivity> activities = new List<GCActivity>();
            activities.Add((GCActivity)lstActivities.SelectedItem);
            DownloadActivities(activities);
        }

        private void mniRefresh_Click(object sender, EventArgs e)
        {
            RefreshList();
        }

        private void lstActivities_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lstActivities.SelectedItem != null) PreviewActivity((GCActivity)lstActivities.SelectedItem);
            else PreviewActivity(null);
        }

        private void mniOpenSelected_Click(object sender, EventArgs e)
        {
            if (lstActivities.SelectedItems.Count == 1)
            {
                Process.Start(GetDefaultBrowserPath(), string.Format(ActivityDetail, ((GCActivity)lstActivities.SelectedItem).ActivityID));
            }
            else if (lstActivities.SelectedItems.Count == 0)
            {
                MessageBox.Show("No activity selected. Please select an activity to open.", "GarminConnect Downloader", MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
            }
            else
            {
                MessageBox.Show("Multiple activities selected. Please select only 1 activity to open.", "GarminConnect Downloader", MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
            }
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            RefreshList();
        }

        private void mniClearCache_Click(object sender, EventArgs e)
        {
            ActivityCache = new List<GCActivity>();
        }

        private void mniSignout_Click(object sender, EventArgs e)
        {
            RedoLogin();
        }

        private void lstActivities_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.C && e.Modifiers == Keys.Control && lstActivities.SelectedItems.Count == 1)
                Clipboard.SetText(((GCActivity)lstActivities.SelectedItem).ActivityName);
        }

        #endregion

        #region Daily Summary

        private void mniSetDownloadFolderDailySummary_Click(object sender, EventArgs e)
        {

        }

        private void mniSelectAllDailySummary_Click(object sender, EventArgs e)
        {

        }

        private void mniDownloadSelectedDailySummary_Click(object sender, EventArgs e)
        {

        }

        private void mniRefreshDailySummary_Click(object sender, EventArgs e)
        {

        }

        private void mniOpenSelectedDailySummary_Click(object sender, EventArgs e)
        {

        }

        private void mniClearCacheDailySummary_Click(object sender, EventArgs e)
        {

        }

        private void mniLogoutDailySummary_Click(object sender, EventArgs e)
        {
            RedoLogin();
        }

        #endregion

        #region Wellness

        private void mniSetDownloadFolderWellness_Click(object sender, EventArgs e)
        {

        }

        private void mniSelectAllWellness_Click(object sender, EventArgs e)
        {

        }

        private void mniDownloadSelectedWellness_Click(object sender, EventArgs e)
        {

        }

        private void mniRefreshWellness_Click(object sender, EventArgs e)
        {

        }

        private void mniOpenSelectedWellness_Click(object sender, EventArgs e)
        {

        }

        private void mniClearCacheWellness_Click(object sender, EventArgs e)
        {

        }

        private void mniLogoutWellness_Click(object sender, EventArgs e)
        {
            RedoLogin();
        }
        #endregion

        #endregion

        #region Custom Methods

        #region General

        private void SetStatus(string message)
        {
            statusMessage.Text = message;
        }

        #endregion

        #region Login

        private void DoRegCheck(string Username = null, string Password = null)
        {
            RegistryKey rkGDDownloader = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\GCDownloader", true);
            if (rkGDDownloader == null) rkGDDownloader = Registry.CurrentUser.CreateSubKey(@"SOFTWARE\GCDownloader", true);

            string[] regKeyValues = rkGDDownloader.GetValueNames();

            if (Username == null && Password == null) //this is a get operation
            {
                if (Array.IndexOf(regKeyValues, "SaveUsername") >= 0) chkSaveUsername.Checked = bool.Parse(rkGDDownloader.GetValue("SaveUsername").ToString());
                if (Array.IndexOf(regKeyValues, "SavePassword") >= 0) chkSavePassword.Checked = bool.Parse(rkGDDownloader.GetValue("SavePassword").ToString());

                if (Array.IndexOf(regKeyValues, "Username") >= 0) if (chkSaveUsername.Checked) txtUsername.Text = rkGDDownloader.GetValue("Username").ToString();
                if (Array.IndexOf(regKeyValues, "Password") >= 0) if (chkSavePassword.Checked) txtPassword.Text = rkGDDownloader.GetValue("Password").ToString();
                if (Array.IndexOf(regKeyValues, "ActivityBatchSize") >= 0) ActivityBatchSize = Int32.Parse(rkGDDownloader.GetValue("ActivityBatchSize").ToString());
                if (Array.IndexOf(regKeyValues, "DownloadFolder") >= 0) DownloadFolder = rkGDDownloader.GetValue("DownloadFolder").ToString();

                numBatchSize.Value = ActivityBatchSize;
            }
            else //this is a set operation
            {
                rkGDDownloader.SetValue("SaveUsername", chkSaveUsername.Checked ? "true" : "false");
                rkGDDownloader.SetValue("SavePassword", chkSavePassword.Checked ? "true" : "false");
                rkGDDownloader.SetValue("ActivityBatchSize", ActivityBatchSize);
                if (DownloadFolder.Length > 0) rkGDDownloader.SetValue("DownloadFolder", DownloadFolder);

                if (chkSaveUsername.Checked) rkGDDownloader.SetValue("Username", Username);
                else rkGDDownloader.SetValue("Username", "");

                if (chkSavePassword.Checked) rkGDDownloader.SetValue("Password", Password);
                else rkGDDownloader.SetValue("Password", "");
            }
        }

        private bool GCAuthenticate()
        {
            if (txtUsername.Text.Trim().Length > 0) Username = txtUsername.Text.Trim();
            else throw new Exception(message: String.Format("Missing username. Please enter a valid username."));
            if (txtPassword.Text.Trim().Length > 0) Password = txtPassword.Text.Trim();
            else throw new Exception(message: String.Format("Missing password. Please enter a valid password."));

            loginStatus = LoginStatus.LoggedOut;
            bool SignInStatus = SignIn(userName: Username, password: Password);
            if (SignInStatus) SignInStatus = GetActivityTypes();
            if (SignInStatus) SignInStatus = GetEventTypes();

            if (SignInStatus)
            {
                toolStripGCDownloader.SendToBack();
                loginStatus = LoginStatus.LoggedIn;
                DoRegCheck(Username, Password);
                ActivityBatchStart = 0;
                toolStripGCDownloader.SendToBack();
                toolStripGCDownloader.Visible = true;
                panelActivities.BringToFront();
                GetActivityList(ActivityBatchStart, ActivityBatchSize);
                if (lstActivities.Items.Count > 0) lstActivities.SetSelected(0, true);
                lstActivities.Select();
            }
            else
            {
                txtPassword.Focus();
                txtPassword.SelectAll();
            }

            return SignInStatus;
        }

        private void RedoLogin()
        {
            SignOut();
            SetStatus("Signed out.");
            toolStripGCDownloader.SendToBack();
            toolStripGCDownloader.Visible = false;
            panelLogin.BringToFront();
            txtPassword.Text = "";
            DoRegCheck();
            if (txtUsername.Text.Length > 0) txtPassword.Focus();
            else txtUsername.Focus();
        }

        #endregion

        #region Activities

        private bool GetActivityTypes()
        {
            try
            {
                HttpWebRequest request = HttpUtils.CreateRequest(ActivityTypesURL, Cookies);
                request.Headers.Add("origin", "https://sso.garmin.com");
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                string responseStr = HttpUtils.GetResponseAsString(response);
                response.Close();

                //ActivityTypes = (GCActivityTypes)JsonConvert.DeserializeObject<GCActivityTypes>(responseStr);
                activityTypes = new GCActivityTypes();
                MemoryStream ms = new MemoryStream(Encoding.UTF8.GetBytes(String.Format(@"{{""activityTypes"":{0}}}", responseStr)));
                DataContractJsonSerializer ser = new DataContractJsonSerializer(activityTypes.GetType());
                activityTypes = ser.ReadObject(ms) as GCActivityTypes;
                ms.Close();
                return true;
            }
            catch (Exception ex)
            {
                if (ex.Message.Equals("The remote server returned an error: (400) Bad Request."))
                {
                    //manually load activity types
                    string activityTypeJsonStri = @"{""dictionary"": [
  {
    ""key"": ""all"",
    ""display"": ""Any Activity Type"",
    ""type"": {
      ""key"": ""all"",
      ""display"": ""Any Activity Type""
    }
  },
  {
    ""key"": ""uncategorized"",
    ""display"": ""Uncategorized"",
    ""parent"": {
      ""key"": ""all"",
      ""display"": ""Any Activity Type""
    },
    ""type"": {
      ""key"": ""uncategorized"",
      ""display"": ""Uncategorized""
    }
  },
  {
    ""key"": ""running"",
    ""display"": ""Running"",
    ""parent"": {
      ""key"": ""all"",
      ""display"": ""Any Activity Type""
    },
    ""type"": {
      ""key"": ""running"",
      ""display"": ""Running""
    }
  },
  {
    ""key"": ""street_running"",
    ""display"": ""Street Running"",
    ""parent"": {
      ""key"": ""running"",
      ""display"": ""Running""
    },
    ""type"": {
      ""key"": ""running"",
      ""display"": ""Running""
    }
  },
  {
    ""key"": ""track_running"",
    ""display"": ""Track Running"",
    ""parent"": {
      ""key"": ""running"",
      ""display"": ""Running""
    },
    ""type"": {
      ""key"": ""running"",
      ""display"": ""Running""
    }
  },
  {
    ""key"": ""trail_running"",
    ""display"": ""Trail Running"",
    ""parent"": {
      ""key"": ""running"",
      ""display"": ""Running""
    },
    ""type"": {
      ""key"": ""running"",
      ""display"": ""Running""
    }
  },
  {
    ""key"": ""treadmill_running"",
    ""display"": ""Treadmill Running"",
    ""parent"": {
      ""key"": ""running"",
      ""display"": ""Running""
    },
    ""type"": {
      ""key"": ""running"",
      ""display"": ""Running""
    }
  },
  {
    ""key"": ""cycling"",
    ""display"": ""Cycling"",
    ""parent"": {
      ""key"": ""all"",
      ""display"": ""Any Activity Type""
    },
    ""type"": {
      ""key"": ""cycling"",
      ""display"": ""Cycling""
    }
  },
  {
    ""key"": ""cyclocross"",
    ""display"": ""Cyclocross"",
    ""parent"": {
      ""key"": ""cycling"",
      ""display"": ""Cycling""
    },
    ""type"": {
      ""key"": ""cycling"",
      ""display"": ""Cycling""
    }
  },
  {
    ""key"": ""downhill_biking"",
    ""display"": ""Downhill Biking"",
    ""parent"": {
      ""key"": ""cycling"",
      ""display"": ""Cycling""
    },
    ""type"": {
      ""key"": ""cycling"",
      ""display"": ""Cycling""
    }
  },
  {
    ""key"": ""indoor_cycling"",
    ""display"": ""Indoor Cycling"",
    ""parent"": {
      ""key"": ""cycling"",
      ""display"": ""Cycling""
    },
    ""type"": {
      ""key"": ""cycling"",
      ""display"": ""Cycling""
    }
  },
  {
    ""key"": ""mountain_biking"",
    ""display"": ""Mountain Biking"",
    ""parent"": {
      ""key"": ""cycling"",
      ""display"": ""Cycling""
    },
    ""type"": {
      ""key"": ""cycling"",
      ""display"": ""Cycling""
    }
  },
  {
    ""key"": ""recumbent_cycling"",
    ""display"": ""Recumbent Cycling"",
    ""parent"": {
      ""key"": ""cycling"",
      ""display"": ""Cycling""
    },
    ""type"": {
      ""key"": ""cycling"",
      ""display"": ""Cycling""
    }
  },
  {
    ""key"": ""road_biking"",
    ""display"": ""Road Cycling"",
    ""parent"": {
      ""key"": ""cycling"",
      ""display"": ""Cycling""
    },
    ""type"": {
      ""key"": ""cycling"",
      ""display"": ""Cycling""
    }
  },
  {
    ""key"": ""track_cycling"",
    ""display"": ""Track Cycling"",
    ""parent"": {
      ""key"": ""cycling"",
      ""display"": ""Cycling""
    },
    ""type"": {
      ""key"": ""cycling"",
      ""display"": ""Cycling""
    }
  },
  {
    ""key"": ""fitness_equipment"",
    ""display"": ""Fitness Equipment"",
    ""parent"": {
      ""key"": ""all"",
      ""display"": ""Any Activity Type""
    },
    ""type"": {
      ""key"": ""fitness_equipment"",
      ""display"": ""Fitness Equipment""
    }
  },
  {
    ""key"": ""elliptical"",
    ""display"": ""Elliptical"",
    ""parent"": {
      ""key"": ""fitness_equipment"",
      ""display"": ""Fitness Equipment""
    },
    ""type"": {
      ""key"": ""fitness_equipment"",
      ""display"": ""Fitness Equipment""
    }
  },
  {
    ""key"": ""indoor_cardio"",
    ""display"": ""Cardio"",
    ""parent"": {
      ""key"": ""fitness_equipment"",
      ""display"": ""Fitness Equipment""
    },
    ""type"": {
      ""key"": ""fitness_equipment"",
      ""display"": ""Fitness Equipment""
    }
  },
  {
    ""key"": ""indoor_rowing"",
    ""display"": ""Indoor Rowing"",
    ""parent"": {
      ""key"": ""fitness_equipment"",
      ""display"": ""Fitness Equipment""
    },
    ""type"": {
      ""key"": ""fitness_equipment"",
      ""display"": ""Fitness Equipment""
    }
  },
  {
    ""key"": ""stair_climbing"",
    ""display"": ""Stair Stepper"",
    ""parent"": {
      ""key"": ""fitness_equipment"",
      ""display"": ""Fitness Equipment""
    },
    ""type"": {
      ""key"": ""fitness_equipment"",
      ""display"": ""Fitness Equipment""
    }
  },
  {
    ""key"": ""strength_training"",
    ""display"": ""Strength Training"",
    ""parent"": {
      ""key"": ""fitness_equipment"",
      ""display"": ""Fitness Equipment""
    },
    ""type"": {
      ""key"": ""fitness_equipment"",
      ""display"": ""Fitness Equipment""
    }
  },
  {
    ""key"": ""hiking"",
    ""display"": ""Hiking"",
    ""parent"": {
      ""key"": ""all"",
      ""display"": ""Any Activity Type""
    },
    ""type"": {
      ""key"": ""hiking"",
      ""display"": ""Hiking""
    }
  },
  {
    ""key"": ""swimming"",
    ""display"": ""Swimming"",
    ""parent"": {
      ""key"": ""all"",
      ""display"": ""Any Activity Type""
    },
    ""type"": {
      ""key"": ""swimming"",
      ""display"": ""Swimming""
    }
  },
  {
    ""key"": ""lap_swimming"",
    ""display"": ""Lap Swimming"",
    ""parent"": {
      ""key"": ""swimming"",
      ""display"": ""Swimming""
    },
    ""type"": {
      ""key"": ""swimming"",
      ""display"": ""Swimming""
    }
  },
  {
    ""key"": ""open_water_swimming"",
    ""display"": ""Open Water Swimming"",
    ""parent"": {
      ""key"": ""swimming"",
      ""display"": ""Swimming""
    },
    ""type"": {
      ""key"": ""swimming"",
      ""display"": ""Swimming""
    }
  },
  {
    ""key"": ""walking"",
    ""display"": ""Walking"",
    ""parent"": {
      ""key"": ""all"",
      ""display"": ""Any Activity Type""
    },
    ""type"": {
      ""key"": ""walking"",
      ""display"": ""Walking""
    }
  },
  {
    ""key"": ""casual_walking"",
    ""display"": ""Casual Walking"",
    ""parent"": {
      ""key"": ""walking"",
      ""display"": ""Walking""
    },
    ""type"": {
      ""key"": ""walking"",
      ""display"": ""Walking""
    }
  },
  {
    ""key"": ""speed_walking"",
    ""display"": ""Speed Walking"",
    ""parent"": {
      ""key"": ""walking"",
      ""display"": ""Walking""
    },
    ""type"": {
      ""key"": ""walking"",
      ""display"": ""Walking""
    }
  },
  {
    ""key"": ""transition"",
    ""display"": ""Transition"",
    ""parent"": {
      ""key"": ""all"",
      ""display"": ""Any Activity Type""
    },
    ""type"": {
      ""key"": ""transition"",
      ""display"": ""Transition""
    }
  },
  {
    ""key"": ""swimToBikeTransition"",
    ""display"": ""Swim to Bike Transition"",
    ""parent"": {
      ""key"": ""transition"",
      ""display"": ""Transition""
    },
    ""type"": {
      ""key"": ""transition"",
      ""display"": ""Transition""
    }
  },
  {
    ""key"": ""bikeToRunTransition"",
    ""display"": ""Bike to Run Transition"",
    ""parent"": {
      ""key"": ""transition"",
      ""display"": ""Transition""
    },
    ""type"": {
      ""key"": ""transition"",
      ""display"": ""Transition""
    }
  },
  {
    ""key"": ""runToBikeTransition"",
    ""display"": ""Run to Bike Transition"",
    ""parent"": {
      ""key"": ""transition"",
      ""display"": ""Transition""
    },
    ""type"": {
      ""key"": ""transition"",
      ""display"": ""Transition""
    }
  },
  {
    ""key"": ""motorcycling"",
    ""display"": ""Motorcycling"",
    ""parent"": {
      ""key"": ""all"",
      ""display"": ""Any Activity Type""
    },
    ""type"": {
      ""key"": ""motorcycling"",
      ""display"": ""Motorcycling""
    }
  },
  {
    ""key"": ""steps"",
    ""display"": ""Steps"",
    ""parent"": {
      ""key"": ""all"",
      ""display"": ""Any Activity Type""
    },
    ""type"": {
      ""key"": ""steps"",
      ""display"": ""Steps""
    }
  },
  {
    ""key"": ""other"",
    ""display"": ""Other"",
    ""parent"": {
      ""key"": ""all"",
      ""display"": ""Any Activity Type""
    },
    ""type"": {
      ""key"": ""other"",
      ""display"": ""Other""
    }
  },
  {
    ""key"": ""backcountry_skiing_snowboarding"",
    ""display"": ""Backcountry Skiing/Snowboarding"",
    ""parent"": {
      ""key"": ""other"",
      ""display"": ""Other""
    },
    ""type"": {
      ""key"": ""other"",
      ""display"": ""Other""
    }
  },
  {
    ""key"": ""boating"",
    ""display"": ""Boating"",
    ""parent"": {
      ""key"": ""other"",
      ""display"": ""Other""
    },
    ""type"": {
      ""key"": ""other"",
      ""display"": ""Other""
    }
  },
  {
    ""key"": ""cross_country_skiing"",
    ""display"": ""Cross Country Skiing"",
    ""parent"": {
      ""key"": ""other"",
      ""display"": ""Other""
    },
    ""type"": {
      ""key"": ""other"",
      ""display"": ""Other""
    }
  },
  {
    ""key"": ""driving_general"",
    ""display"": ""Driving"",
    ""parent"": {
      ""key"": ""other"",
      ""display"": ""Other""
    },
    ""type"": {
      ""key"": ""other"",
      ""display"": ""Other""
    }
  },
  {
    ""key"": ""flying"",
    ""display"": ""Flying"",
    ""parent"": {
      ""key"": ""other"",
      ""display"": ""Other""
    },
    ""type"": {
      ""key"": ""other"",
      ""display"": ""Other""
    }
  },
  {
    ""key"": ""golf"",
    ""display"": ""Golf"",
    ""parent"": {
      ""key"": ""other"",
      ""display"": ""Other""
    },
    ""type"": {
      ""key"": ""other"",
      ""display"": ""Other""
    }
  },
  {
    ""key"": ""horseback_riding"",
    ""display"": ""Horseback Riding"",
    ""parent"": {
      ""key"": ""other"",
      ""display"": ""Other""
    },
    ""type"": {
      ""key"": ""other"",
      ""display"": ""Other""
    }
  },
  {
    ""key"": ""inline_skating"",
    ""display"": ""Inline Skating"",
    ""parent"": {
      ""key"": ""other"",
      ""display"": ""Other""
    },
    ""type"": {
      ""key"": ""other"",
      ""display"": ""Other""
    }
  },
  {
    ""key"": ""mountaineering"",
    ""display"": ""Mountaineering"",
    ""parent"": {
      ""key"": ""other"",
      ""display"": ""Other""
    },
    ""type"": {
      ""key"": ""other"",
      ""display"": ""Other""
    }
  },
  {
    ""key"": ""paddling"",
    ""display"": ""Paddling"",
    ""parent"": {
      ""key"": ""other"",
      ""display"": ""Other""
    },
    ""type"": {
      ""key"": ""other"",
      ""display"": ""Other""
    }
  },
  {
    ""key"": ""resort_skiing_snowboarding"",
    ""display"": ""Resort Skiing/Snowboarding"",
    ""parent"": {
      ""key"": ""other"",
      ""display"": ""Other""
    },
    ""type"": {
      ""key"": ""other"",
      ""display"": ""Other""
    }
  },
  {
    ""key"": ""rowing"",
    ""display"": ""Rowing"",
    ""parent"": {
      ""key"": ""other"",
      ""display"": ""Other""
    },
    ""type"": {
      ""key"": ""other"",
      ""display"": ""Other""
    }
  },
  {
    ""key"": ""sailing"",
    ""display"": ""Sailing"",
    ""parent"": {
      ""key"": ""other"",
      ""display"": ""Other""
    },
    ""type"": {
      ""key"": ""other"",
      ""display"": ""Other""
    }
  },
  {
    ""key"": ""skate_skiing"",
    ""display"": ""Skate Skiing"",
    ""parent"": {
      ""key"": ""other"",
      ""display"": ""Other""
    },
    ""type"": {
      ""key"": ""other"",
      ""display"": ""Other""
    }
  },
  {
    ""key"": ""skating"",
    ""display"": ""Skating"",
    ""parent"": {
      ""key"": ""other"",
      ""display"": ""Other""
    },
    ""type"": {
      ""key"": ""other"",
      ""display"": ""Other""
    }
  },
  {
    ""key"": ""snowmobiling"",
    ""display"": ""Snowmobiling"",
    ""parent"": {
      ""key"": ""other"",
      ""display"": ""Other""
    },
    ""type"": {
      ""key"": ""other"",
      ""display"": ""Other""
    }
  },
  {
    ""key"": ""snow_shoe"",
    ""display"": ""Snowshoeing"",
    ""parent"": {
      ""key"": ""other"",
      ""display"": ""Other""
    },
    ""type"": {
      ""key"": ""other"",
      ""display"": ""Other""
    }
  },
  {
    ""key"": ""stand_up_paddleboarding"",
    ""display"": ""Stand Up Paddleboarding"",
    ""parent"": {
      ""key"": ""other"",
      ""display"": ""Other""
    },
    ""type"": {
      ""key"": ""other"",
      ""display"": ""Other""
    }
  },
  {
    ""key"": ""whitewater_rafting_kayaking"",
    ""display"": ""Whitewater Kayaking/Rafting"",
    ""parent"": {
      ""key"": ""other"",
      ""display"": ""Other""
    },
    ""type"": {
      ""key"": ""other"",
      ""display"": ""Other""
    }
  },
  {
    ""key"": ""wind_kite_surfing"",
    ""display"": ""Wind/Kite Surfing"",
    ""parent"": {
      ""key"": ""other"",
      ""display"": ""Other""
    },
    ""type"": {
      ""key"": ""other"",
      ""display"": ""Other""
    }
  },
  {
    ""key"": ""bmx"",
    ""display"": ""BMX"",
    ""parent"": {
      ""key"": ""cycling"",
      ""display"": ""Cycling""
    },
    ""type"": {
      ""key"": ""cycling"",
      ""display"": ""Cycling""
    }
  },
  {
    ""key"": ""atv"",
    ""display"": ""ATV"",
    ""parent"": {
      ""key"": ""motorcycling"",
      ""display"": ""Motorcycling""
    },
    ""type"": {
      ""key"": ""motorcycling"",
      ""display"": ""Motorcycling""
    }
  },
  {
    ""key"": ""hunting_fishing"",
    ""display"": ""Hunting/Fishing"",
    ""parent"": {
      ""key"": ""other"",
      ""display"": ""Other""
    },
    ""type"": {
      ""key"": ""other"",
      ""display"": ""Other""
    }
  },
  {
    ""key"": ""sky_diving"",
    ""display"": ""Sky Diving"",
    ""parent"": {
      ""key"": ""other"",
      ""display"": ""Other""
    },
    ""type"": {
      ""key"": ""other"",
      ""display"": ""Other""
    }
  },
  {
    ""key"": ""motocross"",
    ""display"": ""Motocross"",
    ""parent"": {
      ""key"": ""motorcycling"",
      ""display"": ""Motorcycling""
    },
    ""type"": {
      ""key"": ""motorcycling"",
      ""display"": ""Motorcycling""
    }
  },
  {
    ""key"": ""rc_drone"",
    ""display"": ""RC/Drone"",
    ""parent"": {
      ""key"": ""other"",
      ""display"": ""Other""
    },
    ""type"": {
      ""key"": ""other"",
      ""display"": ""Other""
    }
  },
  {
    ""key"": ""surfing"",
    ""display"": ""Surfing"",
    ""parent"": {
      ""key"": ""other"",
      ""display"": ""Other""
    },
    ""type"": {
      ""key"": ""other"",
      ""display"": ""Other""
    }
  },
  {
    ""key"": ""wakeboarding"",
    ""display"": ""Wakeboarding"",
    ""parent"": {
      ""key"": ""other"",
      ""display"": ""Other""
    },
    ""type"": {
      ""key"": ""other"",
      ""display"": ""Other""
    }
  },
  {
    ""key"": ""rock_climbing"",
    ""display"": ""Rock Climbing"",
    ""parent"": {
      ""key"": ""other"",
      ""display"": ""Other""
    },
    ""type"": {
      ""key"": ""other"",
      ""display"": ""Other""
    }
  },
  {
    ""key"": ""hang_gliding"",
    ""display"": ""Hang Gliding"",
    ""parent"": {
      ""key"": ""other"",
      ""display"": ""Other""
    },
    ""type"": {
      ""key"": ""other"",
      ""display"": ""Other""
    }
  },
  {
    ""key"": ""wingsuit_flying"",
    ""display"": ""Wingsuit Flying"",
    ""parent"": {
      ""key"": ""other"",
      ""display"": ""Other""
    },
    ""type"": {
      ""key"": ""other"",
      ""display"": ""Other""
    }
  },
  {
    ""key"": ""tennis"",
    ""display"": ""TENNIS"",
    ""parent"": {
      ""key"": ""other"",
      ""display"": ""Other""
    },
    ""type"": {
      ""key"": ""other"",
      ""display"": ""Other""
    }
  },
  {
    ""key"": ""gravel_cycling"",
    ""display"": ""Gravel/Unpaved Cycling"",
    ""parent"": {
      ""key"": ""cycling"",
      ""display"": ""Cycling""
    },
    ""type"": {
      ""key"": ""cycling"",
      ""display"": ""Cycling""
    }
  },
  {
    ""key"": ""diving"",
    ""display"": ""Diving"",
    ""parent"": {
      ""key"": ""all"",
      ""display"": ""Any Activity Type""
    },
    ""type"": {
      ""key"": ""diving"",
      ""display"": ""Diving""
    }
  },
  {
    ""key"": ""single_gas_diving"",
    ""display"": ""Single-Gas Dive"",
    ""parent"": {
      ""key"": ""diving"",
      ""display"": ""Diving""
    },
    ""type"": {
      ""key"": ""diving"",
      ""display"": ""Diving""
    }
  },
  {
    ""key"": ""multi_gas_diving"",
    ""display"": ""Multi-Gas Dive"",
    ""parent"": {
      ""key"": ""diving"",
      ""display"": ""Diving""
    },
    ""type"": {
      ""key"": ""diving"",
      ""display"": ""Diving""
    }
  },
  {
    ""key"": ""gauge_diving"",
    ""display"": ""Gauge Dive"",
    ""parent"": {
      ""key"": ""diving"",
      ""display"": ""Diving""
    },
    ""type"": {
      ""key"": ""diving"",
      ""display"": ""Diving""
    }
  },
  {
    ""key"": ""apnea_diving"",
    ""display"": ""Apnea"",
    ""parent"": {
      ""key"": ""diving"",
      ""display"": ""Diving""
    },
    ""type"": {
      ""key"": ""diving"",
      ""display"": ""Diving""
    }
  },
  {
    ""key"": ""yoga"",
    ""display"": ""Yoga"",
    ""parent"": {
      ""key"": ""all"",
      ""display"": ""Any Activity Type""
    },
    ""type"": {
      ""key"": ""yoga"",
      ""display"": ""Yoga""
    }
  },
  {
    ""key"": ""floor_climbing"",
    ""display"": ""Floor Climbing"",
    ""parent"": {
      ""key"": ""other"",
      ""display"": ""Other""
    },
    ""type"": {
      ""key"": ""other"",
      ""display"": ""Other""
    }
  },
  {
    ""key"": ""stop_watch"",
    ""display"": ""Stopwatch"",
    ""parent"": {
      ""key"": ""other"",
      ""display"": ""Other""
    },
    ""type"": {
      ""key"": ""other"",
      ""display"": ""Other""
    }
  },
  {
    ""key"": ""virtual_ride"",
    ""display"": ""Virtual Cycling"",
    ""parent"": {
      ""key"": ""cycling"",
      ""display"": ""Cycling""
    },
    ""type"": {
      ""key"": ""cycling"",
      ""display"": ""Cycling""
    }
  },
  {
    ""key"": ""virtual_run"",
    ""display"": ""Virtual Running"",
    ""parent"": {
      ""key"": ""running"",
      ""display"": ""Running""
    },
    ""type"": {
      ""key"": ""running"",
      ""display"": ""Running""
    }
  },
  {
    ""key"": ""obstacle_run"",
    ""display"": ""Obstacle Running"",
    ""parent"": {
      ""key"": ""running"",
      ""display"": ""Running""
    },
    ""type"": {
      ""key"": ""running"",
      ""display"": ""Running""
    }
  },
  {
    ""key"": ""apnea_hunting"",
    ""display"": ""Apnea Hunting"",
    ""parent"": {
      ""key"": ""diving"",
      ""display"": ""Diving""
    },
    ""type"": {
      ""key"": ""diving"",
      ""display"": ""Diving""
    }
  },
  {
    ""key"": ""indoor_running"",
    ""display"": ""Indoor Running"",
    ""parent"": {
      ""key"": ""running"",
      ""display"": ""Running""
    },
    ""type"": {
      ""key"": ""running"",
      ""display"": ""Running""
    }
  },
  {
    ""key"": ""emergency"",
    ""display"": ""emergency"",
    ""parent"": {
      ""key"": ""all"",
      ""display"": ""Any Activity Type""
    },
    ""type"": {
      ""key"": ""emergency"",
      ""display"": ""emergency""
    }
  },
  {
    ""key"": ""assistance"",
    ""display"": ""assistance"",
    ""parent"": {
      ""key"": ""emergency"",
      ""display"": ""emergency""
    },
    ""type"": {
      ""key"": ""emergency"",
      ""display"": ""emergency""
    }
  },
  {
    ""key"": ""incident_detected"",
    ""display"": ""incident_detected"",
    ""parent"": {
      ""key"": ""emergency"",
      ""display"": ""emergency""
    },
    ""type"": {
      ""key"": ""emergency"",
      ""display"": ""emergency""
    }
  }
]}";
                    ActivityTypes = (GCActivityTypes)JsonConvert.DeserializeObject<GCActivityTypes>(activityTypeJsonStri);
                    return true;
                }
                else
                {
                    SetStatus(string.Format("Error: {0}", ex.Message));
                    return false;
                }
            }
        }

        private bool GetEventTypes()
        {
            try
            {
                HttpWebRequest request = HttpUtils.CreateRequest(EventTypesURL, Cookies);
                request.Headers.Add("origin", "https://sso.garmin.com");
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                string responseStr = HttpUtils.GetResponseAsString(response);
                response.Close();

                //EventTypes = (GCEventTypes)JsonConvert.DeserializeObject<GCEventTypes>(responseStr);
                eventTypes = new GCEventTypes();
                MemoryStream ms = new MemoryStream(Encoding.UTF8.GetBytes(String.Format(@"{{""eventTypes"":{0}}}", responseStr)));
                DataContractJsonSerializer ser = new DataContractJsonSerializer(eventTypes.GetType());
                eventTypes = ser.ReadObject(ms) as GCEventTypes;
                ms.Close();
                return true;
            }
            catch (Exception ex)
            {
                if (ex.Message.Equals("The remote server returned an error: (400) Bad Request."))
                {
                    //manually load activity types
                    string eventTypeJsonStri = @"{""dictionary"": [
  {
    ""key"": ""all"",
    ""display"": ""Any Event Type""
  },
  {
    ""key"": ""geocaching"",
    ""display"": ""Geocaching""
  },
  {
    ""key"": ""fitness"",
    ""display"": ""Fitness""
  },
  {
    ""key"": ""recreation"",
    ""display"": ""Recreation""
  },
  {
    ""key"": ""race"",
    ""display"": ""Race""
  },
  {
    ""key"": ""specialEvent"",
    ""display"": ""Special Event""
  },
  {
    ""key"": ""training"",
    ""display"": ""Training""
  },
  {
    ""key"": ""transportation"",
    ""display"": ""Transportation""
  },
  {
    ""key"": ""touring"",
    ""display"": ""Touring""
  },
  {
    ""key"": ""uncategorized"",
    ""display"": ""Uncategorized""
  }
]}";

                    EventTypes = (GCEventTypes)JsonConvert.DeserializeObject<GCEventTypes>(eventTypeJsonStri);
                    return true;
                }
                else
                {
                    SetStatus(string.Format("Error: {0}", ex.Message));
                    return false;
                }
            }
        }

        private void GetActivityList(int start, int batchsize)
        {
            try
            {   //an extended period of inactivity may cause the session to expire, in which case, log in again
                btnPrevious.Enabled = start > 0;

                HttpWebRequest request = HttpUtils.CreateRequest(string.Format(ACTIVITIES, start, batchsize), Cookies);
                request.Headers.Add("origin", "https://sso.garmin.com");
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                string responseStr = HttpUtils.GetResponseAsString(response);
                response.Close();

                JArray activities = (JArray)JsonConvert.DeserializeObject(responseStr);

                List<GCActivity> gcActivities = new List<GCActivity>();
                for (int i = 0; i < activities.Count; i++)
                {
                    var activity = activities[i];

                    GCActivity gcActivity = new GCActivity();
                    gcActivity.ActivityID = (long)((JValue)activity["activityId"]).Value;
                    gcActivity.ActivityName = ((JValue)activity["activityName"]).Value.ToString();
                    gcActivity.ActivityStartTime = DateTime.Parse(((JValue)activity["startTimeLocal"]).Value.ToString());
                    gcActivity.GMTStartTime = DateTime.Parse(((JValue)activity["startTimeGMT"]).Value.ToString());
                    gcActivity.ActivityType = ((JValue)activity["activityType"]["typeKey"]).ToString();
                    gcActivity.EventType = ((JValue)activity["eventType"]["typeKey"]).ToString();
                    gcActivity.Distance = ((JValue)activity["distance"]) == null ? 0 : (double)((JValue)activity["distance"]).Value;
                    gcActivity.Duration = (double)((JValue)activity["duration"]).Value;
                    gcActivity.Calories = (double)((JValue)activity["calories"]).Value;
                    gcActivity.Steps = ((JValue)activity["steps"]).Value == null ? 0 : (long)((JValue)activity["steps"]).Value;
                    gcActivities.Add(gcActivity);

                    //cache it
                    CacheActivity(gcActivity, false);
                }

                lstActivities.DataSource = gcActivities;
                lstActivities.ValueMember = "ActivityID";
                lstActivities.DisplayMember = "ActivityDisplayName";

                btnNext.Enabled = gcActivities.Count > 0;

                lstActivities.Focus();
            }
            catch (Exception ex)
            {
                SetStatus(ex.Message);
                RedoLogin();
            }
        }

        private int DownloadActivities(List<GCActivity> activityList)
        {
            int retVal = 0;
            string folderLocation = "";
            if (DownloadFolder.Length == 0)
            {
                MessageBox.Show(this, "Please set a download folder before downloading: Ctrl+D or right-click Set Dowload Folder", "Garmin Connect Downloader", MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1);
                retVal = 0;
            }
            else
            {
                statusMessage.Text = "Downloading activities...";

                int counter = 0;
                string Filename;
                bool downloadComplete = false;

                foreach (GCActivity activity in activityList)
                {
                    Filename = Path.Combine(DownloadFolder, Username, activity.ActivityStartTime.Year.ToString("0000"), activity.ActivityStartTime.Month.ToString("00"), String.Format("{0}-{1}-{2}_{3}", activity.ActivityStartTime.Year.ToString("0000"), activity.ActivityStartTime.Month.ToString("00"), activity.ActivityStartTime.Day.ToString("00"), activity.ActivityID));
                    if (folderLocation == "") folderLocation = Path.GetDirectoryName(Filename); //open first location as this is most recent returned from GC
                    if (!Directory.Exists(folderLocation)) Directory.CreateDirectory(folderLocation);

                    HttpWebRequest request;
                    HttpWebResponse response;
                    downloadComplete = true;

                    try
                    {
                        statusMessage.Text = string.Format("Downloading \"{0}\" TCX {1}", activity.ActivityName, activity.ActivityStartTime.ToString("yyyy-MM-dd"));
                        request = HttpUtils.CreateRequest(string.Format(TCX, activity.ActivityID), Cookies);
                        request.Headers.Add("origin", "https://sso.garmin.com");
                        request.Timeout = 1000 * 10;
                        response = (HttpWebResponse)request.GetResponse();
                        if (!File.Exists(string.Format("{0}.tcx", Filename))) response.SaveResponseToFile(string.Format("{0}.tcx", Filename));
                        response.Close();
                    }
                    catch (Exception ex)
                    {
                        statusMessage.Text = string.Format("Error downloading \"{0}\" ({1}) TCX: {2}", activity.ActivityName, activity.ActivityStartTime.ToString("yyyy-MM-dd"), ex.Message);
                        downloadComplete = false;
                    }

                    try
                    {
                        statusMessage.Text = string.Format("Downloading \"{0}\" GPX {1}", activity.ActivityName, activity.ActivityStartTime.ToString("yyyy-MM-dd"));
                        request = HttpUtils.CreateRequest(string.Format(GPX, activity.ActivityID), Cookies);
                        request.Headers.Add("origin", "https://sso.garmin.com");
                        request.Timeout = 1000 * 10;
                        response = (HttpWebResponse)request.GetResponse();
                        if (!File.Exists(string.Format("{0}.gpx", Filename))) response.SaveResponseToFile(string.Format("{0}.gpx", Filename));
                        response.Close();
                    }
                    catch (Exception ex)
                    {
                        statusMessage.Text = string.Format("Error downloading \"{0}\" ({1}) GPX: {2}", activity.ActivityName, activity.ActivityStartTime.ToString("yyyy-MM-dd"), ex.Message);
                        downloadComplete = false;
                    }

                    if (downloadComplete)
                    {
                        int idx = lstActivities.Items.IndexOf(activity);
                        if (idx >= 0) lstActivities.SetSelected(idx, false);
                    }

                    counter++;
                }

                if (downloadComplete) statusMessage.Text = String.Format("{0} activities downloaded", counter);

                retVal = counter;
            }

            //Open the downloads folder to the most recent month
            if (folderLocation != "")
            {
                try
                {
                    Process df = new Process();
                    ProcessStartInfo dfInfo = new ProcessStartInfo();
                    dfInfo.FileName = folderLocation;
                    df.StartInfo = dfInfo;
                    df.Start();
                    //df.WaitForInputIdle();
                    //df.CloseMainWindow();
                    df.Close();
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message);
                }
            }

            return retVal;
        }

        private void RefreshList()
        {
            try
            {
                if (loginStatus == LoginStatus.LoggedIn) GetActivityList(ActivityBatchStart, ActivityBatchSize);
            }
            catch
            {
                SignOut();
                toolStripGCDownloader.SendToBack();
                toolStripGCDownloader.Visible = false;
                panelLogin.BringToFront();
                txtPassword.Text = "";
                txtPassword.Focus();
            }
        }

        private void PreviewActivity(GCActivity activity)
        {
            if (activity != null)
            {
                //GCActivityType activityType = ActivityTypes.dictionary.Find(res => res.key == activity.ActivityType);
                //GCEventType eventType = EventTypes.dictionary.Find(res => res.key == activity.EventType);

                txtActivityID.Text = activity.ActivityID.ToString();
                txtActivityName.Text = activity.ActivityName;
                txtActivityTime.Text = activity.ActivityStartTime.ToString("yyyy-MM-dd HH:mm");
                txtActivityType.Text = activity.ActivityType;
                txtEventType.Text = activity.EventType;
                txtDistance.Text = (activity.Distance / 1000).ToString("#,0.00 km");
                txtDuration.Text = TimeSpan.FromSeconds(activity.Duration).ToString(@"hh\:mm\:ss");
                txtCalories.Text = activity.Calories.ToString("0 C");
                txtSteps.Text = activity.Steps.HasValue ? activity.Steps.Value.ToString("#,0") : "";
            }
            else
            {
                txtActivityID.Text = "";
                txtActivityName.Text = "";
                txtActivityTime.Text = "";
                txtActivityType.Text = "";
                txtEventType.Text = "";
                txtDistance.Text = "";
                txtDuration.Text = "";
                txtCalories.Text = "";
                txtSteps.Text = "";
            }
        }

        private bool CacheActivity(GCActivity activity, bool refresh = false)
        {
            try
            {
                if (ActivityCache == null) ActivityCache = new List<GCActivity>();

                int idx = ActivityCache.FindIndex(x => x.ActivityID == activity.ActivityID);

                //not found
                if (idx < 0) ActivityCache.Add(activity);
                else //found
                {
                    if (refresh) ActivityCache[idx] = activity;
                }

                idx = ActivityCache.FindIndex(x => x.ActivityID == activity.ActivityID);
                return idx >= 0; //cached successfully
            }
            catch
            {
                return false;
            }
        }

        #endregion

        #region Daily Summary

        private bool CacheDailySummary(GCDailySummary dailySummary, bool refresh = false)
        {
            try
            {
                if (DailySummaryCache == null) DailySummaryCache = new List<GCDailySummary>();

                int idx = DailySummaryCache.FindIndex(x => x.StartGMT == dailySummary.StartGMT);

                //not found
                if (idx < 0) DailySummaryCache.Add(dailySummary);
                else //found
                {
                    if (refresh) DailySummaryCache[idx] = dailySummary;
                }

                idx = DailySummaryCache.FindIndex(x => x.StartGMT == dailySummary.StartGMT);
                return idx >= 0; //cached successfully
            }
            catch
            {
                return false;
            }
        }

        private void GetDailySummary(DateTime date)
        {
            try
            {   //an extended period of inactivity may cause the session to expire, in which case, log in again

                HttpWebRequest request = HttpUtils.CreateRequest(string.Format(DAILYSUMMARY, Username, date.ToString("yyyy-MM-dd")), Cookies);
                request.Headers.Add("origin", "https://sso.garmin.com");
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                string responseStr = HttpUtils.GetResponseAsString(response);
                response.Close();

                JArray dailySummary = (JArray)JsonConvert.DeserializeObject(responseStr);

                List<GCDailySummary> gcDailySummary = new List<GCDailySummary>();
                for (int i = 0; i < dailySummary.Count; i++)
                {
                    var item = dailySummary[i];

                    GCDailySummary entry = new GCDailySummary();
                    entry.StartGMT = DateTime.Parse(((JValue)item["startGMT"]).Value.ToString());
                    entry.EndGMT = DateTime.Parse(((JValue)item["endGMT"]).Value.ToString());
                    entry.Steps = ((JValue)item["steps"]).Value == null ? 0 : (long)((JValue)item["steps"]).Value;
                    entry.PrimaryActivityLevel = ((JValue)item["primaryActivityLevel"]).Value.ToString();
                    entry.ActivityLevelConstant = ((JValue)item["activityLevelConstant"]).Value.ToString() == "true" ? true : false;
                    gcDailySummary.Add(entry);

                    //cache it
                    CacheDailySummary(entry, false);
                }

                lstDailySummary.DataSource = gcDailySummary;
                lstDailySummary.ValueMember = "StartGMT";
                lstDailySummary.DisplayMember = "DailySummaryDisplay";

                btnNext.Enabled = gcDailySummary.Count > 0;

                lstDailySummary.Focus();
            }
            catch
            {
                RedoLogin();
            }
        }

        #endregion

        #region Wellness

        #endregion

        #endregion
    }

    public class GCActivity
    {
        public GCActivity() { }

        /// <summary>
        /// Garmin Connect Activity ID 
        /// </summary>
        public long ActivityID { get; set; }
        /// <summary>
        /// Activity Name
        /// </summary>
        public string ActivityName { get; set; }
        /// <summary>
        /// Activity Local Start Time
        /// </summary>
        public DateTime ActivityStartTime { get; set; }
        /// <summary>
        /// Activity GMT Start Time
        /// </summary>
        public DateTime GMTStartTime { get; set; }
        /// <summary>
        /// Activity Type (running, cycling, walking, yoga)
        /// </summary>
        public string ActivityType { get; set; }
        /// <summary>
        /// Event Type (fitness, geocaching, race, recreational, special event, touring, training, transportation, uncategorized)
        /// </summary>
        public string EventType { get; set; }
        /// <summary>
        /// Distance (meters)
        /// </summary>
        public double Distance { get; set; }
        /// <summary>
        /// Activity Duration (seconds)
        /// </summary>
        public double Duration { get; set; }
        /// <summary>
        /// KCal
        /// </summary>
        public double Calories { get; set; }
        /// <summary>
        /// Activity Steps
        /// </summary>
        public long? Steps { get; set; }

        public string ActivityDisplayName
        {
            get { return String.Format("({0})  {1}  :  {2}", ActivityID, ActivityStartTime.ToString("yyyy-MM-dd"), ActivityName); }
        }
    }

    [DataContract]
    public class GCActivityType
    {
        [DataMember]
        public int typeId { get; set; }
        [DataMember]
        public string typeKey { get; set; }
        [DataMember]
        public int parentTypeId { get; set; }
        [DataMember]
        public int sortOrder { get; set; }
    }

    public class GCActivityTypes
    {
        public List<GCActivityType> activityTypes { get; set; }
    }

    [DataContract]
    public class GCEventType
    {
        [DataMember]
        public int typeId { get; set; }
        [DataMember]
        public string typeKey { get; set; }
        [DataMember]
        public int sortOrder { get; set; }
    }

    public class GCEventTypes
    {
        public List<GCEventType> eventTypes { get; set; }
    }

    public class GCDailySummary
    {
        public DateTime StartGMT { get; set; }
        public DateTime EndGMT { get; set; }
        public long Steps { get; set; }
        public string PrimaryActivityLevel { get; set; }
        public bool ActivityLevelConstant { get; set; }
        public string DailySummaryDisplay
        {
            get { return String.Format("{0} - {1}: {2} Steps", StartGMT.ToString("yyyy-MM-dd"), EndGMT.ToString("yyyy-MM-dd"), Steps); }
        }
    }

    static class HttpUtils
    {
        /// <summary>
        /// Creates a NameValueCollection that can be easily converted to a query string by calling ToString()
        /// </summary>
        public static NameValueCollection CreateQueryString()
        {
            return HttpUtility.ParseQueryString(String.Empty);
        }

        public static void AddIfNotNull(this NameValueCollection collection, string key, object value)
        {
            if (value != null) collection.Add(key, value.ToString());
        }

        public static void AddIfNotNull(this NameValueCollection collection, string key, bool? value)
        {
            if (value != null) collection.Add(key, value.ToString().ToLower());
        }

        public static void AddIfNotNull(this NameValueCollection collection, string key, int? value)
        {
            if (value != null) collection.Add(key, value.ToString());
        }

        public static HttpWebRequest CreateRequest(string url, CookieContainer cookies)
        {
            var request = (HttpWebRequest)WebRequest.Create(url);
            request.CookieContainer = cookies;
            request.KeepAlive = true;
            request.Method = "GET";
            return request;
        }

        public static void WriteFormData(this HttpWebRequest request, NameValueCollection data)
        {
            request.Method = "POST";
            request.ContentType = "application/x-www-form-urlencoded";
            WriteBinary(request, Encoding.UTF8.GetBytes(data.ToString()));
        }

        public static void WriteBinary(this HttpWebRequest request, byte[] data)
        {
            if (request == null) throw new ArgumentNullException("request");
            if (data == null) throw new ArgumentNullException("data");

            request.ContentLength = data.Length;

            using (Stream stream = request.GetRequestStream())
            {
                stream.Write(data, 0, data.Length);
                stream.Flush();
                stream.Close();
            }
        }

        public static void SaveResponseToFile(this HttpWebResponse response, string targetFilePath)
        {
            if (response == null) throw new ArgumentNullException("response");
            if (String.IsNullOrEmpty(targetFilePath)) throw new ArgumentException("No target file path specified.", "targetFilePath");

            using (Stream responseStream = response.GetResponseStream())
            using (FileStream fileStream = File.Create(targetFilePath))
            {
                if (responseStream != null)
                    responseStream.CopyTo(fileStream);
            }
        }

        public static string GetResponseAsString(this HttpWebResponse response)
        {
            if (response == null) throw new ArgumentNullException("response");

            Stream responseStream = response.GetResponseStream();
            if (responseStream == null) return null;

            using (var stream = new StreamReader(responseStream, Encoding.UTF8))
            {
                return stream.ReadToEnd();
            }
        }
    }
}
