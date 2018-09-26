using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.UI.WebControls;

namespace NBK.ECService.WMS.Utility
{
    public class FileUploader
    {
        /// <summary>
        /// ftp地址
        /// </summary>
        private static string ftpAddress = ConfigurationManager.AppSettings["FtpUrl"].ToString();
        /// <summary>
        /// ftp展示地址
        /// </summary>
        public static string httpAddress = ConfigurationManager.AppSettings["FtpShowUrl"].ToString();
        /// <summary>
        /// ftp登陸名
        /// </summary>
        private static string userName = ConfigurationManager.AppSettings["FtpUserName"].ToString();

        /// <summary>
        /// ftp登陸密碼
        /// </summary>
        private static string password = ConfigurationManager.AppSettings["FtpPassword"].ToString();


        /// <summary>
        /// ftp地址
        /// </summary>
        public string FtpAddress
        {
            get
            {
                if (!ftpAddress.EndsWith("/"))
                {
                    return ftpAddress + "/";
                }
                return ftpAddress;
            }
            set
            {
                ftpAddress = value;
            }
        }

        public string HttpAddress
        {
            get
            {
                if (!httpAddress.EndsWith("/"))
                {
                    return httpAddress + "/";
                }
                return httpAddress;
            }
            set
            {
                httpAddress = value;
            }
        }

        /// <summary>
        /// ftp登陸名
        /// </summary>
        public string UserName
        {
            get
            {
                return userName;
            }
            set
            {
                userName = value;
            }

        }

        /// <summary>
        /// 登陸密碼
        /// </summary>
        public string Password
        {
            get
            {
                return password;
            }
            set
            {
                password = value;
            }

        }
        /// <summary>
        /// 损益图片目录 
        /// </summary>
        public const string Adjustment = "Adjustment";

        /// <summary>
        /// 商品外借图片目录 
        /// </summary>
        public const string SkuBorrow = "SkuBorrow";

        /// <summary>
        /// 上传方法
        /// </summary>
        /// <param name="modelName">模块名称</param>
        /// <param name="fs">上传的流</param>
        /// <param name="fileName">新文件名称</param>
        /// <param name="isImage"></param>
        /// <returns></returns>
        public static UploadResultInformation UploadFile(string modelName, Stream fs, string fileName, FtpFileType fileType)
        {
            string dicName = DateTime.Now.ToString("yyyyMMdd");
            return UploadFile(modelName, dicName, fs, fileName, fileType);
        }


        /// <summary>
        /// 上传方法
        /// </summary>
        /// <param name="modelName">模块名称</param>
        /// <param name="fs">上传的流</param>
        /// <param name="fileName">新文件名称</param>
        /// <param name="isImage"></param>
        /// <returns></returns>
        public static UploadResultInformation UploadFileMemberInfoPhoto(string modelName, Stream fs, string fileName, FtpFileType fileType)
        {
            string dicName = DateTime.Now.ToString("yyyyMMdd");
            UploadResultInformation result = new UploadResultInformation(); //上傳結果信息
            //检查文件类型
            result.FileTypeCorrect = CheckFileType(fileName, fileType);
            if (result.FileTypeCorrect == false)
            {
                return result;
            }

            fs.Position = 0; //從開始位置讀取文件
            var ftpPath = "/" + modelName;   //去掉时间
            result = FtpUpload(fileName, fs, ftpPath + "/"); //上傳文件
            result.FileTypeCorrect = true;
            fs.Close();
            return result;
        }

        public static UploadResultInformation UploadFileOther(string modelName, Stream fs, string fileName, FtpFileType fileType)
        {
            string dicName = DateTime.Now.ToString("yyyyMMdd");
            return UploadFileOther(modelName, dicName, fs, fileName, fileType);
        }

        /// <summary>
        /// 上传方法
        /// </summary>
        /// <param name="modelName">模块名称</param>
        /// <param name="fs">上传的流</param>
        /// <param name="fileName">新文件名称</param>
        /// <param name="isImage"></param>
        /// <returns></returns>       
        public static UploadResultInformation UploadFileOther(string modelName, string dicName, Stream fs, string fileName, FtpFileType fileType)
        {
            UploadResultInformation result = new UploadResultInformation(); //上傳結果信息
            //检查文件类型
            result.FileTypeCorrect = CheckFileType(fileName, fileType);
            if (result.FileTypeCorrect == false)
            {
                return result;
            }

            fs.Position = 0; //從開始位置讀取文件
            var ftpPath = "/" + modelName; //去掉时间
            result = FtpUpload(fileName, fs, ftpPath + "/"); //上傳文件
            result.FileTypeCorrect = true;
            return result;
        }



        /// <summary>
        /// 上传方法
        /// </summary>
        /// <param name="modelName">模块名称</param>  
        /// <param name="modelName">文件夹名</param>
        /// <param name="fs">上传的流</param>
        /// <param name="fileName">新文件名称</param>
        /// <param name="isImage"></param>
        /// <returns></returns>
        public static UploadResultInformation UploadFile(string modelName, string dicName, Stream fs, string fileName,
            FtpFileType fileType)
        {
            UploadResultInformation result = new UploadResultInformation(); //上傳結果信息
            //检查文件类型
            result.FileTypeCorrect = CheckFileType(fileName, fileType);
            if (result.FileTypeCorrect == false)
            {
                return result;
            }

            fs.Position = 0; //從開始位置讀取文件
            //  string ftpPath = CreateFolderAtFtp(modelName, dicName);
            var ftpPath = "/" + modelName; //去掉时间

            var ftpPathThumbnail = "/" + modelName + "/Thumbnail"; //去掉时间
            var f = ImageThumbnail.AutoResizeImage(fs);
            result = FtpUpload(fileName, f, ftpPath + "/"); //上傳文件
            result.FileTypeCorrect = true;
            fs.Close();
            return result;
        }

