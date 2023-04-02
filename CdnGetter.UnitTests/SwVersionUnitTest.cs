namespace CdnGetter.UnitTests;

public class SwVersionUnitTest
{
    public record VersionValues(string? Prefix, int Major, int? Minor, int? Patch, int? Revision, int[]? AdditionalNumerical, SwVersion.PreReleaseSegment[]? PreRelease, SwVersion.BuildSegment[]? Build, SwVersion.VersionStringFormat Format);
    
    /// <summary>
    /// Test data for constructor <see cref="SwVersion(string?)" />
    /// </summary>
    public class ParsingConstructorTestData : TheoryData<string?, VersionValues>
    {
        public ParsingConstructorTestData()
        {
            Add(null,
                new(null,   0,              null,   null,   null,   null,
                null, null, SwVersion.VersionStringFormat.NonNumerical));
            Add("",
                new(null,   0,              null,   null,   null,   null,
                null, null, SwVersion.VersionStringFormat.NonNumerical));
            Add(" ",
                new(null,   0,              null,   null,   null,   null, 
                null, null, SwVersion.VersionStringFormat.NonNumerical));
            Add("\t",
                new(null,   0,              null,   null,   null,   null, 
                null, null, SwVersion.VersionStringFormat.NonNumerical));
            Add("\n",
                new(null,   0,              null,   null,   null,   null, 
                null, null, SwVersion.VersionStringFormat.NonNumerical));
            Add("null",
                new("null", 0,              null,   null,   null,   null, 
                null, null, SwVersion.VersionStringFormat.NonNumerical));
            Add("0",
                new(null,   0,              null,   null,   null,   null, 
                null, null, SwVersion.VersionStringFormat.Standard));
            Add("-0",
                new("-",    0,              null,   null,   null,   null, 
                null, null, SwVersion.VersionStringFormat.Standard));
            Add("1",
                new(null,   1,              null,   null,   null,   null, 
                null, null, SwVersion.VersionStringFormat.Standard));
            Add(" 1 ",
                new(null,   1,              null,   null,   null,   null, 
                null, null, SwVersion.VersionStringFormat.Standard));
            Add("2147483647",
                new(null,   2147483647,     null,   null,   null,   null, 
                null, null, SwVersion.VersionStringFormat.Standard));
            Add("-2147483648",
                new(null,   -2147483648,    null,   null,   null,   null, 
                null, null, SwVersion.VersionStringFormat.Standard));
            Add("-1",
                new(null,   -1,             null,   null,   null,   null, 
                null, null, SwVersion.VersionStringFormat.Standard));
            Add("1.0",
                new(null,   1,              0,      null,   null,   null, 
                null, null, SwVersion.VersionStringFormat.Standard));
            Add("1.0.0",
                new(null,   1,              0,      0,      null,   null, 
                null, null, SwVersion.VersionStringFormat.Standard));
            Add("1.0.0.0",
                new(null,   1,              0,      0,      0,      null, 
                null,   null, SwVersion.VersionStringFormat.Standard));
            Add("1.0.0.0.0",
                new(null,   1,              0,      0,      0,      new int [] { 0 }, 
                null,   null, SwVersion.VersionStringFormat.Standard));
            Add("2.1.1-rc1",
                new(null,   2,              1,      1,      null,   null,
                new SwVersion.PreReleaseSegment[] { new(false, "rc1") }, null, SwVersion.VersionStringFormat.Standard));
            Add("18.0.0-alpha-34308b5ad-20210729",
                new(null,   18,             0,      0,      null,   null,
                new SwVersion.PreReleaseSegment[] { new(false, "alpha"), new(false, "34308b5ad"), new(false, "20210729") }, null, SwVersion.VersionStringFormat.Standard));
            Add("2.7-beta.2",
                new(null,   2,              7,      null,   null,   null,
                new SwVersion.PreReleaseSegment[] { new(false, "beta"), new(true, "2") }, null, SwVersion.VersionStringFormat.Standard));
            Add("1.0.0rc10",
                new(null,   1,              0,      0,      null,   null,
                new SwVersion.PreReleaseSegment[] { new(true, "rc10") }, null, SwVersion.VersionStringFormat.Alt));
            Add("1-pre",
                new(null,   1,              null,   null,   null,   null,
                new SwVersion.PreReleaseSegment[] { new(false, "pre") }, null, SwVersion.VersionStringFormat.Standard));
            Add("r45",
                new("r",    45,             null,   null,   null,   null, 
                null, null, SwVersion.VersionStringFormat.Alt));
            Add("Utah",
                new("Utah", 0,              null,   null,   null,   null, 
                null, null, SwVersion.VersionStringFormat.NonNumerical));
        }
    }

    /// <summary>
    /// Unit test for constructor <see cref="SwVersion(string?)" />
    /// </summary>
    [Theory]
    [ClassData(typeof(ParsingConstructorTestData))]
    public void ParsingConstructorTest(string? versionString, VersionValues expected)
    {
        SwVersion result = new(versionString);
        Assert.Equal(expected.Prefix, result.Prefix);
        Assert.Equal(expected.Major, result.Major);
        Assert.Equal(expected.Minor, result.Minor);
        Assert.Equal(expected.Patch, result.Patch);
        Assert.Equal(expected.Revision, result.Revision);
        if (expected.AdditionalNumerical is null)
            Assert.Null(result.AdditionalNumerical);
        else
        {
            Assert.NotNull(result.AdditionalNumerical);
            Assert.Equal(expected.AdditionalNumerical, result.AdditionalNumerical);
        }
        if (expected.PreRelease is null)
            Assert.Null(result.PreRelease);
        else
        {
            Assert.NotNull(result.PreRelease);
            Assert.Equal(expected.PreRelease, result.PreRelease);
        }
        if (expected.Build is null)
            Assert.Null(result.Build);
        else
        {
            Assert.NotNull(result.Build);
            Assert.Equal(expected.Build, result.Build);
        }
        Assert.Equal(expected.Format, result.Format);
    }

    [Fact]
    public void ComponentConstructor0Test()
    {
        SwVersion result = new();
        Assert.Null(result.Prefix);
        Assert.Equal(0, result.Major);
        Assert.Null(result.Minor);
        Assert.Null(result.Patch);
        Assert.Null(result.Revision);
        Assert.Null(result.AdditionalNumerical);
        Assert.Null(result.PreRelease);
        Assert.Null(result.Build);
    }

    
    /// <summary>
    /// Test data for constructor <see cref="SwVersion(string, int, int, int, int, IEnumerable{int}, IEnumerable{SwVersion.PreReleaseSegment}, SwVersion.BuildSegment, SwVersion.BuildSegment[])" />.
    /// </summary>
    public class ComponentConstructor1TestData : TheoryData<string, int, int, int, int, IEnumerable<int>, IEnumerable<SwVersion.PreReleaseSegment>, SwVersion.BuildSegment, SwVersion.BuildSegment[], VersionValues>
    {
        public ComponentConstructor1TestData()
        {
            throw new NotImplementedException();
            // TODO: Implement test
            // foreach (string prefix in new string[] { "v", "package:", "R." })
            //     foreach (int major in new int[] { 0, 1, -1, int.MinValue, int.MaxValue })
            //         foreach (int minor in new int[] { 0, 1, int.MaxValue })
            //             foreach (int patch in new int[] { 0, 1, int.MaxValue })
            //                 foreach (int revision in new int[] { 0, 1, int.MaxValue })
            //                     foreach (int[] additionalNumerical in new int[][] { new int[] { 0 }, new int[] { 1, int.MaxValue } })
            //                         Add(prefix, major, minor, patch, revision, additionalNumerical, preRelease, build, additionalBuild, new(prefix, major, minor, patch, revision, additionalNumerical, preRelease, additionalBuild.Prepend(build), SwVersion.VersionStringFormat.Standard));
        }
    }

    /// <summary>
    /// Unit test for constructor <see cref="SwVersion(string, int, int, int, int, IEnumerable{int}, IEnumerable{SwVersion.PreReleaseSegment}, SwVersion.BuildSegment, SwVersion.BuildSegment[])" />.
    /// </summary>
    [Theory]
    [ClassData(typeof(ComponentConstructor1TestData))]
    public void ComponentConstructor1Test(string prefix, int major, int minor, int patch, int revision, IEnumerable<int> additionalNumerical, IEnumerable<SwVersion.PreReleaseSegment> preRelease, SwVersion.BuildSegment build, SwVersion.BuildSegment[] additionalBuild, VersionValues expected)
    {
        SwVersion target = new(prefix, major, minor, patch, revision, additionalNumerical, preRelease, build, additionalBuild);
        Assert.Equal(expected.Prefix, target.Prefix);
        Assert.Equal(expected.Major, target.Major);
        Assert.Equal(expected.Minor, target.Minor);
        Assert.Equal(expected.Patch, target.Patch);
        Assert.Equal(expected.Revision, target.Revision);
        if (expected.AdditionalNumerical is null)
            Assert.Null(target.AdditionalNumerical);
        else
        {
            Assert.NotNull(target.AdditionalNumerical);
            Assert.Equal(expected.AdditionalNumerical, target.AdditionalNumerical);
        }
        if (expected.PreRelease is null)
            Assert.Null(target.PreRelease);
        else
        {
            Assert.NotNull(target.PreRelease);
            Assert.Equal(expected.PreRelease, target.PreRelease);
        }
        if (expected.Build is null)
            Assert.Null(target.Build);
        else
        {
            Assert.NotNull(target.Build);
            Assert.Equal(expected.Build, target.Build);
        }
        Assert.Equal(expected.Format, target.Format);
    }

    /// <summary>
    /// Test data for constructor <see cref="SwVersion(int, int, int, int, IEnumerable{int}, IEnumerable{SwVersion.PreReleaseSegment}, SwVersion.BuildSegment, SwVersion.BuildSegment[])" />.
    /// </summary>
    public class ComponentConstructor2TestData : TheoryData<int, int, int, int, IEnumerable<int>, IEnumerable<SwVersion.PreReleaseSegment>, SwVersion.BuildSegment, SwVersion.BuildSegment[], VersionValues>
    {
        public ComponentConstructor2TestData()
        {
            throw new NotImplementedException();
            // TODO: Implement test
            // foreach (int major in new int[] { 0, 1, -1, int.MinValue, int.MaxValue })
            //     foreach (int minor in new int[] { 0, 1, int.MaxValue })
            //         foreach (int patch in new int[] { 0, 1, int.MaxValue })
            //             foreach (int revision in new int[] { 0, 1, int.MaxValue })
            //                 foreach (int[] additionalNumerical in new int[][] { new int[] { 0 }, new int[] { 1, int.MaxValue } })
            //                     Add(major, minor, patch, revision, additionalNumerical, preRelease, build, additionalBuild, new(null, major, minor, patch, revision, additionalNumerical, preRelease, additionalBuild.Prepend(build), SwVersion.VersionStringFormat.Standard));
        }
    }

