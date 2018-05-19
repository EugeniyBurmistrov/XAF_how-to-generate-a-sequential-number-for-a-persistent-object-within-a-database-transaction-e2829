Imports Microsoft.VisualBasic
Imports System
Imports DevExpress.ExpressApp
Imports GenerateUserFriendlyId.Module.BO
Imports GenerateUserFriendlyId.Module.Utils

Namespace GenerateUserFriendlyId.Module
	Public NotInheritable Partial Class GenerateUserFriendlyIdModule
		Inherits ModuleBase
		Public Sub New()
			InitializeComponent()
		End Sub
		Public Overrides Overloads Sub Setup(ByVal application As XafApplication)
			MyBase.Setup(application)
			If (Not XafTypesInfo.IsInitialized) Then
				XafTypesInfo.Instance.RegisterEntity("Document", GetType(IDocument), GetType(BasePersistentObject))
				SequenceGeneratorInitializer.Register(application)
			End If
		End Sub
	End Class
End Namespace