        #region 上传固定比例图片
        public static UploadResultInformation UploadFileOther(string modelName, Stream fs, string fileName, FtpFileType fileType, int width, int height)
        {
            string dicName = DateTime.Now.ToString("yyyyMMdd");
            UploadResultInformation result = new UploadResultInformation(); //上傳結果信息
            //检查文件类型
            result.FileTypeCorrect = CheckFileType(fileName, fileType);
            if (result.FileTypeCorrect == false)
            {
                return result;
            }

            fs.Position = 0; //從開始位置讀取文件
            //  string ftpPath = CreateFolderAtFtp(modelName, dicName);
            var ftpPath = "/" + modelName; //去掉时间

            var ftpPathThumbnail = "/" + modelName + "/Thumbnail"; //去掉时间
            var f = ImageThumbnail.GetFixImage(fs, width, height);//获取固定图片大小
            result = FtpUpload(fileName, f, ftpPath + "/"); //上傳文件
            result.FileTypeCorrect = true;
            fs.Close();
            return result;
        }
        #endregion


        #region 上传固定大小图片140*140
        /// <summary>
        /// 上传固定大小图片140*140
        /// </summary>
        /// <param name="modelName"></param>
        /// <param name="fs"></param>
        /// <param name="fileName"></param>
        /// <param name="fileType"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <returns></returns>
        public static UploadResultInformation UploadFileOtherTwo(string modelName, Stream fs, string fileName, FtpFileType fileType, int width, int height)
        {
            string dicName = DateTime.Now.ToString("yyyyMMdd");
            UploadResultInformation result = new UploadResultInformation(); //上傳結果信息
            //检查文件类型
            result.FileTypeCorrect = CheckFileType(fileName, fileType);
            if (result.FileTypeCorrect == false)
            {
                return result;
            }

            fs.Position = 0; //從開始位置讀取文件
            //  string ftpPath = CreateFolderAtFtp(modelName, dicName);
            var ftpPath = "/" + modelName; //去掉时间

            var ftpPathThumbnail = "/" + modelName + "/Thumbnail"; //去掉时间
            //var f = ImageThumbnail.GetFixImageOne(fs, width, height);//获取固定图片大小
            var f = ImageThumbnail.CutForSquare(fs, width, height);//获取固定图片大小
            result = FtpUpload(fileName, f, ftpPath + "/"); //上傳文件
            result.FileTypeCorrect = true;
            if (width != 140)
            {
                fs.Close();
            }
            //fs.Close();
            return result;
        }
        #endregion



        #region 
        public static UploadResultInformation UploadFilProduct(string modelName, Stream fs, string fileName, FtpFileType fileType, int width)
        {

            string dicName = DateTime.Now.ToString("yyyyMMdd");
            UploadResultInformation result = new UploadResultInformation(); //上傳結果信息
                                                                            //检查文件类型
            result.FileTypeCorrect = CheckFileType(fileName, fileType);
            if (result.FileTypeCorrect == false)
            {
                return result;
            }

            fs.Position = 0; //從開始位置讀取文件
                             //  string ftpPath = CreateFolderAtFtp(modelName, dicName);
            var ftpPath = "/" + modelName; //去掉时间

            var ftpPathThumbnail = "/" + modelName + "/Thumbnail"; //去掉时间
            var f = ImageThumbnail.CutForSquare(fs, width, 50);//获取固定图片大小
            result = FtpUpload(fileName, f, ftpPath + "/"); //上傳文件
            result.FileTypeCorrect = true;
            fs.Close();
            return result;


        }
        #endregion


        public static UploadResultInformation ThumbnailFtpUpload(string fileName, Stream fs, string ftpPath, int width, int height)
        {
            UploadResultInformation result = null;
            //检查目录是否存在，不存在创建  
            FtpCheckDirectoryExist(ftpPath);

            var thumbnailfs = ImageThumbnail.GetFixImage(fs, width, height);//获取固定图片大小

            long length = thumbnailfs.Length;
            var Url = ftpAddress + ftpPath + fileName;
            FtpWebRequest req = (FtpWebRequest)WebRequest.Create(Url);
            req.Credentials = new NetworkCredential(userName, password);
            req.Method = WebRequestMethods.Ftp.UploadFile;
            req.ContentLength = length;
            req.Timeout = 10 * 1000;
            try
            {
                Stream stream = req.GetRequestStream();
                int BufferLength = Convert.ToInt32(thumbnailfs.Length); //2K     
                byte[] b = new byte[BufferLength];
                int i;
                while ((i = thumbnailfs.Read(b, 0, BufferLength)) > 0)
                {
                    stream.Write(b, 0, i);
                }
                stream.Close();
                stream.Dispose();

                result = new UploadResultInformation();
                result.FTPPath = ftpAddress + ftpPath;
                result.FilePathName = ftpPath + fileName;
                result.ServerAdress = ftpAddress;
                result.FullPathName = result.ServerAdress + result.FilePathName;
                result.HttpFullPathName = Replace(result.FullPathName);
                result.FileName = fileName;
                return result;
            }
            catch (Exception e)
            {
                FtpUploadAgain(fileName, thumbnailfs, ftpPath);
                result = new UploadResultInformation();
                result.FTPPath = ftpAddress + ftpPath;
                result.FilePathName = ftpPath + fileName;
                result.ServerAdress = ftpAddress;
                result.FullPathName = result.ServerAdress + result.FilePathName;
                result.HttpFullPathName = Replace(result.FullPathName);
                result.FileName = fileName;
            }
            finally
            {
                thumbnailfs.Close();
                req.Abort();
            }
            req.Abort();
            return result;
        }

