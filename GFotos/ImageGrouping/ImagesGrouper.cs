using System.Collections.Generic;
using GFotos.ViewModel;

namespace GFotos.ImageGrouping
{
    internal class ImagesGrouper
    {
        public static IEnumerable<ImagesGroup> GroupImages(IEnumerable<DirectoryRecord> chosenDirectories)
        {
            System.Threading.Thread.Sleep(2000);
            return new List<ImagesGroup>();
        }
    }
}