Namespace SampQueryService.QueryResult
    Public MustInherit Class SampQueryResult
        Public Property OpCode As Char
        Public Property IsCompleted As Boolean
        Friend MustOverride Sub Deserialize(ByVal data As Byte())
    End Class
End Namespace