        /// <summary>
        /// 上传缩略图
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="fs"></param>
        /// <param name="ftpPath"></param>
        /// <returns></returns>
        public static UploadResultInformation ThumbnailFtpUpload(string fileName, Stream fs, string ftpPath)
        {
            UploadResultInformation result = null;
            //检查目录是否存在，不存在创建  
            FtpCheckDirectoryExist(ftpPath);

            var thumbnailfs = ImageThumbnail.CutForSquare(fs, 100, 1);

            long length = thumbnailfs.Length;
            var Url = ftpAddress + ftpPath + fileName;
            FtpWebRequest req = (FtpWebRequest)WebRequest.Create(Url);
            req.Credentials = new NetworkCredential(userName, password);
            req.Method = WebRequestMethods.Ftp.UploadFile;
            req.ContentLength = length;
            req.Timeout = 10 * 1000;
            try
            {
                Stream stream = req.GetRequestStream();
                int BufferLength = Convert.ToInt32(thumbnailfs.Length); //2K     
                byte[] b = new byte[BufferLength];
                int i;
                while ((i = thumbnailfs.Read(b, 0, BufferLength)) > 0)
                {
                    stream.Write(b, 0, i);
                }
                stream.Close();
                stream.Dispose();

                result = new UploadResultInformation();
                result.FTPPath = ftpAddress + ftpPath;
                result.FilePathName = ftpPath + fileName;
                result.ServerAdress = ftpAddress;
                result.FullPathName = result.ServerAdress + result.FilePathName;
                result.HttpFullPathName = Replace(result.FullPathName);
                result.FileName = fileName;
                return result;
            }
            catch (Exception e)
            {
                FtpUploadAgain(fileName, thumbnailfs, ftpPath);
                result = new UploadResultInformation();
                result.FTPPath = ftpAddress + ftpPath;
                result.FilePathName = ftpPath + fileName;
                result.ServerAdress = ftpAddress;
                result.FullPathName = result.ServerAdress + result.FilePathName;
                result.HttpFullPathName = Replace(result.FullPathName);
                result.FileName = fileName;
            }
            finally
            {
                thumbnailfs.Close();
                req.Abort();
            }
            req.Abort();
            return result;
        }

        //上传文件  
        public static UploadResultInformation FtpUpload(string fileName, Stream fs, string ftpPath)
        {
            UploadResultInformation result = new UploadResultInformation();
            //检查目录是否存在，不存在创建  
            FtpCheckDirectoryExist(ftpPath);
            long length = fs.Length;
            var Url = ftpAddress + ftpPath + fileName;
            FtpWebRequest req = (FtpWebRequest)WebRequest.Create(Url);
            req.Credentials = new NetworkCredential(userName, password);
            req.Method = WebRequestMethods.Ftp.UploadFile;
            req.ContentLength = length;
            req.Timeout = 10 * 1000;
            try
            {
                Stream stream = req.GetRequestStream();
                int BufferLength = Convert.ToInt32(fs.Length); //2K     
                byte[] b = new byte[BufferLength];
                int i;
                while ((i = fs.Read(b, 0, BufferLength)) > 0)
                {
                    stream.Write(b, 0, i);
                }
                stream.Close();
                stream.Dispose();


                result.FTPPath = ftpAddress + ftpPath;
                result.FilePathName = ftpPath + fileName;
                result.ServerAdress = ftpAddress;
                result.FullPathName = result.ServerAdress + result.FilePathName;
                result.HttpFullPathName = Replace(result.FullPathName);
                result.FileName = fileName;
                return result;
            }
            catch (Exception e)
            {
                FtpUploadAgain(fileName, fs, ftpPath);
                result.FTPPath = ftpAddress + ftpPath;
                result.FilePathName = ftpPath + fileName;
                result.ServerAdress = ftpAddress;
                result.FullPathName = result.ServerAdress + result.FilePathName;
                result.HttpFullPathName = Replace(result.FullPathName);
                result.FileName = fileName;
            }
            finally
            {
                // fs.Close();
                req.Abort();
            }
            req.Abort();
            return result;
        }

        //上传文件  
        public static UploadResultInformation FtpUploadAgain(string fileName, Stream fs, string ftpPath)
        {
            UploadResultInformation result = new UploadResultInformation();
            //检查目录是否存在，不存在创建  
            FtpCheckDirectoryExist(ftpPath);
            long length = fs.Length;
            var Url = ftpAddress + ftpPath + fileName;
            FtpWebRequest req = (FtpWebRequest)WebRequest.Create(Url);
            req.Credentials = new NetworkCredential(userName, password);
            req.Method = WebRequestMethods.Ftp.UploadFile;
            req.ContentLength = length;
            req.Timeout = 10 * 1000;
            try
            {
                System.IO.Stream stream = req.GetRequestStream();
                int BufferLength = 2048; //2K     
                byte[] b = new byte[BufferLength];
                int i;
                while ((i = fs.Read(b, 0, BufferLength)) > 0)
                {
                    stream.Write(b, 0, i);
                }
                stream.Close();
                stream.Dispose();


                result.FTPPath = ftpAddress + ftpPath;
                result.FilePathName = ftpPath + fileName;
                result.ServerAdress = ftpAddress;
                result.FullPathName = result.ServerAdress + result.FilePathName;
                result.HttpFullPathName = Replace(result.FullPathName);
                result.FileName = fileName;
                return result;
            }
            catch (Exception e)
            {
                FtpUploadAgain(fileName, fs, ftpPath);
                Url = "";
            }
            finally
            {
                // fs.Close();
                req.Abort();
            }
            req.Abort();
            return result;
        }


