﻿using FluentAssertions;
using Inspector.CodeMetrics.VisualBasic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;

namespace InspectionTests.CodeMetricsTests.VisualBasic
{
    [TestClass]
    public class NestingLevelTests : VisualBasicMetricTest
    {
        [TestMethod]
        public void EmptyMethod_ShouldHave_NestingLevel0()
        {
            var parsedNode = GetSourceAsSyntaxTree(@"
                Imports System
                Imports System.Text

                <Serializable>_
                Public Class TestClass
                    Sub New()
                    End Sub

                    Public Function TestMe(i as Integer) As Boolean
                        return false
                    End Function
                End Class
                ");

            var sut = new NestingLevel();
            var results = sut.GetMetrics(parsedNode);

            results.Count().Should().Be(1);
            results.First().Method.Should().Be("Function TestMe(i as Integer)");
            results.First().Score.Should().Be(0);
        }

        [TestMethod]
        public void SingleIfStatement_ShouldHave_NestingLevel1()
        {
            var parsedNode = GetSourceAsSyntaxTree(@"
                Imports System
                Imports System.Text

                <Serializable>_
                Public Class TestClass
                    Sub New()
                    
                    Public Function TestMe(i as Integer) As Boolean
                        If i > 10 Then
                            return true
                        End If
                        return false
                    End Function
                End Class
                ");

            var sut = new NestingLevel();
            var results = sut.GetMetrics(parsedNode);

            results.Count().Should().Be(1);
            results.First().Method.Should().Be("Function TestMe(i as Integer)");
            results.First().Score.Should().Be(1);
        }

        [TestMethod]
        public void NestedIfStatement_ShouldHave_NestingLevel2()
        {
            var parsedNode = GetSourceAsSyntaxTree(@"
                Imports System
                Imports System.Text

                <Serializable>_
                Public Class TestClass
                    Sub New()
                    
                    Public Function TestMe(i as Integer) As Boolean
                        If i > 10 Then 
                            If true Then 
                                return true
                            End if                            
                        End If
                        return false
                    End Function
                End Class
                ");

            var sut = new NestingLevel();
            var results = sut.GetMetrics(parsedNode);

            results.Count().Should().Be(1);
            results.First().Method.Should().Be("Function TestMe(i as Integer)");
            results.First().Score.Should().Be(2);
        }

        [TestMethod]
        public void TwoIfStatements_ShouldHave_NestingLevel1()
        {
            var parsedNode = GetSourceAsSyntaxTree(@"
                Imports System
                Imports System.Text

                <Serializable>_
                Public Class TestClass
                    Sub New()
                    
                    Public Function TestMe(i as Integer) As Boolean
                        if (i > 10) then
                            return true
                        end if

                        if (i < 0) then
                            return true                            
                        end if

                        return false
                    End Function
                End Class
                ");

            var sut = new NestingLevel();
            var results = sut.GetMetrics(parsedNode);

            results.Count().Should().Be(1);
            results.First().Method.Should().Be("Function TestMe(i as Integer)");
            results.First().Score.Should().Be(1);
        }

        [TestMethod]
        public void NestedIfStatements_ShouldHave_NestingLevel3()
        {
            var parsedNode = GetSourceAsSyntaxTree(@"
                Imports System
                Imports System.Text

                <Serializable>_
                Public Class TestClass
                    Sub New()
                    
                    Public Function TestMe(i as Integer) As Boolean
                        If (i > 10) Then
                            if (true) Then
                                if (i==0) Then
                                    return true
                                End If
                            End If 
                        End If
                        return false
                    End Function
                End Class
                ");

            var sut = new NestingLevel();
            var results = sut.GetMetrics(parsedNode);

            results.Count().Should().Be(1);
            results.First().Method.Should().Be("Function TestMe(i as Integer)");
            results.First().Score.Should().Be(3);
        }

        [TestMethod]
        public void SimpleCase_ShouldHave_NestingLevel1()
        {
            var parsedNode = GetSourceAsSyntaxTree(@"
                Imports System
                Imports System.Text

                <Serializable>_
                Public Class TestClass
                    Sub New()
                    
                    Public Function TestMe(i as Integer) As Boolean
                        Select Case i
                            case 10:
                                return true
                        End Select
                        
                        return false
                    End Function
                End Class
                ");

            var sut = new NestingLevel();
            var results = sut.GetMetrics(parsedNode);

            results.Count().Should().Be(1);
            results.First().Method.Should().Be("Function TestMe(i as Integer)");
            results.First().Score.Should().Be(1);
        }

        [TestMethod]
        public void CaseWithNestedIf_ShouldHave_NestingLevel2()
        {
            var parsedNode = GetSourceAsSyntaxTree(@"
                Imports System
                Imports System.Text

                <Serializable>_
                Public Class TestClass
                    Sub New()
                    
                    Public Function TestMe(i as Integer) As Boolean
                        Select Case i
                            case 10:
                                if (i>0) then
                                    return true
                                End If
                        End Select
                        
                        return false
                    End Function
                End Class
                ");

            var sut = new NestingLevel();
            var results = sut.GetMetrics(parsedNode);

            results.Count().Should().Be(1);
            results.First().Method.Should().Be("Function TestMe(i as Integer)");
            results.First().Score.Should().Be(2);
        }

        [TestMethod]
        public void CaseWithNestedCase_ShouldHave_NestingLevel2()
        {
            var parsedNode = GetSourceAsSyntaxTree(@"
                Imports System
                Imports System.Text

                <Serializable>_
                Public Class TestClass
                    Sub New()
                    
                    Public Function TestMe(i as Integer) As Boolean
                        Dim j As Integer = i * 2
                        Select Case i
                            Case 10:
                                Select Case i {
                                    Case 20
                                        return false
                                End Select
                                return true
                        End Select
                        
                        return false
                    End Function
                End Class
                ");

            var sut = new NestingLevel();
            var results = sut.GetMetrics(parsedNode);

            results.Count().Should().Be(1);
            results.First().Method.Should().Be("Function TestMe(i as Integer)");
            results.First().Score.Should().Be(2);
        }

        [TestMethod]
        public void CaseWithNestedCaseWithNestedIf_ShouldHave_NestingLevel3()
        {
            var parsedNode = GetSourceAsSyntaxTree(@"
                Imports System
                Imports System.Text

                <Serializable>_
                Public Class TestClass
                    Sub New()
                    
                    Public Function TestMe(i as Integer) As Boolean
                        Dim j As Integer = i * 2
                        Select Case i
                            Case 10
                                Select Case j
                                    Case 20
                                        If (i+j == 30) Then
                                            return false
                                        End If
                                End Select
                                return true
                                break;
                        End Select
                        
                        return false
                    End Function
                End Class
                ");

            var sut = new NestingLevel();
            var results = sut.GetMetrics(parsedNode);

            results.Count().Should().Be(1);
            results.First().Method.Should().Be("Function TestMe(i as Integer)");
            results.First().Score.Should().Be(3);
        }

        [TestMethod]
        public void TwoCaseStatements_ShouldHave_NestingLevel1()
        {
            var parsedNode = GetSourceAsSyntaxTree(@"
                Imports System
                Imports System.Text

                <Serializable>_
                Public Class TestClass
                    Sub New()
                    
                    Public Function TestMe(i as Integer) As Boolean
                        Select Case i 
                            Case 10
                                return true
                        End Select

                        Select Case i
                            Case 20
                                return false
                        End Select

                        return false
                    End Function
                End Class
                ");

            var sut = new NestingLevel();
            var results = sut.GetMetrics(parsedNode);

            results.Count().Should().Be(1);
            results.First().Method.Should().Be("Function TestMe(i as Integer)");
            results.First().Score.Should().Be(1);
        }
    }
}
