#If False Then
' Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

Imports System.Collections.Immutable
Imports Microsoft.VisualStudio.TableManager

Namespace Microsoft.VisualStudio.LanguageServices.UnitTests.Diagnostics
    Friend Class TestTableManagerProvider
        Implements ITableManagerProvider

        Public Function GetTableManager(identifier As Guid) As ITableManager Implements ITableManagerProvider.GetTableManager
            Return New TestTableManager(identifier)
        End Function

        Public Class TestTableManager
            Implements ITableManager

            Private ReadOnly _identifier As Guid
            Private ReadOnly _sources As Dictionary(Of ITableDataSource, String())
            Private ReadOnly _sinks As Dictionary(Of ITableDataSink, IDisposable)

            Public Sub New(identifier As Guid)
                Me._identifier = identifier
                Me._sources = New Dictionary(Of ITableDataSource, String())()
                Me._sinks = New Dictionary(Of ITableDataSink, IDisposable)()
            End Sub

            Public ReadOnly Property Identifier As Guid Implements ITableManager.Identifier
                Get
                    Return _identifier
                End Get
            End Property

            Public ReadOnly Property Sources As IReadOnlyList(Of ITableDataSource) Implements ITableManager.Sources
                Get
                    Return _sources.Keys.ToImmutableArray()
                End Get
            End Property

            Public Event SourcesChanged As EventHandler Implements ITableManager.SourcesChanged

            Public Function AddSource(source As ITableDataSource, ParamArray columns() As String) As Boolean Implements ITableManager.AddSource
                _sources.Add(source, columns)

                Dim sink = New TestSink()
                Dim subscription = source.Subscribe(sink)

                _sinks.Add(sink, subscription)

                Return True
            End Function

            Public Function AddSource(source As ITableDataSource, columns As IReadOnlyCollection(Of String)) As Boolean Implements ITableManager.AddSource
                Return AddSource(source, columns.ToArray())
            End Function

            Public Function GetColumnsForSources(sources As IEnumerable(Of ITableDataSource)) As IReadOnlyList(Of String) Implements ITableManager.GetColumnsForSources
                Dim list = New List(Of String)

                For Each source In _sources.Keys
                    Dim columns As String() = Nothing
                    If _sources.TryGetValue(source, columns) Then
                        list.AddRange(columns)
                    End If
                Next

                Return list
            End Function

            Public Function RemoveSource(source As ITableDataSource) As Boolean Implements ITableManager.RemoveSource
                Return _sources.Remove(source)
            End Function

            Public ReadOnly Property Sinks_TestOnly As IEnumerable(Of KeyValuePair(Of ITableDataSink, IDisposable))
                Get
                    Return _sinks
                End Get
            End Property

            Public Class TestSink
                Implements ITableDataSink

                Public ReadOnly Entries As HashSet(Of ITableEntriesSnapshotFactory)

                Public Sub New()
                    Me.Entries = New HashSet(Of ITableEntriesSnapshotFactory)()
                End Sub

                Public Sub AddFactory(newFactory As ITableEntriesSnapshotFactory, Optional removeEverything As Boolean = False) Implements ITableDataSink.AddFactory
                    Me.Entries.Add(newFactory)
                End Sub

                Public Sub RemoveFactory(oldFactory As ITableEntriesSnapshotFactory) Implements ITableDataSink.RemoveFactory
                    Me.Entries.Remove(oldFactory)
                End Sub

                Public Sub ReplaceFactory(oldFactory As ITableEntriesSnapshotFactory, newFactory As ITableEntriesSnapshotFactory) Implements ITableDataSink.ReplaceFactory
                    Me.Entries.Remove(oldFactory)
                    Me.Entries.Add(newFactory)
                End Sub

                Public Sub FactoryUpdated(factory As ITableEntriesSnapshotFactory) Implements ITableDataSink.FactoryUpdated
                End Sub

                Public Sub AddEntries(newEntries As IReadOnlyList(Of ITableEntry), Optional removeEverything As Boolean = False) Implements ITableDataSink.AddEntries
                    Throw New NotImplementedException()
                End Sub

                Public Sub AddSnapshot(newSnapshot As ITableEntriesSnapshot, Optional removeEverything As Boolean = False) Implements ITableDataSink.AddSnapshot
                    Throw New NotImplementedException()
                End Sub

                Public Sub PostChange(Optional oldEntries As IReadOnlyList(Of ITableEntry) = Nothing, Optional newEntries As IReadOnlyList(Of ITableEntry) = Nothing, Optional oldSnapshot As ITableEntriesSnapshot = Nothing, Optional newSnapshot As ITableEntriesSnapshot = Nothing, Optional oldFactory As ITableEntriesSnapshotFactory = Nothing, Optional newFactory As ITableEntriesSnapshotFactory = Nothing, Optional removeEverything As Boolean = False) Implements ITableDataSink.PostChange
                    Throw New NotImplementedException()
                End Sub

                Public Sub RemoveEntries(oldEntries As IReadOnlyList(Of ITableEntry)) Implements ITableDataSink.RemoveEntries
                    Throw New NotImplementedException()
                End Sub

                Public Sub RemoveSnapshot(oldSnapshot As ITableEntriesSnapshot) Implements ITableDataSink.RemoveSnapshot
                    Throw New NotImplementedException()
                End Sub

                Public Sub ReplaceEntries(oldEntries As IReadOnlyList(Of ITableEntry), newEntries As IReadOnlyList(Of ITableEntry)) Implements ITableDataSink.ReplaceEntries
                    Throw New NotImplementedException()
                End Sub

                Public Sub ReplaceSnapshot(oldSnapshot As ITableEntriesSnapshot, newSnapshot As ITableEntriesSnapshot) Implements ITableDataSink.ReplaceSnapshot
                    Throw New NotImplementedException()
                End Sub

                Public Property IsStable As Boolean Implements ITableDataSink.IsStable
                    Get
                        Throw New NotImplementedException()
                    End Get
                    Set(value As Boolean)
                        Throw New NotImplementedException()
                    End Set
                End Property
            End Class
        End Class
    End Class
End Namespace
#End If