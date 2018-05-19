using System;
using DevExpress.Xpo;
using System.Windows.Forms;
using System.Configuration;
using DevExpress.ExpressApp.Security;
using GenerateUserFriendlyId.Module.Utils;

namespace GenerateUserFriendlyId.Win {
    static class Program {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main() {
#if EASYTEST
			DevExpress.ExpressApp.EasyTest.WinAdapter.RemotingRegistration.Register(4100);
#endif

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            EditModelPermission.AlwaysGranted = System.Diagnostics.Debugger.IsAttached;

            GenerateUserFriendlyIdWindowsFormsApplication winApplication = new GenerateUserFriendlyIdWindowsFormsApplication();
#if EASYTEST
			if(ConfigurationManager.ConnectionStrings["EasyTestConnectionString"] != null) {
				winApplication.ConnectionString = ConfigurationManager.ConnectionStrings["EasyTestConnectionString"].ConnectionString;
			}
#endif
            if (ConfigurationManager.ConnectionStrings["ConnectionString"] != null) {
                winApplication.ConnectionString = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;
            }
            SequenceGenerator.DefaultDataLayer = XpoDefault.GetDataLayer(winApplication.Connection == null ? winApplication.ConnectionString : winApplication.Connection.ConnectionString, DevExpress.Xpo.DB.AutoCreateOption.DatabaseAndSchema);
            try {
                winApplication.Setup();
                winApplication.Start();
            } catch (Exception e) {
                winApplication.HandleException(e);
            }
        }
    }
}