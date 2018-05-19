Imports Microsoft.VisualBasic
Imports System
Imports System.Linq
Imports DevExpress.ExpressApp
Imports System.Threading.Tasks
Imports System.Collections.Generic
Imports DevExpress.ExpressApp.Actions
Imports GenerateUserFriendlyId.Module.Utils

Namespace GenerateUserFriendlyId.Module.Controllers
	Partial Public Class TestListViewController
		Inherits ViewController
		Private Const MaxTestersCount As Integer = 10
		Private testAction As SimpleAction
		Public Sub New()
			testAction = New SimpleAction(Me, "Test", "View")
			TargetViewType = ViewType.ListView
			AddHandler testAction.Execute, AddressOf testAction_Execute
		End Sub
		Private Sub testAction_Execute(ByVal sender As Object, ByVal e As SimpleActionExecuteEventArgs)
			testAction.Caption = "Test is running..."
			Dim tasks(MaxTestersCount - 1) As Task
			For i As Integer = 0 To MaxTestersCount - 1
				tasks(i) = Task.Factory.StartNew(Function() AnonymousMethod1())
			Next i
			Try
				Task.WaitAll(tasks)
				View.ObjectSpace.Refresh()
 				testAction.Caption = "Succeeded"
			Catch
				testAction.Caption = "Failed"
			End Try
			testAction.Enabled("Run only once") = False
		End Sub
		
		Private Function AnonymousMethod1() As Boolean
			For j As Integer = 0 To 49
				Using os As IObjectSpace = Application.CreateObjectSpace()
					Try
						DatabaseHelper.CreateContact(os)
						DatabaseHelper.CreateAddress(os)
						DatabaseHelper.CreateDocument(os)
						os.CommitChanges()
					Catch
						os.Rollback()
						Throw
					End Try
				End Using
			Next j
			Return True
		End Function
	End Class
End Namespace