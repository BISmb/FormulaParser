using System.Linq.Expressions;
using FluentAssertions;
using Formula.Expressions;
using Formula.Models;
using Formula.Tests.Infrastructure;
using Formula.Visitors;
using Moq;

namespace Formula.Tests;

public class Array_Sum_Tests : FormulaTest
{
    private GridExpressionVisitor _gridVisitor;

    public Array_Sum_Tests()
    {
        var mockGrid = new Mock<IGrid>();
        mockGrid.Setup(g => g.GetAllCellReferencesFromArray(It.IsIn(new CellReferenceArray(new GridCellReference("A1"), new GridCellReference("A3")))))
            .Returns([
                new GridCellReference("A1"), 
                new GridCellReference("A2"), 
                new GridCellReference("A3")
            ])
            .Verifiable();
    
        mockGrid.Setup(g => g.GetValueForCell(It.IsIn(new GridCellReference("A1"))))
            .Returns(2)
            .Verifiable();
    
        mockGrid.Setup(g => g.GetValueForCell(It.IsIn(new GridCellReference("A2"))))
            .Returns(2)
            .Verifiable();
    
        mockGrid.Setup(g => g.GetValueForCell(It.IsIn(new GridCellReference("A3"))))
            .Returns(1)
            .Verifiable();
        
        _gridVisitor = new GridExpressionVisitor(mockGrid.Object);
    }

    [Theory]
    [InlineData("SUM(1,2,3)", 6)]
    public void Should_Sum_Numbers(string formula, int expectedResult)
    {
        var result = Evaluator.EvaluateFormula(formula);
        
        result
            .Should()
            .Be(expectedResult);
    }

    [Theory]
    [InlineData("SUM(A1:A3)", 6)]
    public void Should_Sum_GridArray(string formula, int expectedResult)
    {
        var result = Evaluator.EvaluateFormula(formula, _gridVisitor);
        
        result
            .Should()
            .Be(expectedResult);
    }

    [Fact]
    public void Should_Sum_GridArrayTest()
    {
        CellReferenceArray refArray = new CellReferenceArray(new("A1"), new("A3"));
        var allCellRefs = refArray.ToArray();
        
        // Parameters and variables
        var counter = Expression.Parameter(typeof(int), "counter");
        var sum = Expression.Parameter(typeof(int), "sum");
        var loopLabel = Expression.Label("Loop");
        
        // Body of the loop
        var loopBody = Expression.Block(
            // Increment sum: sum += counter
            Expression.AddAssign(sum, counter),
            // 1 + 2 + 3

            // Increment counter: counter++
            Expression.PreIncrementAssign(counter),

            // Break if condition is met: if (counter > 10) break;
            Expression.IfThen(
                Expression.GreaterThan(counter, Expression.Constant(allCellRefs.Length)),
                Expression.Break(loopLabel)
            )
        );

        // Loop construct
        var loop = Expression.Loop(loopBody, loopLabel);

        // Create a block that initializes variables, runs the loop, and returns the result
        var block = Expression.Block(
            new[] { counter, sum }, // Declare variables
            Expression.Assign(counter, Expression.Constant(0)), // counter = 0
            Expression.Assign(sum, Expression.Constant(0)),     // sum = 0
            loop,                                               // Execute the loop
            sum                                                 // Return the sum
        );

        // Compile and execute the expression
        var lambda = Expression.Lambda<Func<int>>(block);
        var actualResult = lambda.Compile()();
        
        // actualResult
        //     .Should()
        //     .Be(expectedResult);
    }
}