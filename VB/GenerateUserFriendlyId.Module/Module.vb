Imports Microsoft.VisualBasic
Imports System
Imports DevExpress.ExpressApp
Imports GenerateUserFriendlyId.Module.BO


Namespace GenerateUserFriendlyId.Module
	Public NotInheritable Partial Class GenerateUserFriendlyIdModule
		Inherits ModuleBase
		Public Sub New()
			InitializeComponent()
		End Sub
		Public Overrides Sub Setup(ByVal application As XafApplication)
			MyBase.Setup(application)
			If (Not XafTypesInfo.IsInitialized) Then
				XafTypesInfo.Instance.AddEntityToGenerate("Document", GetType(IDocument))
			End If
		End Sub
	End Class
End Namespace
