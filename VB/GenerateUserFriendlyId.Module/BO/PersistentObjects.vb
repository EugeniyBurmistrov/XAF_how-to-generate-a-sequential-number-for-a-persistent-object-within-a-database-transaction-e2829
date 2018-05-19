Imports Microsoft.VisualBasic
Imports System
Imports DevExpress.Xpo
Imports System.ComponentModel
Imports System.Collections.Generic
Imports DevExpress.Persistent.Base
Imports DevExpress.Persistent.BaseImpl
Imports GenerateUserFriendlyId.Module.Utils

Namespace GenerateUserFriendlyId.Module.BO
	'Dennis: Uncomment this code if you want to have the SequentialNumber column created in each derived class table.
	<NonPersistent> _
	Public MustInherit Class BasePersistentObject
		Inherits BaseObject
		Implements ISupportSequentialNumber
		Private _SequentialNumber As Long
		Private Shared sequenceGenerator As SequenceGenerator
		Private Shared syncRoot As Object = New Object()
		Public Sub New(ByVal session As Session)
			MyBase.New(session)
		End Sub
		Protected Overrides Sub OnSaving()
			Try
				MyBase.OnSaving()
				If Session.IsNewObject(Me) AndAlso (Not GetType(NestedUnitOfWork).IsInstanceOfType(Session)) Then
					GenerateSequence()
				End If
			Catch
				CancelSequence()
				Throw
			End Try
		End Sub
		Public Sub GenerateSequence()
			SyncLock syncRoot
				Dim typeToExistsMap As New Dictionary(Of String, Boolean)()
				For Each item As Object In Session.GetObjectsToSave()
					typeToExistsMap(Session.GetClassInfo(item).FullName) = True
				Next item
				If sequenceGenerator Is Nothing Then
					sequenceGenerator = New SequenceGenerator(typeToExistsMap)
				End If
				SubscribeToEvents()
				OnSequenceGenerated(sequenceGenerator.GetNextSequence(ClassInfo))
			End SyncLock
		End Sub
		Private Sub AcceptSequence()
			SyncLock syncRoot
				If sequenceGenerator IsNot Nothing Then
					Try
						sequenceGenerator.Accept()
					Finally
						CancelSequence()
					End Try
				End If
			End SyncLock
		End Sub
		Private Sub CancelSequence()
			SyncLock syncRoot
				UnSubscribeFromEvents()
				If sequenceGenerator IsNot Nothing Then
					sequenceGenerator.Close()
					sequenceGenerator = Nothing
				End If
			End SyncLock
		End Sub
		Private Sub Session_AfterCommitTransaction(ByVal sender As Object, ByVal e As SessionManipulationEventArgs)
			AcceptSequence()
		End Sub
		Private Sub Session_AfterRollBack(ByVal sender As Object, ByVal e As SessionManipulationEventArgs)
			CancelSequence()
		End Sub
		Private Sub Session_FailedCommitTransaction(ByVal sender As Object, ByVal e As SessionOperationFailEventArgs)
			CancelSequence()
		End Sub
		Private Sub SubscribeToEvents()
			If Not(TypeOf Session Is NestedUnitOfWork) Then
				AddHandler Session.AfterCommitTransaction, AddressOf Session_AfterCommitTransaction
				AddHandler Session.AfterRollbackTransaction, AddressOf Session_AfterRollBack
				AddHandler Session.FailedCommitTransaction, AddressOf Session_FailedCommitTransaction
			End If
		End Sub
		Private Sub UnSubscribeFromEvents()
			If Not(TypeOf Session Is NestedUnitOfWork) Then
				RemoveHandler Session.AfterCommitTransaction, AddressOf Session_AfterCommitTransaction
				RemoveHandler Session.AfterRollbackTransaction, AddressOf Session_AfterRollBack
				RemoveHandler Session.FailedCommitTransaction, AddressOf Session_FailedCommitTransaction
			End If
		End Sub
		Private Sub OnSequenceGenerated(ByVal newId As Long)
			SequentialNumber = newId
		End Sub
		'Dennis: Comment out this code if you do not want to have the SequentialNumber column created in each derived class table.
		<Browsable(False), Indexed(Unique := False)> _
		Public Property SequentialNumber() As Long Implements ISupportSequentialNumber.SequentialNumber
			Get
				Return _SequentialNumber
			End Get
			Set(ByVal value As Long)
				SetPropertyValue("SequentialNumber", _SequentialNumber, value)
			End Set
		End Property
	End Class
	<DefaultClassOptions, DefaultProperty("FullAddress"), ImageName("BO_Address")> _
	Public Class Address
		Inherits BasePersistentObject
		Private zipCode_Renamed As String
		Private country_Renamed As String
		Private province_Renamed As String
		Private city_Renamed As String
		Private address1_Renamed As String
		Private address2_Renamed As String
		<PersistentAlias("concat('A', ToStr(SequentialNumber))")> _
		Public ReadOnly Property AddressId() As String
			Get
				Return Convert.ToString(EvaluateAlias("AddressId"))
			End Get
		End Property
		Public Property Province() As String
			Get
				Return province_Renamed
			End Get
			Set(ByVal value As String)
				SetPropertyValue("Province", province_Renamed, value)
			End Set
		End Property
		Public Property ZipCode() As String
			Get
				Return zipCode_Renamed
			End Get
			Set(ByVal value As String)
				SetPropertyValue("ZipCode", zipCode_Renamed, value)
			End Set
		End Property
		Public Property Country() As String
			Get
				Return country_Renamed
			End Get
			Set(ByVal value As String)
				SetPropertyValue("Country", country_Renamed, value)
			End Set
		End Property
		Public Property City() As String
			Get
				Return city_Renamed
			End Get
			Set(ByVal value As String)
				SetPropertyValue("City", city_Renamed, value)
			End Set
		End Property
		Public Property Address1() As String
			Get
				Return address1_Renamed
			End Get
			Set(ByVal value As String)
				SetPropertyValue("Address1", address1_Renamed, value)
			End Set
		End Property
		Public Property Address2() As String
			Get
				Return address2_Renamed
			End Get
			Set(ByVal value As String)
				SetPropertyValue("Address2", address2_Renamed, value)
			End Set
		End Property
		<Association> _
		Public ReadOnly Property Persons() As XPCollection(Of Contact)
			Get
				Return GetCollection(Of Contact)("Persons")
			End Get
		End Property
		<PersistentAlias("concat(Country, Province, City, ZipCode)")> _
		Public ReadOnly Property FullAddress() As String
			Get
				Return ObjectFormatter.Format("{Country}; {Province}; {City}; {ZipCode}", Me, EmptyEntriesMode.RemoveDelimeterWhenEntryIsEmpty)
			End Get
		End Property
		Public Sub New(ByVal session As Session)
			MyBase.New(session)
		End Sub
	End Class
	<DefaultClassOptions, DefaultProperty("FullName"), ImageName("BO_Person")> _
	Public Class Contact
		Inherits BasePersistentObject
		Private firstName_Renamed As String
		Private lastName_Renamed As String
		Private sex_Renamed As Sex
		Private age_Renamed As Integer
		Private address_Renamed As Address
		<PersistentAlias("concat('C', ToStr(SequentialNumber))")> _
		Public ReadOnly Property ContactId() As String
			Get
				Return Convert.ToString(EvaluateAlias("ContactId"))
			End Get
		End Property
		Public Property FirstName() As String
			Get
				Return firstName_Renamed
			End Get
			Set(ByVal value As String)
				SetPropertyValue("FirstName", firstName_Renamed, value)
			End Set
		End Property
		Public Property LastName() As String
			Get
				Return lastName_Renamed
			End Get
			Set(ByVal value As String)
				SetPropertyValue("LastName", lastName_Renamed, value)
			End Set
		End Property
		Public Property Age() As Integer
			Get
				Return age_Renamed
			End Get
			Set(ByVal value As Integer)
				SetPropertyValue("Age", age_Renamed, value)
			End Set
		End Property
		Public Property Sex() As Sex
			Get
				Return sex_Renamed
			End Get
			Set(ByVal value As Sex)
				SetPropertyValue("Sex", sex_Renamed, value)
			End Set
		End Property
		<Association> _
		Public Property Address() As Address
			Get
				Return address_Renamed
			End Get
			Set(ByVal value As Address)
				SetPropertyValue("Address", address_Renamed, value)
			End Set
		End Property
		<PersistentAlias("concat(FirstName, LastName)")> _
		Public ReadOnly Property FullName() As String
			Get
				Return ObjectFormatter.Format("{FirstName} {LastName}", Me, EmptyEntriesMode.RemoveDelimeterWhenEntryIsEmpty)
			End Get
		End Property
		Public Sub New(ByVal session As Session)
			MyBase.New(session)
		End Sub
	End Class
	Public Enum Sex
		Male
		Female
	End Enum
End Namespace