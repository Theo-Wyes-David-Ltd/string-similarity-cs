using NUnit.Framework;
using StringSimilarityUtils;
using System;
using System.Collections.Generic;

namespace StringSimilarityUtils.Tests;

class TestDataObject
{
    public string first;
    public string second;
    public double expected;
}

[TestFixture]
public class StringSimilarityUtilsTests
{
    [SetUp]
    public void Setup()
    {
        //StringSimilarityUtils.StringSimilarity.Console = TestContext.Out;
    }

    [Test]
    public void TestCompareTwoStrings()
    {
        TestDataObject[] testData = new TestDataObject[]{
            new TestDataObject(){ first="french", second="quebec", expected=0 },
            new TestDataObject(){ first="france", second="france", expected=1 },
            new TestDataObject(){ first="fRaNce", second="france", expected=0.2 },
            new TestDataObject(){ first="healed", second="sealed", expected=0.8 },
            new TestDataObject(){ first="web applications", second="applications of the web", expected=0.7878787878787878 },
            new TestDataObject(){ first="this will have a typo somewhere", second="this will huve a typo somewhere", expected=0.92 },
            new TestDataObject(){ first="Olive-green table for sale, in extremely good condition.", second="For sale: table in very good condition, olive green in colour.", expected=0.6060606060606061 },
            new TestDataObject(){ first="Olive-green table for sale, in extremely good condition.", second="For sale: green Subaru Impreza, 210,000 miles", expected=0.2558139534883721 },
            new TestDataObject(){ first="Olive-green table for sale, in extremely good condition.", second="Wanted: mountain bike with at least 21 gears.", expected=0.1411764705882353 },
            new TestDataObject(){ first="this has one extra word", second="this has one word", expected=0.7741935483870968 },
            new TestDataObject(){ first="a", second="a", expected=1 },
            new TestDataObject(){ first="a", second="b", expected=0 },
            new TestDataObject(){ first="", second="", expected=1 },
            new TestDataObject(){ first="a", second="", expected=0 },
            new TestDataObject(){ first="", second="a", expected=0 },
            new TestDataObject(){ first="apple event", second="apple event", expected=1 },
            new TestDataObject(){ first="iphone", second="iphone x", expected=0.9090909090909091 }
        };

        foreach (var td in testData)
        {
            Assert.AreEqual(td.expected, StringSimilarityUtils.StringSimilarity.CompareTwoStrings(td.first, td.second), 0, td.first + " and " + td.second);
        }
    }

    [Test]
    public void TestFindBestMatch()
    {
        string testString = "healed";
        var testArray = new List<String> { "mailed", "edward", "sealed", "theatre" };

        BestMatchResult? matches = StringSimilarityUtils.StringSimilarity.FindBestMatch(testString, testArray);
        List<Rating> testExpected = new List<Rating>{
            new Rating(){ target= "mailed", rating= 0.4 },
            new Rating(){ target= "edward", rating= 0.2 },
            new Rating(){ target= "sealed", rating= 0.8 },
            new Rating(){ target= "theatre", rating= 0.36363636363636365 }
        };

        Assert.AreEqual(testExpected, matches?.ratings);
        Assert.AreEqual(new Rating() { target = "sealed", rating = 0.8 }, matches?.bestMatch);
        Assert.AreEqual(2, matches?.bestMatchIndex);
    }

    [Test]
    public void TestHasSimilarSubstring()
    {
        string testSearchString = "lin Pack";
        string testFullString = "My name is Carlin Jackson";

        SimilarSubstringResult result = StringSimilarityUtils.StringSimilarity.HasSimilarSubstring(testSearchString, testFullString);

        Assert.AreEqual("lin jack", result.subString);
    }
}