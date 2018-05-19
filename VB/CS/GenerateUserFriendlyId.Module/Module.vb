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
		Public Overrides Sub Setup(ByVal application As XafApplication)
			MyBase.Setup(application)
			XafTypesInfo.Instance.RegisterEntity("Document", GetType(IDocument), GetType(BasePersistentObject))
			SequenceGeneratorInitializer.Register(application)
		End Sub
	End Class
End Namespace