        //判断文件的目录是否存,不存则创建  
        public static void FtpCheckDirectoryExist(string destFilePath)
        {
            string fullDir = FtpParseDirectory(destFilePath);
            string[] dirs = fullDir.Split('/');
            string curDir = "/";
            for (int i = 0; i < dirs.Length; i++)
            {
                string dir = dirs[i];
                //如果是以/开始的路径,第一个为空    
                if (dir != null && dir.Length > 0)
                {
                    try
                    {
                        curDir += dir + "/";
                        FtpMakeDir(curDir);
                    }
                    catch (Exception)
                    {
                    }
                }
            }
        }

        public static string FtpParseDirectory(string destFilePath)
        {
            return destFilePath.Substring(0, destFilePath.LastIndexOf("/"));
        }

        //创建目录  
        public static Boolean FtpMakeDir(string localFile)
        {
            FtpWebRequest req = (FtpWebRequest)WebRequest.Create(ftpAddress + localFile);
            req.Credentials = new NetworkCredential(userName, password);
            req.Method = WebRequestMethods.Ftp.MakeDirectory;
            try
            {
                FtpWebResponse response = (FtpWebResponse)req.GetResponse();
                response.Close();
            }
            catch (Exception)
            {
                req.Abort();
                return false;
            }
            req.Abort();
            return true;
        }


        /// <summary>
        /// 上传方法
        /// </summary>
        /// <param name="modelName">模块名称</param>
        /// <param name="fs">上传的流</param>
        /// <param name="fileName">新文件名称</param>
        /// <param name="isImage"></param>
        /// <returns></returns>
        public static UploadResultInformation UploadFileWithProcess(string modelName, Stream fs, string fileName, bool isImage, FileUploader uploader, FtpFileType fileType)
        {
            string dicName = DateTime.Now.ToString("yyyyMMdd");
            return UploadFileWithProcess(modelName, dicName, fs, fileName, isImage, uploader, fileType);

        }

        /// <summary>
        /// 上传方法
        /// </summary>
        /// <param name="modelName">模块名称</param>
        /// <param name="modelName">文件夹名</param>
        /// <param name="fs">上传的流</param>
        /// <param name="fileName">新文件名称</param>
        /// <param name="isImage"></param>
        /// <returns></returns>
        public static UploadResultInformation UploadFileWithProcess(string modelName, string dicName, Stream fs, string fileName, bool isImage, FileUploader uploader, FtpFileType fileType = FtpFileType.All)
        {


            UploadResultInformation result = new UploadResultInformation();         //上傳結果信息
            //检查文件类型
            result.FileTypeCorrect = CheckFileType(fileName, fileType);
            if (result.FileTypeCorrect == false)
            {
                return result;
            }

            //上傳結果信息
            fs.Position = 0;                                    //從開始位置讀取文件
            string ftpPath = CreateFolderAtFtp(modelName, dicName);

            result = UploadToFtpServerWithProcess(fileName, fs, ftpPath + "/", uploader);  //上傳文件
            result.FileTypeCorrect = true;
            return result;
        }

        /// <summary>
        /// 获取图片尺寸
        /// </summary>
        /// <param name="fs"></param>
        /// <returns>宽，高像素</returns>
        public static void GetImgSize(Stream fs, out int width, out int height)
        {
            width = 0;
            height = 0;
            System.Drawing.Image image = System.Drawing.Image.FromStream(fs);
            if (image != null)
            {
                width = image.Width;
                height = image.Height;
            }

            fs.Position = 0;
        }

        /// <summary>
        /// 檢測圖片的長和寬是否超過限制
        /// </summary>
        /// <param name="maxLength"></param>
        /// <param name="maxWidth"></param>
        /// <param name="width"></param>
        /// <param name="length"></param>
        private static bool IsOverImageSize(int maxLength, int length, int maxWidth, int width)
        {
            return (length > maxLength || width > maxWidth) ? true : false;
        }

        /// <summary>
        /// 在FTP創建一個新文件夾
        /// </summary>
        /// <param name="root">要在那个路径下创建文件夹</param>
        /// <param name="DicLayer3"></param>
        /// <returns>创建成功的ftp上的全路径</returns>
        private static string CreateDirectoryAtFtp(string DicLayer1, string DicLayer2, string DicLayer3)
        {

            try
            {
                //在ftp上的路径
                string ftpPath = DicLayer1;
                if (!string.IsNullOrEmpty(DicLayer2))
                {
                    ftpPath += "/" + DicLayer2;
                }
                if (!string.IsNullOrEmpty(DicLayer3))
                {
                    ftpPath += "/" + DicLayer3;
                }
                Uri uri = new Uri(ftpAddress + ftpPath);
                FtpWebRequest listRequest = (FtpWebRequest)WebRequest.Create(uri);
                if (!IsDirectoryExist(uri.ToString()))
                {
                    listRequest.Method = WebRequestMethods.Ftp.MakeDirectory;
                }
                else
                {
                    CreateFullDirectoryAtFtp(uri.ToString());
                    listRequest.Method = WebRequestMethods.Ftp.ListDirectory;
                }
                listRequest.Credentials = new NetworkCredential(userName, password);
                listRequest.KeepAlive = false;                                  //執行一個命令后關閉連接
                FtpWebResponse listResponse = (FtpWebResponse)listRequest.GetResponse();

                string fullPath = ftpAddress + ftpPath + "/";
                Stream write = GetWriteStream(fullPath + "ftpPath.ini");  //在ftp上創建文件
                byte[] context = System.Text.Encoding.Default.GetBytes("ftpPath=" + ftpPath);
                write.Write(context, 0, context.Length);
                write.Close();
                return ftpPath;    // 返回創建目錄路徑
            }
            catch (Exception ex)
            {
                //LogHelper.GetInstance().WriteMessage("创建文件夹失败" + ex.Message);
                return String.Empty;
            }
        }

