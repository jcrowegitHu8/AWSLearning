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
[FeatureFlagApi Sample](https://raw.githack.com/jcrowegitHu8/AWSLearning/master/media/strykernet-example-mutation-report.html)

## Some lessions learned
<sup>Note: links to html were done via https://raw.githack.com/ since they couldn't be done natively from github </sup>
- [Found 1 area of code just never tested](https://raw.githack.com/jcrowegitHu8/AWSLearning/master/media/strykernet-example-mutation-report.html#Services/JwtPayloadParseMatchInListRuleService.cs)

- [Some things you are just never going to test.  And that's OK (Lines 82 & 97)](https://raw.githack.com/jcrowegitHu8/AWSLearning/master/media/strykernet-example-mutation-report.html#Services/RulesEngineService.cs)

Closing thoughts:

Remember mutant killing is fun!  And this tool is great at letting you look through the scroll blindness and directly at your true business logic.  But perfect is the enemy of the good and if you beleive that 20% of your code does 80% of the work.  Then it will take you 80% MORE effort to test that last 20% of code.  Meaning, while every project is different.  As a general rule I try and shoot for the 60-70% mutation score.  As you will be surprised at how hard it can be to do more than that.

<sup>At time of writing I had FeatureFlagApi mutant score at 72.22%</sup>
