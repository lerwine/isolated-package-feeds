namespace IsolatedPackageFeeds.Shared.Tests;

public class TempStagingFolderTest
{
    [Test]
    public void TempStagingFolderConstructorTest()
    {
        var target = new TempStagingFolder();
        var directory = target.Directory;
        Assert.That(directory, Is.Not.Null);
        Assert.That(directory.Exists, Is.True);
        var rootFile = target.NewRandomFileInfo();
        var subdir = target.NewRandomDirectoryInfo();
        Assert.That(subdir, Is.Not.Null);
        var nestedFile = new FileInfo(Path.Combine(subdir.FullName, Guid.NewGuid().ToString("d")));
        using (var writer = nestedFile.CreateText())
        {
            writer.WriteLine("Test");
            writer.Flush();
        }
        nestedFile.Refresh();
        Assert.That(nestedFile.Exists, Is.True);
        rootFile.Refresh();
        Assert.That(rootFile.Exists, Is.True);
        target.Dispose();
        nestedFile.Refresh();
        Assert.That(nestedFile.Exists, Is.False);
        subdir.Refresh();
        Assert.That(subdir.Exists, Is.False);
        rootFile.Refresh();
        Assert.That(rootFile.Exists, Is.False);
        directory.Refresh();
        Assert.That(directory.Exists, Is.False);
        Assert.Throws<ObjectDisposedException>(() => directory = target.Directory);
    }
}