        /// <summary>
        /// 在FTP上创建文件夹路径 返回  luwenlong/product/0000001
        /// </summary>
        /// <param name="userAccount"></param>
        /// <param name="modelName"></param>
        /// <param name="dicName"></param>
        /// <returns></returns>
        public static string CreateFolderAtFtp(string modelName, string dicName)
        {
            string ftpPath = string.Empty;
            if (!string.IsNullOrEmpty(modelName))
            {
                //检测是否有该企业的模块文件夹
                bool isExist = IsDirectoryExist(ftpAddress + modelName + "/");
                if (!isExist)
                {
                    string dic = CreateDirectoryAtFtp(modelName, "", "");
                    if (string.IsNullOrEmpty(dic))
                    {
                        throw new Exception("创建文件夹失败");
                    }
                }
                ftpPath += "/" + modelName;
            }

            if (!string.IsNullOrEmpty(dicName))
            {

                //检测是否有该企业模块下的子文件夹
                bool isExist = IsDirectoryExist(ftpAddress + modelName + "/" + dicName + "/");
                if (!isExist)
                {
                    string dic = CreateDirectoryAtFtp(modelName, dicName, "");
                    if (string.IsNullOrEmpty(dic))
                    {
                        throw new Exception("创建文件夹失败");
                    }
                }
                ftpPath += "/" + dicName;
            }

            return ftpPath;
        }

