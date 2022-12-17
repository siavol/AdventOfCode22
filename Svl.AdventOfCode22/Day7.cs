using System.Text;
using System.Text.RegularExpressions;
using Xunit;

namespace Svl.AdventOfCode22;

public partial class Day7
{
    public static int SolveTaskPart1(Stream stream)
    {
        var fileSystem = FileSystemBuilder.FromStream(stream);
        var result = fileSystem.Root.Flatten()
            .OfType<Directory>()
            .Where(d => d.Size <= 100000)
            .Sum(d => d.Size);
        return result;
    }

    public static int SolveTaskPart2(Stream stream)
    {
        const int totalSpace = 70000000;
        const int needUnusedSpace = 30000000;
        
        var fileSystem = FileSystemBuilder.FromStream(stream);
        var unusedSpace = totalSpace - fileSystem.Root.Size;

        if (unusedSpace < needUnusedSpace)
        {
            var requredSpace = needUnusedSpace - unusedSpace;
            var dirToDelete = fileSystem.Root.Flatten()
                .OfType<Directory>()
                .Where(d => d.Size >= requredSpace)
                .OrderBy(d => d.Size)
                .First();
            return dirToDelete.Size;
        }
        else
        {
            return 0;
        }
    }

    public partial class FileSystemBuilder
    {
        private FileSystem fileSystem { get; } = new();

        public static FileSystem FromStream(Stream stream)
        {
            var fileSystemBuilder = new FileSystemBuilder(stream);
            return fileSystemBuilder.fileSystem;
        }

        private FileSystemBuilder(Stream stream)
        {
            using var streamReader = new StreamReader(stream, Encoding.UTF8);
            var nextCommand = streamReader.ReadLine();
            do
            {
                nextCommand = ProcessCommand(nextCommand, streamReader);
            } while (nextCommand != null);
        }

        private string ProcessCommand(string command, StreamReader streamReader)
        {
            if (!IsCommandString(command))
            {
                throw new ApplicationException("Command must start with $");
            }

            var cdMatch = CdCommandRegex().Match(command);
            if (cdMatch.Success)
            {
                var path = cdMatch.Groups["path"];
                fileSystem.ChangeCurrentDirectory(path.Value);
                return streamReader.EndOfStream ? null : streamReader.ReadLine();
            }
            else
            {
                var lsMatch = ListCommandRegex().Match(command);
                if (lsMatch.Success)
                {
                    var items = new List<IFileSystemItem>();
                    string line;
                    while (true)
                    {
                        line = streamReader.EndOfStream ? null : streamReader.ReadLine();
                        if (line == null || IsCommandString(line))
                        {
                            break;
                        }
                        items.Add(ParseFileSystemItem(line));
                    }

                    fileSystem.CurrentFolder.RegisterContent(items);
                    return line;
                }
            }

            throw new ApplicationException();
        }

        private static bool IsCommandString(string command)
        {
            return command is { Length: > 0 } && command[0] == '$';
        }

        private IFileSystemItem ParseFileSystemItem(string line)
        {
            var dirMatch = DirectoryItemRegex().Match(line);
            if (dirMatch.Success)
            {
                return new Directory(dirMatch.Groups["name"].Value, fileSystem.CurrentFolder);
            }

            var fileMatch = FileItemRegex().Match(line);
            if (fileMatch.Success)
            {
                var size = Int32.Parse(fileMatch.Groups["size"].Value);
                var name = fileMatch.Groups["name"].Value;
                return new File(name, size);
            }

            throw new ApplicationException();
        }

        [GeneratedRegex("\\$\\s+cd\\s+(?<path>.*)")]
        private static partial Regex CdCommandRegex();
        
        [GeneratedRegex("\\$\\s+ls")]
        private static partial Regex ListCommandRegex();

        [GeneratedRegex("dir\\s+(?<name>.*)")]
        private static partial Regex DirectoryItemRegex();
        
        [GeneratedRegex("(?<size>\\d+)\\s+(?<name>.*)")]
        private static partial Regex FileItemRegex();
    }

    public class FileSystem
    {
        public Directory? Root { get; }
        public Directory? CurrentFolder { get; private set; }

        public FileSystem()
        {
            Root = new("/", null);
            CurrentFolder = Root;
        }

        public void ChangeCurrentDirectory(string path)
        {
            if (path == "/")
            {
                CurrentFolder = Root;
            }
            else if (path == "..")
            {
                CurrentFolder = CurrentFolder.Parent;
            }
            else
            {
                if (CurrentFolder.Items == null)
                {
                    throw new ApplicationException("Current folder has no registered items");
                }

                CurrentFolder = CurrentFolder.Items
                    .OfType<Directory>()
                    .Single(d => d.Name == path);
            }
        }
    }

