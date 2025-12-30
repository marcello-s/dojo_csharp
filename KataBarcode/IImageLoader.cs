#region license and copyright
/*
 * The MIT License, Copyright (c) 2011-2025 Marcel Schneider
 * for details see License.txt
 */
#endregion

using System.Drawing;
using System.Drawing.Imaging;

namespace KataBarcode;

public interface IImageLoader
{
    Image LoadFromFile(string path);
    void Save(Image image, string path, ImageFormat format);
}
