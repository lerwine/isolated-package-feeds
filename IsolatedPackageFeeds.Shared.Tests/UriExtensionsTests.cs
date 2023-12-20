using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NUnit.Framework;

namespace IsolatedPackageFeeds.Shared.Tests
{
    public class UriExtensionsTests
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void SplitQueryAndFragmentTest()
        {
            foreach ((var target, var expectedResult) in new (Uri? Target, Uri Expected)[]
            {
                (null, new Uri(string.Empty, UriKind.Relative)),
                (new Uri(string.Empty, UriKind.Relative), new Uri(string.Empty, UriKind.Relative)),
                (new Uri("http://tempuri.org", UriKind.Absolute), new Uri("http://tempuri.org", UriKind.Absolute))
            })
            {
                var actual = target.SplitQueryAndFragment(out string? actualQuery, out string? actualFragment);
                Assert.That(actual, Is.Not.Null);
                Assert.Multiple(() =>
                {
                    if (expectedResult.IsAbsoluteUri)
                    {
                        Assert.That(actual.IsAbsoluteUri, Is.True);
                        Assert.That(actual.AbsoluteUri, Is.EqualTo(expectedResult.AbsoluteUri));
                    }
                    else
                    {
                        Assert.That(actual.IsAbsoluteUri, Is.False);
                        Assert.That(actual.OriginalString, Is.EqualTo(expectedResult.OriginalString));
                    }
                    Assert.That(actualQuery, Is.Null);
                    Assert.That(actualFragment, Is.Null);
                });
            }
            foreach ((var target, var expectedResult, var expectedQuery) in new (Uri Target, Uri Expected, string expectedQuery)[]
            {
                (new Uri("?", UriKind.Relative), new Uri(string.Empty, UriKind.Relative), string.Empty),
                (new Uri("?My=Query", UriKind.Relative), new Uri(string.Empty, UriKind.Relative), "My=Query"),
                (new Uri("http://tempuri.org?", UriKind.Absolute), new Uri("http://tempuri.org", UriKind.Absolute), string.Empty),
                (new Uri("http://tempuri.org?My=Query", UriKind.Absolute), new Uri("http://tempuri.org", UriKind.Absolute), "My=Query")
            })
            {
                var actual = target.SplitQueryAndFragment(out string? actualQuery, out string? actualFragment);
                Assert.That(actual, Is.Not.Null);
                Assert.Multiple(() =>
                {
                    if (expectedResult.IsAbsoluteUri)
                    {
                        Assert.That(actual.IsAbsoluteUri, Is.True);
                        Assert.That(actual.AbsoluteUri, Is.EqualTo(expectedResult.AbsoluteUri));
                    }
                    else
                    {
                        Assert.That(actual.IsAbsoluteUri, Is.False);
                        Assert.That(actual.OriginalString, Is.EqualTo(expectedResult.OriginalString));
                    }
                    Assert.That(actualQuery, Is.Not.Null);
                    Assert.That(actualQuery, Is.EqualTo(expectedQuery));
                    Assert.That(actualFragment, Is.Null);
                });
            }
            foreach ((var target, var expectedResult, var expectedFragment) in new (Uri Target, Uri Expected, string expectedFragment)[]
            {
                (new Uri("#", UriKind.Relative), new Uri(string.Empty, UriKind.Relative), string.Empty),
                (new Uri("#TheHash", UriKind.Relative), new Uri(string.Empty, UriKind.Relative), "TheHash"),
                (new Uri("http://tempuri.org#", UriKind.Absolute), new Uri("http://tempuri.org", UriKind.Absolute), string.Empty),
                (new Uri("http://tempuri.org#TheHash", UriKind.Absolute), new Uri("http://tempuri.org", UriKind.Absolute), "TheHash")
            })
            {
                var actual = target.SplitQueryAndFragment(out string? actualQuery, out string? actualFragment);
                Assert.That(actual, Is.Not.Null);
                Assert.Multiple(() =>
                {
                    if (expectedResult.IsAbsoluteUri)
                    {
                        Assert.That(actual.IsAbsoluteUri, Is.True);
                        Assert.That(actual.AbsoluteUri, Is.EqualTo(expectedResult.AbsoluteUri));
                    }
                    else
                    {
                        Assert.That(actual.IsAbsoluteUri, Is.False);
                        Assert.That(actual.OriginalString, Is.EqualTo(expectedResult.OriginalString));
                    }
                    Assert.That(actualQuery, Is.Null);
                    Assert.That(actualFragment, Is.Not.Null);
                    Assert.That(actualFragment, Is.EqualTo(expectedFragment));
                });
            }
            foreach ((var target, var expectedResult, var expectedQuery, var expectedFragment) in new (Uri Target, Uri Expected, string expectedQuery, string expectedFragment)[]
            {
                (new Uri("?#", UriKind.Relative), new Uri(string.Empty, UriKind.Relative), string.Empty, string.Empty),
                (new Uri("?My=Query#", UriKind.Relative), new Uri(string.Empty, UriKind.Relative), "My=Query", string.Empty),
                (new Uri("?#TheHash", UriKind.Relative), new Uri(string.Empty, UriKind.Relative), string.Empty, "TheHash"),
                (new Uri("?My=Query#TheHash", UriKind.Relative), new Uri(string.Empty, UriKind.Relative), "My=Query", "TheHash"),
                (new Uri("http://tempuri.org?#", UriKind.Absolute), new Uri("http://tempuri.org", UriKind.Absolute), string.Empty, string.Empty),
                (new Uri("http://tempuri.org?My=Query#", UriKind.Absolute), new Uri("http://tempuri.org", UriKind.Absolute), "My=Query", string.Empty),
                (new Uri("http://tempuri.org?#TheHash", UriKind.Absolute), new Uri("http://tempuri.org", UriKind.Absolute), string.Empty, "TheHash"),
                (new Uri("http://tempuri.org?My=Query#TheHash", UriKind.Absolute), new Uri("http://tempuri.org", UriKind.Absolute), "My=Query", "TheHash")
            })
            {
                var actual = target.SplitQueryAndFragment(out string? actualQuery, out string? actualFragment);
                Assert.That(actual, Is.Not.Null);
                Assert.Multiple(() =>
                {
                    if (expectedResult.IsAbsoluteUri)
                    {
                        Assert.That(actual.IsAbsoluteUri, Is.True);
                        Assert.That(actual.AbsoluteUri, Is.EqualTo(expectedResult.AbsoluteUri));
                    }
                    else
                    {
                        Assert.That(actual.IsAbsoluteUri, Is.False);
                        Assert.That(actual.OriginalString, Is.EqualTo(expectedResult.OriginalString));
                    }
                    Assert.That(actualQuery, Is.Not.Null);
                    Assert.That(actualQuery, Is.EqualTo(expectedQuery));
                    Assert.That(actualFragment, Is.Not.Null);
                    Assert.That(actualFragment, Is.EqualTo(expectedFragment));
                });
            }
        }

