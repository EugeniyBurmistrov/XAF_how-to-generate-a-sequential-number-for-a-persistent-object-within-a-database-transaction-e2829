Imports Microsoft.VisualBasic
Imports System
Imports DevExpress.Xpo
Imports System.Threading
Imports DevExpress.ExpressApp
Imports DevExpress.Xpo.Metadata
Imports DevExpress.ExpressApp.DC
Imports DevExpress.Data.Filtering
Imports System.Collections.Generic
Imports DevExpress.Xpo.DB.Exceptions
Imports DevExpress.ExpressApp.Utils

Namespace GenerateUserFriendlyId.Module.Utils
	'Dennis: This class is used to generate sequential numbers for persistent objects.
	'Use the GetNextSequence method to get the next number and the Accept method, to save these changes to the database.
	Public Class SequenceGenerator
		Implements IDisposable
		Public Const MaxGenerationAttemptsCount As Integer = 10
		Public Const MinGenerationAttemptsDelay As Integer = 100
'INSTANT VB TODO TASK: There is no VB.NET equivalent to 'volatile':
'ORIGINAL LINE: private static volatile IDataLayer defaultDataLayer;
		Private Shared defaultDataLayer_Renamed As IDataLayer
		Private euow As ExplicitUnitOfWork
		Private seq As Sequence
		Public Sub New(ByVal lockedSequenceTypes As Dictionary(Of String, Boolean))
			Dim count As Integer = MaxGenerationAttemptsCount
			Do
				Try
					euow = New ExplicitUnitOfWork(DefaultDataLayer)
					'Dennis: It is necessary to update all sequences because objects graphs may be complex enough, and so their sequences should be locked to avoid a deadlock.
					Dim sequences As New XPCollection(Of Sequence)(euow, New InOperator("TypeName", lockedSequenceTypes.Keys), New SortProperty("TypeName", DevExpress.Xpo.DB.SortingDirection.Ascending))
					For Each seq As Sequence In sequences
						seq.Save()
					Next seq
					euow.FlushChanges()
					Exit Do
				Catch e1 As LockingException
					Close()
					count -= 1
					If count <= 0 Then
						Throw
					End If
					Thread.Sleep(MinGenerationAttemptsDelay * count)
				End Try
			Loop
		End Sub
		Public Sub Accept()
			euow.CommitChanges()
		End Sub
		Public Sub Close()
			If euow IsNot Nothing Then
                If euow.InTransaction Then
                    euow.RollbackTransaction()
                End If
				euow.Dispose()
				euow = Nothing
			End If
		End Sub
		Public Sub Dispose() Implements IDisposable.Dispose
			Close()
		End Sub
		Public Function GetNextSequence(ByVal theObject As Object) As Long
			If theObject Is Nothing Then
				Throw New ArgumentNullException("theObject")
			End If
			Return GetNextSequence(XafTypesInfo.Instance.FindTypeInfo(theObject.GetType()))
		End Function
		Public Function GetNextSequence(ByVal typeInfo As ITypeInfo) As Long
			If typeInfo Is Nothing Then
				Throw New ArgumentNullException("typeInfo")
			End If
			Return GetNextSequence(XafTypesInfo.XpoTypeInfoSource.XPDictionary.GetClassInfo(typeInfo.Type))
		End Function
		Public Function GetNextSequence(ByVal classInfo As XPClassInfo) As Long
			If classInfo Is Nothing Then
				Throw New ArgumentNullException("classInfo")
			End If
			Dim ci As XPClassInfo = classInfo
			'Dennis: Uncomment this code if you want to have the SequentialNumber column created in each derived class table.
			Do While ci.BaseClass IsNot Nothing AndAlso ci.BaseClass.IsPersistent
				ci = ci.BaseClass
			Loop
			seq = euow.GetObjectByKey(Of Sequence)(ci.FullName, True)
			If seq Is Nothing Then
				Throw New InvalidOperationException(String.Format("Sequence for the {0} type was not found.", ci.FullName))
			End If
			Dim nextSequence As Long = seq.NextSequence
			seq.NextSequence += 1
			euow.FlushChanges()
			Return nextSequence
		End Function
		'Dennis: It is necessary to generate (only once) sequences for all the persistent types before using the GetNextSequence method.
		Public Shared Sub RegisterSequences(ByVal persistentTypes As IEnumerable(Of ITypeInfo))
			If persistentTypes IsNot Nothing Then
				Using uow As New UnitOfWork(DefaultDataLayer)
					Dim sequenceList As New XPCollection(Of Sequence)(uow)
					Dim typeToExistsMap As New Dictionary(Of String, Boolean)()
					For Each seq As Sequence In sequenceList
						typeToExistsMap(seq.TypeName) = True
					Next seq
					For Each typeInfo As ITypeInfo In persistentTypes
						Dim ti As ITypeInfo = typeInfo
						If typeToExistsMap.ContainsKey(ti.FullName) Then
							Continue For
						End If
						'Dennis: Uncomment this code if you want to have the SequentialNumber column created in each derived class table.
						Do While ti.Base IsNot Nothing AndAlso ti.Base.IsPersistent
							ti = ti.Base
						Loop
						Dim typeName As String = ti.FullName
						'Dennis: This code is required for the Domain Components only.
						If ti.IsInterface AndAlso ti.IsPersistent Then
							Dim generatedEntityType As Type = XafTypesInfo.XpoTypeInfoSource.GetGeneratedEntityType(ti.Type)
							If generatedEntityType IsNot Nothing Then
								typeName = generatedEntityType.FullName
							End If
						End If
						If typeToExistsMap.ContainsKey(typeName) Then
							Continue For
						End If
						If ti.IsPersistent Then
							typeToExistsMap(typeName) = True
							Dim seq As New Sequence(uow)
							seq.TypeName = typeName
							seq.NextSequence = 0
						End If
					Next typeInfo
					uow.CommitChanges()
				End Using
			End If
		End Sub
		Private Shared syncRoot As New Object()
		Public Shared Property DefaultDataLayer() As IDataLayer
			Get
				If defaultDataLayer_Renamed Is Nothing Then
					Throw New ArgumentNullException("DefaultDataLayer")
				End If
				Return defaultDataLayer_Renamed
			End Get
			Set(ByVal value As IDataLayer)
				SyncLock syncRoot
					defaultDataLayer_Renamed = value
				End SyncLock
			End Set
		End Property
	End Class
	'This persistent class is used to store last sequential number for persistent objects.
	Public Class Sequence
		Inherits XPBaseObject
		Private typeName_Renamed As String
		Private nextSequence_Renamed As Long
		Public Sub New(ByVal session As Session)
			MyBase.New(session)
		End Sub
		'Dennis: The size should be enough to store a full type name. However, you cannot use unlimited size for key columns.
		<Key, Size(1024)> _
		Public Property TypeName() As String
			Get
				Return typeName_Renamed
			End Get
			Set(ByVal value As String)
				SetPropertyValue("TypeName", typeName_Renamed, value)
			End Set
		End Property
		Public Property NextSequence() As Long
			Get
				Return nextSequence_Renamed
			End Get
			Set(ByVal value As Long)
				SetPropertyValue("NextSequence", nextSequence_Renamed, value)
			End Set
		End Property
	End Class
	Public Interface ISupportSequentialNumber
		Property SequentialNumber() As Long
	End Interface
	Public NotInheritable Class SequenceGeneratorInitializer
		Private Shared application_Renamed As XafApplication
		Private Sub New()
		End Sub
		Private Shared ReadOnly Property Application() As XafApplication
			Get
				Return application_Renamed
			End Get
		End Property
		Public Shared Sub Register(ByVal app As XafApplication)
			application_Renamed = app
			If application_Renamed IsNot Nothing Then
				AddHandler application_Renamed.LoggedOn, AddressOf application_LoggedOn
			End If
		End Sub
		Private Shared Sub application_LoggedOn(ByVal sender As Object, ByVal e As LogonEventArgs)
			Initialize()
		End Sub
		'Dennis: It is important to set the SequenceGenerator.DefaultDataLayer property to the main application data layer.
		'If you use a custom IObjectSpaceProvider implementation, ensure that it exposes a working IDataLayer.
		Public Shared Sub Initialize()
			Guard.ArgumentNotNull(Application, "Application")
			Dim provider As ObjectSpaceProvider = TryCast(Application.ObjectSpaceProvider, ObjectSpaceProvider)
			Guard.ArgumentNotNull(provider, "provider")
			If provider.WorkingDataLayer Is Nothing Then
				'Dennis: This call is necessary to initialize a working data layer.
				provider.CreateObjectSpace()
			End If
			If TypeOf provider.WorkingDataLayer Is ThreadSafeDataLayer Then
				'Dennis: We have to use a separate datalayer for the sequence generator because ThreadSafeDataLayer is usually used for ASP.NET applications.
				SequenceGenerator.DefaultDataLayer = XpoDefault.GetDataLayer(If(Application.Connection Is Nothing, Application.ConnectionString, Application.Connection.ConnectionString), DevExpress.Xpo.DB.AutoCreateOption.None)
			Else
				SequenceGenerator.DefaultDataLayer = provider.WorkingDataLayer
			End If
		End Sub
	End Class
End Namespace