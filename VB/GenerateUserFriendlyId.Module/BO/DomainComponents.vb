Imports Microsoft.VisualBasic
Imports System
Imports DevExpress.ExpressApp.DC
Imports DevExpress.Persistent.Base
Imports DevExpress.Persistent.Validation
Imports GenerateUserFriendlyId.Module.Utils

Namespace GenerateUserFriendlyId.Module.BO
	 <DomainComponent> _
	 Public Interface IBaseDomainComponent
	Inherits ISupportSequentialNumber
	 End Interface

	<DefaultClassOptions, DomainComponent, XafDefaultProperty("Title"), ImageName("BO_Note")> _
	Public Interface IDocument
	Inherits IBaseDomainComponent
		<Calculated("concat('D', ToStr(SequentialNumber))")> _
		ReadOnly Property DocumentId() As String
		<RuleRequiredField("IDocument.Title.RuleRequiredField", DefaultContexts.Save), FieldSize(255)> _
		Property Title() As String
		<FieldSize(8192)> _
		Property Text() As String
	End Interface
End Namespace