        [Test]
        public void SplitLeaf1Test()
        {
            foreach ((var target, var expectedResult) in new (Uri? Target, Uri Expected)[]
            {
                (null, new Uri(string.Empty, UriKind.Relative)),
                (new Uri(string.Empty, UriKind.Relative), new Uri(string.Empty, UriKind.Relative)),
                (new Uri("?", UriKind.Relative), new Uri("?", UriKind.Relative)),
                (new Uri("?My=Query", UriKind.Relative), new Uri("?My=Query", UriKind.Relative)),
                (new Uri("#", UriKind.Relative), new Uri("#", UriKind.Relative)),
                (new Uri("#TheHash", UriKind.Relative), new Uri("#TheHash", UriKind.Relative)),
                (new Uri("?#", UriKind.Relative), new Uri("?#", UriKind.Relative)),
                (new Uri("?#TheHash", UriKind.Relative), new Uri("?#TheHash", UriKind.Relative)),
                (new Uri("?My=Query#", UriKind.Relative), new Uri("?My=Query#", UriKind.Relative)),
                (new Uri("?My=Query#TheHash", UriKind.Relative), new Uri("?My=Query#TheHash", UriKind.Relative)),
                (new Uri("/", UriKind.Relative), new Uri("/", UriKind.Relative)),
                (new Uri("/?", UriKind.Relative), new Uri("/?", UriKind.Relative)),
                (new Uri("/?My=Query", UriKind.Relative), new Uri("/?My=Query", UriKind.Relative)),
                (new Uri("/#", UriKind.Relative), new Uri("/#", UriKind.Relative)),
                (new Uri("/#TheHash", UriKind.Relative), new Uri("/#TheHash", UriKind.Relative)),
                (new Uri("/?#", UriKind.Relative), new Uri("/?#", UriKind.Relative)),
                (new Uri("/?#TheHash", UriKind.Relative), new Uri("/?#TheHash", UriKind.Relative)),
                (new Uri("/?My=Query#", UriKind.Relative), new Uri("/?My=Query#", UriKind.Relative)),
                (new Uri("/?My=Query#TheHash", UriKind.Relative), new Uri("/?My=Query#TheHash", UriKind.Relative)),
                (new Uri("http://tempuri.org", UriKind.Absolute), new Uri("http://tempuri.org", UriKind.Absolute)),
                (new Uri("http://tempuri.org?", UriKind.Absolute), new Uri("http://tempuri.org?", UriKind.Absolute)),
                (new Uri("http://tempuri.org?My=Query", UriKind.Absolute), new Uri("http://tempuri.org?My=Query", UriKind.Absolute)),
                (new Uri("http://tempuri.org#", UriKind.Absolute), new Uri("http://tempuri.org#", UriKind.Absolute)),
                (new Uri("http://tempuri.org#TheHash", UriKind.Absolute), new Uri("http://tempuri.org#TheHash", UriKind.Absolute)),
                (new Uri("http://tempuri.org?#", UriKind.Absolute), new Uri("http://tempuri.org?#", UriKind.Absolute)),
                (new Uri("http://tempuri.org?#TheHash", UriKind.Absolute), new Uri("http://tempuri.org?#TheHash", UriKind.Absolute)),
                (new Uri("http://tempuri.org?My=Query#", UriKind.Absolute), new Uri("http://tempuri.org?My=Query#", UriKind.Absolute)),
                (new Uri("http://tempuri.org?My=Query#TheHash", UriKind.Absolute), new Uri("http://tempuri.org?My=Query#TheHash", UriKind.Absolute))
            })
            {
                var actual = target.SplitLeaf(out string? actualLeaf);
                Assert.That(actual, Is.Not.Null);
                Assert.Multiple(() =>
                {
                    if (expectedResult.IsAbsoluteUri)
                    {
                        Assert.That(actual.IsAbsoluteUri, Is.True);
                        Assert.That(actual.AbsoluteUri, Is.EqualTo(expectedResult.AbsoluteUri));
                    }
                    else
                    {
                        Assert.That(actual.IsAbsoluteUri, Is.False);
                        Assert.That(actual.OriginalString, Is.EqualTo(expectedResult.OriginalString));
                    }
                    Assert.That(actualLeaf, Is.Null);
                });
            }
            foreach ((var target, var expectedResult, var expectedLeaf) in new (Uri Target, Uri Expected, string ExpectedLeaf)[]
            {
                (new Uri("Test/", UriKind.Relative), new Uri("Test", UriKind.Relative), string.Empty),
                (new Uri("Test/?", UriKind.Relative), new Uri("Test?", UriKind.Relative), string.Empty),
                (new Uri("Test/?My=Query", UriKind.Relative), new Uri("Test?My=Query", UriKind.Relative), string.Empty),
                (new Uri("Test/#", UriKind.Relative), new Uri("Test#", UriKind.Relative), string.Empty),
                (new Uri("Test/#TheHash", UriKind.Relative), new Uri("Test#TheHash", UriKind.Relative), string.Empty),
                (new Uri("Test/?#", UriKind.Relative), new Uri("Test?#", UriKind.Relative), string.Empty),
                (new Uri("Test/?#TheHash", UriKind.Relative), new Uri("Test?#TheHash", UriKind.Relative), string.Empty),
                (new Uri("Test/?My=Query#", UriKind.Relative), new Uri("Test?My=Query#", UriKind.Relative), string.Empty),
                (new Uri("Test/?My=Query#TheHash", UriKind.Relative), new Uri("Test?My=Query#TheHash", UriKind.Relative), string.Empty),
                (new Uri("Test", UriKind.Relative), new Uri(string.Empty, UriKind.Relative), "Test"),
                (new Uri("Test?", UriKind.Relative), new Uri("?", UriKind.Relative), "Test"),
                (new Uri("Test?My=Query", UriKind.Relative), new Uri("?My=Query", UriKind.Relative), "Test"),
                (new Uri("Test#", UriKind.Relative), new Uri("#", UriKind.Relative), "Test"),
                (new Uri("Test#TheHash", UriKind.Relative), new Uri("#TheHash", UriKind.Relative), "Test"),
                (new Uri("Test?#", UriKind.Relative), new Uri("?#", UriKind.Relative), "Test"),
                (new Uri("Test?#TheHash", UriKind.Relative), new Uri("?#TheHash", UriKind.Relative), "Test"),
                (new Uri("Test?My=Query#", UriKind.Relative), new Uri("?My=Query#", UriKind.Relative), "Test"),
                (new Uri("Test?My=Query#TheHash", UriKind.Relative), new Uri("?My=Query#TheHash", UriKind.Relative), "Test"),
                (new Uri("http://tempuri.org/Test/", UriKind.Absolute), new Uri("http://tempuri.org/Test", UriKind.Absolute), string.Empty),
                (new Uri("http://tempuri.org/Test/?", UriKind.Absolute), new Uri("http://tempuri.org/Test?", UriKind.Absolute), string.Empty),
                (new Uri("http://tempuri.org/Test/?My=Query", UriKind.Absolute), new Uri("http://tempuri.org/Test?My=Query", UriKind.Absolute), string.Empty),
                (new Uri("http://tempuri.org/Test/#", UriKind.Absolute), new Uri("http://tempuri.org/Test#", UriKind.Absolute), string.Empty),
                (new Uri("http://tempuri.org/Test/#TheHash", UriKind.Absolute), new Uri("http://tempuri.org/Test#TheHash", UriKind.Absolute), string.Empty),
                (new Uri("http://tempuri.org/Test/?#", UriKind.Absolute), new Uri("http://tempuri.org/Test?#", UriKind.Absolute), string.Empty),
                (new Uri("http://tempuri.org/Test/?#TheHash", UriKind.Absolute), new Uri("http://tempuri.org/Test?#TheHash", UriKind.Absolute), string.Empty),
                (new Uri("http://tempuri.org/Test/?My=Query#", UriKind.Absolute), new Uri("http://tempuri.org/Test?My=Query#", UriKind.Absolute), string.Empty),
                (new Uri("http://tempuri.org/Test/?My=Query#TheHash", UriKind.Absolute), new Uri("http://tempuri.org/Test?My=Query#TheHash", UriKind.Absolute), string.Empty),
                (new Uri("http://tempuri.org/Test", UriKind.Absolute), new Uri("http://tempuri.org", UriKind.Absolute), "Test"),
                (new Uri("http://tempuri.org/Test?", UriKind.Absolute), new Uri("http://tempuri.org?", UriKind.Absolute), "Test"),
                (new Uri("http://tempuri.org/Test?My=Query", UriKind.Absolute), new Uri("http://tempuri.orgMy=Query?", UriKind.Absolute), "Test"),
                (new Uri("http://tempuri.org/Test#", UriKind.Absolute), new Uri("http://tempuri.org#", UriKind.Absolute), "Test"),
                (new Uri("http://tempuri.org/Test#TheHash", UriKind.Absolute), new Uri("http://tempuri.org#TheHash", UriKind.Absolute), "Test"),
                (new Uri("http://tempuri.org/Test?#", UriKind.Absolute), new Uri("http://tempuri.org?#", UriKind.Absolute), "Test"),
                (new Uri("http://tempuri.org/Test?#TheHash", UriKind.Absolute), new Uri("http://tempuri.org?#TheHash", UriKind.Absolute), "Test"),
                (new Uri("http://tempuri.org/Test?My=Query#", UriKind.Absolute), new Uri("http://tempuri.org?My=Query#", UriKind.Absolute), "Test"),
                (new Uri("http://tempuri.org/Test?My=Query#TheHash", UriKind.Absolute), new Uri("http://tempuri.org?My=Query#TheHash", UriKind.Absolute), "Test"),
                (new Uri("http://tempuri.org/", UriKind.Absolute), new Uri("http://tempuri.org", UriKind.Absolute), string.Empty),
                (new Uri("http://tempuri.org/?", UriKind.Absolute), new Uri("http://tempuri.org?", UriKind.Absolute), string.Empty),
                (new Uri("http://tempuri.org/?My=Query", UriKind.Absolute), new Uri("http://tempuri.org?My=Query", UriKind.Absolute), string.Empty),
                (new Uri("http://tempuri.org/#", UriKind.Absolute), new Uri("http://tempuri.org#", UriKind.Absolute), string.Empty),
                (new Uri("http://tempuri.org/#TheHash", UriKind.Absolute), new Uri("http://tempuri.org#TheHash", UriKind.Absolute), string.Empty),
                (new Uri("http://tempuri.org/?#", UriKind.Absolute), new Uri("http://tempuri.org?#", UriKind.Absolute), string.Empty),
                (new Uri("http://tempuri.org/?#TheHash", UriKind.Absolute), new Uri("http://tempuri.org?#TheHash", UriKind.Absolute), string.Empty),
                (new Uri("http://tempuri.org/?My=Query#", UriKind.Absolute), new Uri("http://tempuri.org?My=Query#", UriKind.Absolute), string.Empty),
                (new Uri("http://tempuri.org/?My=Query#TheHash", UriKind.Absolute), new Uri("http://tempuri.org?My=Query#TheHash", UriKind.Absolute), string.Empty)
            })
            {
                var actual = target.SplitLeaf(out string? actualLeaf);
                Assert.That(actual, Is.Not.Null);
                Assert.Multiple(() =>
                {
                    if (expectedResult.IsAbsoluteUri)
                    {
                        Assert.That(actual.IsAbsoluteUri, Is.True);
                        Assert.That(actual.AbsoluteUri, Is.EqualTo(expectedResult.AbsoluteUri));
                    }
                    else
                    {
                        Assert.That(actual.IsAbsoluteUri, Is.False);
                        Assert.That(actual.OriginalString, Is.EqualTo(expectedResult.OriginalString));
                    }
                    Assert.That(actualLeaf, Is.Not.Null);
                    Assert.That(actualLeaf, Is.EqualTo(expectedLeaf));
                });
            }
        }
        
