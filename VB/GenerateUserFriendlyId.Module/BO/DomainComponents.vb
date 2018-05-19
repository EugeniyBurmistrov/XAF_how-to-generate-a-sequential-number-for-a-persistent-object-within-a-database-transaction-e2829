Imports Microsoft.VisualBasic
Imports System.ComponentModel
Imports DevExpress.ExpressApp.DC
Imports DevExpress.Persistent.Base
Imports DevExpress.Persistent.Validation
Imports GenerateUserFriendlyId.Module.Utils

Namespace GenerateUserFriendlyId.Module.BO
	<DomainComponent> _
	Public Interface IBaseDomainComponent
	Inherits ISupportSequentialNumber
	End Interface

	'Dennis: Unfortunately, the SequentialNumber property will not will not work untill the "B184328 - DC - Overridden interface property is not initialized when a base persistent class has the same property" is fixed.
	'Dennis: Thus, I am hiding this entity from the navigation system of the application.
	'[DefaultClassOptions]
	<DomainComponent, XafDefaultProperty("Title"), ImageName("BO_Note")> _
	Public Interface IDocument
	Inherits IBaseDomainComponent
		<Calculated("concat('D', ToStr(SequentialNumber))")> _
		ReadOnly Property DocumentId() As String
		<RuleRequiredField("IDocument.Title.RuleRequiredField", DefaultContexts.Save), FieldSize(255)> _
		Property Title() As String
		<FieldSize(4000)> _
		Property Text() As String
		<PersistentDc, Browsable(False)> _
		Shadows Property SequentialNumber() As Long
	End Interface
End Namespace