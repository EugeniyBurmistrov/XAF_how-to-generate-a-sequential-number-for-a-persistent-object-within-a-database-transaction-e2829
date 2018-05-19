Imports Microsoft.VisualBasic
Imports System
Namespace GenerateUserFriendlyId.Module.Controllers
	Partial Public Class ImportDataListViewController
		''' <summary>
		''' Required designer variable.
		''' </summary>
		Private components As System.ComponentModel.IContainer = Nothing

		''' <summary> 
		''' Clean up any resources being used.
		''' </summary>
		''' <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		Protected Overrides Sub Dispose(ByVal disposing As Boolean)
			If disposing AndAlso (components IsNot Nothing) Then
				components.Dispose()
			End If
			MyBase.Dispose(disposing)
		End Sub

		#Region "Component Designer generated code"

		''' <summary>
		''' Required method for Designer support - do not modify
		''' the contents of this method with the code editor.
		''' </summary>
		Private Sub InitializeComponent()
			Me.components = New System.ComponentModel.Container()
			Me.saImportData = New DevExpress.ExpressApp.Actions.SimpleAction(Me.components)
			' 
			' saImportData
			' 
			Me.saImportData.Caption = "Click to import data"
			Me.saImportData.ConfirmationMessage = Nothing
			Me.saImportData.Id = "ImportData"
			Me.saImportData.ImageName = "Attention"
			Me.saImportData.PaintStyle = DevExpress.ExpressApp.Templates.ActionItemPaintStyle.CaptionAndImage
			Me.saImportData.Shortcut = Nothing
			Me.saImportData.Tag = Nothing
			Me.saImportData.TargetObjectsCriteria = Nothing
			Me.saImportData.TargetViewId = Nothing
			Me.saImportData.ToolTip = Nothing
			Me.saImportData.TypeOfView = Nothing
'			Me.saImportData.Execute += New DevExpress.ExpressApp.Actions.SimpleActionExecuteEventHandler(Me.saImportData_Execute);
			' 
			' ImportDataListViewController
			' 
			Me.TargetObjectType = GetType(GenerateUserFriendlyId.Module.Utils.ISupportSequentialNumber)
			Me.TargetViewType = DevExpress.ExpressApp.ViewType.ListView
			Me.TypeOfView = GetType(DevExpress.ExpressApp.ListView)

		End Sub

		#End Region

		Private WithEvents saImportData As DevExpress.ExpressApp.Actions.SimpleAction
	End Class
End Namespace
