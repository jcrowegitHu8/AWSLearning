# Mutation Testing

## What is it?
Mutation Testing is a method to measure the <b>long-term</b> quality of your tests.
- Meaning, as developers goes about their days of coding under pressure of deadlines making changes to the evolving business logic.  How well will the test catch the tired developers logic flaws?
- An example is such small things as '==' vs '!='

## How does it compare to <u>Code Coverage</u>?
Code Coverage - shows how much of your code has been <b>touched</b> by testing.
- We all know how easy it is to game code coverage with "not real tests"

 - Mutation Testing - complements this and builds upon it more so by help you write test that are geared towards preventing regression.
- This is done by calling out areas in code where current test cases have not asserted definatively the intent of a given operator.

## How can you do mutation testing with dotnet code?
[Stryker.Net](https://stryker-mutator.io/docs/stryker-net/Introduction/)

## What does it look like
<iframe src="./media/strykernet-example-mutation-report.html" title="Strykernet Mutation Report"></iframe>