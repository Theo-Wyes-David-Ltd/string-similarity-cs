using System.Text.RegularExpressions;

namespace StringSimilarityUtils;

public class Rating : IEquatable<Rating>
{
    public string target;
    public double rating;

    public override bool Equals(Object obj)
    {
        var other = obj as Rating;
        if (other == null) return false;
        return Equals(other);
    }

    public bool Equals(Rating other)
    {
        if (other == null) return false;
        if (ReferenceEquals(this, other)) return true;
        if (StringComparer.CurrentCulture.Compare(target, other.target) != 0) return false;
        if (rating != other.rating) return false;
        return true;
    }

    public override int GetHashCode()
    {
        return target.GetHashCode() + rating.GetHashCode();
    }

    public override string ToString()
    {
        return "Rating { target: " + target + ", rating: " + rating + " }";
    }
}

public class BestMatchResult
{
    public List<Rating> ratings;
    public Rating? bestMatch;
    public int? bestMatchIndex;

    public override string ToString()
    {
        string ratingsList = String.Join(", ", ratings);
        return "BestMatchResult { bestMatchIndex: " + bestMatchIndex + ", bestMatch: " + bestMatch + ", ratings: " + ratingsList + " }";
    }
}

public class SimilarSubstringResult
{
    public double result;
    public bool isSimilar;
    public string searchString;
    public string fullString;
    public double threshold;
    public string? subString;

    public override string ToString()
    {
        return "SimilarSubstringResult { result: " + result + ", isSimilar: " + isSimilar + ", searchString: " + searchString + ", fullString: " + fullString + ", threshold: " + threshold + ", subString: " + subString + " }";
    }
}

/// <summary>
/// Finds degree of similarity between two strings, based on <a href="http://en.wikipedia.org/wiki/S%C3%B8rensen%E2%80%93Dice_coefficient">Dice's Coefficient</a>, 
/// which is mostly better than <a href="http://en.wikipedia.org/wiki/Levenshtein_distance">Levenshtein distance</a>.
/// Note: This library is a port of the 
/// <a href="https://github.com/aceakash/string-similarity">string-similarity</a> <a href="https://www.npmjs.com/package/string-similarity">npm package</a>. 
/// </summary>
public static class StringSimilarity
{
    private static string NormalizeString(string? str)
    {
        return String.IsNullOrEmpty(str) ? "" : Regex.Replace(str.Trim(), @"\s", " ").ToLower();
    }

    /// <summary>
    /// Returns a fraction between 0 and 1, which indicates the degree of similarity between the two strings. 
    /// 0 indicates completely different strings, 1 indicates identical strings. 
    /// The comparison is case-sensitive. Order of parameters does not make a difference.
    /// </summary>
    /// <returns>
    /// A fraction from 0 to 1, both inclusive. Higher number indicates more similarity.
    /// </returns>
    /// <param name="first">
    /// The first string.
    /// </param>
    /// <param name="second">
    /// The second string.
    /// </param>
    public static double CompareTwoStrings(string? first, string? second)
    {
        first = String.IsNullOrEmpty(first) ? "" : Regex.Replace(first, @"\s+", "");
        second = String.IsNullOrEmpty(second) ? "" : Regex.Replace(second, @"\s+", "");

        if (first.Equals(second)) return 1;  // identical or empty
        if (first.Length < 2 || second.Length < 2) return 0; // if either is a 0-letter or 1-letter string

        Dictionary<string, int> firstBigrams = new Dictionary<string, int>();
        for (int i = 0; i < first.Length - 1; i++)
        {
            string bigram = first.Substring(i, 2);
            int count = firstBigrams.GetValueOrDefault(bigram, 0) + 1;
            firstBigrams[bigram] = count;
        };

        int intersectionSize = 0;
        for (int i = 0; i < second.Length - 1; i++)
        {
            string bigram = second.Substring(i, 2);
            int count = firstBigrams.GetValueOrDefault(bigram, 0);

            if (count > 0)
            {
                firstBigrams[bigram] = count - 1;
                intersectionSize++;
            }
        }

        return (2.0 * intersectionSize) / (first.Length + second.Length - 2);
    }

    /// <summary>
    /// Compares mainString against each string in targetStrings.
    /// </summary>
    /// <returns>
    /// An object with a ratings property, which gives a similarity rating for each target string, 
    ///a bestMatch property, which specifies which target string was most similar to the main string, 
    /// and a bestMatchIndex property, which specifies the index of the bestMatch in the targetStrings list.
    /// </returns>
    /// <param name="mainString">
    /// The string to match each target string against.
    /// </param>
    /// <param name="targetStrings">
    /// Each string in this list will be matched against the main string.
    /// </param>
    public static BestMatchResult? FindBestMatch(string? mainString, List<string>? targetStrings)
    {
        if (targetStrings == null || !targetStrings.Any())
        {
            return null;
        }

        mainString = String.IsNullOrEmpty(mainString) ? "" : mainString;

        List<Rating> ratings = new List<Rating>();
        int bestMatchIndex = 0;

        int i = 0;
        foreach (string currentTargetString in targetStrings)
        {
            double currentRating = CompareTwoStrings(mainString, currentTargetString);
            ratings.Add(new Rating() { target = currentTargetString, rating = currentRating });
            if (currentRating > ratings[bestMatchIndex].rating)
            {
                bestMatchIndex = i;
            }
            i++;
        }

        Rating bestMatch = ratings[bestMatchIndex];

        return new BestMatchResult() { ratings = ratings, bestMatch = bestMatch, bestMatchIndex = bestMatchIndex };
    }

    /// <summary>
    /// Determine if fullString contains a substring that is similar to searchString.
    /// </summary>
    /// <returns>
    /// An object with a isSimilar property, that will be true if a substring of fullString 
    /// is found to have a similarity rating > than `threshold` when compared to `searchString`.
    /// </returns>
    /// <param name="searchString">
    /// The substring to search for.
    /// </param>
    /// <param name="fullString">
    /// The full string to search within.
    /// </param>
    /// <param name="threshold">
    /// Optional. Value between 0 - 1. Default is .55.
    /// </param>
    public static SimilarSubstringResult HasSimilarSubstring(string searchString, string fullString, double threshold = 0.55)
    {
        searchString = NormalizeString(searchString);
        fullString = NormalizeString(fullString);

        if (searchString.Length == 0 || fullString.Length == 0 || searchString.Length > fullString.Length)
        {
            return new SimilarSubstringResult() { result = 0, isSimilar = false, searchString = searchString, fullString = fullString, threshold = threshold };
        }

        int substringIndex = fullString.IndexOf(searchString);
        if (substringIndex > -1)
        {
            string subString = fullString.Substring(substringIndex, searchString.Length);
            return new SimilarSubstringResult() { result = 1, isSimilar = true, searchString = searchString, fullString = fullString, threshold = threshold, subString = subString };
        }

        int lengthDiff = fullString.Length - searchString.Length;
        var allSubStrings = new List<string>();
        for (int i = 0; i <= lengthDiff; i++)
        {
            allSubStrings.Add(fullString.Substring(i, searchString.Length));
        }

        BestMatchResult? match = FindBestMatch(searchString, allSubStrings);
        double result = match?.bestMatch?.rating == null ? 0 : match.bestMatch.rating;
        string? similarSubString = String.IsNullOrEmpty(match?.bestMatch?.target) ? null : match.bestMatch.target;

        return new SimilarSubstringResult() { result = result, isSimilar = result >= threshold, searchString = searchString, fullString = fullString, threshold = threshold, subString = similarSubString };
    }
}