    /// <summary>
    /// Unit test for constructor <see cref="SwVersion(int, int, int, int, IEnumerable{int}, IEnumerable{SwVersion.PreReleaseSegment}, SwVersion.BuildSegment, SwVersion.BuildSegment[])" />.
    /// </summary>
    [Theory]
    [ClassData(typeof(ComponentConstructor2TestData))]
    public void ComponentConstructor2Test(int major, int minor, int patch, int revision, IEnumerable<int> additionalNumerical, IEnumerable<SwVersion.PreReleaseSegment> preRelease, SwVersion.BuildSegment build, SwVersion.BuildSegment[] additionalBuild, VersionValues expected)
    {
        SwVersion target = new(major, minor, patch, revision, additionalNumerical, preRelease, build, additionalBuild);
        Assert.Equal(expected.Prefix, target.Prefix);
        Assert.Equal(expected.Major, target.Major);
        Assert.Equal(expected.Minor, target.Minor);
        Assert.Equal(expected.Patch, target.Patch);
        Assert.Equal(expected.Revision, target.Revision);
        if (expected.AdditionalNumerical is null)
            Assert.Null(target.AdditionalNumerical);
        else
        {
            Assert.NotNull(target.AdditionalNumerical);
            Assert.Equal(expected.AdditionalNumerical, target.AdditionalNumerical);
        }
        if (expected.PreRelease is null)
            Assert.Null(target.PreRelease);
        else
        {
            Assert.NotNull(target.PreRelease);
            Assert.Equal(expected.PreRelease, target.PreRelease);
        }
        if (expected.Build is null)
            Assert.Null(target.Build);
        else
        {
            Assert.NotNull(target.Build);
            Assert.Equal(expected.Build, target.Build);
        }
        Assert.Equal(expected.Format, target.Format);
    }

    /// <summary>
    /// Test data for constructor <see cref="SwVersion(string, int, int, int, int, IEnumerable{int}, SwVersion.PreReleaseSegment, SwVersion.PreReleaseSegment[])" />.
    /// </summary>
    public class ComponentConstructor3TestData : TheoryData<string, int, int, int, int, IEnumerable<int>, SwVersion.PreReleaseSegment, SwVersion.PreReleaseSegment[], VersionValues>
    {
        public ComponentConstructor3TestData()
        {
            throw new NotImplementedException();
            // TODO: Implement test
            // foreach (string prefix in new string[] { "v", "package:", "R." })
            //     foreach (int major in new int[] { 0, 1, -1, int.MinValue, int.MaxValue })
            //         foreach (int minor in new int[] { 0, 1, int.MaxValue })
            //             foreach (int patch in new int[] { 0, 1, int.MaxValue })
            //                 foreach (int revision in new int[] { 0, 1, int.MaxValue })
            //                     foreach (int[] additionalNumerical in new int[][] { new int[] { 0 }, new int[] { 1, int.MaxValue } })
            //                         Add(prefix, major, minor, patch, revision, additionalNumerical, preRelease, additionalPreRelease, new(prefix, major, minor, patch, revision, additionalNumerical, additionalPreRelease.Prepend(preRelease), null, SwVersion.VersionStringFormat.Standard));
        }
    }

    /// <summary>
    /// Unit test for constructor <see cref="SwVersion(string, int, int, int, int, IEnumerable{int}, SwVersion.PreReleaseSegment, SwVersion.PreReleaseSegment[])" />.
    /// </summary>
    [Theory]
    [ClassData(typeof(ComponentConstructor3TestData))]
    public void ComponentConstructor3Test(string prefix, int major, int minor, int patch, int revision, IEnumerable<int> additionalNumerical, SwVersion.PreReleaseSegment preRelease, SwVersion.PreReleaseSegment[] additionalPreRelease, VersionValues expected)
    {
        SwVersion target = new(prefix, major, minor, patch, revision, additionalNumerical, preRelease, additionalPreRelease);
        Assert.Equal(expected.Prefix, target.Prefix);
        Assert.Equal(expected.Major, target.Major);
        Assert.Equal(expected.Minor, target.Minor);
        Assert.Equal(expected.Patch, target.Patch);
        Assert.Equal(expected.Revision, target.Revision);
        if (expected.AdditionalNumerical is null)
            Assert.Null(target.AdditionalNumerical);
        else
        {
            Assert.NotNull(target.AdditionalNumerical);
            Assert.Equal(expected.AdditionalNumerical, target.AdditionalNumerical);
        }
        if (expected.PreRelease is null)
            Assert.Null(target.PreRelease);
        else
        {
            Assert.NotNull(target.PreRelease);
            Assert.Equal(expected.PreRelease, target.PreRelease);
        }
        if (expected.Build is null)
            Assert.Null(target.Build);
        else
        {
            Assert.NotNull(target.Build);
            Assert.Equal(expected.Build, target.Build);
        }
        Assert.Equal(expected.Format, target.Format);
    }

    /// <summary>
    /// Test data for constructor <see cref="SwVersion(int, int, int, int, IEnumerable{int}, SwVersion.PreReleaseSegment, SwVersion.PreReleaseSegment[])" />.
    /// </summary>
    public class ComponentConstructor4TestData : TheoryData<int, int, int, int, IEnumerable<int>, SwVersion.PreReleaseSegment, SwVersion.PreReleaseSegment[], VersionValues>
    {
        public ComponentConstructor4TestData()
        {
            throw new NotImplementedException();
            // TODO: Implement test
            // foreach (int major in new int[] { 0, 1, -1, int.MinValue, int.MaxValue })
            //     foreach (int minor in new int[] { 0, 1, int.MaxValue })
            //         foreach (int patch in new int[] { 0, 1, int.MaxValue })
            //             foreach (int revision in new int[] { 0, 1, int.MaxValue })
            //                 foreach (int[] additionalNumerical in new int[][] { new int[] { 0 }, new int[] { 1, int.MaxValue } })
            //                     Add(major, minor, patch, revision, additionalNumerical, preRelease, additionalPreRelease, new(null, major, minor, patch, revision, additionalNumerical, additionalPreRelease.Prepend(preRelease), null, SwVersion.VersionStringFormat.Standard));
        }
    }

    /// <summary>
    /// Unit test for constructor <see cref="SwVersion(int, int, int, int, IEnumerable{int}, SwVersion.PreReleaseSegment, SwVersion.PreReleaseSegment[])" />.
    /// </summary>
    [Theory]
    [ClassData(typeof(ComponentConstructor4TestData))]
    public void ComponentConstructor4Test(int major, int minor, int patch, int revision, IEnumerable<int> additionalNumerical, SwVersion.PreReleaseSegment preRelease, SwVersion.PreReleaseSegment[] additionalPreRelease, VersionValues expected)
    {
        SwVersion target = new(major, minor, patch, revision, additionalNumerical, preRelease, additionalPreRelease);
        Assert.Equal(expected.Prefix, target.Prefix);
        Assert.Equal(expected.Major, target.Major);
        Assert.Equal(expected.Minor, target.Minor);
        Assert.Equal(expected.Patch, target.Patch);
        Assert.Equal(expected.Revision, target.Revision);
        if (expected.AdditionalNumerical is null)
            Assert.Null(target.AdditionalNumerical);
        else
        {
            Assert.NotNull(target.AdditionalNumerical);
            Assert.Equal(expected.AdditionalNumerical, target.AdditionalNumerical);
        }
        if (expected.PreRelease is null)
            Assert.Null(target.PreRelease);
        else
        {
            Assert.NotNull(target.PreRelease);
            Assert.Equal(expected.PreRelease, target.PreRelease);
        }
        if (expected.Build is null)
            Assert.Null(target.Build);
        else
        {
            Assert.NotNull(target.Build);
            Assert.Equal(expected.Build, target.Build);
        }
        Assert.Equal(expected.Format, target.Format);
    }

    /// <summary>
    /// Test data for constructor <see cref="SwVersion(string, int, int, int, int, IEnumerable{int}, SwVersion.BuildSegment, SwVersion.BuildSegment[])" />.
    /// </summary>
    public class ComponentConstructor5TestData : TheoryData<string, int, int, int, int, IEnumerable<int>, SwVersion.BuildSegment, SwVersion.BuildSegment[], VersionValues>
    {
        public ComponentConstructor5TestData()
        {
            throw new NotImplementedException();
            // TODO: Implement test
            // foreach (string prefix in new string[] { "v", "package:", "R." })
            //     foreach (int major in new int[] { 0, 1, -1, int.MinValue, int.MaxValue })
            //         foreach (int minor in new int[] { 0, 1, int.MaxValue })
            //             foreach (int patch in new int[] { 0, 1, int.MaxValue })
            //                 foreach (int revision in new int[] { 0, 1, int.MaxValue })
            //                     foreach (int[] additionalNumerical in new int[][] { new int[] { 0 }, new int[] { 1, int.MaxValue } })
            //                         Add(prefix, major, minor, patch, revision, additionalNumerical, null, build, additionalBuild, new(prefix, major, minor, patch, revision, additionalNumerical, additionalBuild.Prepend(build), SwVersion.VersionStringFormat.Standard));
        }
    }

    /// <summary>
    /// Unit test for constructor <see cref="SwVersion(string, int, int, int, int, IEnumerable{int}, SwVersion.BuildSegment, SwVersion.BuildSegment[])" />.
    /// </summary>
    [Theory]
    [ClassData(typeof(ComponentConstructor5TestData))]
    public void ComponentConstructor5Test(string prefix, int major, int minor, int patch, int revision, IEnumerable<int> additionalNumerical, SwVersion.BuildSegment build, SwVersion.BuildSegment[] additionalBuild, VersionValues expected)
    {
        SwVersion target = new(prefix, major, minor, patch, revision, additionalNumerical, build, additionalBuild);
        Assert.Equal(expected.Prefix, target.Prefix);
        Assert.Equal(expected.Major, target.Major);
        Assert.Equal(expected.Minor, target.Minor);
        Assert.Equal(expected.Patch, target.Patch);
        Assert.Equal(expected.Revision, target.Revision);
        if (expected.AdditionalNumerical is null)
            Assert.Null(target.AdditionalNumerical);
        else
        {
            Assert.NotNull(target.AdditionalNumerical);
            Assert.Equal(expected.AdditionalNumerical, target.AdditionalNumerical);
        }
        if (expected.PreRelease is null)
            Assert.Null(target.PreRelease);
        else
        {
            Assert.NotNull(target.PreRelease);
            Assert.Equal(expected.PreRelease, target.PreRelease);
        }
        if (expected.Build is null)
            Assert.Null(target.Build);
        else
        {
            Assert.NotNull(target.Build);
            Assert.Equal(expected.Build, target.Build);
        }
        Assert.Equal(expected.Format, target.Format);
    }

