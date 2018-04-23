Imports Microsoft.VisualBasic
Imports System
Imports System.Collections.Generic
Imports System.Linq
Imports System.Text
Imports System.Windows
Imports System.Windows.Controls
Imports System.Windows.Data
Imports System.Windows.Documents
Imports System.Windows.Input
Imports System.Windows.Media
Imports System.Windows.Media.Imaging
Imports System.Windows.Navigation
Imports System.Windows.Shapes
Imports System.Collections.ObjectModel
Imports System.Windows.Threading
Imports DevExpress.Xpf.Core
Imports System.ComponentModel
Imports DevExpress.Xpf.Grid
Imports System.Collections

Namespace Q409157
	Partial Public Class MainWindow
		Inherits Window
		Public Sub New()
			InitializeComponent()
		End Sub
	End Class

	Public Class ViewModel
		Public Sub New()
			ItemsSource = DataGenerator.GenerateObjects(0)
		End Sub

		Private privateItemsSource As BindingList(Of TestObject)
		Public Property ItemsSource() As BindingList(Of TestObject)
			Get
				Return privateItemsSource
			End Get
			Set(ByVal value As BindingList(Of TestObject))
				privateItemsSource = value
			End Set
		End Property
	End Class

	Public Class TestObject
		Private privateID As Integer
		Public Property ID() As Integer
			Get
				Return privateID
			End Get
			Set(ByVal value As Integer)
				privateID = value
			End Set
		End Property
		Private privateName As String
		Public Property Name() As String
			Get
				Return privateName
			End Get
			Set(ByVal value As String)
				privateName = value
			End Set
		End Property
		Private privateChildren As BindingList(Of TestObject)
		Public Property Children() As BindingList(Of TestObject)
			Get
				Return privateChildren
			End Get
			Set(ByVal value As BindingList(Of TestObject))
				privateChildren = value
			End Set
		End Property
	End Class

	Public NotInheritable Class DataGenerator
        Shared _count As Integer

        Public Shared Property Count() As Integer
            Get
                Return DataGenerator._count
            End Get
            Set(value As Integer)
                DataGenerator._count = value
            End Set
        End Property
		Private Sub New()
		End Sub
		Public Shared Function GenerateObjects(ByVal id As Integer) As BindingList(Of TestObject)
			Dim list As New BindingList(Of TestObject)()
			For i As Integer = 0 To 29999
				list.Add(New TestObject() With {.ID = i, .Name = String.Format("TestObject{0}", Count)})
				Count += 1
			Next i
			Return list
		End Function
	End Class


    Public Class MyChildSelector
        Implements IChildNodesSelector
        Function SelectChildren(item As Object) As System.Collections.IEnumerable Implements IChildNodesSelector.SelectChildren
            Dim obj As TestObject = TryCast(item, TestObject)
            If obj IsNot Nothing Then
                Dim list As New BindingList(Of TestObject)()
                FillChildren(list, obj)
                Return list


            End If
            Return Nothing
        End Function
        Private mList As BindingList(Of TestObject)
        Private mObj As TestObject
        Private currentDispatcher As Dispatcher
        Private Sub FillChildren(list As BindingList(Of TestObject), obj As TestObject)
            mList = list
            Dim bw As New BackgroundWorker()
            currentDispatcher = Dispatcher.CurrentDispatcher
            mObj = obj
            AddHandler bw.DoWork, AddressOf bw_DoWork
            bw.RunWorkerAsync()
        End Sub

        Private Sub bw_DoWork(sender As Object, e As DoWorkEventArgs)
            System.Threading.Thread.Sleep(3000)
            mList.RaiseListChangedEvents = False
            For Each item As TestObject In DataGenerator.GenerateObjects(mObj.ID)
                mList.Add(item)
            Next
            mList.RaiseListChangedEvents = True
            currentDispatcher.BeginInvoke(New Action(AddressOf MyReset))
        End Sub
        Private Sub MyReset()
            mList.ResetBindings()
        End Sub

    End Class

   
End Namespace
