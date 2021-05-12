using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;

namespace ConsoleApplication1
{
    internal static class Program
    {
        public static void UploadFile(string localName, string remoteName)
        {
            FtpWebRequest request;

            request = WebRequest.Create(new Uri(remoteName)) as FtpWebRequest;
            request.Method = WebRequestMethods.Ftp.UploadFile;
            request.UseBinary = true;
            request.UsePassive = true;
            request.KeepAlive = true;
            request.Credentials = new NetworkCredential("User", "test123");

            using (var fs = File.OpenRead(localName))
            {
                var buffer = new byte[fs.Length];
                fs.Read(buffer, 0, buffer.Length);
                fs.Close();
                var requestStream = request.GetRequestStream();
                requestStream.Write(buffer, 0, buffer.Length);
                requestStream.Flush();
                requestStream.Close();
            }
        }

        public static void DownloagFile(string localName, string remoteName)
        {
            FtpWebRequest request;

            request = WebRequest.Create(new Uri(remoteName)) as FtpWebRequest;
            request.Method = WebRequestMethods.Ftp.DownloadFile;
            request.UseBinary = true;
            request.UsePassive = true;
            request.KeepAlive = true;
            request.Credentials = new NetworkCredential("User", "test123");

            using (var ftpStream = request.GetResponse().GetResponseStream())
            {
                using (Stream fileStream = File.Create(localName))
                {
                    ftpStream.CopyTo(fileStream);
                }
            }
        }

        public static FtpWebRequest GetRequest(string remoteName)
        {
            FtpWebRequest request;
            request = WebRequest.Create(new Uri(remoteName)) as FtpWebRequest;
            request.UseBinary = true;
            request.UsePassive = true;
            request.KeepAlive = true;
            request.Credentials = new NetworkCredential("User", "test123");
            return request;
        }


        public static void DeleteFile(string remoteName)
        {
            var request = GetRequest(remoteName);
            request.Method = WebRequestMethods.Ftp.DeleteFile;

            var response = request.GetResponse();
            response.Close();
        }

        public static void CreateDir(string remoteName)
        {
            FtpWebRequest request;

            request = WebRequest.Create(new Uri(remoteName)) as FtpWebRequest;
            request.Method = WebRequestMethods.Ftp.MakeDirectory;
            request.UseBinary = true;
            request.UsePassive = true;
            request.KeepAlive = true;
            request.Credentials = new NetworkCredential("User", "test123");

            var response = request.GetResponse();
            response.Close();
        }
        
        public static void RecursiveDeleteDir(string remoteName)
        {
            var reg = new Regex(@"^(.{1}).+\d{2}:\d{2}\s(.+)$"); 
            
            foreach (var item in GetFilesList(remoteName))
            {
                
                var m=reg.Match(item);

                string fileName = m.Groups[2].Value;
                bool isDir = m.Groups[1].Value=="d";

                if (isDir)
                {
                    RecursiveDeleteDir(remoteName+"/"+ fileName);
                }
                else
                {
                    DeleteFile(remoteName+"/"+ fileName);
                }
            }
            var request = GetRequest(remoteName);
            request.Method = WebRequestMethods.Ftp.RemoveDirectory;
            var response = request.GetResponse();
            response.Close();
        }
        
        public static List<string> GetFilesList(string remoteName)
        {
            var request = GetRequest(remoteName);
            request.Method = WebRequestMethods.Ftp.ListDirectoryDetails;
            
            
            
            FtpWebResponse response = (FtpWebResponse)request.GetResponse();  
            Stream responseStream = response.GetResponseStream();  
            StreamReader reader = new StreamReader(responseStream);  
            string names = reader.ReadToEnd();  
            
            reader.Close();  
            response.Close();  
  
            return names.Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries).ToList();  
        }

        public static void Main(string[] args)
        {
            /*var localFile = "E:\\file.txt";
            var remoteFile = "ftp://127.0.0.1/test.txt";
            
            UploadFile(localFile, remoteFile);*/


            /*var localFile = "E:\\file.txt";
            var remoteFile = "ftp://127.0.0.1/file.txt";

            DownloagFile(localFile, remoteFile);*/

            /*var remoteFile = "ftp://127.0.0.1/file.txt";
            
            DeleteFile(remoteFile);*/

            /*var remoteFile = "ftp://127.0.0.1/dir1";

            CreateDir(remoteFile);*/

            
            
            /*var remoteFile = "ftp://127.0.0.1/dir1";
            
            DeleteDir(remoteFile);*/
            
            
            var remoteFile = "ftp://127.0.0.1/dir1";
            
            RecursiveDeleteDir(remoteFile);
            
            
        }
    }
}