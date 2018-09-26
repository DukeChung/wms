using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NBK.ECService.WMS.Utility
{
    public class ImageThumbnail
    {
        public Image ResourceImage;
        private int ImageWidth;
        private int ImageHeight;
        public string ErrorMessage;

        public ImageThumbnail(string ImageFileName)
        {
            ResourceImage = Image.FromFile(ImageFileName);
            ErrorMessage = "";
        }

        public void DisImage()
        {
            ResourceImage.Dispose();
        }

        public bool ThumbnailCallback()
        {
            return false;
        }

        // 方法1，按大小
        /// <summary>
        /// 按大小缩放图片
        /// </summary>
        /// <param name="Width">缩放到的宽</param>
        /// <param name="Height">缩放到的高</param>
        /// <param name="targetFilePath">图片的名字</param>
        /// <returns>bool</returns>
        public bool ReducedImage(int Width, int Height, string targetFilePath)
        {
            try
            {
                Image ReducedImage;
                Image.GetThumbnailImageAbort callb = new Image.GetThumbnailImageAbort(ThumbnailCallback);
                ReducedImage = ResourceImage.GetThumbnailImage(Width, Height, callb, IntPtr.Zero);
                ReducedImage.Save(@targetFilePath, ImageFormat.Jpeg);
                ReducedImage.Dispose();
                return true;
            }
            catch (Exception e)
            {
                ErrorMessage = e.Message;
                return false;
            }
        }

        // 方法2，按百分比 缩小60% Percent为0.6 targetFilePath为目标路径
        /// <summary>
        /// 按百分比缩放
        /// </summary>
        /// <param name="Percent">小数：0.4表示百分之40</param>
        /// <param name="targetFilePath">图片的名称</param>
        /// <returns>bool</returns>
        public bool ReducedImage(double Percent, string targetFilePath)
        {
            try
            {
                Image ReducedImage;
                Image.GetThumbnailImageAbort callb = new Image.GetThumbnailImageAbort(ThumbnailCallback);
                ImageWidth = Convert.ToInt32(ResourceImage.Width * Percent);
                ImageHeight = (ResourceImage.Height) * ImageWidth / ResourceImage.Width; //等比例缩放
                ReducedImage = ResourceImage.GetThumbnailImage(ImageWidth, ImageHeight, callb, IntPtr.Zero);
                ReducedImage.Save(@targetFilePath, ImageFormat.Jpeg);
                ReducedImage.Dispose();
                return true;
            }
            catch (Exception e)
            {
                ErrorMessage = e.Message;
                return false;
            }
        }


        #region 资讯上传列表预览图(465*320)
        /// <summary>
        /// 资讯上传列表预览图(465*320)
        /// </summary>
        /// <param name="fromFile"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <returns></returns>
        public static Stream GetFixImage(Stream fromFile, int width, int height)
        {
            var Ratio = Convert.ToDecimal(height) / Convert.ToDecimal(width);
            MemoryStream stream = null;
            Image initImage = Image.FromStream(fromFile, true);
            var w = Convert.ToDecimal(initImage.Width);
            var h = Convert.ToDecimal(initImage.Height);

            if (h / w >= Ratio)
            {
                h = w * Ratio;
            }
            else if (h / w < Ratio)
            {
                w = h / Ratio;
            }

            Rectangle fromR = new Rectangle(0, 0, Convert.ToInt32(w), Convert.ToInt32(h));
            Rectangle toR = new Rectangle(0, 0, Convert.ToInt32(w), Convert.ToInt32(h));


            System.Drawing.Image pickedImage = new System.Drawing.Bitmap(Convert.ToInt32(w), Convert.ToInt32(h));
            System.Drawing.Graphics pickedG = System.Drawing.Graphics.FromImage(pickedImage);

            pickedG.DrawImage(initImage, toR, fromR, System.Drawing.GraphicsUnit.Pixel);

            //按模版大小生成最终图片
            System.Drawing.Image templateImage = new System.Drawing.Bitmap(Convert.ToInt32(w), Convert.ToInt32(h));
            System.Drawing.Graphics templateG = System.Drawing.Graphics.FromImage(templateImage);
            templateG.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.High;
            templateG.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            templateG.Clear(Color.White);
            templateG.DrawImage(pickedImage, new System.Drawing.Rectangle(0, 0, Convert.ToInt32(w), Convert.ToInt32(h)), new System.Drawing.Rectangle(0, 0, Convert.ToInt32(w), Convert.ToInt32(h)), System.Drawing.GraphicsUnit.Pixel);
            var objNewPic = (System.Drawing.Image)pickedImage.Clone();
            stream = new MemoryStream();
            try
            {
                objNewPic.Save(stream, ImageFormat.Png);
                stream.Position = 0;
            }
            catch (Exception ex)
            {
                throw;
            }
            finally
            {
                //objPic.Dispose();
                objNewPic.Dispose();
            }
            return stream;

        }
        #endregion


        #region 裁剪图片到140*140大小
        /// <summary>
        /// 裁剪图片到140*140大小
        /// </summary>
        /// <param name="fromFile"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <returns></returns>
        public static Stream GetFixImageOne(Stream fromFile, int width, int height)
        {
            var Ratio = Convert.ToDecimal(height) / Convert.ToDecimal(width);
            MemoryStream stream = null;
            Image initImage = Image.FromStream(fromFile, true);
            var w = width; // Convert.ToDecimal(initImage.Width);
            var h = height; // Convert.ToDecimal(initImage.Height);
                            //if (h / w >= Ratio)
                            //{
                            //    h = w * Ratio;
                            //}
                            //else if (h / w < Ratio)
                            //{
                            //    w = h / Ratio;
                            //}

            Rectangle fromR = new Rectangle(0, 0, Convert.ToInt32(w), Convert.ToInt32(h));
            Rectangle toR = new Rectangle(0, 0, Convert.ToInt32(w), Convert.ToInt32(h));


            System.Drawing.Image pickedImage = new System.Drawing.Bitmap(Convert.ToInt32(w), Convert.ToInt32(h));
            System.Drawing.Graphics pickedG = System.Drawing.Graphics.FromImage(pickedImage);

            pickedG.DrawImage(initImage, toR, fromR, System.Drawing.GraphicsUnit.Pixel);

            //按模版大小生成最终图片
            System.Drawing.Image templateImage = new System.Drawing.Bitmap(Convert.ToInt32(w), Convert.ToInt32(h));
            System.Drawing.Graphics templateG = System.Drawing.Graphics.FromImage(templateImage);
            templateG.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.High;
            templateG.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            templateG.Clear(Color.White);
            templateG.DrawImage(pickedImage, new System.Drawing.Rectangle(0, 0, Convert.ToInt32(w), Convert.ToInt32(h)), new System.Drawing.Rectangle(0, 0, Convert.ToInt32(w), Convert.ToInt32(h)), System.Drawing.GraphicsUnit.Pixel);
            var objNewPic = (System.Drawing.Image)pickedImage.Clone();
            stream = new MemoryStream();
            try
            {
                objNewPic.Save(stream, ImageFormat.Png);
                stream.Position = 0;
            }
            catch (Exception ex)
            {
                throw;
            }
            finally
            {
                //objPic.Dispose();
                objNewPic.Dispose();
            }
            return stream;

        }
        #endregion

        /// <summary>
        /// 按照最大最小尺寸处理图片
        /// </summary>
        /// <param name="fromFile"></param>
        /// <param name="maxWidth"></param>
        /// <param name="maxHeight"></param>
        public static Stream AutoResizeImage(Stream fromFile)
        {
            // 宽大于高 670 503
            // 高大于宽 531 800

            var maxWidth = 0;
            var maxHeight = 0;
            MemoryStream stream = null;
            Image initImage = Image.FromStream(fromFile, true);
            if (initImage.Width > initImage.Height)
            {
                maxWidth = 670;
                maxHeight = 530;
            }
            else if (initImage.Width < initImage.Height)
            {
                maxWidth = 531;
                maxHeight = 800;
            }
            else
            {
                maxWidth = 550;
                maxHeight = 550;
            }
            var w = Convert.ToDecimal(initImage.Width);
            var h = Convert.ToDecimal(initImage.Height);
            if (maxWidth < initImage.Width || maxHeight < initImage.Height)
            {
                var Ratio = 1m;

                var wRatio = Convert.ToDecimal(maxWidth) / w;
                var hRatio = Convert.ToDecimal(maxHeight) / h;
                if (maxWidth == 0 && maxHeight == 0)
                {
                    Ratio = 1;
                }
                else if (maxWidth == 0)
                {
                    //
                    if (hRatio < 1)
                        Ratio = hRatio;
                }
                else if (maxHeight == 0)
                {
                    if (wRatio < 1)
                        Ratio = wRatio;
                }
                else if (wRatio < 1 || hRatio < 1)
                {
                    Ratio = (wRatio <= hRatio ? wRatio : hRatio);
                }
                if (Ratio < 1)
                {
                    w = w * Ratio;
                    h = h * Ratio;
                }
            }
            var objPic = new System.Drawing.Bitmap(fromFile);
            var objNewPic = new System.Drawing.Bitmap(objPic, Convert.ToInt32(w), Convert.ToInt32(h));
            stream = new MemoryStream();
            try
            {
                objNewPic.Save(stream, ImageFormat.Jpeg);
                stream.Position = 0;
            }
            catch (Exception ex)
            {
                throw;
            }
            finally
            {
                objPic.Dispose();
                objNewPic.Dispose();
            }
            return stream;
        }



        /// <summary>
        /// 正方型裁剪
        /// 以图片中心为轴心，截取正方型，然后等比缩放
        /// 用于头像处理
        /// </summary>
        /// <remarks>吴剑 2012-08-08</remarks>
        /// <param name="fromFile">原图Stream对象</param>
        /// <param name="side">指定的边长（正方型）</param>
        /// <param name="quality">质量（范围0-100）</param>
        /// 

        public static MemoryStream CutForSquare(System.IO.Stream fromFile, int side, int quality)
        {

            //原始图片（获取原始图片创建对象，并使用流中嵌入的颜色管理信息）
            System.Drawing.Image initImage = System.Drawing.Image.FromStream(fromFile, true);



            byte[] img = null;
            MemoryStream stream = null;


            //原图宽高均小于模版，不作处理，直接保存
            if (initImage.Width <= side && initImage.Height <= side)
            {
                //initImage.Save(fileSaveUrl, System.Drawing.Imaging.ImageFormat.Jpeg);
            }
            else
            {
                //原始图片的宽、高
                int initWidth = initImage.Width;
                int initHeight = initImage.Height;

                //非正方型先裁剪为正方型
                if (initWidth != initHeight)
                {
                    //截图对象
                    System.Drawing.Image pickedImage = null;
                    System.Drawing.Graphics pickedG = null;

                    //宽大于高的横图
                    if (initWidth > initHeight)
                    {
                        //对象实例化
                        pickedImage = new System.Drawing.Bitmap(initHeight, initHeight);
                        pickedG = System.Drawing.Graphics.FromImage(pickedImage);
                        //设置质量
                        pickedG.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                        pickedG.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
                        //定位
                        Rectangle fromR = new Rectangle((initWidth - initHeight) / 2, 0, initHeight, initHeight);
                        Rectangle toR = new Rectangle(0, 0, initHeight, initHeight);
                        //画图
                        pickedG.DrawImage(initImage, toR, fromR, System.Drawing.GraphicsUnit.Pixel);
                        //重置宽
                        initWidth = initHeight;
                    }
                    //高大于宽的竖图
                    else
                    {
                        //对象实例化
                        pickedImage = new System.Drawing.Bitmap(initWidth, initWidth);
                        pickedG = System.Drawing.Graphics.FromImage(pickedImage);
                        //设置质量
                        pickedG.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                        pickedG.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
                        //定位
                        Rectangle fromR = new Rectangle(0, (initHeight - initWidth) / 2, initWidth, initWidth);
                        Rectangle toR = new Rectangle(0, 0, initWidth, initWidth);
                        //画图
                        pickedG.DrawImage(initImage, toR, fromR, System.Drawing.GraphicsUnit.Pixel);
                        //重置高
                        initHeight = initWidth;
                    }

                    //将截图对象赋给原图
                    initImage = (System.Drawing.Image)pickedImage.Clone();
                    //释放截图资源
                    pickedG.Dispose();
                    pickedImage.Dispose();
                }

                //缩略图对象
                System.Drawing.Image resultImage = new System.Drawing.Bitmap(side, side);
                System.Drawing.Graphics resultG = System.Drawing.Graphics.FromImage(resultImage);
                //设置质量
                resultG.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                resultG.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
                //用指定背景色清空画布
                resultG.Clear(Color.White);
                //绘制缩略图
                resultG.DrawImage(initImage, new System.Drawing.Rectangle(0, 0, side, side), new System.Drawing.Rectangle(0, 0, initWidth, initHeight), System.Drawing.GraphicsUnit.Pixel);

                //关键质量控制
                //获取系统编码类型数组,包含了jpeg,bmp,png,gif,tiff
                ImageCodecInfo[] icis = ImageCodecInfo.GetImageEncoders();
                ImageCodecInfo ici = null;
                foreach (ImageCodecInfo i in icis)
                {
                    if (i.MimeType == "image/jpeg" || i.MimeType == "image/bmp" || i.MimeType == "image/png" || i.MimeType == "image/gif")
                    {
                        ici = i;
                    }
                }
                EncoderParameters ep = new EncoderParameters(1);
                ep.Param[0] = new EncoderParameter(System.Drawing.Imaging.Encoder.Quality, (long)quality);

                try
                {
                    stream = new MemoryStream();
                    resultImage.Save(stream, ImageFormat.Png);
                    resultG.Dispose();
                    resultImage.Dispose();
                    stream.Position = 0;
                    //img = new byte[stream.Length];
                    //stream.Read(img, 0, Convert.ToInt32(stream.Length));

                }
                catch (Exception ex)
                {
                    throw;
                }


                //释放关键质量控制所用资源
                ep.Dispose();

                //释放缩略图资源


                //释放原始图片资源
                initImage.Dispose();
            }
            return stream;
        }
    }
}