    /// <summary>
    /// Test data for constructor <see cref="SwVersion(int, int, int, int, IEnumerable{int}, SwVersion.BuildSegment, SwVersion.BuildSegment[])" />.
    /// </summary>
    public class ComponentConstructor6TestData : TheoryData<int, int, int, int, IEnumerable<int>, SwVersion.BuildSegment, SwVersion.BuildSegment[], VersionValues>
    {
        public ComponentConstructor6TestData()
        {
            throw new NotImplementedException();
            // TODO: Implement test
            // foreach (int major in new int[] { 0, 1, -1, int.MinValue, int.MaxValue })
            //     foreach (int minor in new int[] { 0, 1, int.MaxValue })
            //         foreach (int patch in new int[] { 0, 1, int.MaxValue })
            //             foreach (int revision in new int[] { 0, 1, int.MaxValue })
            //                 foreach (int[] additionalNumerical in new int[][] { new int[] { 0 }, new int[] { 1, int.MaxValue } })
            //                     Add(major, minor, patch, revision, additionalNumerical, null, build, additionalBuild, new(null, major, minor, patch, revision, additionalNumerical, additionalBuild.Prepend(build), SwVersion.VersionStringFormat.Standard));
        }
    }

    /// <summary>
    /// Unit test for constructor <see cref="SwVersion(int, int, int, int, IEnumerable{int}, SwVersion.BuildSegment, SwVersion.BuildSegment[])" />.
    /// </summary>
    [Theory]
    [ClassData(typeof(ComponentConstructor6TestData))]
    public void ComponentConstructor6Test(int major, int minor, int patch, int revision, IEnumerable<int> additionalNumerical, SwVersion.BuildSegment build, SwVersion.BuildSegment[] additionalBuild, VersionValues expected)
    {
        SwVersion target = new(major, minor, patch, revision, additionalNumerical, build, additionalBuild);
        Assert.Equal(expected.Prefix, target.Prefix);
        Assert.Equal(expected.Major, target.Major);
        Assert.Equal(expected.Minor, target.Minor);
        Assert.Equal(expected.Patch, target.Patch);
        Assert.Equal(expected.Revision, target.Revision);
        if (expected.AdditionalNumerical is null)
            Assert.Null(target.AdditionalNumerical);
        else
        {
            Assert.NotNull(target.AdditionalNumerical);
            Assert.Equal(expected.AdditionalNumerical, target.AdditionalNumerical);
        }
        if (expected.PreRelease is null)
            Assert.Null(target.PreRelease);
        else
        {
            Assert.NotNull(target.PreRelease);
            Assert.Equal(expected.PreRelease, target.PreRelease);
        }
        if (expected.Build is null)
            Assert.Null(target.Build);
        else
        {
            Assert.NotNull(target.Build);
            Assert.Equal(expected.Build, target.Build);
        }
        Assert.Equal(expected.Format, target.Format);
    }

    /// <summary>
    /// Test data for constructor <see cref="SwVersion(string, int, int, int, IEnumerable{SwVersion.PreReleaseSegment}, SwVersion.BuildSegment, SwVersion.BuildSegment[])" />.
    /// </summary>
    public class ComponentConstructor7TestData : TheoryData<string, int, int, int, IEnumerable<SwVersion.PreReleaseSegment>, SwVersion.BuildSegment, SwVersion.BuildSegment[], VersionValues>
    {
        public ComponentConstructor7TestData()
        {
            throw new NotImplementedException();
            // TODO: Implement test
            // foreach (string prefix in new string[] { "v", "package:", "R." })
            //     foreach (int major in new int[] { 0, 1, -1, int.MinValue, int.MaxValue })
            //         foreach (int minor in new int[] { 0, 1, int.MaxValue })
            //             foreach (int patch in new int[] { 0, 1, int.MaxValue })
            //                 Add(prefix, major, minor, patch, preRelease, build, additionalBuild, new(prefix, major, minor, patch, null, null, preRelease, additionalBuild.Prepend(build), SwVersion.VersionStringFormat.Standard));
        }
    }

    /// <summary>
    /// Unit test for constructor <see cref="SwVersion(string, int, int, int, IEnumerable{SwVersion.PreReleaseSegment}, SwVersion.BuildSegment, SwVersion.BuildSegment[])" />.
    /// </summary>
    [Theory]
    [ClassData(typeof(ComponentConstructor7TestData))]
    public void ComponentConstructor7Test(string prefix, int major, int minor, int patch, IEnumerable<SwVersion.PreReleaseSegment> preRelease, SwVersion.BuildSegment build, SwVersion.BuildSegment[] additionalBuild, VersionValues expected)
    {
        SwVersion target = new(prefix, major, minor, patch, preRelease, build, additionalBuild);
        Assert.Equal(expected.Prefix, target.Prefix);
        Assert.Equal(expected.Major, target.Major);
        Assert.Equal(expected.Minor, target.Minor);
        Assert.Equal(expected.Patch, target.Patch);
        Assert.Equal(expected.Revision, target.Revision);
        if (expected.AdditionalNumerical is null)
            Assert.Null(target.AdditionalNumerical);
        else
        {
            Assert.NotNull(target.AdditionalNumerical);
            Assert.Equal(expected.AdditionalNumerical, target.AdditionalNumerical);
        }
        if (expected.PreRelease is null)
            Assert.Null(target.PreRelease);
        else
        {
            Assert.NotNull(target.PreRelease);
            Assert.Equal(expected.PreRelease, target.PreRelease);
        }
        if (expected.Build is null)
            Assert.Null(target.Build);
        else
        {
            Assert.NotNull(target.Build);
            Assert.Equal(expected.Build, target.Build);
        }
        Assert.Equal(expected.Format, target.Format);
    }

    /// <summary>
    /// Test data for constructor <see cref="SwVersion(string, int, int, int, int, int[])" />.
    /// </summary>
    public class ComponentConstructor8TestData : TheoryData<string, int, int, int, int, int[], VersionValues>
    {
        public ComponentConstructor8TestData()
        {
            foreach (string prefix in new string[] { "v", "package:", "R." })
                foreach (int major in new int[] { 0, 1, -1, int.MinValue, int.MaxValue })
                    foreach (int minor in new int[] { 0, 1, int.MaxValue })
                        foreach (int patch in new int[] { 0, 1, int.MaxValue })
                            foreach (int revision in new int[] { 0, 1, int.MaxValue })
                                foreach (int[] additionalNumerical in new int[][] { new int[] { 0 }, new int[] { 1, int.MaxValue } })
                                    Add(prefix, major, minor, patch, revision, additionalNumerical, new(prefix, major, minor, patch, revision, additionalNumerical, null, null, SwVersion.VersionStringFormat.Standard));
        }
    }

    /// <summary>
    /// Unit test for constructor <see cref="SwVersion(string, int, int, int, int, int[])" />.
    /// </summary>
    [Theory]
    [ClassData(typeof(ComponentConstructor8TestData))]
    public void ComponentConstructor8Test(string prefix, int major, int minor, int patch, int revision, int[] additionalNumerical, VersionValues expected)
    {
        SwVersion target = new(prefix, major, minor, patch, revision, additionalNumerical);
        Assert.Equal(expected.Prefix, target.Prefix);
        Assert.Equal(expected.Major, target.Major);
        Assert.Equal(expected.Minor, target.Minor);
        Assert.Equal(expected.Patch, target.Patch);
        Assert.Equal(expected.Revision, target.Revision);
        if (expected.AdditionalNumerical is null)
            Assert.Null(target.AdditionalNumerical);
        else
        {
            Assert.NotNull(target.AdditionalNumerical);
            Assert.Equal(expected.AdditionalNumerical, target.AdditionalNumerical);
        }
        if (expected.PreRelease is null)
            Assert.Null(target.PreRelease);
        else
        {
            Assert.NotNull(target.PreRelease);
            Assert.Equal(expected.PreRelease, target.PreRelease);
        }
        if (expected.Build is null)
            Assert.Null(target.Build);
        else
        {
            Assert.NotNull(target.Build);
            Assert.Equal(expected.Build, target.Build);
        }
        Assert.Equal(expected.Format, target.Format);
    }

    /// <summary>
    /// Test data for constructor <see cref="SwVersion(int, int, int, IEnumerable{SwVersion.PreReleaseSegment}, SwVersion.BuildSegment, SwVersion.BuildSegment[])" />.
    /// </summary>
    public class ComponentConstructor9TestData : TheoryData<int, int, int, IEnumerable<SwVersion.PreReleaseSegment>, SwVersion.BuildSegment, SwVersion.BuildSegment[], VersionValues>
    {
        public ComponentConstructor9TestData()
        {
            throw new NotImplementedException();
            // TODO: Implement test
            // foreach (int major in new int[] { 0, 1, -1, int.MinValue, int.MaxValue })
            //     foreach (int minor in new int[] { 0, 1, int.MaxValue })
            //         foreach (int patch in new int[] { 0, 1, int.MaxValue })
            //             Add(major, minor, patch, preRelease, build, additionalBuild, new(null, major, minor, patch, null, null, preRelease, additionalBuild.Prepend(build), SwVersion.VersionStringFormat.Standard));
        }
    }

    /// <summary>
    /// Unit test for constructor <see cref="SwVersion(int, int, int, IEnumerable{SwVersion.PreReleaseSegment}, SwVersion.BuildSegment, SwVersion.BuildSegment[])" />.
    /// </summary>
    [Theory]
    [ClassData(typeof(ComponentConstructor9TestData))]
    public void ComponentConstructor9Test(int major, int minor, int patch, IEnumerable<SwVersion.PreReleaseSegment> preRelease, SwVersion.BuildSegment build, SwVersion.BuildSegment[] additionalBuild, VersionValues expected)
    {
        SwVersion target = new(major, minor, patch, preRelease, build, additionalBuild);
        Assert.Equal(expected.Prefix, target.Prefix);
        Assert.Equal(expected.Major, target.Major);
        Assert.Equal(expected.Minor, target.Minor);
        Assert.Equal(expected.Patch, target.Patch);
        Assert.Equal(expected.Revision, target.Revision);
        if (expected.AdditionalNumerical is null)
            Assert.Null(target.AdditionalNumerical);
        else
        {
            Assert.NotNull(target.AdditionalNumerical);
            Assert.Equal(expected.AdditionalNumerical, target.AdditionalNumerical);
        }
        if (expected.PreRelease is null)
            Assert.Null(target.PreRelease);
        else
        {
            Assert.NotNull(target.PreRelease);
            Assert.Equal(expected.PreRelease, target.PreRelease);
        }
        if (expected.Build is null)
            Assert.Null(target.Build);
        else
        {
            Assert.NotNull(target.Build);
            Assert.Equal(expected.Build, target.Build);
        }
        Assert.Equal(expected.Format, target.Format);
    }

    /// <summary>
    /// Test data for constructor <see cref="SwVersion(string, int, int, int, SwVersion.PreReleaseSegment, SwVersion.PreReleaseSegment[])" />.
    /// </summary>
    public class ComponentConstructor10TestData : TheoryData<string, int, int, int, SwVersion.PreReleaseSegment, SwVersion.PreReleaseSegment[], VersionValues>
    {
        public ComponentConstructor10TestData()
        {
            throw new NotImplementedException();
            // TODO: Implement test
            // foreach (string prefix in new string[] { "v", "package:", "R." })
            //     foreach (int major in new int[] { 0, 1, -1, int.MinValue, int.MaxValue })
            //         foreach (int minor in new int[] { 0, 1, int.MaxValue })
            //             foreach (int patch in new int[] { 0, 1, int.MaxValue })
            //                 Add(prefix, major, minor, patch, preRelease, additionalPreRelease, new(prefix, major, minor, patch, null, null, additionalPreRelease.Prepend(preRelease), null, SwVersion.VersionStringFormat.Standard));
        }
    }