        [Test]
        public void SplitLeaf2Test()
        {
            foreach ((var target, var expectedResult) in new (Uri? Target, Uri Expected)[]
            {
                (null, new Uri(string.Empty, UriKind.Relative)),
                (new Uri(string.Empty, UriKind.Relative), new Uri(string.Empty, UriKind.Relative)),
                (new Uri("/", UriKind.Relative), new Uri("/", UriKind.Relative)),
                (new Uri("http://tempuri.org", UriKind.Absolute), new Uri("http://tempuri.org", UriKind.Absolute))
            })
            {
                var actual = target.SplitLeaf(out string? actualLeaf, out string? actualQueryAndFragment);
                Assert.That(actual, Is.Not.Null);
                Assert.Multiple(() =>
                {
                    if (expectedResult.IsAbsoluteUri)
                    {
                        Assert.That(actual.IsAbsoluteUri, Is.True);
                        Assert.That(actual.AbsoluteUri, Is.EqualTo(expectedResult.AbsoluteUri));
                    }
                    else
                    {
                        Assert.That(actual.IsAbsoluteUri, Is.False);
                        Assert.That(actual.OriginalString, Is.EqualTo(expectedResult.OriginalString));
                    }
                    Assert.That(actualLeaf, Is.Null);
                    Assert.That(actualQueryAndFragment, Is.Null);
                });
            }
            foreach ((var target, var expectedResult, var expectedLeaf) in new (Uri Target, Uri Expected, string ExpectedLeaf)[]
            {
                (new Uri("Test/Folder/", UriKind.Relative), new Uri("Test/Folder", UriKind.Relative), string.Empty),
                (new Uri("Test/Folder", UriKind.Relative), new Uri("Test", UriKind.Relative), "Folder"),
                (new Uri("Test/", UriKind.Relative), new Uri("Test", UriKind.Relative), string.Empty),
                (new Uri("Test", UriKind.Relative), new Uri(string.Empty, UriKind.Relative), "Test"),
                (new Uri("/Test/Folder/", UriKind.Relative), new Uri("/Test/Folder", UriKind.Relative), string.Empty),
                (new Uri("/Test/Folder", UriKind.Relative), new Uri("/Test", UriKind.Relative), "Folder"),
                (new Uri("/Test/", UriKind.Relative), new Uri("/Test", UriKind.Relative), string.Empty),
                (new Uri("/Test", UriKind.Relative), new Uri("/", UriKind.Relative), "Test"),
                (new Uri("http://tempuri.org/Test/Folder/", UriKind.Absolute), new Uri("http://tempuri.org/Test/Folder", UriKind.Absolute), string.Empty),
                (new Uri("http://tempuri.org/Test/Folder", UriKind.Absolute), new Uri("http://tempuri.org/Test", UriKind.Absolute), "Folder"),
                (new Uri("http://tempuri.org/Test/", UriKind.Absolute), new Uri("http://tempuri.org/Test", UriKind.Absolute), string.Empty),
                (new Uri("http://tempuri.org/Test", UriKind.Absolute), new Uri("http://tempuri.org", UriKind.Absolute), "Test"),
                (new Uri("http://tempuri.org/", UriKind.Absolute), new Uri("http://tempuri.org", UriKind.Absolute), string.Empty)
            })
            {
                var actual = target.SplitLeaf(out string? actualLeaf, out string? actualQueryAndFragment);
                Assert.That(actual, Is.Not.Null);
                Assert.Multiple(() =>
                {
                    if (expectedResult.IsAbsoluteUri)
                    {
                        Assert.That(actual.IsAbsoluteUri, Is.True);
                        Assert.That(actual.AbsoluteUri, Is.EqualTo(expectedResult.AbsoluteUri));
                    }
                    else
                    {
                        Assert.That(actual.IsAbsoluteUri, Is.False);
                        Assert.That(actual.OriginalString, Is.EqualTo(expectedResult.OriginalString));
                    }
                    Assert.That(actualLeaf, Is.Not.Null);
                    Assert.That(actualLeaf, Is.EqualTo(expectedLeaf));
                    Assert.That(actualQueryAndFragment, Is.Null);
                });
            }
            foreach ((var target, var expectedResult, var expectedQueryAndFragment) in new (Uri Target, Uri Expected, string ExpectedQueryAndFragment)[]
            {
                (new Uri("?", UriKind.Relative), new Uri(string.Empty, UriKind.Relative), "?"),
                (new Uri("?My=Query", UriKind.Relative), new Uri(string.Empty, UriKind.Relative), "?My=Query"),
                (new Uri("#", UriKind.Relative), new Uri(string.Empty, UriKind.Relative), "#"),
                (new Uri("#TheHash", UriKind.Relative), new Uri(string.Empty, UriKind.Relative), "#TheHash"),
                (new Uri("?#", UriKind.Relative), new Uri(string.Empty, UriKind.Relative), "?#"),
                (new Uri("?#TheHash", UriKind.Relative), new Uri(string.Empty, UriKind.Relative), "?#TheHash"),
                (new Uri("?My=Query#", UriKind.Relative), new Uri(string.Empty, UriKind.Relative), "?My=Query#"),
                (new Uri("?My=Query#TheHash", UriKind.Relative), new Uri(string.Empty, UriKind.Relative), "?My=Query#TheHash"),
                (new Uri("/?", UriKind.Relative), new Uri("/", UriKind.Relative), "?"),
                (new Uri("/?My=Query", UriKind.Relative), new Uri("/", UriKind.Relative), "?My=Query"),
                (new Uri("/#", UriKind.Relative), new Uri("/", UriKind.Relative), "#"),
                (new Uri("/#TheHash", UriKind.Relative), new Uri("/", UriKind.Relative), "#TheHash"),
                (new Uri("/?#", UriKind.Relative), new Uri("/", UriKind.Relative), "?#"),
                (new Uri("/?#TheHash", UriKind.Relative), new Uri("/", UriKind.Relative), "?#TheHash"),
                (new Uri("/My=Query?#", UriKind.Relative), new Uri("/", UriKind.Relative), "?My=Query#"),
                (new Uri("/My=Query?#TheHash", UriKind.Relative), new Uri("/", UriKind.Relative), "?My=Query#TheHash"),
                (new Uri("http://tempuri.org?", UriKind.Absolute), new Uri("http://tempuri.org", UriKind.Absolute), "?"),
                (new Uri("http://tempuri.org?My=Query", UriKind.Absolute), new Uri("http://tempuri.org", UriKind.Absolute), "?My=Query"),
                (new Uri("http://tempuri.org#", UriKind.Absolute), new Uri("http://tempuri.org", UriKind.Absolute), "#"),
                (new Uri("http://tempuri.org#TheHash", UriKind.Absolute), new Uri("http://tempuri.org", UriKind.Absolute), "#TheHash"),
                (new Uri("http://tempuri.org?#", UriKind.Absolute), new Uri("http://tempuri.org", UriKind.Absolute), "?#"),
                (new Uri("http://tempuri.org?#TheHash", UriKind.Absolute), new Uri("http://tempuri.org", UriKind.Absolute), "?#TheHash"),
                (new Uri("http://tempuri.org?My=Query#", UriKind.Absolute), new Uri("http://tempuri.org", UriKind.Absolute), "?My=Query#"),
                (new Uri("http://tempuri.org?My=Query#TheHash", UriKind.Absolute), new Uri("http://tempuri.org", UriKind.Absolute), "?My=Query#TheHash")
            })
            {
                var actual = target.SplitLeaf(out string? actualLeaf, out string? actualQueryAndFragment);
                Assert.That(actual, Is.Not.Null);
                Assert.Multiple(() =>
                {
                    if (expectedResult.IsAbsoluteUri)
                    {
                        Assert.That(actual.IsAbsoluteUri, Is.True);
                        Assert.That(actual.AbsoluteUri, Is.EqualTo(expectedResult.AbsoluteUri));
                    }
                    else
                    {
                        Assert.That(actual.IsAbsoluteUri, Is.False);
                        Assert.That(actual.OriginalString, Is.EqualTo(expectedResult.OriginalString));
                    }
                    Assert.That(actualLeaf, Is.Null);
                    Assert.That(actualQueryAndFragment, Is.Not.Null);
                    Assert.That(actualQueryAndFragment, Is.EqualTo(expectedQueryAndFragment));
                });
            }
            foreach ((var target, var expectedResult, var expectedLeaf, var expectedQueryAndFragment) in new (Uri Target, Uri Expected, string ExpectedLeaf, string ExpectedQueryAndFragment)[]
            {
                (new Uri("Test/Folder/?", UriKind.Relative), new Uri("Test/Folder", UriKind.Relative), string.Empty, "?"),
                (new Uri("Test/Folder/?My=Query", UriKind.Relative), new Uri("Test/Folder", UriKind.Relative), string.Empty, "?My=Query"),
                (new Uri("Test/Folder/#", UriKind.Relative), new Uri("Test/Folder", UriKind.Relative), string.Empty, "#"),
                (new Uri("Test/Folder/#TheHash", UriKind.Relative), new Uri("Test/Folder", UriKind.Relative), string.Empty, "#TheHash"),
                (new Uri("Test/Folder/?#", UriKind.Relative), new Uri("Test/Folder", UriKind.Relative), string.Empty, "?#"),
                (new Uri("Test/Folder/?My=Query#", UriKind.Relative), new Uri("Test/Folder", UriKind.Relative), string.Empty, "?My=Query#"),
                (new Uri("Test/Folder/?#TheHash", UriKind.Relative), new Uri("Test/Folder", UriKind.Relative), string.Empty, "?#TheHash"),
                (new Uri("Test/Folder/?My=Query#TheHash", UriKind.Relative), new Uri("Test/Folder", UriKind.Relative), string.Empty, "?My=Query#TheHash"),
                (new Uri("Test/Folder?", UriKind.Relative), new Uri("Test", UriKind.Relative), "Folder", "?"),
                (new Uri("Test/Folder?My=Query", UriKind.Relative), new Uri("Test", UriKind.Relative), "Folder", "?My=Query"),
                (new Uri("Test/Folder#", UriKind.Relative), new Uri("Test", UriKind.Relative), "Folder", "#"),
                (new Uri("Test/Folder#TheHash", UriKind.Relative), new Uri("Test", UriKind.Relative), "Folder", "#TheHash"),
                (new Uri("Test/Folder?#", UriKind.Relative), new Uri("Test", UriKind.Relative), "Folder", "?#"),
                (new Uri("Test/Folder?My=Query#", UriKind.Relative), new Uri("Test", UriKind.Relative), "Folder", "?My=Query#"),
                (new Uri("Test/Folder?#TheHash", UriKind.Relative), new Uri("Test", UriKind.Relative), "Folder", "?#TheHash"),
                (new Uri("Test/Folder?My=Query#TheHash", UriKind.Relative), new Uri("Test", UriKind.Relative), "Folder", "?My=Query#TheHash"),
                (new Uri("Test/?", UriKind.Relative), new Uri("Test", UriKind.Relative), string.Empty, "?"),
                (new Uri("Test/?My=Query", UriKind.Relative), new Uri("Test", UriKind.Relative), string.Empty, "?My=Query"),
                (new Uri("Test/#", UriKind.Relative), new Uri("Test", UriKind.Relative), string.Empty, "#"),
                (new Uri("Test/#TheHash", UriKind.Relative), new Uri("Test", UriKind.Relative), string.Empty, "#TheHash"),
                (new Uri("Test/?#", UriKind.Relative), new Uri("Test", UriKind.Relative), string.Empty, "?#"),
                (new Uri("Test/?My=Query#", UriKind.Relative), new Uri("Test", UriKind.Relative), string.Empty, "?My=Query#"),
                (new Uri("Test/?#TheHash", UriKind.Relative), new Uri("Test", UriKind.Relative), string.Empty, "?#TheHash"),
                (new Uri("Test/?My=Query#TheHash", UriKind.Relative), new Uri("Test", UriKind.Relative), string.Empty, "?My=Query#TheHash"),
                (new Uri("Test?", UriKind.Relative), new Uri(string.Empty, UriKind.Relative), "Test", "?"),
                (new Uri("Test?My=Query", UriKind.Relative), new Uri(string.Empty, UriKind.Relative), "Test", "?My=Query"),
                (new Uri("Test#", UriKind.Relative), new Uri(string.Empty, UriKind.Relative), "Test", "#"),
                (new Uri("Test#TheHash", UriKind.Relative), new Uri(string.Empty, UriKind.Relative), "Test", "#TheHash"),
                (new Uri("Test?#", UriKind.Relative), new Uri(string.Empty, UriKind.Relative), "Test", "?#"),
                (new Uri("Test?My=Query#", UriKind.Relative), new Uri(string.Empty, UriKind.Relative), "Test", "?My=Query#"),
                (new Uri("Test?#TheHash", UriKind.Relative), new Uri(string.Empty, UriKind.Relative), "Test", "?#TheHash"),
                (new Uri("Test?My=Query#TheHash", UriKind.Relative), new Uri(string.Empty, UriKind.Relative), "Test", "?My=Query#TheHash"),
                (new Uri("/Test/Folder/?", UriKind.Relative), new Uri("/Test/Folder", UriKind.Relative), string.Empty, "?"),
                (new Uri("/Test/Folder/?My=Query", UriKind.Relative), new Uri("Test/Folder", UriKind.Relative), string.Empty, "?My=Query"),
                (new Uri("/Test/Folder/#", UriKind.Relative), new Uri("/Test/Folder", UriKind.Relative), string.Empty, "#"),
                (new Uri("/Test/Folder/#TheHash", UriKind.Relative), new Uri("/Test/Folder", UriKind.Relative), string.Empty, "#TheHash"),
                (new Uri("/Test/Folder/?#", UriKind.Relative), new Uri("/Test/Folder", UriKind.Relative), string.Empty, "?#"),
                (new Uri("/Test/Folder/?My=Query#", UriKind.Relative), new Uri("/Test/Folder", UriKind.Relative), string.Empty, "?My=Query#"),
                (new Uri("/Test/Folder/?#TheHash", UriKind.Relative), new Uri("/Test/Folder", UriKind.Relative), string.Empty, "?#TheHash"),
                (new Uri("/Test/Folder/?My=Query#TheHash", UriKind.Relative), new Uri("/Test/Folder", UriKind.Relative), string.Empty, "?My=Query#TheHash"),
                (new Uri("/Test/Folder?", UriKind.Relative), new Uri("/Test", UriKind.Relative), "Folder", "?"),
                (new Uri("/Test/Folder?My=Query", UriKind.Relative), new Uri("/Test", UriKind.Relative), "Folder", "?My=Query"),
                (new Uri("/Test/Folder#", UriKind.Relative), new Uri("/Test", UriKind.Relative), "Folder", "#"),
                (new Uri("/Test/Folder#TheHash", UriKind.Relative), new Uri("/Test", UriKind.Relative), "Folder", "#TheHash"),
                (new Uri("/Test/Folder?#", UriKind.Relative), new Uri("/Test", UriKind.Relative), "Folder", "?#"),
                (new Uri("/Test/Folder?My=Query#", UriKind.Relative), new Uri("/Test", UriKind.Relative), "Folder", "?My=Query#"),
                (new Uri("/Test/Folder?#TheHash", UriKind.Relative), new Uri("/Test", UriKind.Relative), "Folder", "?#TheHash"),
                (new Uri("/Test/Folder?My=Query#TheHash", UriKind.Relative), new Uri("/Test", UriKind.Relative), "Folder", "?My=Query#TheHash"),
                (new Uri("/Test/?", UriKind.Relative), new Uri("/Test", UriKind.Relative), string.Empty, "?"),
                (new Uri("/Test/?My=Query", UriKind.Relative), new Uri("/Test", UriKind.Relative), string.Empty, "?My=Query"),
                (new Uri("/Test/#", UriKind.Relative), new Uri("/Test", UriKind.Relative), string.Empty, "#"),
                (new Uri("/Test/#TheHash", UriKind.Relative), new Uri("/Test", UriKind.Relative), string.Empty, "#TheHash"),
                (new Uri("/Test/?#", UriKind.Relative), new Uri("/Test", UriKind.Relative), string.Empty, "?#"),
                (new Uri("/Test/?My=Query#", UriKind.Relative), new Uri("/Test", UriKind.Relative), string.Empty, "?My=Query#"),
                (new Uri("/Test/?#TheHash", UriKind.Relative), new Uri("/Test", UriKind.Relative), string.Empty, "?#TheHash"),
                (new Uri("/Test/?My=Query#TheHash", UriKind.Relative), new Uri("/Test", UriKind.Relative), string.Empty, "?My=Query#TheHash"),
                (new Uri("/Test?", UriKind.Relative), new Uri("/", UriKind.Relative), "Test", "?"),
                (new Uri("/Test?My=Query", UriKind.Relative), new Uri("/", UriKind.Relative), "Test", "?My=Query"),
                (new Uri("/Test#", UriKind.Relative), new Uri("/", UriKind.Relative), "Test", "#"),
                (new Uri("/Test#TheHash", UriKind.Relative), new Uri("/", UriKind.Relative), "Test", "#TheHash"),
                (new Uri("/Test?#", UriKind.Relative), new Uri("/", UriKind.Relative), "Test", "?#"),
                (new Uri("/Test?My=Query#", UriKind.Relative), new Uri("/", UriKind.Relative), "Test", "?My=Query#"),
                (new Uri("/Test?#TheHash", UriKind.Relative), new Uri("/", UriKind.Relative), "Test", "?#TheHash"),
                (new Uri("/Test?My=Query#TheHash", UriKind.Relative), new Uri("/", UriKind.Relative), "Test", "?My=Query#TheHash"),
                (new Uri("http://tempuri.org/Test/Folder/?", UriKind.Absolute), new Uri("http://tempuri.org/Test/Folder", UriKind.Absolute), string.Empty, "?"),
                (new Uri("http://tempuri.org/Test/Folder/?My=Query", UriKind.Absolute), new Uri("http://tempuri.org/Test/Folder", UriKind.Absolute), string.Empty, "?My=Query"),
                (new Uri("http://tempuri.org/Test/Folder/#", UriKind.Absolute), new Uri("http://tempuri.org/Test/Folder", UriKind.Absolute), string.Empty, "#"),
                (new Uri("http://tempuri.org/Test/Folder/#TheHash", UriKind.Absolute), new Uri("http://tempuri.org/Test/Folder", UriKind.Absolute), string.Empty, "#TheHash"),
                (new Uri("http://tempuri.org/Test/Folder/?#", UriKind.Absolute), new Uri("http://tempuri.org/Test/Folder", UriKind.Absolute), string.Empty, "?#"),
                (new Uri("http://tempuri.org/Test/Folder/?My=Query#", UriKind.Absolute), new Uri("http://tempuri.org/Test/Folder", UriKind.Absolute), string.Empty, "?My=Query#"),
                (new Uri("http://tempuri.org/Test/Folder/?#TheHash", UriKind.Absolute), new Uri("http://tempuri.org/Test/Folder", UriKind.Absolute), string.Empty, "?#TheHash"),
                (new Uri("http://tempuri.org/Test/Folder/?My=Query#TheHash", UriKind.Absolute), new Uri("http://tempuri.org/Test/Folder", UriKind.Absolute), string.Empty, "?My=Query#TheHash"),
                (new Uri("http://tempuri.org/Test/Folder?", UriKind.Absolute), new Uri("http://tempuri.org/Test", UriKind.Absolute), "Folder", "?"),
                (new Uri("http://tempuri.org/Test/Folder?My=Query", UriKind.Absolute), new Uri("http://tempuri.org/Test", UriKind.Absolute), "Folder", "?My=Query"),
                (new Uri("http://tempuri.org/Test/Folder#", UriKind.Absolute), new Uri("http://tempuri.org/Test", UriKind.Absolute), "Folder", "#"),
                (new Uri("http://tempuri.org/Test/Folder#TheHash", UriKind.Absolute), new Uri("http://tempuri.org/Test", UriKind.Absolute), "Folder", "#TheHash"),
                (new Uri("http://tempuri.org/Test/Folder?#", UriKind.Absolute), new Uri("http://tempuri.org/Test", UriKind.Absolute), "Folder", "?#"),
                (new Uri("http://tempuri.org/Test/Folder?My=Query#", UriKind.Absolute), new Uri("http://tempuri.org/Test", UriKind.Absolute), "Folder", "?My=Query#"),
                (new Uri("http://tempuri.org/Test/Folder?#TheHash", UriKind.Absolute), new Uri("http://tempuri.org/Test", UriKind.Absolute), "Folder", "?#TheHash"),
                (new Uri("http://tempuri.org/Test/Folder?My=Query#TheHash", UriKind.Absolute), new Uri("http://tempuri.org/Test", UriKind.Absolute), "Folder", "?My=Query#TheHash"),
                (new Uri("http://tempuri.org/Test/?", UriKind.Absolute), new Uri("http://tempuri.org/Test", UriKind.Absolute), string.Empty, "?"),
                (new Uri("http://tempuri.org/Test/?My=Query", UriKind.Absolute), new Uri("http://tempuri.org/Test", UriKind.Absolute), string.Empty, "?My=Query"),
                (new Uri("http://tempuri.org/Test/#", UriKind.Absolute), new Uri("http://tempuri.org/Test", UriKind.Absolute), string.Empty, "#"),
                (new Uri("http://tempuri.org/Test/#TheHash", UriKind.Absolute), new Uri("http://tempuri.org/Test", UriKind.Absolute), string.Empty, "#TheHash"),
                (new Uri("http://tempuri.org/Test/?#", UriKind.Absolute), new Uri("http://tempuri.org/Test", UriKind.Absolute), string.Empty, "?#"),
                (new Uri("http://tempuri.org/Test/?My=Query#", UriKind.Absolute), new Uri("http://tempuri.org/Test", UriKind.Absolute), string.Empty, "?My=Query#"),
                (new Uri("http://tempuri.org/Test/?#TheHash", UriKind.Absolute), new Uri("http://tempuri.org/Test", UriKind.Absolute), string.Empty, "?#TheHash"),
                (new Uri("http://tempuri.org/Test/?My=Query#TheHash", UriKind.Absolute), new Uri("http://tempuri.org/Test", UriKind.Absolute), string.Empty, "?My=Query#TheHash"),
                (new Uri("http://tempuri.org/Test?", UriKind.Absolute), new Uri("http://tempuri.org", UriKind.Absolute), "Test", "?"),
                (new Uri("http://tempuri.org/Test?My=Query", UriKind.Absolute), new Uri("http://tempuri.org", UriKind.Absolute), "Test", "?My=Query"),
                (new Uri("http://tempuri.org/Test#", UriKind.Absolute), new Uri("http://tempuri.org", UriKind.Absolute), "Test", "#"),
                (new Uri("http://tempuri.org/Test#TheHash", UriKind.Absolute), new Uri("http://tempuri.org", UriKind.Absolute), "Test", "#TheHash"),
                (new Uri("http://tempuri.org/Test?#", UriKind.Absolute), new Uri("http://tempuri.org", UriKind.Absolute), "Test", "?#"),
                (new Uri("http://tempuri.org/Test?My=Query#", UriKind.Absolute), new Uri("http://tempuri.org", UriKind.Absolute), "Test", "?My=Query#"),
                (new Uri("http://tempuri.org/Test?#TheHash", UriKind.Absolute), new Uri("http://tempuri.org", UriKind.Absolute), "Test", "?#TheHash"),
                (new Uri("http://tempuri.org/Test?My=Query#TheHash", UriKind.Absolute), new Uri("http://tempuri.org", UriKind.Absolute), "Test", "?My=Query#TheHash"),
                (new Uri("http://tempuri.org/?", UriKind.Absolute), new Uri("http://tempuri.org", UriKind.Absolute), string.Empty, "?"),
                (new Uri("http://tempuri.org/?My=Query", UriKind.Absolute), new Uri("http://tempuri.org", UriKind.Absolute), string.Empty, "?My=Query"),
                (new Uri("http://tempuri.org/#", UriKind.Absolute), new Uri("http://tempuri.org", UriKind.Absolute), string.Empty, "#"),
                (new Uri("http://tempuri.org/#TheHash", UriKind.Absolute), new Uri("http://tempuri.org", UriKind.Absolute), string.Empty, "#TheHash"),
                (new Uri("http://tempuri.org/?#", UriKind.Absolute), new Uri("http://tempuri.org", UriKind.Absolute), string.Empty, "?My=Query#"),
                (new Uri("http://tempuri.org/?My=Query#", UriKind.Absolute), new Uri("http://tempuri.org", UriKind.Absolute), string.Empty, "?#"),
                (new Uri("http://tempuri.org/?#TheHash", UriKind.Absolute), new Uri("http://tempuri.org", UriKind.Absolute), string.Empty, "?My=Query#TheHash"),
                (new Uri("http://tempuri.org/My=Query#TheHash", UriKind.Absolute), new Uri("http://tempuri.org", UriKind.Absolute), string.Empty, "?#TheHash")
            })
            {
                var actual = target.SplitLeaf(out string? actualLeaf, out string? actualQueryAndFragment);
                Assert.That(actual, Is.Not.Null);
                Assert.Multiple(() =>
                {
                    if (expectedResult.IsAbsoluteUri)
                    {
                        Assert.That(actual.IsAbsoluteUri, Is.True);
                        Assert.That(actual.AbsoluteUri, Is.EqualTo(expectedResult.AbsoluteUri));
                    }
                    else
                    {
                        Assert.That(actual.IsAbsoluteUri, Is.False);
                        Assert.That(actual.OriginalString, Is.EqualTo(expectedResult.OriginalString));
                    }
                    Assert.That(actualLeaf, Is.Not.Null);
                    Assert.That(actualLeaf, Is.EqualTo(expectedLeaf));
                    Assert.That(actualQueryAndFragment, Is.Not.Null);
                    Assert.That(actualQueryAndFragment, Is.EqualTo(expectedQueryAndFragment));
                });
            }
        }
        
