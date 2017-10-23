Imports System.Globalization
Imports Mono.Cecil

Public MustInherit Class Settings
    Public Property Iterations As Integer
    Public Property Neglecting As Double
    Public Property Distribution As Integer
    Public Property Sequence As List(Of Double)
    Protected Friend Functions As List(Of Functions)
    Protected Friend Definitions As Dictionary(Of Types, String)
    Protected Friend Property Culture As CultureInfo
    Public Const Chars As String = "1234567890"
    Public Const Marker As Char = Strings.ChrW(0)
    Public Const Numbers As NumberStyles = CType(&HA7, NumberStyles)
    Public Const DefType As TypeAttributes = CType(&H1, TypeAttributes)
    Public Const DefMethod As MethodAttributes = CType(&H16, MethodAttributes)
    Sub New()
        Me.Iterations = 1                                                   '// Iterations, more then 3+ can be slow and performance costly
        Me.Distribution = 1                                                 '// Deposit classes where additional methods will be created
        Me.Neglecting = 0.00001R                                            '// Decimal neglecting tolerance

        Me.Culture = New CultureInfo("en-US")                               '// Used to define the decimal formats

        Me.Sequence = New List(Of Double)                                   '// Number pool, a larger sequence is performance costly
        Me.Sequence.AddRange({1, 2, 3, 100, 200, 300, 1000})
        Me.Sequence.AddRange(Randomizer.ShuffleF(1, 30, 4, 100))

        Me.Functions = New List(Of Functions)                               '// Functions pool, changing order can impact performance
        Me.Functions.Add(New Functions("({0}+{1})", Function(x, y) x + y))
        Me.Functions.Add(New Functions("({0}-{1})", Function(x, y) x - y))
        Me.Functions.Add(New Functions("({0}*{1})", Function(x, y) x * y))
        Me.Functions.Add(New Functions("({0}/{1})", Function(x, y) x / y))
        Me.Functions.Add(New Functions("squared({0})", Function(x, y) x * x))
        Me.Functions.Add(New Functions("squared({1})", Function(x, y) y * y))
        Me.Functions.Add(New Functions("log({0})", Function(x, y) Math.Log(x)))
        Me.Functions.Add(New Functions("log({1})", Function(x, y) Math.Log(y)))
        Me.Functions.Add(New Functions("sin({0})", Function(x, y) Math.Sin(x)))
        Me.Functions.Add(New Functions("tan({0})", Function(x, y) Math.Tan(x)))
        Me.Functions.Add(New Functions("cos({0})", Function(x, y) Math.Cos(x)))
        Me.Functions.Add(New Functions("sin({1})", Function(x, y) Math.Sin(y)))
        Me.Functions.Add(New Functions("tan({1})", Function(x, y) Math.Tan(y)))
        Me.Functions.Add(New Functions("cos({1})", Function(x, y) Math.Cos(y)))
        Me.Functions.Add(New Functions("sqrt({0})", Function(x, y) Math.Sqrt(x)))
        Me.Functions.Add(New Functions("sqrt({1})", Function(x, y) Math.Sqrt(y)))
        Me.Functions.Add(New Functions("round({0})", Function(x, y) Math.Round(x, 1)))
        Me.Functions.Add(New Functions("round({1})", Function(x, y) Math.Round(y, 1)))

        Me.Definitions = New Dictionary(Of Types, String)                       '// Syntax definitions used for the lexer
        Me.Definitions.Add(Types.Open, "^\(")
        Me.Definitions.Add(Types.Close, "^\)")
        Me.Definitions.Add(Types.Plus, "^\+")
        Me.Definitions.Add(Types.Minus, "^-")
        Me.Definitions.Add(Types.Div, "^/")
        Me.Definitions.Add(Types.Mult, "^\*")
        Me.Definitions.Add(Types.Sin, "^\bsin\b")
        Me.Definitions.Add(Types.Tan, "^\btan\b")
        Me.Definitions.Add(Types.Cos, "^\bcos\b")
        Me.Definitions.Add(Types.Log, "^\blog\b")
        Me.Definitions.Add(Types.Sqrt, "^\bsqrt\b")
        Me.Definitions.Add(Types.Round, "^\bround\b")
        Me.Definitions.Add(Types.Squared, "^\bsquared\b")
        Me.Definitions.Add(Types.Number, "^[-+]?\d*\.\d+([eE][-+]?\d+)?|^[-+]?\d*([eE]?\d+)")

    End Sub
End Class
