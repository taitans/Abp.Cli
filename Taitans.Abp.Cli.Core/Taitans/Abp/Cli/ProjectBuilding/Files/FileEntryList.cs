using System.Collections.Generic;

namespace Taitans.Abp.Cli.ProjectBuilding.Files
{
    public class FileEntryList : List<FileEntry>
    {
        public FileEntryList(IEnumerable<FileEntry> entries)
            : base(entries)
        {
            
        }
    }
}