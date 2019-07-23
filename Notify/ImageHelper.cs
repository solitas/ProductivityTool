namespace ProductivityTool.Notify
{
    class ImageHelper
    {
    }
    #region FileToImageIconConverter
    public static class FileToImageIconConverter
    {

        public static System.Windows.Media.ImageSource Icon(string filePath)
        {
            System.Windows.Media.ImageSource icon = null;
            if (System.IO.File.Exists(filePath))
            {
                using (System.Drawing.Icon sysicon = System.Drawing.Icon.ExtractAssociatedIcon(filePath))
                {
                    icon = System.Windows.Interop.Imaging.CreateBitmapSourceFromHIcon(
                              sysicon.Handle,
                              System.Windows.Int32Rect.Empty,
                              System.Windows.Media.Imaging.BitmapSizeOptions.FromEmptyOptions());
                }
            }

            return icon;

        }
        
    }
    #endregion
}