    /// <summary>
    /// Unit test for constructor <see cref="SwVersion(string, int, int, int, SwVersion.PreReleaseSegment, SwVersion.PreReleaseSegment[])" />.
    /// </summary>
    [Theory]
    [ClassData(typeof(ComponentConstructor10TestData))]
    public void ComponentConstructor10Test(string prefix, int major, int minor, int patch, SwVersion.PreReleaseSegment preRelease, SwVersion.PreReleaseSegment[] additionalPreRelease, VersionValues expected)
    {
        SwVersion target = new(prefix, major, minor, patch, preRelease, additionalPreRelease);
        Assert.Equal(expected.Prefix, target.Prefix);
        Assert.Equal(expected.Major, target.Major);
        Assert.Equal(expected.Minor, target.Minor);
        Assert.Equal(expected.Patch, target.Patch);
        Assert.Equal(expected.Revision, target.Revision);
        if (expected.AdditionalNumerical is null)
            Assert.Null(target.AdditionalNumerical);
        else
        {
            Assert.NotNull(target.AdditionalNumerical);
            Assert.Equal(expected.AdditionalNumerical, target.AdditionalNumerical);
        }
        if (expected.PreRelease is null)
            Assert.Null(target.PreRelease);
        else
        {
            Assert.NotNull(target.PreRelease);
            Assert.Equal(expected.PreRelease, target.PreRelease);
        }
        if (expected.Build is null)
            Assert.Null(target.Build);
        else
        {
            Assert.NotNull(target.Build);
            Assert.Equal(expected.Build, target.Build);
        }
        Assert.Equal(expected.Format, target.Format);
    }

    /// <summary>
    /// Test data for constructor <see cref="SwVersion(int, int, int, SwVersion.PreReleaseSegment, SwVersion.PreReleaseSegment[])" />.
    /// </summary>
    public class ComponentConstructor11TestData : TheoryData<int, int, int, SwVersion.PreReleaseSegment, SwVersion.PreReleaseSegment[], VersionValues>
    {
        public ComponentConstructor11TestData()
        {
            throw new NotImplementedException();
            // TODO: Implement test
            // foreach (int major in new int[] { 0, 1, -1, int.MinValue, int.MaxValue })
            //     foreach (int minor in new int[] { 0, 1, int.MaxValue })
            //         foreach (int patch in new int[] { 0, 1, int.MaxValue })
            //             Add(major, minor, patch, preRelease, additionalPreRelease, new(null, major, minor, patch, null, null, additionalPreRelease.Prepend(preRelease), null, SwVersion.VersionStringFormat.Standard));
        }
    }

    /// <summary>
    /// Unit test for constructor <see cref="SwVersion(int, int, int, SwVersion.PreReleaseSegment, SwVersion.PreReleaseSegment[])" />.
    /// </summary>
    [Theory]
    [ClassData(typeof(ComponentConstructor11TestData))]
    public void ComponentConstructor11Test(int major, int minor, int patch, SwVersion.PreReleaseSegment preRelease, SwVersion.PreReleaseSegment[] additionalPreRelease, VersionValues expected)
    {
        SwVersion target = new(major, minor, patch, preRelease, additionalPreRelease);
        Assert.Equal(expected.Prefix, target.Prefix);
        Assert.Equal(expected.Major, target.Major);
        Assert.Equal(expected.Minor, target.Minor);
        Assert.Equal(expected.Patch, target.Patch);
        Assert.Equal(expected.Revision, target.Revision);
        if (expected.AdditionalNumerical is null)
            Assert.Null(target.AdditionalNumerical);
        else
        {
            Assert.NotNull(target.AdditionalNumerical);
            Assert.Equal(expected.AdditionalNumerical, target.AdditionalNumerical);
        }
        if (expected.PreRelease is null)
            Assert.Null(target.PreRelease);
        else
        {
            Assert.NotNull(target.PreRelease);
            Assert.Equal(expected.PreRelease, target.PreRelease);
        }
        if (expected.Build is null)
            Assert.Null(target.Build);
        else
        {
            Assert.NotNull(target.Build);
            Assert.Equal(expected.Build, target.Build);
        }
        Assert.Equal(expected.Format, target.Format);
    }

    /// <summary>
    /// Test data for constructor <see cref="SwVersion(string, int, int, int, SwVersion.BuildSegment, SwVersion.BuildSegment[])" />.
    /// </summary>
    public class ComponentConstructor12TestData : TheoryData<string, int, int, int, SwVersion.BuildSegment, SwVersion.BuildSegment[], VersionValues>
    {
        public ComponentConstructor12TestData()
        {
            throw new NotImplementedException();
            // TODO: Implement test
            // foreach (string prefix in new string[] { "v", "package:", "R." })
            //     foreach (int major in new int[] { 0, 1, -1, int.MinValue, int.MaxValue })
            //         foreach (int minor in new int[] { 0, 1, int.MaxValue })
            //             foreach (int patch in new int[] { 0, 1, int.MaxValue })
            //                 Add(prefix, major, minor, patch, null, build, additionalBuild, new(prefix, major, minor, patch, null, null, additionalBuild.Prepend(build), SwVersion.VersionStringFormat.Standard));
        }
    }

    /// <summary>
    /// Unit test for constructor <see cref="SwVersion(string, int, int, int, SwVersion.BuildSegment, SwVersion.BuildSegment[])" />.
    /// </summary>
    [Theory]
    [ClassData(typeof(ComponentConstructor12TestData))]
    public void ComponentConstructor12Test(string prefix, int major, int minor, int patch, SwVersion.BuildSegment build, SwVersion.BuildSegment[] additionalBuild, VersionValues expected)
    {
        SwVersion target = new(prefix, major, minor, patch, build, additionalBuild);
        Assert.Equal(expected.Prefix, target.Prefix);
        Assert.Equal(expected.Major, target.Major);
        Assert.Equal(expected.Minor, target.Minor);
        Assert.Equal(expected.Patch, target.Patch);
        Assert.Equal(expected.Revision, target.Revision);
        if (expected.AdditionalNumerical is null)
            Assert.Null(target.AdditionalNumerical);
        else
        {
            Assert.NotNull(target.AdditionalNumerical);
            Assert.Equal(expected.AdditionalNumerical, target.AdditionalNumerical);
        }
        if (expected.PreRelease is null)
            Assert.Null(target.PreRelease);
        else
        {
            Assert.NotNull(target.PreRelease);
            Assert.Equal(expected.PreRelease, target.PreRelease);
        }
        if (expected.Build is null)
            Assert.Null(target.Build);
        else
        {
            Assert.NotNull(target.Build);
            Assert.Equal(expected.Build, target.Build);
        }
        Assert.Equal(expected.Format, target.Format);
    }

    /// <summary>
    /// Test data for constructor <see cref="SwVersion(string, int, int, IEnumerable{SwVersion.PreReleaseSegment}, SwVersion.BuildSegment, SwVersion.BuildSegment[])" />.
    /// </summary>
    public class ComponentConstructor13TestData : TheoryData<string, int, int, IEnumerable<SwVersion.PreReleaseSegment>, SwVersion.BuildSegment, SwVersion.BuildSegment[], VersionValues>
    {
        public ComponentConstructor13TestData()
        {
            throw new NotImplementedException();
            // TODO: Implement test
            // foreach (string prefix in new string[] { "v", "package:", "R." })
            //     foreach (int major in new int[] { 0, 1, -1, int.MinValue, int.MaxValue })
            //         foreach (int minor in new int[] { 0, 1, int.MaxValue })
            //             Add(prefix, major, minor, preRelease, build, additionalBuild, new(prefix, major, minor, null, null, null, preRelease, additionalBuild.Prepend(build), SwVersion.VersionStringFormat.Standard));
        }
    }

    /// <summary>
    /// Unit test for constructor <see cref="SwVersion(string, int, int, IEnumerable{SwVersion.PreReleaseSegment}, SwVersion.BuildSegment, SwVersion.BuildSegment[])" />.
    /// </summary>
    [Theory]
    [ClassData(typeof(ComponentConstructor13TestData))]
    public void ComponentConstructor13Test(string prefix, int major, int minor, IEnumerable<SwVersion.PreReleaseSegment> preRelease, SwVersion.BuildSegment build, SwVersion.BuildSegment[] additionalBuild, VersionValues expected)
    {
        SwVersion target = new(prefix, major, minor, preRelease, build, additionalBuild);
        Assert.Equal(expected.Prefix, target.Prefix);
        Assert.Equal(expected.Major, target.Major);
        Assert.Equal(expected.Minor, target.Minor);
        Assert.Equal(expected.Patch, target.Patch);
        Assert.Equal(expected.Revision, target.Revision);
        if (expected.AdditionalNumerical is null)
            Assert.Null(target.AdditionalNumerical);
        else
        {
            Assert.NotNull(target.AdditionalNumerical);
            Assert.Equal(expected.AdditionalNumerical, target.AdditionalNumerical);
        }
        if (expected.PreRelease is null)
            Assert.Null(target.PreRelease);
        else
        {
            Assert.NotNull(target.PreRelease);
            Assert.Equal(expected.PreRelease, target.PreRelease);
        }
        if (expected.Build is null)
            Assert.Null(target.Build);
        else
        {
            Assert.NotNull(target.Build);
            Assert.Equal(expected.Build, target.Build);
        }
        Assert.Equal(expected.Format, target.Format);
    }

    /// <summary>
    /// Test data for constructor <see cref="SwVersion(int, int, int, int, int[])" />.
    /// </summary>
    public class ComponentConstructor14TestData : TheoryData<int, int, int, int, int[], VersionValues>
    {
        public ComponentConstructor14TestData()
        {
            foreach (int major in new int[] { 0, 1, -1, int.MinValue, int.MaxValue })
                foreach (int minor in new int[] { 0, 1, int.MaxValue })
                    foreach (int patch in new int[] { 0, 1, int.MaxValue })
                        foreach (int revision in new int[] { 0, 1, int.MaxValue })
                            foreach (int[] additionalNumerical in new int[][] { new int[] { 0 }, new int[] { 1, int.MaxValue } })
                                Add(major, minor, patch, revision, additionalNumerical, new(null, major, minor, patch, revision, additionalNumerical, null, null, SwVersion.VersionStringFormat.Standard));
        }
    }

