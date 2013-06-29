using System;
using System.IO;

namespace GFotos.Framework
{
    class ComparableDirectoryInfo : IComparable<ComparableDirectoryInfo>
    {
        public DirectoryInfo Info { get; private set; }

        public ComparableDirectoryInfo(DirectoryInfo info)
        {
            Info = info;
        }

        public int CompareTo(ComparableDirectoryInfo other)
        {
            return String.Compare(
                Info.FullName.ToLower(),
                other.Info.FullName.ToLower(),
                StringComparison.Ordinal);
        }

        public override bool Equals(object obj)
        {
            var other = obj as ComparableDirectoryInfo;
            return other != null && CompareTo(other) == 0;
        }

        public override int GetHashCode()
        {
            return Info.FullName.ToLower().GetHashCode();
        }
    }
}
