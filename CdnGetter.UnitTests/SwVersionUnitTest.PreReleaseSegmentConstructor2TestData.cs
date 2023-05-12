namespace CdnGetter.UnitTests;

public partial class SwVersionUnitTest
{
    /// <summary>
    /// Generates test data for <see cref="PreReleaseSegmentConstructor2Test(bool, string)" />.
    /// </summary>
    public class PreReleaseSegmentConstructor2TestData : TheoryData<bool, string>
    {
        public PreReleaseSegmentConstructor2TestData()
        {
            foreach (string s in new string[] {
                "-",  ".", "+",
                " - ",  " . ", " + ",
                "r- ",  "r. ", "r+ ",
                " -v",  " .v", " +v" })
            {
                Add(false, s);
                Add(true, s);
            }
        }
    }
}