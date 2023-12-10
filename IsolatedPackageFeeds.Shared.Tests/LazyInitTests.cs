using IsolatedPackageFeeds.Shared.LazyInit;

namespace IsolatedPackageFeeds.Shared.Tests
{
    public class LazyInitTests
    {
        [Test]
        public void LazyInitializerTest()
        {
            Helpers.InvocationCounter<string> counter = new("Value1", "Value2", "Value3");
            var expectedResult = counter.Current;
            LazyInitializer<string> target = new LazyDelegatedInitializer<string>(counter.Invoke);
            Assert.Multiple(() =>
            {
                Assert.That(target.WasInvoked, Is.False);
                Assert.That(counter.Count, Is.Zero);
            });
            var actualResult = target.GetResult();
            Assert.Multiple(() =>
            {
                Assert.That(actualResult, Is.Not.Null);
                Assert.That(actualResult, Is.EqualTo(expectedResult));
                Assert.That(target.WasInvoked, Is.True);
                Assert.That(counter.Count, Is.EqualTo(1));
            });
            var actualFault = target.GetFault();
            Assert.Multiple(() =>
            {
                Assert.That(actualFault, Is.Null);
                Assert.That(target.WasInvoked, Is.True);
                Assert.That(counter.Count, Is.EqualTo(1));
            });
            var wasInvoked = target.Invoke();
            Assert.Multiple(() =>
            {
                Assert.That(wasInvoked, Is.False);
                Assert.That(target.WasInvoked, Is.True);
                Assert.That(counter.Count, Is.EqualTo(1));
            });
            string expectedMessage = "Testing!!!";
            target = new LazyDelegatedInitializer<string>(() => throw new Exception(expectedMessage));
            Assert.Throws<Exception>(() => target.GetResult(), expectedMessage);
            actualFault = target.GetFault()!;
            Assert.Multiple(() =>
            {
                Assert.That(actualFault, Is.Not.Null);
                Assert.That(target.WasInvoked, Is.True);
                Assert.That(actualFault.Message, Is.EqualTo(expectedMessage));
                Assert.That(target.Invoke(), Is.False);
            });
            var f = target.GetFault()!;
            Assert.Multiple(() =>
            {
                Assert.That(f, Is.Not.Null);
                Assert.That(f, Is.SameAs(actualFault));
                Assert.That(target.WasInvoked, Is.True);
            });
        }

        [Test]
        public void LazyOptionalInitializerTest()
        {
            Helpers.InvocationCounter<string?> counter = new("Value1", null, "Value3");
            LazyOptionalInitializer<string> target = new LazyOptionalDelegatedInitializer<string>((out string? value) => (value = counter.Invoke()) is not null);
            Assert.Multiple(() =>
            {
                Assert.That(target.WasInvoked, Is.False);
                Assert.That(counter.Count, Is.Zero);
            });
            var expectedResult = counter.Current!;
            var returnValue = target.TryGetResult(out string? actualResult);
            Assert.Multiple(() =>
            {
                Assert.That(returnValue, Is.True);
                Assert.That(actualResult, Is.Not.Null);
                Assert.That(actualResult, Is.EqualTo(expectedResult));
                Assert.That(target.WasInvoked, Is.True);
                Assert.That(counter.Count, Is.EqualTo(1));
            });
            actualResult = target.GetResult();
            Assert.Multiple(() =>
            {
                Assert.That(actualResult, Is.Not.Null);
                Assert.That(actualResult, Is.EqualTo(expectedResult));
                Assert.That(target.WasInvoked, Is.True);
                Assert.That(counter.Count, Is.EqualTo(1));
            });
            var actualFault = target.GetFault();
            Assert.Multiple(() =>
            {
                Assert.That(actualFault, Is.Null);
                Assert.That(target.WasInvoked, Is.True);
                Assert.That(counter.Count, Is.EqualTo(1));
            });
            var wasInvoked = target.Invoke();
            Assert.Multiple(() =>
            {
                Assert.That(wasInvoked, Is.False);
                Assert.That(target.WasInvoked, Is.True);
                Assert.That(counter.Count, Is.EqualTo(1));
            });
            target = new LazyOptionalDelegatedInitializer<string>((out string? value) => (value = counter.Invoke()) is not null);
            returnValue = target.TryGetResult(out actualResult);
            Assert.Multiple(() =>
            {
                Assert.That(returnValue, Is.False);
                Assert.That(actualResult, Is.Null);
                Assert.That(target.WasInvoked, Is.True);
                Assert.That(counter.Count, Is.EqualTo(2));
            });
            actualResult = target.GetResult();
            Assert.Multiple(() =>
            {
                Assert.That(actualResult, Is.Null);
                Assert.That(target.WasInvoked, Is.True);
                Assert.That(counter.Count, Is.EqualTo(2));
            });
        }
    }
}