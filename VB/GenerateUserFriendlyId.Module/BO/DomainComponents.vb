Imports Microsoft.VisualBasic
Imports System
Imports DevExpress.ExpressApp
Imports DevExpress.ExpressApp.DC
Imports DevExpress.Persistent.Base
Imports DevExpress.Persistent.Validation
Imports GenerateUserFriendlyId.Module.Utils
Imports System.ComponentModel

Namespace GenerateUserFriendlyId.Module.BO
	<DomainComponent> _
	Public Interface IBaseDomainComponent
	Inherits ISupportSequentialNumber
	End Interface

	<DomainLogic(GetType(IBaseDomainComponent))> _
	Public Class BaseDomainComponentDomainLogic
		Private Shared sequenceGenerator As SequenceGenerator
		Public Sub OnSaving(ByVal theObject As IBaseDomainComponent, ByVal os As IObjectSpace)
			If os.IsNewObject(theObject) Then
				GenerateSequence(theObject, os)
			End If
		End Sub
		Public Shared Sub GenerateSequence(ByVal theObject As IBaseDomainComponent, ByVal os As IObjectSpace)
			If sequenceGenerator Is Nothing Then
				sequenceGenerator = New SequenceGenerator()
			End If
			theObject.SequentialNumber = sequenceGenerator.GetNextSequence(theObject)
			If Not(TypeOf os Is NestedObjectSpace) Then
				AddHandler os.Committed, AddressOf OnCommitted
			End If
		End Sub
		Private Shared Sub OnCommitted(ByVal sender As Object, ByVal e As EventArgs)
			Dim os As IObjectSpace = CType(sender, IObjectSpace)
			If sequenceGenerator IsNot Nothing Then
				Try
					sequenceGenerator.Accept()
				Finally
					RemoveHandler os.Committed, AddressOf OnCommitted
					sequenceGenerator.Dispose()
					sequenceGenerator = Nothing
				End Try
			End If
		End Sub
	End Class
	<DomainComponent, XafDefaultProperty("Title"), DefaultClassOptions> _
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