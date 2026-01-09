#region license and copyright
/*
 * The MIT License, Copyright (c) 2011-2026 Marcel Schneider
 * for details see License.txt
 */
#endregion

using System.Drawing;
using System.Drawing.Imaging;

namespace KataBarcode;

class ImageLoader : IImageLoader
{
    public Image LoadFromFile(string path)
    {
        return Image.FromFile(path);
    }

    public void Save(Image image, string path, ImageFormat format)
    {
        image.Save(path, format);
    }
}