        [Test]
        public void SplitLeaf3Test()
        {
            foreach ((var target, var expectedResult) in new (Uri? Target, Uri Expected)[]
            {
                (null, new Uri(string.Empty, UriKind.Relative)),
                (new Uri(string.Empty, UriKind.Relative), new Uri(string.Empty, UriKind.Relative)),
                (new Uri("/", UriKind.Relative), new Uri("/", UriKind.Relative)),
                (new Uri("http://tempuri.org", UriKind.Absolute), new Uri("http://tempuri.org", UriKind.Absolute))
            })
            {
                var actual = target.SplitLeaf(out string? actualLeaf, out string? actualQuery, out string? actualFragment);
                Assert.That(actual, Is.Not.Null);
                Assert.Multiple(() =>
                {
                    if (expectedResult.IsAbsoluteUri)
                    {
                        Assert.That(actual.IsAbsoluteUri, Is.True);
                        Assert.That(actual.AbsoluteUri, Is.EqualTo(expectedResult.AbsoluteUri));
                    }
                    else
                    {
                        Assert.That(actual.IsAbsoluteUri, Is.False);
                        Assert.That(actual.OriginalString, Is.EqualTo(expectedResult.OriginalString));
                    }
                    Assert.That(actualLeaf, Is.Null);
                    Assert.That(actualQuery, Is.Null);
                    Assert.That(actualFragment, Is.Null);
                });
            }
            foreach ((var target, var expectedResult, var expectedLeaf) in new (Uri Target, Uri Expected, string ExpectedLeaf)[]
            {
                (new Uri("Test/Folder/", UriKind.Relative), new Uri("Test/Folder", UriKind.Relative), string.Empty),
                (new Uri("Test/Folder", UriKind.Relative), new Uri("Test", UriKind.Relative), "Folder"),
                (new Uri("Test/", UriKind.Relative), new Uri("Test", UriKind.Relative), string.Empty),
                (new Uri("Test", UriKind.Relative), new Uri(string.Empty, UriKind.Relative), "Test"),
                (new Uri("/Test/Folder/", UriKind.Relative), new Uri("/Test/Folder", UriKind.Relative), string.Empty),
                (new Uri("/Test/Folder", UriKind.Relative), new Uri("/Test", UriKind.Relative), "Folder"),
                (new Uri("/Test/", UriKind.Relative), new Uri("/Test", UriKind.Relative), string.Empty),
                (new Uri("/Test", UriKind.Relative), new Uri("/", UriKind.Relative), "Test"),
                (new Uri("http://tempuri.org/Test/Folder/", UriKind.Absolute), new Uri("http://tempuri.org/Test/Folder", UriKind.Absolute), string.Empty),
                (new Uri("http://tempuri.org/Test/Folder", UriKind.Absolute), new Uri("http://tempuri.org/Test", UriKind.Absolute), "Folder"),
                (new Uri("http://tempuri.org/Test/", UriKind.Absolute), new Uri("http://tempuri.org/Test", UriKind.Absolute), string.Empty),
                (new Uri("http://tempuri.org/Test", UriKind.Absolute), new Uri("http://tempuri.org", UriKind.Absolute), "Test"),
                (new Uri("http://tempuri.org/", UriKind.Absolute), new Uri("http://tempuri.org", UriKind.Absolute), string.Empty)
            })
            {
                var actual = target.SplitLeaf(out string? actualLeaf, out string? actualQuery, out string? actualFragment);
                Assert.That(actual, Is.Not.Null);
                Assert.Multiple(() =>
                {
                    if (expectedResult.IsAbsoluteUri)
                    {
                        Assert.That(actual.IsAbsoluteUri, Is.True);
                        Assert.That(actual.AbsoluteUri, Is.EqualTo(expectedResult.AbsoluteUri));
                    }
                    else
                    {
                        Assert.That(actual.IsAbsoluteUri, Is.False);
                        Assert.That(actual.OriginalString, Is.EqualTo(expectedResult.OriginalString));
                    }
                    Assert.That(actualLeaf, Is.Not.Null);
                    Assert.That(actualLeaf, Is.EqualTo(expectedLeaf));
                    Assert.That(actualQuery, Is.Null);
                    Assert.That(actualFragment, Is.Null);
                });
            }
            foreach ((var target, var expectedResult, var expectedQuery) in new (Uri Target, Uri Expected, string ExpectedQuery)[]
            {
                (new Uri("?", UriKind.Relative), new Uri(string.Empty, UriKind.Relative), string.Empty),
                (new Uri("?My=Query", UriKind.Relative), new Uri(string.Empty, UriKind.Relative), "My=Query"),
                (new Uri("/?", UriKind.Relative), new Uri("/", UriKind.Relative), string.Empty),
                (new Uri("/?My=Query", UriKind.Relative), new Uri("/", UriKind.Relative), string.Empty),
                (new Uri("http://tempuri.org?", UriKind.Absolute), new Uri("http://tempuri.org", UriKind.Absolute), string.Empty),
                (new Uri("http://tempuri.org?My=Query", UriKind.Absolute), new Uri("http://tempuri.org", UriKind.Absolute), "My=Query")
            })
            {
                var actual = target.SplitLeaf(out string? actualLeaf, out string? actualQuery, out string? actualFragment);
                Assert.That(actual, Is.Not.Null);
                Assert.Multiple(() =>
                {
                    if (expectedResult.IsAbsoluteUri)
                    {
                        Assert.That(actual.IsAbsoluteUri, Is.True);
                        Assert.That(actual.AbsoluteUri, Is.EqualTo(expectedResult.AbsoluteUri));
                    }
                    else
                    {
                        Assert.That(actual.IsAbsoluteUri, Is.False);
                        Assert.That(actual.OriginalString, Is.EqualTo(expectedResult.OriginalString));
                    }
                    Assert.That(actualLeaf, Is.Null);
                    Assert.That(actualQuery, Is.Not.Null);
                    Assert.That(actualQuery, Is.EqualTo(expectedQuery));
                    Assert.That(actualFragment, Is.Null);
                });
            }
            foreach ((var target, var expectedResult, var expectedFragment) in new (Uri Target, Uri Expected, string ExpectedFragment)[]
            {
                (new Uri("#", UriKind.Relative), new Uri(string.Empty, UriKind.Relative), string.Empty),
                (new Uri("#TheHash", UriKind.Relative), new Uri(string.Empty, UriKind.Relative), "TheHash"),
                (new Uri("/#", UriKind.Relative), new Uri("/", UriKind.Relative), string.Empty),
                (new Uri("/#TheHash", UriKind.Relative), new Uri("/", UriKind.Relative), "TheHash"),
                (new Uri("http://tempuri.org#", UriKind.Absolute), new Uri("http://tempuri.org", UriKind.Absolute), string.Empty),
                (new Uri("http://tempuri.org#TheHash", UriKind.Absolute), new Uri("http://tempuri.org", UriKind.Absolute), "TheHash")
            })
            {
                var actual = target.SplitLeaf(out string? actualLeaf, out string? actualQuery, out string? actualFragment);
                Assert.That(actual, Is.Not.Null);
                Assert.Multiple(() =>
                {
                    if (expectedResult.IsAbsoluteUri)
                    {
                        Assert.That(actual.IsAbsoluteUri, Is.True);
                        Assert.That(actual.AbsoluteUri, Is.EqualTo(expectedResult.AbsoluteUri));
                    }
                    else
                    {
                        Assert.That(actual.IsAbsoluteUri, Is.False);
                        Assert.That(actual.OriginalString, Is.EqualTo(expectedResult.OriginalString));
                    }
                    Assert.That(actualLeaf, Is.Null);
                    Assert.That(actualQuery, Is.Null);
                    Assert.That(actualFragment, Is.Not.Null);
                    Assert.That(actualFragment, Is.EqualTo(expectedFragment));
                });
            }
            foreach ((var target, var expectedResult, var expectedQuery, var expectedFragment) in new (Uri Target, Uri Expected, string ExpectedQuery, string ExpectedFragment)[]
            {
                (new Uri("?#", UriKind.Relative), new Uri(string.Empty, UriKind.Relative), string.Empty, string.Empty),
                (new Uri("?#TheHash", UriKind.Relative), new Uri(string.Empty, UriKind.Relative), string.Empty, "TheHash"),
                (new Uri("?My=Query#", UriKind.Relative), new Uri(string.Empty, UriKind.Relative), "My=Query", string.Empty),
                (new Uri("?My=Query#TheHash", UriKind.Relative), new Uri(string.Empty, UriKind.Relative), "My=Query", "TheHash"),
                (new Uri("/?#", UriKind.Relative), new Uri("/", UriKind.Relative), string.Empty, string.Empty),
                (new Uri("/?#TheHash", UriKind.Relative), new Uri("/", UriKind.Relative), string.Empty, "TheHash"),
                (new Uri("/?My=Query#", UriKind.Relative), new Uri("/", UriKind.Relative), string.Empty, string.Empty),
                (new Uri("/?My=Query#TheHash", UriKind.Relative), new Uri("/", UriKind.Relative), string.Empty, "TheHash"),
                (new Uri("http://tempuri.org?#", UriKind.Absolute), new Uri("http://tempuri.org", UriKind.Absolute), string.Empty, string.Empty),
                (new Uri("http://tempuri.org?#TheHash", UriKind.Absolute), new Uri("http://tempuri.org", UriKind.Absolute), string.Empty, "TheHash"),
                (new Uri("http://tempuri.org?My=Query#", UriKind.Absolute), new Uri("http://tempuri.org", UriKind.Absolute), "My=Query", string.Empty),
                (new Uri("http://tempuri.org?My=Query#TheHash", UriKind.Absolute), new Uri("http://tempuri.org", UriKind.Absolute), "My=Query", "TheHash")
            })
            {
                var actual = target.SplitLeaf(out string? actualLeaf, out string? actualQuery, out string? actualFragment);
                Assert.That(actual, Is.Not.Null);
                Assert.Multiple(() =>
                {
                    if (expectedResult.IsAbsoluteUri)
                    {
                        Assert.That(actual.IsAbsoluteUri, Is.True);
                        Assert.That(actual.AbsoluteUri, Is.EqualTo(expectedResult.AbsoluteUri));
                    }
                    else
                    {
                        Assert.That(actual.IsAbsoluteUri, Is.False);
                        Assert.That(actual.OriginalString, Is.EqualTo(expectedResult.OriginalString));
                    }
                    Assert.That(actualLeaf, Is.Null);
                    Assert.That(actualQuery, Is.Not.Null);
                    Assert.That(actualQuery, Is.EqualTo(expectedQuery));
                    Assert.That(actualFragment, Is.Not.Null);
                    Assert.That(actualFragment, Is.EqualTo(expectedFragment));
                });
            }
            foreach ((var target, var expectedResult, var expectedLeaf, var expectedQuery) in new (Uri Target, Uri Expected, string expectedLeaf, string ExpectedQuery)[]
            {
                (new Uri("Test/Folder?", UriKind.Relative), new Uri("Test", UriKind.Relative), "Folder", string.Empty),
                (new Uri("Test/Folder?My=Query", UriKind.Relative), new Uri("Test", UriKind.Relative), "Folder", "My=Query"),
                (new Uri("Test/?", UriKind.Relative), new Uri("Test", UriKind.Relative), string.Empty, string.Empty),
                (new Uri("Test/?My=Query", UriKind.Relative), new Uri("Test", UriKind.Relative), string.Empty, "My=Query"),
                (new Uri("Test?", UriKind.Relative), new Uri(string.Empty, UriKind.Relative), "Test", string.Empty),
                (new Uri("Test?My=Query", UriKind.Relative), new Uri(string.Empty, UriKind.Relative), "Test", "My=Query"),
                (new Uri("/Test/Folder?", UriKind.Relative), new Uri("/Test", UriKind.Relative), "Folder", string.Empty),
                (new Uri("/Test/Folder?My=Query", UriKind.Relative), new Uri("/Test", UriKind.Relative), "Folder", "My=Query"),
                (new Uri("/Test/?", UriKind.Relative), new Uri("/Test", UriKind.Relative), string.Empty, string.Empty),
                (new Uri("/Test/?My=Query", UriKind.Relative), new Uri("T/est", UriKind.Relative), string.Empty, "My=Query"),
                (new Uri("/Test?", UriKind.Relative), new Uri("/", UriKind.Relative), "Test", string.Empty),
                (new Uri("/Test?My=Query", UriKind.Relative), new Uri("/", UriKind.Relative), "Test", "My=Query"),
                (new Uri("https://tempuri.org/Test/Folder/?", UriKind.Absolute), new Uri("https://tempuri.org/Test/Folder", UriKind.Absolute), string.Empty, string.Empty),
                (new Uri("https://tempuri.org/Test/Folder/?My=Query", UriKind.Absolute), new Uri("https://tempuri.org/Test/Folder", UriKind.Absolute), string.Empty, "My=Query"),
                (new Uri("https://tempuri.org/Test/Folder?", UriKind.Absolute), new Uri("https://tempuri.org/Test", UriKind.Absolute), "Folder", string.Empty),
                (new Uri("https://tempuri.org/Test/Folder?My=Query", UriKind.Absolute), new Uri("https://tempuri.org/Test", UriKind.Absolute), "Folder", "My=Query"),
                (new Uri("https://tempuri.org/Test/?", UriKind.Absolute), new Uri("https://tempuri.org/Test", UriKind.Absolute), string.Empty, string.Empty),
                (new Uri("https://tempuri.org/Test/?My=Query", UriKind.Absolute), new Uri("https://tempuri.org/Test", UriKind.Absolute), string.Empty, "My=Query"),
                (new Uri("https://tempuri.org/Test?", UriKind.Absolute), new Uri("https://tempuri.org/", UriKind.Absolute), "Test", string.Empty),
                (new Uri("https://tempuri.org/Test?My=Query", UriKind.Absolute), new Uri("https://tempuri.org/", UriKind.Absolute), "Test", "My=Query"),
                (new Uri("https://tempuri.org/?", UriKind.Absolute), new Uri("https://tempuri.org", UriKind.Absolute), string.Empty, string.Empty),
                (new Uri("https://tempuri.org/?My=Query", UriKind.Absolute), new Uri("https://tempuri.org", UriKind.Absolute), string.Empty, "My=Query")
            })
            {
                var actual = target.SplitLeaf(out string? actualLeaf, out string? actualQuery, out string? actualFragment);
                Assert.That(actual, Is.Not.Null);
                Assert.Multiple(() =>
                {
                    if (expectedResult.IsAbsoluteUri)
                    {
                        Assert.That(actual.IsAbsoluteUri, Is.True);
                        Assert.That(actual.AbsoluteUri, Is.EqualTo(expectedResult.AbsoluteUri));
                    }
                    else
                    {
                        Assert.That(actual.IsAbsoluteUri, Is.False);
                        Assert.That(actual.OriginalString, Is.EqualTo(expectedResult.OriginalString));
                    }
                    Assert.That(actualLeaf, Is.Not.Null);
                    Assert.That(actualLeaf, Is.EqualTo(expectedLeaf));
                    Assert.That(actualQuery, Is.Not.Null);
                    Assert.That(actualQuery, Is.EqualTo(expectedQuery));
                    Assert.That(actualFragment, Is.Null);
                });
            }
            foreach ((var target, var expectedResult, var expectedLeaf, var expectedFragment) in new (Uri Target, Uri Expected, string expectedLeaf, string ExpectedFragment)[]
            {
                (new Uri("Test/Folder#", UriKind.Relative), new Uri("Test", UriKind.Relative), "Folder", string.Empty),
                (new Uri("Test/Folder#TheHash", UriKind.Relative), new Uri("Test", UriKind.Relative), "Folder", "TheHash"),
                (new Uri("Test/#", UriKind.Relative), new Uri("Test", UriKind.Relative), string.Empty, string.Empty),
                (new Uri("Test/#TheHash", UriKind.Relative), new Uri("Test", UriKind.Relative), string.Empty, "TheHash"),
                (new Uri("Test#", UriKind.Relative), new Uri(string.Empty, UriKind.Relative), "Test", string.Empty),
                (new Uri("Test#TheHash", UriKind.Relative), new Uri(string.Empty, UriKind.Relative), "Test", "TheHash"),
                (new Uri("/Test/Folder#", UriKind.Relative), new Uri("/Test", UriKind.Relative), "Folder", string.Empty),
                (new Uri("/Test/Folder#TheHash", UriKind.Relative), new Uri("/Test", UriKind.Relative), "Folder", "TheHash"),
                (new Uri("/Test/#", UriKind.Relative), new Uri("/Test", UriKind.Relative), string.Empty, string.Empty),
                (new Uri("/Test/#TheHash", UriKind.Relative), new Uri("/Test", UriKind.Relative), string.Empty, "TheHash"),
                (new Uri("/Test#", UriKind.Relative), new Uri("/", UriKind.Relative), "Test", string.Empty),
                (new Uri("/Test#TheHash", UriKind.Relative), new Uri("/", UriKind.Relative), "Test", "TheHash"),
                (new Uri("https://tempuri.org/Test/Folder/#", UriKind.Absolute), new Uri("https://tempuri.org/Test/Folder", UriKind.Absolute), string.Empty, string.Empty),
                (new Uri("https://tempuri.org/Test/Folder/#TheHash", UriKind.Absolute), new Uri("https://tempuri.org/Test/Folder", UriKind.Absolute), string.Empty, "TheHash"),
                (new Uri("https://tempuri.org/Test/Folder#", UriKind.Absolute), new Uri("https://tempuri.org/Test", UriKind.Absolute), "Folder", string.Empty),
                (new Uri("https://tempuri.org/Test/Folder#TheHash", UriKind.Absolute), new Uri("https://tempuri.org/Test", UriKind.Absolute), "Folder", "TheHash"),
                (new Uri("https://tempuri.org/Test/#", UriKind.Absolute), new Uri("https://tempuri.org/Test", UriKind.Absolute), string.Empty, string.Empty),
                (new Uri("https://tempuri.org/Test/#TheHash", UriKind.Absolute), new Uri("https://tempuri.org/Test", UriKind.Absolute), string.Empty, "TheHash"),
                (new Uri("https://tempuri.org/Test#", UriKind.Absolute), new Uri("https://tempuri.org/", UriKind.Absolute), "Test", string.Empty),
                (new Uri("https://tempuri.org/Test#TheHash", UriKind.Absolute), new Uri("https://tempuri.org/", UriKind.Absolute), "Test", "TheHash"),
                (new Uri("https://tempuri.org/#", UriKind.Absolute), new Uri("https://tempuri.org", UriKind.Absolute), string.Empty, string.Empty),
                (new Uri("https://tempuri.org/#TheHash", UriKind.Absolute), new Uri("https://tempuri.org", UriKind.Absolute), string.Empty, "TheHash")
            })
            {
                var actual = target.SplitLeaf(out string? actualLeaf, out string? actualQuery, out string? actualFragment);
                Assert.That(actual, Is.Not.Null);
                Assert.Multiple(() =>
                {
                    if (expectedResult.IsAbsoluteUri)
                    {
                        Assert.That(actual.IsAbsoluteUri, Is.True);
                        Assert.That(actual.AbsoluteUri, Is.EqualTo(expectedResult.AbsoluteUri));
                    }
                    else
                    {
                        Assert.That(actual.IsAbsoluteUri, Is.False);
                        Assert.That(actual.OriginalString, Is.EqualTo(expectedResult.OriginalString));
                    }
                    Assert.That(actualLeaf, Is.Not.Null);
                    Assert.That(actualLeaf, Is.EqualTo(expectedLeaf));
                    Assert.That(actualQuery, Is.Null);
                    Assert.That(actualFragment, Is.Not.Null);
                    Assert.That(actualFragment, Is.EqualTo(expectedFragment));
                });
            }
            foreach ((var target, var expectedResult, var expectedLeaf, var expectedQuery, var expectedFragment) in new (Uri Target, Uri Expected, string expectedLeaf, string ExpectedQuery, string ExpectedFragment)[]
            {
                (new Uri("Test/Folder?#", UriKind.Relative), new Uri("Test", UriKind.Relative), "Folder", string.Empty, string.Empty),
                (new Uri("Test/Folder?#TheHash", UriKind.Relative), new Uri("Test", UriKind.Relative), "Folder", string.Empty, "TheHash"),
                (new Uri("Test/Folder?My=Query#", UriKind.Relative), new Uri("Test", UriKind.Relative), "Folder", "My=Query", string.Empty),
                (new Uri("Test/Folder?My=Query#TheHash", UriKind.Relative), new Uri("Test", UriKind.Relative), "Folder", "My=Query", "TheHash"),
                (new Uri("Test/?#", UriKind.Relative), new Uri("Test", UriKind.Relative), string.Empty, string.Empty, string.Empty),
                (new Uri("Test/?#TheHash", UriKind.Relative), new Uri("Test", UriKind.Relative), string.Empty, string.Empty, "TheHash"),
                (new Uri("Test/?My=Query#", UriKind.Relative), new Uri("Test", UriKind.Relative), string.Empty, "My=Query", string.Empty),
                (new Uri("Test/?My=Query#TheHash", UriKind.Relative), new Uri("Test", UriKind.Relative), string.Empty, "My=Query", "TheHash"),
                (new Uri("Test?#", UriKind.Relative), new Uri(string.Empty, UriKind.Relative), "Test", string.Empty, string.Empty),
                (new Uri("Test?#TheHash", UriKind.Relative), new Uri(string.Empty, UriKind.Relative), "Test", string.Empty, "TheHash"),
                (new Uri("Test?My=Query#", UriKind.Relative), new Uri(string.Empty, UriKind.Relative), "Test", "My=Query", string.Empty),
                (new Uri("Test?My=Query#TheHash", UriKind.Relative), new Uri(string.Empty, UriKind.Relative), "Test", "My=Query", "TheHash"),
                (new Uri("/Test/Folder?#", UriKind.Relative), new Uri("/Test", UriKind.Relative), "Folder", string.Empty, string.Empty),
                (new Uri("/Test/Folder?#TheHash", UriKind.Relative), new Uri("/Test", UriKind.Relative), "Folder", string.Empty, "TheHash"),
                (new Uri("/Test/Folder?My=Query#", UriKind.Relative), new Uri("/Test", UriKind.Relative), "Folder", "My=Query", string.Empty),
                (new Uri("/Test/Folder?My=Query#TheHash", UriKind.Relative), new Uri("/Test", UriKind.Relative), "Folder", "My=Query", "TheHash"),
                (new Uri("/Test/?#", UriKind.Relative), new Uri("/Test", UriKind.Relative), string.Empty, string.Empty, string.Empty),
                (new Uri("/Test/?#TheHash", UriKind.Relative), new Uri("/Test", UriKind.Relative), string.Empty, string.Empty, "TheHash"),
                (new Uri("/Test/?My=Query#", UriKind.Relative), new Uri("/Test", UriKind.Relative), string.Empty, "My=Query", string.Empty),
                (new Uri("/Test/?My=Query#TheHash", UriKind.Relative), new Uri("/Test", UriKind.Relative), string.Empty, "My=Query", "TheHash"),
                (new Uri("/Test?#", UriKind.Relative), new Uri("/", UriKind.Relative), "Test", string.Empty, string.Empty),
                (new Uri("/Test?#TheHash", UriKind.Relative), new Uri("/", UriKind.Relative), "Test", string.Empty, "TheHash"),
                (new Uri("/Test?My=Query#", UriKind.Relative), new Uri("/", UriKind.Relative), "Test", "My=Query", string.Empty),
                (new Uri("/Test?My=Query#TheHash", UriKind.Relative), new Uri("/", UriKind.Relative), "Test", "My=Query", "TheHash"),
                (new Uri("https://tempuri.org/Test/Folder/?#", UriKind.Absolute), new Uri("https://tempuri.org/Test/Folder", UriKind.Absolute), string.Empty, string.Empty, string.Empty),
                (new Uri("https://tempuri.org/Test/Folder/?#TheHash", UriKind.Absolute), new Uri("https://tempuri.org/Test/Folder", UriKind.Absolute), string.Empty, string.Empty, "TheHash"),
                (new Uri("https://tempuri.org/Test/Folder/?My=Query#", UriKind.Absolute), new Uri("https://tempuri.org/Test/Folder", UriKind.Absolute), string.Empty, "My=Query", string.Empty),
                (new Uri("https://tempuri.org/Test/Folder/?My=Query#TheHash", UriKind.Absolute), new Uri("https://tempuri.org/Test/Folder", UriKind.Absolute), string.Empty, "My=Query", "TheHash"),
                (new Uri("https://tempuri.org/Test/Folder?#", UriKind.Absolute), new Uri("https://tempuri.org/Test", UriKind.Absolute), "Folder", string.Empty, string.Empty),
                (new Uri("https://tempuri.org/Test/Folder?#TheHash", UriKind.Absolute), new Uri("https://tempuri.org/Test", UriKind.Absolute), "Folder", string.Empty, "TheHash"),
                (new Uri("https://tempuri.org/Test/Folder?My=Query#", UriKind.Absolute), new Uri("https://tempuri.org/Test", UriKind.Absolute), "Folder", "My=Query", string.Empty),
                (new Uri("https://tempuri.org/Test/Folder?My=Query#TheHash", UriKind.Absolute), new Uri("https://tempuri.org/Test", UriKind.Absolute), "Folder", "My=Query", "TheHash"),
                (new Uri("https://tempuri.org/Test/?#", UriKind.Absolute), new Uri("https://tempuri.org/Test", UriKind.Absolute), string.Empty, string.Empty, string.Empty),
                (new Uri("https://tempuri.org/Test/?#TheHash", UriKind.Absolute), new Uri("https://tempuri.org/Test", UriKind.Absolute), string.Empty, string.Empty, "TheHash"),
                (new Uri("https://tempuri.org/Test/?My=Query#", UriKind.Absolute), new Uri("https://tempuri.org/Test", UriKind.Absolute), string.Empty, "My=Query", string.Empty),
                (new Uri("https://tempuri.org/Test/?My=Query#TheHash", UriKind.Absolute), new Uri("https://tempuri.org/Test", UriKind.Absolute), string.Empty, "My=Query", "TheHash"),
                (new Uri("https://tempuri.org/Test?#", UriKind.Absolute), new Uri("https://tempuri.org/", UriKind.Absolute), "Test", string.Empty, string.Empty),
                (new Uri("https://tempuri.org/Test?#TheHash", UriKind.Absolute), new Uri("https://tempuri.org/", UriKind.Absolute), "Test", string.Empty, "TheHash"),
                (new Uri("https://tempuri.org/Test?My=Query#", UriKind.Absolute), new Uri("https://tempuri.org/", UriKind.Absolute), "Test", "My=Query", string.Empty),
                (new Uri("https://tempuri.org/Test?My=Query#TheHash", UriKind.Absolute), new Uri("https://tempuri.org/", UriKind.Absolute), "Test", "My=Query", "TheHash"),
                (new Uri("https://tempuri.org/?#", UriKind.Absolute), new Uri("https://tempuri.org", UriKind.Absolute), string.Empty, string.Empty, string.Empty),
                (new Uri("https://tempuri.org/?#TheHash", UriKind.Absolute), new Uri("https://tempuri.org", UriKind.Absolute), string.Empty, string.Empty, "TheHash"),
                (new Uri("https://tempuri.org/?My=Query#", UriKind.Absolute), new Uri("https://tempuri.org", UriKind.Absolute), string.Empty, "My=Query", string.Empty),
                (new Uri("https://tempuri.org/?My=Query#TheHash", UriKind.Absolute), new Uri("https://tempuri.org", UriKind.Absolute), string.Empty, "My=Query", "TheHash")
            })
            {
                var actual = target.SplitLeaf(out string? actualLeaf, out string? actualQuery, out string? actualFragment);
                Assert.That(actual, Is.Not.Null);
                Assert.Multiple(() =>
                {
                    if (expectedResult.IsAbsoluteUri)
                    {
                        Assert.That(actual.IsAbsoluteUri, Is.True);
                        Assert.That(actual.AbsoluteUri, Is.EqualTo(expectedResult.AbsoluteUri));
                    }
                    else
                    {
                        Assert.That(actual.IsAbsoluteUri, Is.False);
                        Assert.That(actual.OriginalString, Is.EqualTo(expectedResult.OriginalString));
                    }
                    Assert.That(actualLeaf, Is.Not.Null);
                    Assert.That(actualLeaf, Is.EqualTo(expectedLeaf));
                    Assert.That(actualQuery, Is.Not.Null);
                    Assert.That(actualQuery, Is.EqualTo(expectedQuery));
                    Assert.That(actualFragment, Is.Not.Null);
                    Assert.That(actualFragment, Is.EqualTo(expectedFragment));
                });
            }
        }
        