        /// <summary>
        /// 在ftp上創建文件夾(若目錄不存在則依序創建)。
        /// </summary>
        /// <param name="directoryName"></param>
        public static void CreateFullDirectoryAtFtp(string directoryPath)
        {
            Uri uriDir = new Uri(directoryPath);
            directoryPath = uriDir.AbsolutePath;
            directoryPath = directoryPath.Replace(@"\", "/");
            directoryPath = directoryPath.Replace("//", "/");
            string[] aryDirctoryName = directoryPath.Split('/');
            string realPath = "";
            realPath = ftpAddress;
            for (int i = 0; i < aryDirctoryName.Length; i++)
            {
                if (aryDirctoryName[i] != String.Empty)
                {
                    realPath = realPath + "/" + aryDirctoryName[i];
                    if (!IsDirectoryExist(realPath))
                    {
                        CreateDirectoryAtFtp(realPath);
                    }

                }
            }
        }

        /// <summary>
        /// 在ftp上創建文件夾，用於對zip文檔得解壓。
        /// </summary>
        /// <param name="directoryName"></param>
        public static void CreateDirectoryAtFtp(string directoryName)
        {
            try
            {
                Uri uri = new Uri(directoryName);
                FtpWebRequest listRequest = (FtpWebRequest)WebRequest.Create(uri);
                listRequest.Method = WebRequestMethods.Ftp.MakeDirectory;
                listRequest.Credentials = new NetworkCredential(userName, password);
                listRequest.KeepAlive = false;                                  //執行一個命令后關閉連接
                FtpWebResponse listResponse = (FtpWebResponse)listRequest.GetResponse();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fileFullName">FTP全路径</param>
        /// <returns></returns>
        public static Stream GetWriteStream(string fileFullName)
        {
            FtpWebRequest uploadRequest = (FtpWebRequest)WebRequest.Create(new Uri(fileFullName));
            uploadRequest.Method = WebRequestMethods.Ftp.UploadFile;
            uploadRequest.Credentials = new NetworkCredential(userName, password);
            uploadRequest.KeepAlive = false;    //執行一個命令后關閉連接.
            uploadRequest.UseBinary = true;
            return uploadRequest.GetRequestStream();
        }

        public static Stream GetReadStream(string fileFullName)
        {
            Uri uriDir = new Uri(fileFullName);
            FtpWebRequest listRequest = (FtpWebRequest)WebRequest.Create(uriDir);
            listRequest.Method = WebRequestMethods.Ftp.DownloadFile;
            listRequest.Credentials = new NetworkCredential(userName, password);
            listRequest.KeepAlive = false;  //執行一個命令后關閉連接
            FtpWebResponse listResponse = (FtpWebResponse)listRequest.GetResponse();
            return listResponse.GetResponseStream();
        }

        /// <summary>
        /// 判斷指定得路徑是否存在于ftp上
        /// </summary>
        /// <param name="fileFullPath"></param> 
        public static bool IsDirectoryExist(string fullDirectory)
        {
            if (!fullDirectory.EndsWith("/"))
                fullDirectory += "/";
            bool result = false;
            //執行ftp命令 活動目錄下所有文件列表
            Uri uriDir = new Uri(fullDirectory);
            FtpWebRequest listRequest = (FtpWebRequest)WebRequest.Create(uriDir);
            listRequest.Method = WebRequestMethods.Ftp.ListDirectory;
            listRequest.Credentials = new NetworkCredential(userName, password);
            listRequest.KeepAlive = false;  //執行一個命令后關閉連接
            FtpWebResponse listResponse = null;

            try
            {
                listResponse = (FtpWebResponse)listRequest.GetResponse();
                result = true;
            }
            catch
            {
                result = false;
            }
            finally
            {
                if (listResponse != null)
                {
                    listResponse.Close();
                }

            }

            return result;
        }

        /// <summary>
        /// 上傳文檔到ftp
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="fs"></param>
        /// <param name="moduleName"></param>
        public static UploadResultInformation UploadToFtpServer(string fileName, Stream fs, string ftpPath)
        {
            UploadResultInformation result = new UploadResultInformation();
            FtpWebRequest uploadRequest = (FtpWebRequest)WebRequest.Create(new Uri(ftpAddress + ftpPath + fileName));
            uploadRequest.Method = WebRequestMethods.Ftp.UploadFile;
            uploadRequest.Credentials = new NetworkCredential(userName, password);
            uploadRequest.KeepAlive = false;    //執行一個命令后關閉連接.
            uploadRequest.UseBinary = true;
            Stream requestStream = uploadRequest.GetRequestStream();

            byte[] buffer = new byte[1024];
            int bytesRead;
            while (true)
            {
                bytesRead = fs.Read(buffer, 0, buffer.Length);
                if (bytesRead == 0)
                {
                    break;
                }
                requestStream.Write(buffer, 0, bytesRead);
            }

            requestStream.Close();      //關閉流
            result.FTPPath = ftpAddress + ftpPath;
            result.FilePathName = ftpPath + fileName;
            result.ServerAdress = ftpAddress;
            result.FullPathName = result.ServerAdress + result.FilePathName;
            result.HttpFullPathName = Replace(result.FullPathName);
            result.FileName = fileName;
            return result;
        }


        /// <summary>
        /// 上傳文檔到ftp
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="fs"></param>
        /// <param name="moduleName"></param>
        public static UploadResultInformation UploadToFtpServerWithProcess(string fileName, Stream fs, string ftpPath, FileUploader uploader)
        {
            long total = fs.Length;//该成员主要记录文件的总字节数,注意这里使用长整型,是为了突破只能传输2G左右的文件的限制
            long finished = 0;//该成员主要记录已经传输完成的字节数,注意这里使用长整型,是为了突破只能传输2G左右的文件的限制
            double speed = 0;//记录传输的速率
            DateTime startTime = DateTime.Now;

            UploadResultInformation result = new UploadResultInformation();
            FtpWebRequest uploadRequest = (FtpWebRequest)WebRequest.Create(new Uri(ftpAddress + ftpPath + fileName));
            uploadRequest.Method = WebRequestMethods.Ftp.UploadFile;
            uploadRequest.Credentials = new NetworkCredential(userName, password);
            uploadRequest.KeepAlive = false;    //執行一個命令后關閉連接.
            uploadRequest.UseBinary = true;
            Stream requestStream = uploadRequest.GetRequestStream();
            byte[] buffer = new byte[1024];
            int bytesRead;

            while ((bytesRead = fs.Read(buffer, 0, buffer.Length)) > 0)//循环从本地数据流中读取数据到缓冲区
            {
                //Console.WriteLine(startTime.ToString());
                requestStream.Write(buffer, 0, bytesRead); //将缓冲区的数据发送到FTP服务器

                DateTime endTime = DateTime.Now;//每次发送数据的结束时间

                TimeSpan ts = endTime - startTime;//计算每次发送数据的时间间隔

                finished += bytesRead;//计算完成的字节数.

            }

            requestStream.Close();      //關閉流
            result.FTPPath = ftpAddress + ftpPath;
            result.FilePathName = ftpPath + fileName;
            result.ServerAdress = ftpAddress;
            result.FullPathName = result.ServerAdress + result.FilePathName;
            result.HttpFullPathName = Replace(result.FullPathName);
            result.FileName = fileName;
            return result;
        }

        /// <summary>
        /// 判斷文件類型是否為圖片
        /// </summary>
        /// <param name="fileUpload"></param>
        /// <returns></returns>
        public static bool IsImage(FileUpload fileUpload)
        {
            return (fileUpload.PostedFile.ContentType.ToLower().IndexOf("image") != -1) ? true : false;
        }

        /// <summary>
        /// 判断上传文件是否为压缩文件
        /// </summary>
        /// <param name="fileUpload"></param>
        /// <returns></returns>
        public static bool IsZip(FileUpload fileUpload)
        {
            bool fileOk = false;
            string fileExtension = System.IO.Path.GetExtension(fileUpload.FileName).ToLower();
            string[] allowExtension = { ".zip" };
            //对上传的文件的类型进行一个个匹对
            for (int i = 0; i < allowExtension.Length; i++)
            {
                if (fileExtension == allowExtension[i])
                {
                    fileOk = true;
                    break;
                }
            }
            return fileOk;
        }

        /// <summary>
        /// 在顯示圖片時，使用http協議來替換ftp
        /// </summary>
        /// <param name="ftpUrl"></param>
        /// <returns></returns>
        public static string Replace(string ftpUrl)
        {
            string http = httpAddress;
            string ftp = ftpAddress;
            if (!string.IsNullOrEmpty(ftpUrl))
            {
                ftpUrl = ftpUrl.ToLower();
                if (ftpUrl.Contains(ftp))
                {
                    ftpUrl = ftpUrl.Replace(ftp, http);
                }
                return ftpUrl = HttpContext.Current.Server.HtmlDecode(ftpUrl);
            }
            else
            {
                return string.Empty;
            }
        }

        /// <summary>
        /// 使用ftp協議來替換http,用於文件刪除、下載
        /// </summary>
        /// <param name="ftpUrl"></param>
        /// <returns></returns>
        public static string ReplaceHttpToFtp(string httpUrl)
        {
            if (!string.IsNullOrEmpty(httpUrl))
            {
                httpUrl = httpUrl.ToLower();
                string http = HttpContext.Current.Application["httpAddress"].ToString().ToLower();
                string ftp = HttpContext.Current.Application["address"].ToString().ToLower();
                httpUrl = httpUrl.Replace(http, ftp);
                return httpUrl = HttpContext.Current.Server.HtmlDecode(httpUrl);
            }
            else
            {
                return string.Empty;
            }
        }

        /// <summary>
        /// 從ftp刪除文檔
        /// </summary>
        /// <param name="filePathname"></param>
        public static void DeleteFileFromFtp(string filePathName)
        {
            if (IsFileExist(filePathName))
            {
                try
                {
                    Uri uri = new Uri(ReplaceHttpToFtp(filePathName));
                    FtpWebRequest listRequest = (FtpWebRequest)WebRequest.Create(uri);
                    listRequest.Method = WebRequestMethods.Ftp.DeleteFile;
                    listRequest.Credentials = new NetworkCredential(userName, password);
                    listRequest.KeepAlive = false;  //執行一個命令后關閉連接
                    FtpWebResponse listResponse = (FtpWebResponse)listRequest.GetResponse();
                }
                catch (Exception ex)
                {
                    //LogHelper.GetInstance().WriteMessage("删除文件失败" + ex.Message);
                }
            }
        }
        /// <summary>
        /// 從ftp刪除文檔
        /// </summary>
        /// <param name="filePathname">原始文件</param>
        /// <param name="rule">配置规则</param>
        public static void DeleteFileFromFtp(string filePathName, string rule)
        {
            string sourcImg = filePathName;
            //获得规则
            for (int i = 0; i < (rule.Split(';')).Length; i++)
            {
                string param = rule.Split(';')[i];
                string name = param.Split(',')[2];
                string str = string.Empty;
                if (filePathName.ToString().ToLower().Contains("_source"))
                {
                    str = filePathName.ToString().ToLower().Replace("_source", name);
                }
                else
                {
                    int index = filePathName.ToString().IndexOf('.', 22);
                    str = filePathName.ToString().Insert(index, name);
                }
                if (IsFileExist(str))
                {
                    try
                    {
                        Uri uri = new Uri(ReplaceHttpToFtp(str));
                        FtpWebRequest listRequest = (FtpWebRequest)WebRequest.Create(uri);
                        listRequest.Method = WebRequestMethods.Ftp.DeleteFile;
                        listRequest.Credentials = new NetworkCredential(userName, password);
                        listRequest.KeepAlive = false;  //執行一個命令后關閉連接
                        FtpWebResponse listResponse = (FtpWebResponse)listRequest.GetResponse();
                    }
                    catch (Exception ex)
                    {
                        //LogHelper.GetInstance().WriteMessage("删除文件失败" + ex.Message);
                    }
                }
            }
            if (IsFileExist(sourcImg))
            {
                try
                {
                    Uri uri = new Uri(ReplaceHttpToFtp(sourcImg));
                    FtpWebRequest listRequest = (FtpWebRequest)WebRequest.Create(uri);
                    listRequest.Method = WebRequestMethods.Ftp.DeleteFile;
                    listRequest.Credentials = new NetworkCredential(userName, password);
                    listRequest.KeepAlive = false;  //執行一個命令后關閉連接
                    FtpWebResponse listResponse = (FtpWebResponse)listRequest.GetResponse();
                }
                catch (Exception ex)
                {
                    //LogHelper.GetInstance().WriteMessage("删除文件失败" + ex.Message);
                }
            }


        }
        /// <summary>
        /// 删除FTP上的一个目录 必须是空目录
        /// </summary>
        /// <param name="fullDicPath"></param>
        public static void DeleteDicFromFtp(string fullDicPath)
        {
            if (IsDirectoryExist(fullDicPath))
            {
                try
                {
                    Uri uri = new Uri(ReplaceHttpToFtp(fullDicPath));
                    FtpWebRequest listRequest = (FtpWebRequest)WebRequest.Create(uri);
                    listRequest.Method = WebRequestMethods.Ftp.RemoveDirectory;
                    listRequest.Credentials = new NetworkCredential(userName, password);
                    listRequest.KeepAlive = false;  //執行一個命令后關閉連接
                    FtpWebResponse listResponse = (FtpWebResponse)listRequest.GetResponse();
                }
                catch (Exception ex)
                {
                    //LogHelper.GetInstance().WriteMessage("删除文件夹失败" + ex.Message);
                }
            }
        }

        /// <summary>
        /// 從ftp下載文檔
        /// </summary>
        /// <param name="filePathName"></param>
        public static void DownLoadFileFromFtp(string filePathName)
        {
            if (IsFileExist(filePathName))
            {
                string fileName = filePathName.Substring(filePathName.LastIndexOf('/') + 1);
                Uri uri = new Uri(filePathName);
                WebClient request = new WebClient();
                request.Credentials = new NetworkCredential(userName, password);
                byte[] context = request.DownloadData(uri.ToString());//文件的內容
                HttpContext.Current.Response.ClearHeaders();
                HttpContext.Current.Response.Clear();
                string userAgent = HttpContext.Current.Request.UserAgent;
                if (userAgent != null && -1 == userAgent.IndexOf("Firefox"))    //当浏览器是不是ff时
                {
                    fileName = HttpUtility.UrlEncode(fileName, Encoding.UTF8);
                }
                HttpContext.Current.Response.ContentType = "application/octet-stream";
                HttpContext.Current.Response.AddHeader("Content-Disposition", "attachment;FileName=" + fileName);
                HttpContext.Current.Response.BinaryWrite(context);

            }
        }

        /// <summary>
        /// 通過給定的文件的全路徑名,判斷文件是否存在
        /// </summary>
        /// <param name="fileFullPath">http://files.dd.com/SS/S/S/AA.JPG</param>
        public static bool IsFileExist(string fileFullPath)
        {
            bool flag = false;
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(new Uri(Replace(fileFullPath)));
            try
            {
                ((HttpWebResponse)request.GetResponse()).Close();
                flag = true;
            }
            catch
            {
                flag = false;
            }
            return flag;
        }

        /// <summary>
        /// 從本地服務器刪除文件
        /// </summary>
        /// <param name="filePathname"></param>
        public static void DeleteFileFromLocal(string filePathname)
        {
            try
            {
                string filePath = HttpContext.Current.Server.MapPath(filePathname);
                File.Delete(filePath);
            }
            catch (Exception ex)
            {
                //LogHelper.GetInstance().WriteMessage("删除文件夹失败" + ex.Message);
            }
        }

        /// <summary>
        /// 检查文件类型
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="fileType"></param>
        /// <returns></returns>
        public static bool CheckFileType(string fileName, FtpFileType fileType)
        {
            bool flag = false;

            //任何文件都可以
            if (fileType == FtpFileType.All)
            {
                flag = true;
                return flag;
            }
            string fileTypeKey = "Ftp" + fileType.ToString();
            string allowFileTypes = ".jpg/.jpeg/.bmp/.gif/.png";
            string expandFileName = Path.GetExtension(fileName);
            if (expandFileName == null)
            {
                expandFileName = string.Empty;
            }
            if (!String.IsNullOrEmpty(allowFileTypes))
            {
                string[] allowFileTypesArr = allowFileTypes.Split('/');
                for (int i = 0; i < allowFileTypesArr.Length; i++)
                {
                    if (allowFileTypesArr[i].ToLower().Equals(expandFileName.ToLower()))
                    {
                        flag = true;
                        break;
                    }
                }
            }

            return flag;
        }

        /// <summary>
        /// 在新創建文件夾時,得到一個新文件夾名
        /// </summary>
        private static string GetNewDirectoryName()
        {
            return DateTime.Now.ToString("yyyyMMddHHmmss");
        }

        /// <summary>
        /// 獲取文件夾下文件數量
        /// </summary>
        /// <param name="moduleName"></param>
        private static int GetFilesCountFromFtp(string moduleName)
        {
            int fileCount = 0;  //文件數量
            string directory = moduleName.ToLower() + "ModuleFtpDirectory";
            string directoryPath = ftpAddress + HttpContext.Current.Application[directory].ToString();

            if (!IsDirectoryExist(directoryPath))
            {
                CreateFullDirectoryAtFtp(directoryPath);
            }
            //執行ftp命令 活動目錄下所有文件列表
            Uri uriDir = new Uri(directoryPath);
            FtpWebRequest listRequest = (FtpWebRequest)WebRequest.Create(uriDir);
            listRequest.Method = WebRequestMethods.Ftp.ListDirectory;
            listRequest.Credentials = new NetworkCredential(userName, password);
            listRequest.KeepAlive = false;  //執行一個命令后關閉連接
            FtpWebResponse listResponse = (FtpWebResponse)listRequest.GetResponse();
            Stream responseStream = listResponse.GetResponseStream();
            StreamReader readStream = new StreamReader(responseStream, System.Text.Encoding.UTF8);
            //判斷文件列表中一共有多少個文件
            if (readStream != null)
            {
                while (readStream.ReadLine() != null)
                {
                    fileCount++;
                }
                readStream.Close();
            }
            responseStream.Close();

            return fileCount;
        }

    }

    [Serializable]
    public class UploadResultInformation
    {
        /// <summary>
        /// 文檔的路徑名
        /// </summary>
        private string _filePathName;
        /// <summary>
        /// 文檔所在服務器
        /// </summary>
        private string _serverAdress;
        /// <summary>
        /// 文檔的全路徑名
        /// </summary>
        private string _fullPathName;
        /// <summary>
        /// 文檔的名稱
        /// </summary>
        private string _fileName;
        /// <summary>
        /// 是否進行縮圖處理
        /// </summary>
        private bool _isMiniatureImage;
        private bool _isFtp = true;
        /// <summary>
        /// 打過水印后的流
        /// </summary>
        private Stream _waterImageStream;
        private string _imagePath;

        /// <summary>
        /// 圖片路徑
        /// </summary>
        public string ImagePath
        {
            get { return this._imagePath; }
            set { this._imagePath = value; }
        }
        /// <summary>
        /// 打過水印后的流
        /// </summary>
        public Stream WaterImageStream
        {
            get { return this._waterImageStream; }
            set { this._waterImageStream = value; }
        }
        /// <summary>
        /// 是否為上傳FTP
        /// </summary>
        public bool IsFtp
        {
            get { return this._isFtp; }
            set { this._isFtp = value; }
        }
        /// <summary>
        /// 是否進行縮圖處理
        /// </summary>
        public bool IsMiniatureImage
        {
            get { return this._isMiniatureImage; }
            set { this._isMiniatureImage = value; }
        }
        /// <summary>
        /// 文檔的路徑名 000001/product/P000001/1121.jpg
        /// </summary>
        public string FilePathName
        {
            get { return _filePathName; }
            set { _filePathName = value; }
        }
        /// <summary>
        /// 文檔所在服務器 ftp
        /// </summary>
        public string ServerAdress
        {
            get { return _serverAdress; }
            set { _serverAdress = value; }
        }
        /// <summary>
        /// 文檔的全路徑名 ftp://files.ss.com/user/product/P00001/111.jpg
        /// </summary>
        public string FullPathName
        {
            get { return _fullPathName; }
            set { _fullPathName = value; }
        }

        /// <summary>
        /// http文檔的全路徑名 http://files.ss.com/user/product/P00001/111.jpg
        /// </summary>
        public string HttpFullPathName { get; set; }

        /// <summary>
        /// 文件所在FTP的路径
        /// </summary>
        public string FTPPath { get; set; }

        /// <summary>
        /// 文檔名稱
        /// </summary>
        public string FileName
        {
            get
            {
                return _fileName;
            }
            set
            {
                _fileName = value;
            }
        }

        /// <summary>
        /// 文件类型正确
        /// </summary>
        public bool FileTypeCorrect
        {
            get;
            set;
        }
    }

    /// <summary>
    /// 文件类型
    /// </summary>
    public enum FtpFileType
    {
        Video,
        Image,
        Apk,
        All
    }
}