    /// <summary>
    /// Unit test for constructor <see cref="SwVersion(int, int, int, int, int[])" />.
    /// </summary>
    [Theory]
    [ClassData(typeof(ComponentConstructor14TestData))]
    public void ComponentConstructor14Test(int major, int minor, int patch, int revision, int[] additionalNumerical, VersionValues expected)
    {
        SwVersion target = new(major, minor, patch, revision, additionalNumerical);
        Assert.Equal(expected.Prefix, target.Prefix);
        Assert.Equal(expected.Major, target.Major);
        Assert.Equal(expected.Minor, target.Minor);
        Assert.Equal(expected.Patch, target.Patch);
        Assert.Equal(expected.Revision, target.Revision);
        if (expected.AdditionalNumerical is null)
            Assert.Null(target.AdditionalNumerical);
        else
        {
            Assert.NotNull(target.AdditionalNumerical);
            Assert.Equal(expected.AdditionalNumerical, target.AdditionalNumerical);
        }
        if (expected.PreRelease is null)
            Assert.Null(target.PreRelease);
        else
        {
            Assert.NotNull(target.PreRelease);
            Assert.Equal(expected.PreRelease, target.PreRelease);
        }
        if (expected.Build is null)
            Assert.Null(target.Build);
        else
        {
            Assert.NotNull(target.Build);
            Assert.Equal(expected.Build, target.Build);
        }
        Assert.Equal(expected.Format, target.Format);
    }

    /// <summary>
    /// Test data for constructor <see cref="SwVersion(int, int, int, SwVersion.BuildSegment, SwVersion.BuildSegment[])" />.
    /// </summary>
    public class ComponentConstructor15TestData : TheoryData<int, int, int, SwVersion.BuildSegment, SwVersion.BuildSegment[], VersionValues>
    {
        public ComponentConstructor15TestData()
        {
            throw new NotImplementedException();
            // TODO: Implement test
            // foreach (int major in new int[] { 0, 1, -1, int.MinValue, int.MaxValue })
            //     foreach (int minor in new int[] { 0, 1, int.MaxValue })
            //         foreach (int patch in new int[] { 0, 1, int.MaxValue })
            //             Add(major, minor, patch, null, build, additionalBuild, new(null, major, minor, patch, null, null, additionalBuild.Prepend(build), SwVersion.VersionStringFormat.Standard));
        }
    }

    /// <summary>
    /// Unit test for constructor <see cref="SwVersion(int, int, int, SwVersion.BuildSegment, SwVersion.BuildSegment[])" />.
    /// </summary>
    [Theory]
    [ClassData(typeof(ComponentConstructor15TestData))]
    public void ComponentConstructor15Test(int major, int minor, int patch, SwVersion.BuildSegment build, SwVersion.BuildSegment[] additionalBuild, VersionValues expected)
    {
        SwVersion target = new(major, minor, patch, build, additionalBuild);
        Assert.Equal(expected.Prefix, target.Prefix);
        Assert.Equal(expected.Major, target.Major);
        Assert.Equal(expected.Minor, target.Minor);
        Assert.Equal(expected.Patch, target.Patch);
        Assert.Equal(expected.Revision, target.Revision);
        if (expected.AdditionalNumerical is null)
            Assert.Null(target.AdditionalNumerical);
        else
        {
            Assert.NotNull(target.AdditionalNumerical);
            Assert.Equal(expected.AdditionalNumerical, target.AdditionalNumerical);
        }
        if (expected.PreRelease is null)
            Assert.Null(target.PreRelease);
        else
        {
            Assert.NotNull(target.PreRelease);
            Assert.Equal(expected.PreRelease, target.PreRelease);
        }
        if (expected.Build is null)
            Assert.Null(target.Build);
        else
        {
            Assert.NotNull(target.Build);
            Assert.Equal(expected.Build, target.Build);
        }
        Assert.Equal(expected.Format, target.Format);
    }

    /// <summary>
    /// Test data for constructor <see cref="SwVersion(int, int, IEnumerable{SwVersion.PreReleaseSegment}, SwVersion.BuildSegment, SwVersion.BuildSegment[])" />.
    /// </summary>
    public class ComponentConstructor16TestData : TheoryData<int, int, IEnumerable<SwVersion.PreReleaseSegment>, SwVersion.BuildSegment, SwVersion.BuildSegment[], VersionValues>
    {
        public ComponentConstructor16TestData()
        {
            throw new NotImplementedException();
            // TODO: Implement test
            // foreach (int major in new int[] { 0, 1, -1, int.MinValue, int.MaxValue })
            //     foreach (int minor in new int[] { 0, 1, int.MaxValue })
            //         Add(major, minor, preRelease, build, additionalBuild, new(null, major, minor, null, null, null, preRelease, additionalBuild.Prepend(build), SwVersion.VersionStringFormat.Standard));
        }
    }

    /// <summary>
    /// Unit test for constructor <see cref="SwVersion(int, int, IEnumerable{SwVersion.PreReleaseSegment}, SwVersion.BuildSegment, SwVersion.BuildSegment[])" />.
    /// </summary>
    [Theory]
    [ClassData(typeof(ComponentConstructor16TestData))]
    public void ComponentConstructor16Test(int major, int minor, IEnumerable<SwVersion.PreReleaseSegment> preRelease, SwVersion.BuildSegment build, SwVersion.BuildSegment[] additionalBuild, VersionValues expected)
    {
        SwVersion target = new(major, minor, preRelease, build, additionalBuild);
        Assert.Equal(expected.Prefix, target.Prefix);
        Assert.Equal(expected.Major, target.Major);
        Assert.Equal(expected.Minor, target.Minor);
        Assert.Equal(expected.Patch, target.Patch);
        Assert.Equal(expected.Revision, target.Revision);
        if (expected.AdditionalNumerical is null)
            Assert.Null(target.AdditionalNumerical);
        else
        {
            Assert.NotNull(target.AdditionalNumerical);
            Assert.Equal(expected.AdditionalNumerical, target.AdditionalNumerical);
        }
        if (expected.PreRelease is null)
            Assert.Null(target.PreRelease);
        else
        {
            Assert.NotNull(target.PreRelease);
            Assert.Equal(expected.PreRelease, target.PreRelease);
        }
        if (expected.Build is null)
            Assert.Null(target.Build);
        else
        {
            Assert.NotNull(target.Build);
            Assert.Equal(expected.Build, target.Build);
        }
        Assert.Equal(expected.Format, target.Format);
    }

    /// <summary>
    /// Test data for constructor <see cref="SwVersion(string, int, int, SwVersion.PreReleaseSegment, SwVersion.PreReleaseSegment[])" />.
    /// </summary>
    public class ComponentConstructor17TestData : TheoryData<string, int, int, SwVersion.PreReleaseSegment, SwVersion.PreReleaseSegment[], VersionValues>
    {
        public ComponentConstructor17TestData()
        {
            throw new NotImplementedException();
            // TODO: Implement test
            // foreach (string prefix in new string[] { "v", "package:", "R." })
            //     foreach (int major in new int[] { 0, 1, -1, int.MinValue, int.MaxValue })
            //         foreach (int minor in new int[] { 0, 1, int.MaxValue })
            //             Add(prefix, major, minor, preRelease, additionalPreRelease, new(prefix, major, minor, null, null, null, additionalPreRelease.Prepend(preRelease), null, SwVersion.VersionStringFormat.Standard));
        }
    }

    /// <summary>
    /// Unit test for constructor <see cref="SwVersion(string, int, int, SwVersion.PreReleaseSegment, SwVersion.PreReleaseSegment[])" />.
    /// </summary>
    [Theory]
    [ClassData(typeof(ComponentConstructor17TestData))]
    public void ComponentConstructor17Test(string prefix, int major, int minor, SwVersion.PreReleaseSegment preRelease, SwVersion.PreReleaseSegment[] additionalPreRelease, VersionValues expected)
    {
        SwVersion target = new(prefix, major, minor, preRelease, additionalPreRelease);
        Assert.Equal(expected.Prefix, target.Prefix);
        Assert.Equal(expected.Major, target.Major);
        Assert.Equal(expected.Minor, target.Minor);
        Assert.Equal(expected.Patch, target.Patch);
        Assert.Equal(expected.Revision, target.Revision);
        if (expected.AdditionalNumerical is null)
            Assert.Null(target.AdditionalNumerical);
        else
        {
            Assert.NotNull(target.AdditionalNumerical);
            Assert.Equal(expected.AdditionalNumerical, target.AdditionalNumerical);
        }
        if (expected.PreRelease is null)
            Assert.Null(target.PreRelease);
        else
        {
            Assert.NotNull(target.PreRelease);
            Assert.Equal(expected.PreRelease, target.PreRelease);
        }
        if (expected.Build is null)
            Assert.Null(target.Build);
        else
        {
            Assert.NotNull(target.Build);
            Assert.Equal(expected.Build, target.Build);
        }
        Assert.Equal(expected.Format, target.Format);
    }

    /// <summary>
    /// Test data for constructor <see cref="SwVersion(int, int, SwVersion.PreReleaseSegment, SwVersion.PreReleaseSegment[])" />.
    /// </summary>
    public class ComponentConstructor18TestData : TheoryData<int, int, SwVersion.PreReleaseSegment, SwVersion.PreReleaseSegment[], VersionValues>
    {
        public ComponentConstructor18TestData()
        {
            throw new NotImplementedException();
            // TODO: Implement test
            // foreach (int major in new int[] { 0, 1, -1, int.MinValue, int.MaxValue })
            //     foreach (int minor in new int[] { 0, 1, int.MaxValue })
            //         Add(major, minor, preRelease, additionalPreRelease, new(null, major, minor, null, null, null, additionalPreRelease.Prepend(preRelease), null, SwVersion.VersionStringFormat.Standard));
        }
    }

    /// <summary>
    /// Unit test for constructor <see cref="SwVersion(int, int, SwVersion.PreReleaseSegment, SwVersion.PreReleaseSegment[])" />.
    /// </summary>
    [Theory]
    [ClassData(typeof(ComponentConstructor18TestData))]
    public void ComponentConstructor18Test(int major, int minor, SwVersion.PreReleaseSegment preRelease, SwVersion.PreReleaseSegment[] additionalPreRelease, VersionValues expected)
    {
        SwVersion target = new(major, minor, preRelease, additionalPreRelease);
        Assert.Equal(expected.Prefix, target.Prefix);
        Assert.Equal(expected.Major, target.Major);
        Assert.Equal(expected.Minor, target.Minor);
        Assert.Equal(expected.Patch, target.Patch);
        Assert.Equal(expected.Revision, target.Revision);
        if (expected.AdditionalNumerical is null)
            Assert.Null(target.AdditionalNumerical);
        else
        {
            Assert.NotNull(target.AdditionalNumerical);
            Assert.Equal(expected.AdditionalNumerical, target.AdditionalNumerical);
        }
        if (expected.PreRelease is null)
            Assert.Null(target.PreRelease);
        else
        {
            Assert.NotNull(target.PreRelease);
            Assert.Equal(expected.PreRelease, target.PreRelease);
        }
        if (expected.Build is null)
            Assert.Null(target.Build);
        else
        {
            Assert.NotNull(target.Build);
            Assert.Equal(expected.Build, target.Build);
        }
        Assert.Equal(expected.Format, target.Format);
    }

