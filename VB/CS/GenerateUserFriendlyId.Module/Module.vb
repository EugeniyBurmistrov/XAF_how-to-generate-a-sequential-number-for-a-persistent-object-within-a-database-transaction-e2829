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
				'Dennis: Starting from version 11.1, set the last parameter to True.
				'XafTypesInfo.Instance.AddEntityToGenerate("Document", typeof(IDocument), typeof(BasePersistentObject), true);
				XafTypesInfo.Instance.AddEntityToGenerate("Document", GetType(IDocument), GetType(BasePersistentObject))
			End If
		End Sub
	End Class
End Namespace
