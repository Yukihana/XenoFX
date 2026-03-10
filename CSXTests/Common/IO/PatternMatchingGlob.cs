using System.IO.Enumeration;

namespace CSXTests.Common.IO;

public class PatternMatchingGlob
{
    // ------------------------------
    // SimpleExpression tests
    // ------------------------------
    [Theory]
    [InlineData("file.txt", "*.*", true)]   // has dot
    [InlineData("file", "*.*", false)]      // no dot
    [InlineData("file.txt", "*", true)]     // matches everything
    [InlineData("file", "*", true)]         // matches everything
    [InlineData("foo.txt", "f?o.txt", true)] // '?' matches single char
    [InlineData("fo.txt", "f?o.txt", false)] // '?' requires exactly 1 char
    public void MatchesSimpleExpression_Behavior(string name, string pattern, bool expected)
    {
        bool result = FileSystemName.MatchesSimpleExpression(pattern, name, ignoreCase: false);
        Assert.Equal(expected, result);
    }

    // ------------------------------
    // Win32Expression tests
    // ------------------------------
    [Theory]
    [InlineData("file.txt", "*.*", true)]   // requires dot, matches
    [InlineData("file", "*.*", false)]      // no dot, no match
    [InlineData("file.txt", "*", true)]     // matches everything
    [InlineData("file", "*", true)]         // matches everything
    [InlineData("foo.txt", "f?o.txt", true)]
    [InlineData("fo.txt", "f?o.txt", false)]
    public void MatchesWin32Expression_Behavior(string name, string pattern, bool expected)
    {
        bool result = FileSystemName.MatchesWin32Expression(pattern, name, ignoreCase: false);
        Assert.Equal(expected, result);
    }

    // ------------------------------
    // Comparison tests (where they differ)
    // ------------------------------
    [Theory]
    // Identical behavior
    [InlineData("file.txt", "*.txt", true, true)]
    [InlineData("file", "*.txt", false, false)]
    [InlineData("foo.txt", "f?o.txt", true, true)]
    [InlineData("fo.txt", "f?o.txt", false, false)]
    [InlineData("file", "*.*", false, false)]

    // Separator differences
    [InlineData("foo\\bar.txt", "*.txt", true, true)]
    [InlineData("foo/bar.txt", "*.txt", true, true)]
    [InlineData("foo\\bar.txt", "foo*txt", true, true)]
    [InlineData("foo/bar.txt", "foo*txt", true, true)]
    public void Compare_Win32_vs_Simple(string name, string pattern, bool expectedWin32, bool expectedSimple)
    {
        bool win32 = FileSystemName.MatchesWin32Expression(pattern, name, ignoreCase: false);
        bool simple = FileSystemName.MatchesSimpleExpression(pattern, name, ignoreCase: false);

        Assert.Equal(expectedWin32, win32);
        Assert.Equal(expectedSimple, simple);
    }
}