Imports Microsoft.VisualBasic
Imports System
Imports DevExpress.Xpo
Imports System.Windows.Forms
Imports System.Configuration
Imports DevExpress.ExpressApp.Security
Imports GenerateUserFriendlyId.Module.Utils

Namespace GenerateUserFriendlyId.Win
	Friend NotInheritable Class Program
		''' <summary>
		''' The main entry point for the application.
		''' </summary>
		Private Sub New()
		End Sub
		<STAThread> _
		Shared Sub Main()
#If EASYTEST Then
			DevExpress.ExpressApp.EasyTest.WinAdapter.RemotingRegistration.Register(4100)
#End If

			Application.EnableVisualStyles()
			Application.SetCompatibleTextRenderingDefault(False)
			EditModelPermission.AlwaysGranted = System.Diagnostics.Debugger.IsAttached

			Dim winApplication As New GenerateUserFriendlyIdWindowsFormsApplication()
#If EASYTEST Then
			If ConfigurationManager.ConnectionStrings("EasyTestConnectionString") IsNot Nothing Then
				winApplication.ConnectionString = ConfigurationManager.ConnectionStrings("EasyTestConnectionString").ConnectionString
			End If
#End If
			If ConfigurationManager.ConnectionStrings("ConnectionString") IsNot Nothing Then
				winApplication.ConnectionString = ConfigurationManager.ConnectionStrings("ConnectionString").ConnectionString
			End If
			SequenceGenerator.DefaultDataLayer = XpoDefault.GetDataLayer(If(winApplication.Connection Is Nothing, winApplication.ConnectionString, winApplication.Connection.ConnectionString), DevExpress.Xpo.DB.AutoCreateOption.DatabaseAndSchema)
			Try
				winApplication.Setup()
				winApplication.Start()
			Catch e As Exception
				winApplication.HandleException(e)
			End Try
		End Sub
	End Class
End Namespace