    public class Directory: IFileSystemItem
    {
        private readonly Directory? _parent;
        private IFileSystemItem[] _items;
        private int _size = -1;
        
        public string Name { get; }

        public Directory? Parent
        {
            get
            {
                if (_parent != null)
                {
                    return _parent;
                }

                throw new ApplicationException("Root folder has no parent");
            }
        }

        public int Size
        {
            get
            {
                if (_items == null)
                {
                    throw new ApplicationException("Can not get size when folder has no content");
                }

                if (_size < 0)
                {
                    _size = Items.Sum(i => i.Size);
                }
                return _size;
            }
        }

        public IReadOnlyList<IFileSystemItem> Items => _items.AsReadOnly();

        public Directory(string name, Directory? parent)
        {
            Name = name;
            _parent = parent;
        }

        public void RegisterContent(IEnumerable<IFileSystemItem> items)
        {
            if (_items != null)
            {
                throw new ApplicationException();
            }

            _items = items.ToArray();
        }

        public IEnumerable<IFileSystemItem> Flatten()
        {
            if (_items == null)
            {
                throw new ApplicationException("Can not enumerate directory when it has no content");
            }

            yield return this;
            foreach (var item in _items)
            {
                switch (item)
                {
                    case File:
                        yield return item;
                        break;
                    case Directory directory:
                    {
                        foreach (var child in directory.Flatten())
                        {
                            yield return child;
                        }
                        break;
                    }
                    default:
                        throw new ApplicationException();
                }
            }
        }
    }

    public class File : IFileSystemItem
    {
        public string Name { get; }
        public int Size { get; }

        public File(string name, int size)
        {
            Name = name;
            Size = size;
        }
    }

    public interface IFileSystemItem
    {
        string Name { get; }
        int Size { get; }
    }
}

public class Day7Tests {
    
    [Fact]
    public void Test_ListOneDirectory()
    {
        const string input = """
$ cd /
$ ls
dir a
14848514 b.txt
8504156 c.dat
dir d
$ cd a
$ ls
dir e
29116 f
2557 g
62596 h.lst
$ cd e
$ ls
584 i
$ cd ..
$ cd ..
$ cd d
$ ls
4060174 j
8033020 d.log
5626152 d.ext
7214296 k
""";
        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(input));
        var fs = Day7.FileSystemBuilder.FromStream(stream);
        Assert.True(fs switch
        {
            {
                CurrentFolder: { Name: "d" },
                Root:
                {
                    Name: "/",
                    Size: 48381165,
                    Items: [
                        Day7.Directory {
                            Name: "a",
                            Size: 94853,
                            Items: [
                                Day7.Directory {
                                    Name: "e",
                                    Size: 584,
                                    Items: [
                                        { Name: "i", Size: 584 }
                                    ]
                                },
                                { Name: "f", Size: 29116 },
                                { Name: "g", Size: 2557 },
                                { Name: "h.lst", Size: 62596 }
                            ]
                        },
                        { Name: "b.txt", Size: 14848514 },
                        { Name: "c.dat", Size: 8504156 },
                        Day7.Directory
                        {
                            Name: "d",
                            Size: 24933642,
                            Items: [
                                { Name: "j", Size: 4060174 },
                                { Name: "d.log", Size: 8033020 },
                                { Name: "d.ext", Size: 5626152 },
                                { Name: "k", Size: 7214296 }
                            ]
                        }
                    ] rooItems
                }
            } => true,
            _ => throw new ApplicationException($"File system does not match {fs}")
        });
    }

    [Fact]
    public void Test_Part1()
    {
        const string input = """
$ cd /
$ ls
dir a
14848514 b.txt
8504156 c.dat
dir d
$ cd a
$ ls
dir e
29116 f
2557 g
62596 h.lst
$ cd e
$ ls
584 i
$ cd ..
$ cd ..
$ cd d
$ ls
4060174 j
8033020 d.log
5626152 d.ext
7214296 k
""";
        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(input));
        var result = Day7.SolveTaskPart1(stream);
        Assert.Equal(95437, result);
    }

    [Fact]
    public void Test_Part2()
    {
        const string input = """
$ cd /
$ ls
dir a
14848514 b.txt
8504156 c.dat
dir d
$ cd a
$ ls
dir e
29116 f
2557 g
62596 h.lst
$ cd e
$ ls
584 i
$ cd ..
$ cd ..
$ cd d
$ ls
4060174 j
8033020 d.log
5626152 d.ext
7214296 k
""";
        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(input));
        var result = Day7.SolveTaskPart2(stream);
        Assert.Equal(24933642, result);
    }
}