        [Test]
        public void ToNormalizedTest()
        {
            Uri? target = null;
            Uri actual = target.ToNormalized();
            Assert.That(actual, Is.Not.Null);
            Assert.Multiple(() =>
            {
                Assert.That(actual.IsAbsoluteUri, Is.False);
                Assert.That(actual.OriginalString, Is.Empty);
            });
            foreach ((var uriString, var expected) in new (string UriString, string Expected)[] {
                (string.Empty, string.Empty),
                ("/", "/"),
                ("//", "/"),
                ("\\", "/"),
                ("\\\\", "/"),
                ("Test", "Test"),
                ("Test/", "Test"),
                ("Test///", "Test"),
                ("Test\\", "Test"),
                ("Test\\\\/", "Test"),
                ("/Test", "/Test"),
                ("/Test/", "/Test"),
                ("/Test///", "/Test"),
                ("/Test\\", "/Test"),
                ("/Test\\\\/", "/Test"),
                ("\\Test", "/Test"),
                ("\\Test/", "/Test"),
                ("\\Test///", "/Test"),
                ("\\Test\\", "/Test"),
                ("\\Test\\\\/", "/Test"),
                ("Test%20Data", "Test%20Data"),
                ("Test Data", "Test%20Data")
            }.SelectMany(a => new (string UriString, string Expected)[]
            {
                (a.UriString, a.Expected),
                ($"{a.UriString}?", a.Expected),
                ($"{a.UriString}?Key=My Value", $"{a.Expected}?Key=My%20Value"),
                ($"{a.UriString}#", a.Expected),
                ($"{a.UriString}#Hash", $"{a.Expected}#Hash"),
                ($"{a.UriString}?#Hash", $"{a.Expected}#Hash"),
                ($"{a.UriString}?#", a.Expected),
                ($"{a.UriString}?Key=My Value#", $"{a.Expected}?Key=My%20Value"),
                ($"{a.UriString}?Key=My Value#Hash", $"{a.Expected}?Key=My%20Value#Hash")
            }))
            {
                target = new(uriString, UriKind.Relative);
                actual = target.ToNormalized();
                Assert.That(actual, Is.Not.Null);
                Assert.Multiple(() =>
                {
                    Assert.That(actual.IsAbsoluteUri, Is.False);
                    Assert.That(actual.OriginalString, Is.EqualTo(expected));
                });
            }
            foreach ((var uriString, var expected) in new (string UriString, string Expected)[] {
                (string.Empty, string.Empty),
                ("http://tempuri.org/", "http://tempuri.org"),
                ("http://tempuri.org//", "http://tempuri.org"),
                ("C:\\", "file:///C:/"),
                ("C:\\\\", "file:///C:/"),
                ("http://tempuri.org/Test", "http://tempuri.org/Test"),
                ("http://tempuri.org/Test/", "http://tempuri.org/Test"),
                ("http://tempuri.org/Test///", "http://tempuri.org/Test"),
                ("http://tempuri.org/Test\\", "http://tempuri.org/Test"),
                ("http://tempuri.org/Test\\\\/", "http://tempuri.org/Test"),
                ("C:\\Test", "file:///C:/Test"),
                ("C:\\Test/", "file:///C:/Test"),
                ("C:\\Test///", "file:///C:/Test"),
                ("C:\\Test\\", "file:///C:/Test"),
                ("C:\\Test\\\\/", "file:///C:/Test"),
                ("http://tempuri.org/Test%20Data", "http://tempuri.org/Test%20Data"),
                ("http://tempuri.org/Test Data", "http://tempuri.org/Test%20Data")
            }.SelectMany(a => new (string UriString, string Expected)[]
            {
                (a.UriString, a.Expected),
                ($"{a.UriString}?", a.Expected),
                ($"{a.UriString}?Key=My Value", $"{a.Expected}?Key=My%20Value"),
                ($"{a.UriString}#", a.Expected),
                ($"{a.UriString}#Hash", $"{a.Expected}#Hash"),
                ($"{a.UriString}?#Hash", $"{a.Expected}#Hash"),
                ($"{a.UriString}?#", a.Expected),
                ($"{a.UriString}?Key=My Value#", $"{a.Expected}?Key=My%20Value"),
                ($"{a.UriString}?Key=My Value#Hash", $"{a.Expected}?Key=My%20Value#Hash")
            }))
            {
                target = new(uriString, UriKind.Relative);
                actual = target.ToNormalized();
                Assert.That(actual, Is.Not.Null);
                Assert.Multiple(() =>
                {
                    Assert.That(actual.IsAbsoluteUri, Is.True);
                    Assert.That(actual.AbsoluteUri, Is.EqualTo(expected));
                });
            }
        }
    }
}