    /// <summary>
    /// Test data for constructor <see cref="SwVersion(string, int, int, SwVersion.BuildSegment, SwVersion.BuildSegment[])" />.
    /// </summary>
    public class ComponentConstructor19TestData : TheoryData<string, int, int, SwVersion.BuildSegment, SwVersion.BuildSegment[], VersionValues>
    {
        public ComponentConstructor19TestData()
        {
            throw new NotImplementedException();
            // TODO: Implement test
            // foreach (string prefix in new string[] { "v", "package:", "R." })
            //     foreach (int major in new int[] { 0, 1, -1, int.MinValue, int.MaxValue })
            //         foreach (int minor in new int[] { 0, 1, int.MaxValue })
            //             Add(prefix, major, minor, null, build, additionalBuild, new(prefix, major, minor, null, null, null, additionalBuild.Prepend(build), SwVersion.VersionStringFormat.Standard));
        }
    }

    /// <summary>
    /// Unit test for constructor <see cref="SwVersion(string, int, int, SwVersion.BuildSegment, SwVersion.BuildSegment[])" />.
    /// </summary>
    [Theory]
    [ClassData(typeof(ComponentConstructor19TestData))]
    public void ComponentConstructor19Test(string prefix, int major, int minor, SwVersion.BuildSegment build, SwVersion.BuildSegment[] additionalBuild, VersionValues expected)
    {
        SwVersion target = new(prefix, major, minor, build, additionalBuild);
        Assert.Equal(expected.Prefix, target.Prefix);
        Assert.Equal(expected.Major, target.Major);
        Assert.Equal(expected.Minor, target.Minor);
        Assert.Equal(expected.Patch, target.Patch);
        Assert.Equal(expected.Revision, target.Revision);
        if (expected.AdditionalNumerical is null)
            Assert.Null(target.AdditionalNumerical);
        else
        {
            Assert.NotNull(target.AdditionalNumerical);
            Assert.Equal(expected.AdditionalNumerical, target.AdditionalNumerical);
        }
        if (expected.PreRelease is null)
            Assert.Null(target.PreRelease);
        else
        {
            Assert.NotNull(target.PreRelease);
            Assert.Equal(expected.PreRelease, target.PreRelease);
        }
        if (expected.Build is null)
            Assert.Null(target.Build);
        else
        {
            Assert.NotNull(target.Build);
            Assert.Equal(expected.Build, target.Build);
        }
        Assert.Equal(expected.Format, target.Format);
    }

    /// <summary>
    /// Test data for constructor <see cref="SwVersion(string, int, IEnumerable{SwVersion.PreReleaseSegment}, SwVersion.BuildSegment, SwVersion.BuildSegment[])" />.
    /// </summary>
    public class ComponentConstructor20TestData : TheoryData<string, int, IEnumerable<SwVersion.PreReleaseSegment>, SwVersion.BuildSegment, SwVersion.BuildSegment[], VersionValues>
    {
        public ComponentConstructor20TestData()
        {
            throw new NotImplementedException();
            // TODO: Implement test
            // foreach (string prefix in new string[] { "v", "package:", "R." })
            //     foreach (int major in new int[] { 0, 1, -1, int.MinValue, int.MaxValue })
            //         Add(prefix, major, preRelease, build, additionalBuild, new(prefix, major, null, null, null, null, preRelease, additionalBuild.Prepend(build), SwVersion.VersionStringFormat.Standard));
        }
    }

    /// <summary>
    /// Unit test for constructor <see cref="SwVersion(string, int, IEnumerable{SwVersion.PreReleaseSegment}, SwVersion.BuildSegment, SwVersion.BuildSegment[])" />.
    /// </summary>
    [Theory]
    [ClassData(typeof(ComponentConstructor20TestData))]
    public void ComponentConstructor20Test(string prefix, int major, IEnumerable<SwVersion.PreReleaseSegment> preRelease, SwVersion.BuildSegment build, SwVersion.BuildSegment[] additionalBuild, VersionValues expected)
    {
        SwVersion target = new(prefix, major, preRelease, build, additionalBuild);
        Assert.Equal(expected.Prefix, target.Prefix);
        Assert.Equal(expected.Major, target.Major);
        Assert.Equal(expected.Minor, target.Minor);
        Assert.Equal(expected.Patch, target.Patch);
        Assert.Equal(expected.Revision, target.Revision);
        if (expected.AdditionalNumerical is null)
            Assert.Null(target.AdditionalNumerical);
        else
        {
            Assert.NotNull(target.AdditionalNumerical);
            Assert.Equal(expected.AdditionalNumerical, target.AdditionalNumerical);
        }
        if (expected.PreRelease is null)
            Assert.Null(target.PreRelease);
        else
        {
            Assert.NotNull(target.PreRelease);
            Assert.Equal(expected.PreRelease, target.PreRelease);
        }
        if (expected.Build is null)
            Assert.Null(target.Build);
        else
        {
            Assert.NotNull(target.Build);
            Assert.Equal(expected.Build, target.Build);
        }
        Assert.Equal(expected.Format, target.Format);
    }

    /// <summary>
    /// Test data for constructor <see cref="SwVersion(int, int, SwVersion.BuildSegment, SwVersion.BuildSegment[])" />.
    /// </summary>
    public class ComponentConstructor21TestData : TheoryData<int, int, SwVersion.BuildSegment, SwVersion.BuildSegment[], VersionValues>
    {
        public ComponentConstructor21TestData()
        {
            throw new NotImplementedException();
            // TODO: Implement test
            // foreach (int major in new int[] { 0, 1, -1, int.MinValue, int.MaxValue })
            //     foreach (int minor in new int[] { 0, 1, int.MaxValue })
            //         Add(major, minor, null, build, additionalBuild, new(null, major, minor, null, null, null, additionalBuild.Prepend(build), SwVersion.VersionStringFormat.Standard));
        }
    }

    /// <summary>
    /// Unit test for constructor <see cref="SwVersion(int, int, SwVersion.BuildSegment, SwVersion.BuildSegment[])" />.
    /// </summary>
    [Theory]
    [ClassData(typeof(ComponentConstructor21TestData))]
    public void ComponentConstructor21Test(int major, int minor, SwVersion.BuildSegment build, SwVersion.BuildSegment[] additionalBuild, VersionValues expected)
    {
        SwVersion target = new(major, minor, build, additionalBuild);
        Assert.Equal(expected.Prefix, target.Prefix);
        Assert.Equal(expected.Major, target.Major);
        Assert.Equal(expected.Minor, target.Minor);
        Assert.Equal(expected.Patch, target.Patch);
        Assert.Equal(expected.Revision, target.Revision);
        if (expected.AdditionalNumerical is null)
            Assert.Null(target.AdditionalNumerical);
        else
        {
            Assert.NotNull(target.AdditionalNumerical);
            Assert.Equal(expected.AdditionalNumerical, target.AdditionalNumerical);
        }
        if (expected.PreRelease is null)
            Assert.Null(target.PreRelease);
        else
        {
            Assert.NotNull(target.PreRelease);
            Assert.Equal(expected.PreRelease, target.PreRelease);
        }
        if (expected.Build is null)
            Assert.Null(target.Build);
        else
        {
            Assert.NotNull(target.Build);
            Assert.Equal(expected.Build, target.Build);
        }
        Assert.Equal(expected.Format, target.Format);
    }

    /// <summary>
    /// Test data for constructor <see cref="SwVersion(int, IEnumerable{SwVersion.PreReleaseSegment}, SwVersion.BuildSegment, SwVersion.BuildSegment[])" />.
    /// </summary>
    public class ComponentConstructor22TestData : TheoryData<int, IEnumerable<SwVersion.PreReleaseSegment>, SwVersion.BuildSegment, SwVersion.BuildSegment[], VersionValues>
    {
        public ComponentConstructor22TestData()
        {
            throw new NotImplementedException();
            // TODO: Implement test
            // foreach (int major in new int[] { 0, 1, -1, int.MinValue, int.MaxValue })
            //     Add(major, preRelease, build, additionalBuild, new(null, major, null, null, null, null, preRelease, additionalBuild.Prepend(build), SwVersion.VersionStringFormat.Standard));
        }
    }

    /// <summary>
    /// Unit test for constructor <see cref="SwVersion(int, IEnumerable{SwVersion.PreReleaseSegment}, SwVersion.BuildSegment, SwVersion.BuildSegment[])" />.
    /// </summary>
    [Theory]
    [ClassData(typeof(ComponentConstructor22TestData))]
    public void ComponentConstructor22Test(int major, IEnumerable<SwVersion.PreReleaseSegment> preRelease, SwVersion.BuildSegment build, SwVersion.BuildSegment[] additionalBuild, VersionValues expected)
    {
        SwVersion target = new(major, preRelease, build, additionalBuild);
        Assert.Equal(expected.Prefix, target.Prefix);
        Assert.Equal(expected.Major, target.Major);
        Assert.Equal(expected.Minor, target.Minor);
        Assert.Equal(expected.Patch, target.Patch);
        Assert.Equal(expected.Revision, target.Revision);
        if (expected.AdditionalNumerical is null)
            Assert.Null(target.AdditionalNumerical);
        else
        {
            Assert.NotNull(target.AdditionalNumerical);
            Assert.Equal(expected.AdditionalNumerical, target.AdditionalNumerical);
        }
        if (expected.PreRelease is null)
            Assert.Null(target.PreRelease);
        else
        {
            Assert.NotNull(target.PreRelease);
            Assert.Equal(expected.PreRelease, target.PreRelease);
        }
        if (expected.Build is null)
            Assert.Null(target.Build);
        else
        {
            Assert.NotNull(target.Build);
            Assert.Equal(expected.Build, target.Build);
        }
        Assert.Equal(expected.Format, target.Format);
    }

    /// <summary>
    /// Test data for constructor <see cref="SwVersion(string, int, SwVersion.PreReleaseSegment, SwVersion.PreReleaseSegment[])" />.
    /// </summary>
    public class ComponentConstructor23TestData : TheoryData<string, int, SwVersion.PreReleaseSegment, SwVersion.PreReleaseSegment[], VersionValues>
    {
        public ComponentConstructor23TestData()
        {
            foreach (string prefix in new string[] { "v", "package:", "R." })
                foreach (int major in new int[] { 0, 1, -1, int.MinValue, int.MaxValue })
            {
                Add(prefix, major, new SwVersion.PreReleaseSegment(true, "A"), Array.Empty<SwVersion.PreReleaseSegment>(), new(null, major, null, null, null, null, new[] { new SwVersion.PreReleaseSegment(true, "A") }, null, SwVersion.VersionStringFormat.Standard));
                Add(prefix, major, new SwVersion.PreReleaseSegment(false, "pre"), new[] { new SwVersion.PreReleaseSegment(true, "Free"), new SwVersion.PreReleaseSegment(false, "Community") }, new(null, major, null, null, null, null, new[] { new SwVersion.PreReleaseSegment(false, "pre"), new SwVersion.PreReleaseSegment(true, "Free"), new SwVersion.PreReleaseSegment(false, "Community") }, null, SwVersion.VersionStringFormat.Alt));
            }
        }
    }

