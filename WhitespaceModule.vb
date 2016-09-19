Imports System.IO
Imports System.Text
Imports System.Web
Imports WebMarkupMin.Core

Public Class WhitespaceModule
	Implements IHttpModule
	#Region "IHttpModule Members"

	Private Sub IHttpModule_Dispose() Implements IHttpModule.Dispose
		' Nothing to dispose;
	End Sub

	Private Sub IHttpModule_Init(app As HttpApplication) Implements IHttpModule.Init
		AddHandler app.PostRequestHandlerExecute, AddressOf PostRequestHandlerExecute
	
	End Sub

	#End Region

	Private Sub PostRequestHandlerExecute(sender As Object, e As System.EventArgs)
		Dim contentType As String = sender.Response.ContentType
		Dim method As String = sender.Request.HttpMethod
		Dim status As Integer = sender.Response.StatusCode
		Dim handler As IHttpHandler = sender.Context.CurrentHandler

		If contentType = "text/html" AndAlso method = "GET" AndAlso status = 200 AndAlso handler IsNot Nothing Then
			sender.Response.Filter = New WhitespaceFilter(sender.Response.Filter, sender.Request.ContentEncoding)
		End If
	End Sub

	#Region "Stream filter"

	Private Class WhitespaceFilter
		Inherits Stream
		Private ReadOnly _encoding As Encoding
		Private ReadOnly _stream As Stream
		Private ReadOnly _cache As MemoryStream
		Private Shared ReadOnly _minifier As New HtmlMinifier(New HtmlMinificationSettings() With { _
			.WhitespaceMinificationMode = WhitespaceMinificationMode.Aggressive, _
			.RemoveRedundantAttributes = False _
		})

		Public Sub New(sink As Stream, encoding As Encoding)
			_stream = sink
			_encoding = encoding
			_cache = New MemoryStream()
		End Sub

		#Region "Properites"

		Public Overrides ReadOnly Property CanRead() As Boolean
			Get
				Return True
			End Get
		End Property

		Public Overrides ReadOnly Property CanSeek() As Boolean
			Get
				Return True
			End Get
		End Property

		Public Overrides ReadOnly Property CanWrite() As Boolean
			Get
				Return True
			End Get
		End Property

		Public Overrides Sub Flush()
			_stream.Flush()
		End Sub

		Public Overrides ReadOnly Property Length() As Long
			Get
				Return 0
			End Get
		End Property

		Private _position As Long
		Public Overrides Property Position() As Long
			Get
				Return _position
			End Get
			Set
				_position = value
			End Set
		End Property

		#End Region

		#Region "Methods"

		Public Overrides Function Read(buffer As Byte(), offset As Integer, count As Integer) As Integer
			Return _stream.Read(buffer, offset, count)
		End Function

		Public Overrides Function Seek(offset As Long, origin As SeekOrigin) As Long
			Return _stream.Seek(offset, origin)
		End Function

		Public Overrides Sub SetLength(value As Long)
			_stream.SetLength(value)
		End Sub

		Public Overrides Sub Write(buffer As Byte(), offset As Integer, count As Integer)
			_cache.Write(buffer, offset, count)
		End Sub

		Public Overrides Sub Close()
			Dim buffer As Byte() = _cache.ToArray()
			Dim cacheSize As Integer = buffer.Length

			Dim original As String = _encoding.GetString(buffer)
			Dim result As String = _minifier.Minify(original).MinifiedContent
			Dim output As Byte() = _encoding.GetBytes(result)

			_stream.Write(output, 0, output.Length)
			_cache.Dispose()
			_stream.Dispose()
		End Sub

		#End Region

	End Class

	#End Region

End Class
