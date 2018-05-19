Imports Microsoft.VisualBasic
Imports System
Imports DevExpress.ExpressApp
Imports DevExpress.ExpressApp.Updating
Imports GenerateUserFriendlyId.Module.Utils

Namespace GenerateUserFriendlyId.Module
	Public Class Updater
		Inherits ModuleUpdater
		Public Sub New(ByVal objectSpace As ObjectSpace, ByVal currentDBVersion As Version)
			MyBase.New(objectSpace, currentDBVersion)
		End Sub
		Public Overrides Sub UpdateDatabaseAfterUpdateSchema()
			MyBase.UpdateDatabaseAfterUpdateSchema()
			SequenceGenerator.RegisterSequences(XafTypesInfo.Instance.PersistentTypes)
		End Sub
	End Class
End Namespace