    /// <summary>
    /// Unit test for constructor <see cref="SwVersion(string, int, SwVersion.PreReleaseSegment, SwVersion.PreReleaseSegment[])" />.
    /// </summary>
    [Theory]
    [ClassData(typeof(ComponentConstructor23TestData))]
    public void ComponentConstructor23Test(string prefix, int major, SwVersion.PreReleaseSegment preRelease, SwVersion.PreReleaseSegment[] additionalPreRelease, VersionValues expected)
    {
        SwVersion target = new(prefix, major, preRelease, additionalPreRelease);
        Assert.Equal(expected.Prefix, target.Prefix);
        Assert.Equal(expected.Major, target.Major);
        Assert.Equal(expected.Minor, target.Minor);
        Assert.Equal(expected.Patch, target.Patch);
        Assert.Equal(expected.Revision, target.Revision);
        if (expected.AdditionalNumerical is null)
            Assert.Null(target.AdditionalNumerical);
        else
        {
            Assert.NotNull(target.AdditionalNumerical);
            Assert.Equal(expected.AdditionalNumerical, target.AdditionalNumerical);
        }
        if (expected.PreRelease is null)
            Assert.Null(target.PreRelease);
        else
        {
            Assert.NotNull(target.PreRelease);
            Assert.Equal(expected.PreRelease, target.PreRelease);
        }
        if (expected.Build is null)
            Assert.Null(target.Build);
        else
        {
            Assert.NotNull(target.Build);
            Assert.Equal(expected.Build, target.Build);
        }
        Assert.Equal(expected.Format, target.Format);
    }

    /// <summary>
    /// Test data for constructor <see cref="SwVersion(int, SwVersion.PreReleaseSegment, SwVersion.PreReleaseSegment[])" />.
    /// </summary>
    public class ComponentConstructor24TestData : TheoryData<int, SwVersion.PreReleaseSegment, SwVersion.PreReleaseSegment[], VersionValues>
    {
        public ComponentConstructor24TestData()
        {
            foreach (int major in new int[] { 0, 1, -1, int.MinValue, int.MaxValue })
            {
                Add(major, new SwVersion.PreReleaseSegment(true, "A"), Array.Empty<SwVersion.PreReleaseSegment>(), new(null, major, null, null, null, null, new[] { new SwVersion.PreReleaseSegment(true, "A") }, null, SwVersion.VersionStringFormat.Standard));
                Add(major, new SwVersion.PreReleaseSegment(false, "pre"), new[] { new SwVersion.PreReleaseSegment(true, "Free"), new SwVersion.PreReleaseSegment(false, "Community") }, new(null, major, null, null, null, null, new[] { new SwVersion.PreReleaseSegment(false, "pre"), new SwVersion.PreReleaseSegment(true, "Free"), new SwVersion.PreReleaseSegment(false, "Community") }, null, SwVersion.VersionStringFormat.Alt));
            }
        }
    }

    /// <summary>
    /// Unit test for constructor <see cref="SwVersion(int, SwVersion.PreReleaseSegment, SwVersion.PreReleaseSegment[])" />.
    /// </summary>
    [Theory]
    [ClassData(typeof(ComponentConstructor24TestData))]
    public void ComponentConstructor24Test(int major, SwVersion.PreReleaseSegment preRelease, SwVersion.PreReleaseSegment[] additionalPreRelease, VersionValues expected)
    {
        SwVersion target = new(major, preRelease, additionalPreRelease);
        Assert.Equal(expected.Prefix, target.Prefix);
        Assert.Equal(expected.Major, target.Major);
        Assert.Equal(expected.Minor, target.Minor);
        Assert.Equal(expected.Patch, target.Patch);
        Assert.Equal(expected.Revision, target.Revision);
        if (expected.AdditionalNumerical is null)
            Assert.Null(target.AdditionalNumerical);
        else
        {
            Assert.NotNull(target.AdditionalNumerical);
            Assert.Equal(expected.AdditionalNumerical, target.AdditionalNumerical);
        }
        if (expected.PreRelease is null)
            Assert.Null(target.PreRelease);
        else
        {
            Assert.NotNull(target.PreRelease);
            Assert.Equal(expected.PreRelease, target.PreRelease);
        }
        if (expected.Build is null)
            Assert.Null(target.Build);
        else
        {
            Assert.NotNull(target.Build);
            Assert.Equal(expected.Build, target.Build);
        }
        Assert.Equal(expected.Format, target.Format);
    }

    /// <summary>
    /// Test data for constructor <see cref="SwVersion(string, int, SwVersion.BuildSegment, SwVersion.BuildSegment[])" />.
    /// </summary>
    public class ComponentConstructor25TestData : TheoryData<string, int, SwVersion.BuildSegment, SwVersion.BuildSegment[], VersionValues>
    {
        public ComponentConstructor25TestData()
        {
            foreach (string prefix in new string[] { "v", "package:", "R." })
                foreach (int major in new int[] { 0, 1, -1, int.MinValue, int.MaxValue })
                {
                    Add(prefix, major, new SwVersion.BuildSegment(SwVersion.BuildSeparator.Plus, "Win32"), Array.Empty<SwVersion.BuildSegment>(), new(null, major, null, null, null, null, null, new[] { new SwVersion.BuildSegment(SwVersion.BuildSeparator.Plus, "Win32") } , SwVersion.VersionStringFormat.Standard));
                    Add(prefix, major, new SwVersion.BuildSegment(SwVersion.BuildSeparator.Plus, ""), new[] { new SwVersion.BuildSegment(SwVersion.BuildSeparator.Dash, "Free"), new SwVersion.BuildSegment(SwVersion.BuildSeparator.Dot, "Community") }, new(null, major, null, null, null, null, null, new[] { new SwVersion.BuildSegment(SwVersion.BuildSeparator.Plus, ""), new SwVersion.BuildSegment(SwVersion.BuildSeparator.Dash, "Free"), new SwVersion.BuildSegment(SwVersion.BuildSeparator.Dot, "Community") }, SwVersion.VersionStringFormat.Standard));
                }
        }
    }

    /// <summary>
    /// Unit test for constructor <see cref="SwVersion(string, int, SwVersion.BuildSegment, SwVersion.BuildSegment[])" />.
    /// </summary>
    [Theory]
    [ClassData(typeof(ComponentConstructor25TestData))]
    public void ComponentConstructor25Test(string prefix, int major, SwVersion.BuildSegment build, SwVersion.BuildSegment[] additionalBuild, VersionValues expected)
    {
        SwVersion target = new(prefix, major, build, additionalBuild);
        Assert.Equal(expected.Prefix, target.Prefix);
        Assert.Equal(expected.Major, target.Major);
        Assert.Equal(expected.Minor, target.Minor);
        Assert.Equal(expected.Patch, target.Patch);
        Assert.Equal(expected.Revision, target.Revision);
        if (expected.AdditionalNumerical is null)
            Assert.Null(target.AdditionalNumerical);
        else
        {
            Assert.NotNull(target.AdditionalNumerical);
            Assert.Equal(expected.AdditionalNumerical, target.AdditionalNumerical);
        }
        if (expected.PreRelease is null)
            Assert.Null(target.PreRelease);
        else
        {
            Assert.NotNull(target.PreRelease);
            Assert.Equal(expected.PreRelease, target.PreRelease);
        }
        if (expected.Build is null)
            Assert.Null(target.Build);
        else
        {
            Assert.NotNull(target.Build);
            Assert.Equal(expected.Build, target.Build);
        }
        Assert.Equal(expected.Format, target.Format);
    }

    /// <summary>
    /// Test data for constructor <see cref="SwVersion(string, int, int, int)" />.
    /// </summary>
    public class ComponentConstructor26TestData : TheoryData<string, int, int, int, VersionValues>
    {
        public ComponentConstructor26TestData()
        {
            foreach (string prefix in new string[] { "v", "package:", "R." })
                foreach (int major in new int[] { 0, 1, -1, int.MinValue, int.MaxValue })
                    foreach (int minor in new int[] { 0, 1, int.MaxValue })
                        foreach (int patch in new int[] { 0, 1, int.MaxValue })
                            Add(prefix, major, minor, patch, new(prefix, major, minor, patch, null, null, null, null, SwVersion.VersionStringFormat.Standard));
        }
    }

    /// <summary>
    /// Unit test for constructor <see cref="SwVersion(string, int, int, int)" />.
    /// </summary>
    [Theory]
    [ClassData(typeof(ComponentConstructor26TestData))]
    public void ComponentConstructor26Test(string prefix, int major, int minor, int patch, VersionValues expected)
    {
        SwVersion target = new(prefix, major, minor, patch);
        Assert.Equal(expected.Prefix, target.Prefix);
        Assert.Equal(expected.Major, target.Major);
        Assert.Equal(expected.Minor, target.Minor);
        Assert.Equal(expected.Patch, target.Patch);
        Assert.Equal(expected.Revision, target.Revision);
        if (expected.AdditionalNumerical is null)
            Assert.Null(target.AdditionalNumerical);
        else
        {
            Assert.NotNull(target.AdditionalNumerical);
            Assert.Equal(expected.AdditionalNumerical, target.AdditionalNumerical);
        }
        if (expected.PreRelease is null)
            Assert.Null(target.PreRelease);
        else
        {
            Assert.NotNull(target.PreRelease);
            Assert.Equal(expected.PreRelease, target.PreRelease);
        }
        if (expected.Build is null)
            Assert.Null(target.Build);
        else
        {
            Assert.NotNull(target.Build);
            Assert.Equal(expected.Build, target.Build);
        }
        Assert.Equal(expected.Format, target.Format);
    }

    /// <summary>
    /// Test data for constructor <see cref="SwVersion(int, int, int)" />.
    /// </summary>
    public class ComponentConstructor27TestData : TheoryData<int, int, int, VersionValues>
    {
        public ComponentConstructor27TestData()
        {
            foreach (int major in new int[] { 0, 1, -1, int.MinValue, int.MaxValue })
                foreach (int minor in new int[] { 0, 1, int.MaxValue })
                    foreach (int patch in new int[] { 0, 1, int.MaxValue })
                        Add(major, minor, patch, new(null, major, minor, patch, null, null, null, null, SwVersion.VersionStringFormat.Standard));
        }
    }

    /// <summary>
    /// Unit test for constructor <see cref="SwVersion(int, int, int)" />.
    /// </summary>
    [Theory]
    [ClassData(typeof(ComponentConstructor27TestData))]
    public void ComponentConstructor27Test(int major, int minor, int patch, VersionValues expected)
    {
        SwVersion target = new(major, minor, patch);
        Assert.Equal(expected.Prefix, target.Prefix);
        Assert.Equal(expected.Major, target.Major);
        Assert.Equal(expected.Minor, target.Minor);
        Assert.Equal(expected.Patch, target.Patch);
        Assert.Equal(expected.Revision, target.Revision);
        if (expected.AdditionalNumerical is null)
            Assert.Null(target.AdditionalNumerical);
        else
        {
            Assert.NotNull(target.AdditionalNumerical);
            Assert.Equal(expected.AdditionalNumerical, target.AdditionalNumerical);
        }
        if (expected.PreRelease is null)
            Assert.Null(target.PreRelease);
        else
        {
            Assert.NotNull(target.PreRelease);
            Assert.Equal(expected.PreRelease, target.PreRelease);
        }
        if (expected.Build is null)
            Assert.Null(target.Build);
        else
        {
            Assert.NotNull(target.Build);
            Assert.Equal(expected.Build, target.Build);
        }
        Assert.Equal(expected.Format, target.Format);
    }

