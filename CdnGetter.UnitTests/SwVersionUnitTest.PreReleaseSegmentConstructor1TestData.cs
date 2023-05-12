namespace CdnGetter.UnitTests;

public partial class SwVersionUnitTest
{
    /// <summary>
    /// Generates test data for <see cref="PreReleaseSegmentConstructor1Test(bool, string?, string, string)" />.
    /// </summary>
    public class PreReleaseSegmentConstructor1TestData : TheoryData<bool, string?, string, string>
    {
        public PreReleaseSegmentConstructor1TestData()
        {
            Add(false, null, string.Empty, "-");
            Add(true, null, string.Empty, ".");
            Add(false, string.Empty, string.Empty, "-");
            Add(true, string.Empty, string.Empty, ".");
            foreach (string s in new string[] { "r" })
            {
                Add(false, s, s, $"-{s}");
                Add(true, s, s, $".{s}");
            }
        }
    }
}