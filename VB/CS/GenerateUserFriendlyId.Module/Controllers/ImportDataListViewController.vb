Imports Microsoft.VisualBasic
Imports System
Imports DevExpress.ExpressApp
Imports DevExpress.ExpressApp.Actions
Imports GenerateUserFriendlyId.Module.BO
Imports GenerateUserFriendlyId.Module.Utils

Namespace GenerateUserFriendlyId.Module.Controllers
	Partial Public Class ImportDataListViewController
		Inherits ViewController
		Public Sub New()
			InitializeComponent()
			RegisterActions(components)
		End Sub
		Private Sub saImportData_Execute(ByVal sender As Object, ByVal e As SimpleActionExecuteEventArgs) Handles saImportData.Execute
			ImportData()
		End Sub
		Private Sub ImportData()
			Dim currentObjectType As Type = View.ObjectTypeInfo.Type
			Using importObjectSpace As IObjectSpace = Application.CreateObjectSpace()
				Try
					For i As Integer = 0 To 19
						If currentObjectType Is GetType(Contact) Then
							DatabaseHelper.CreateContact(importObjectSpace)
						End If
						If currentObjectType Is GetType(Address) Then
							DatabaseHelper.CreateAddress(importObjectSpace)
						End If
						If currentObjectType Is GetType(IDocument) Then
							DatabaseHelper.CreateDocument(importObjectSpace)
						End If
					Next i
					importObjectSpace.CommitChanges()
				Catch
					importObjectSpace.Rollback()
					Throw
				End Try
			End Using
			View.ObjectSpace.Refresh()
		End Sub
	End Class
End Namespace