# string-similarity-cs

Finds degree of similarity between two strings, based on [Dice's Coefficient](http://en.wikipedia.org/wiki/S%C3%B8rensen%E2%80%93Dice_coefficient), which is mostly better than [Levenshtein distance](http://en.wikipedia.org/wiki/Levenshtein_distance).

Note: This is a direct port of https://github.com/aceakash/string-similarity

## API

The package contains three methods:

### public static double CompareTwoStrings(string? first, string? second)

Returns a fraction between 0 and 1, which indicates the degree of similarity between the two strings. 0 indicates completely different strings, 1 indicates identical strings. The comparison is case-sensitive.

##### Arguments

1. string1 (string): The first string
2. string2 (string): The second string

Order of parameters does not make a difference.

##### Returns

(number): A fraction from 0 to 1, both inclusive. Higher number indicates more similarity.

##### Examples

```csharp
CompareTwoStrings("healed", "sealed");
// → 0.8

CompareTwoStrings(
  "Olive-green table for sale, in extremely good condition.",
  "For sale: table in very good  condition, olive green in colour."
);
// → 0.6060606060606061

CompareTwoStrings(
  "Olive-green table for sale, in extremely good condition.",
  "For sale: green Subaru Impreza, 210,000 miles"
);
// → 0.2558139534883721

CompareTwoStrings(
  "Olive-green table for sale, in extremely good condition.",
  "Wanted: mountain bike with at least 21 gears."
);
// → 0.1411764705882353
```

### public static BestMatchResult? FindBestMatch(string? mainString, List<string>? targetStrings)

Compares `mainString` against each string in `targetStrings`.

##### Arguments

1. mainString (string): The string to match each target string against.
2. targetStrings (List<string>): Each string in this array will be matched against the main string.

##### Returns

(BestMatchResult): An object with a `ratings` property, which gives a similarity rating for each target string, a `bestMatch` property, which specifies which target string was most similar to the main string, and a `bestMatchIndex` property, which specifies the index of the bestMatch in the targetStrings array.

##### Examples

```csharp
FindBestMatch('Olive-green table for sale, in extremely good condition.', [
  'For sale: green Subaru Impreza, 210,000 miles',
  'For sale: table in very good condition, olive green in colour.',
  'Wanted: mountain bike with at least 21 gears.'
]);
// →
{ ratings:
   [ { target: 'For sale: green Subaru Impreza, 210,000 miles',
       rating: 0.2558139534883721 },
     { target: 'For sale: table in very good condition, olive green in colour.',
       rating: 0.6060606060606061 },
     { target: 'Wanted: mountain bike with at least 21 gears.',
       rating: 0.1411764705882353 } ],
  bestMatch:
   { target: 'For sale: table in very good condition, olive green in colour.',
     rating: 0.6060606060606061 },
  bestMatchIndex: 1
}
```

### public static SimilarSubstringResult HasSimilarSubstring(string searchString, string fullString, double threshold = 0.55)

Determine if `fullString` contains a substring that is similar to `searchString`.

##### Arguments

1. searchString (string): The substring to search for.
2. fullString (List<string>): The full string to search within.
3. threshold (double): Optional. Value between 0 - 1. Default is .55.

##### Returns

(BestMatchResult): An object with a `isSimilar` property, that will be true if a substring of fullString is found to have a similarity rating > than `threshold` when compared to `searchString`.

## Release Notes

### 0.0.1

- Initial release