    /// <summary>
    /// Test data for constructor <see cref="SwVersion(int, SwVersion.BuildSegment, SwVersion.BuildSegment[])" />.
    /// </summary>
    public class ComponentConstructor28TestData : TheoryData<int, SwVersion.BuildSegment, SwVersion.BuildSegment[], VersionValues>
    {
        public ComponentConstructor28TestData()
        {
            foreach (int major in new int[] { 0, 1, -1, int.MinValue, int.MaxValue })
            {
                Add(major, new SwVersion.BuildSegment(SwVersion.BuildSeparator.Plus, "Win32"), Array.Empty<SwVersion.BuildSegment>(), new(null, major, null, null, null, null, null, new[] { new SwVersion.BuildSegment(SwVersion.BuildSeparator.Plus, "Win32") } , SwVersion.VersionStringFormat.Standard));
                Add(major, new SwVersion.BuildSegment(SwVersion.BuildSeparator.Plus, ""), new[] { new SwVersion.BuildSegment(SwVersion.BuildSeparator.Dash, "Free"), new SwVersion.BuildSegment(SwVersion.BuildSeparator.Dot, "Community") }, new(null, major, null, null, null, null, null, new[] { new SwVersion.BuildSegment(SwVersion.BuildSeparator.Plus, ""), new SwVersion.BuildSegment(SwVersion.BuildSeparator.Dash, "Free"), new SwVersion.BuildSegment(SwVersion.BuildSeparator.Dot, "Community") }, SwVersion.VersionStringFormat.Standard));
            }
        }
    }

    /// <summary>
    /// Unit test for constructor <see cref="SwVersion(int, SwVersion.BuildSegment, SwVersion.BuildSegment[])" />.
    /// </summary>
    [Theory]
    [ClassData(typeof(ComponentConstructor28TestData))]
    public void ComponentConstructor28Test(int major, SwVersion.BuildSegment build, SwVersion.BuildSegment[] additionalBuild, VersionValues expected)
    {
        SwVersion target = new(major, build, additionalBuild);
        Assert.Equal(expected.Prefix, target.Prefix);
        Assert.Equal(expected.Major, target.Major);
        Assert.Equal(expected.Minor, target.Minor);
        Assert.Equal(expected.Patch, target.Patch);
        Assert.Equal(expected.Revision, target.Revision);
        if (expected.AdditionalNumerical is null)
            Assert.Null(target.AdditionalNumerical);
        else
        {
            Assert.NotNull(target.AdditionalNumerical);
            Assert.Equal(expected.AdditionalNumerical, target.AdditionalNumerical);
        }
        if (expected.PreRelease is null)
            Assert.Null(target.PreRelease);
        else
        {
            Assert.NotNull(target.PreRelease);
            Assert.Equal(expected.PreRelease, target.PreRelease);
        }
        if (expected.Build is null)
            Assert.Null(target.Build);
        else
        {
            Assert.NotNull(target.Build);
            Assert.Equal(expected.Build, target.Build);
        }
        Assert.Equal(expected.Format, target.Format);
    }

    /// <summary>
    /// Test data for constructor <see cref="SwVersion(string, int, int)" />.
    /// </summary>
    public class ComponentConstructor29TestData : TheoryData<string, int, int, VersionValues>
    {
        public ComponentConstructor29TestData()
        {
            foreach (string prefix in new string[] { "v", "package:", "R." })
                foreach (int major in new int[] { 0, 1, -1, int.MinValue, int.MaxValue })
                    foreach (int minor in new int[] { 0, 1, int.MaxValue })
                        Add(prefix, major, minor, new(prefix, major, minor, null, null, null, null, null, SwVersion.VersionStringFormat.Standard));
        }
    }

    /// <summary>
    /// Unit test for constructor <see cref="SwVersion(string, int, int)" />.
    /// </summary>
    [Theory]
    [ClassData(typeof(ComponentConstructor29TestData))]
    public void ComponentConstructor29Test(string prefix, int major, int minor, VersionValues expected)
    {
        SwVersion target = new(prefix, major, minor);
        Assert.Equal(expected.Prefix, target.Prefix);
        Assert.Equal(expected.Major, target.Major);
        Assert.Equal(expected.Minor, target.Minor);
        Assert.Equal(expected.Patch, target.Patch);
        Assert.Equal(expected.Revision, target.Revision);
        if (expected.AdditionalNumerical is null)
            Assert.Null(target.AdditionalNumerical);
        else
        {
            Assert.NotNull(target.AdditionalNumerical);
            Assert.Equal(expected.AdditionalNumerical, target.AdditionalNumerical);
        }
        if (expected.PreRelease is null)
            Assert.Null(target.PreRelease);
        else
        {
            Assert.NotNull(target.PreRelease);
            Assert.Equal(expected.PreRelease, target.PreRelease);
        }
        if (expected.Build is null)
            Assert.Null(target.Build);
        else
        {
            Assert.NotNull(target.Build);
            Assert.Equal(expected.Build, target.Build);
        }
        Assert.Equal(expected.Format, target.Format);
    }

    /// <summary>
    /// Test data for constructor <see cref="SwVersion(int, int)" />.
    /// </summary>
    public class ComponentConstructor30TestData : TheoryData<int, int, VersionValues>
    {
        public ComponentConstructor30TestData()
        {
            foreach (int major in new int[] { 0, 1, -1, int.MinValue, int.MaxValue })
                foreach (int minor in new int[] { 0, 1, int.MaxValue })
                    Add(major, minor, new(null, major, minor, null, null, null, null, null, SwVersion.VersionStringFormat.Standard));
        }
    }

    /// <summary>
    /// Unit test for constructor <see cref="SwVersion(int, int)" />.
    /// </summary>
    [Theory]
    [ClassData(typeof(ComponentConstructor30TestData))]
    public void ComponentConstructor30Test(int major, int minor, VersionValues expected)
    {
        SwVersion target = new(major, minor);
        Assert.Equal(expected.Prefix, target.Prefix);
        Assert.Equal(expected.Major, target.Major);
        Assert.Equal(expected.Minor, target.Minor);
        Assert.Equal(expected.Patch, target.Patch);
        Assert.Equal(expected.Revision, target.Revision);
        if (expected.AdditionalNumerical is null)
            Assert.Null(target.AdditionalNumerical);
        else
        {
            Assert.NotNull(target.AdditionalNumerical);
            Assert.Equal(expected.AdditionalNumerical, target.AdditionalNumerical);
        }
        if (expected.PreRelease is null)
            Assert.Null(target.PreRelease);
        else
        {
            Assert.NotNull(target.PreRelease);
            Assert.Equal(expected.PreRelease, target.PreRelease);
        }
        if (expected.Build is null)
            Assert.Null(target.Build);
        else
        {
            Assert.NotNull(target.Build);
            Assert.Equal(expected.Build, target.Build);
        }
        Assert.Equal(expected.Format, target.Format);
    }

    /// <summary>
    /// Test data for constructor <see cref="SwVersion(string, int)" />.
    /// </summary>
    public class ComponentConstructor31TestData : TheoryData<string, int, VersionValues>
    {
        public ComponentConstructor31TestData()
        {
            foreach (string prefix in new string[] { "v", "package:", "R." })
                foreach (int major in new int[] { 0, 1, -1, int.MinValue, int.MaxValue })
                    Add(prefix, major, new(prefix, major, null, null, null, null, null, null, SwVersion.VersionStringFormat.Standard));
        }
    }

    /// <summary>
    /// Unit test for constructor <see cref="SwVersion(string, int)" />.
    /// </summary>
    [Theory]
    [ClassData(typeof(ComponentConstructor31TestData))]
    public void ComponentConstructor31Test(string prefix, int major, VersionValues expected)
    {
        SwVersion target = new(prefix, major);
        Assert.Equal(expected.Prefix, target.Prefix);
        Assert.Equal(expected.Major, target.Major);
        Assert.Equal(expected.Minor, target.Minor);
        Assert.Equal(expected.Patch, target.Patch);
        Assert.Equal(expected.Revision, target.Revision);
        if (expected.AdditionalNumerical is null)
            Assert.Null(target.AdditionalNumerical);
        else
        {
            Assert.NotNull(target.AdditionalNumerical);
            Assert.Equal(expected.AdditionalNumerical, target.AdditionalNumerical);
        }
        if (expected.PreRelease is null)
            Assert.Null(target.PreRelease);
        else
        {
            Assert.NotNull(target.PreRelease);
            Assert.Equal(expected.PreRelease, target.PreRelease);
        }
        if (expected.Build is null)
            Assert.Null(target.Build);
        else
        {
            Assert.NotNull(target.Build);
            Assert.Equal(expected.Build, target.Build);
        }
        Assert.Equal(expected.Format, target.Format);
    }

    /// <summary>
    /// Test data for constructor <see cref="SwVersion(int)" />.
    /// </summary>
    public class ComponentConstructor32TestData : TheoryData<int, VersionValues>
    {
        public ComponentConstructor32TestData()
        {
            foreach (int major in new int[] { 0, 1, -1, int.MinValue, int.MaxValue })
                Add(major, new(null, major, null, null, null, null, null, null, SwVersion.VersionStringFormat.Standard));
        }
    }

    /// <summary>
    /// Unit test for constructor <see cref="SwVersion(int)" />.
    /// </summary>
    [Theory]
    [ClassData(typeof(ComponentConstructor32TestData))]
    public void ComponentConstructor32Test(int major, VersionValues expected)
    {
        SwVersion target = new(major);
        Assert.Equal(expected.Prefix, target.Prefix);
        Assert.Equal(expected.Major, target.Major);
        Assert.Equal(expected.Minor, target.Minor);
        Assert.Equal(expected.Patch, target.Patch);
        Assert.Equal(expected.Revision, target.Revision);
        if (expected.AdditionalNumerical is null)
            Assert.Null(target.AdditionalNumerical);
        else
        {
            Assert.NotNull(target.AdditionalNumerical);
            Assert.Equal(expected.AdditionalNumerical, target.AdditionalNumerical);
        }
        if (expected.PreRelease is null)
            Assert.Null(target.PreRelease);
        else
        {
            Assert.NotNull(target.PreRelease);
            Assert.Equal(expected.PreRelease, target.PreRelease);
        }
        if (expected.Build is null)
            Assert.Null(target.Build);
        else
        {
            Assert.NotNull(target.Build);
            Assert.Equal(expected.Build, target.Build);
        }
        Assert.Equal(expected.Format, target.Format);
    }

}