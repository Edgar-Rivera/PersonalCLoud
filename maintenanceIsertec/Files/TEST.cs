Const int port = 22; //port
                 const string host = "100.121.192.66"; //sftp address
                 const string username = "root"; //user name
                 const string password = "XXXXXX";//Password
                 const string workingdirectory = "/emeeting/";//The directory for reading and uploading files "/" is the root directory
                 const string uploadfile = @"D:\Test screenshot\Pending.png"; //Upload file address
 
        /// <summary>
                 /// Upload files to SFTP
        /// </summary>
                 /// <param name="FilePath">local file path @"D:\test screenshot\pending.png</param>
                 /// <param name="WorkDirectory">Upload to working directory /emeeting/</param>
                 /// <returns>Upload results</returns>
        public bool UploadSftpFile(string FilePath, string WorkDirectory)
        {
            bool flag = false;
                         // file name 
            string fileName = FilePath.Substring(FilePath.LastIndexOf('\\')+1);
            try
            {
                                 using (var client = new SftpClient(host, port, username, password)) //Create connection object
                {
                                         client.Connect(); //Connect
                                         client.ChangeDirectory(WorkDirectory); //Switch directory
                   
 
                    List<string> fileNameList = new List<string>();
                    using (var fileStream = new FileStream(FilePath, FileMode.Open))
                    {
                        client.BufferSize = 4 * 1024;
                        if (client.Exists(WorkDirectory + fileName))
                        {
                            client.DeleteFile(fileName); 
                        }
                        client.UploadFile(fileStream, Path.GetFileName(FilePath));
 
                                                 var listDirectory = client.ListDirectory(WorkDirectory); //Get all files in the directory
                                                 foreach (var fi in listDirectory) //traverse the file
                        {
                            fileNameList.Add(fi.Name);
                        }
 
                                                 // "To-do.png"
                        if (fileNameList.Contains(fileName))
                            flag = true;
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
 
            return flag;
        }
 
 
        /// <summary>
                 /// Delete files from SFTP server
        /// </summary>
                 /// <param name="filePath">SFTP file path /emeeting/pending.png</param>
                 /// <returns>Whether the deletion was successful</returns>
        public bool DeleteSFtpFile(string filePath)
        {
            bool flag = false;
 
            string workdir = filePath.Substring(0,filePath.LastIndexOf('/')+1);
            string fileName= filePath.Substring(filePath.LastIndexOf('/') + 1);
            try
            {
                List<string> fileNameList = new List<string>();
                                 using (var client = new SftpClient(host, port, username, password)) //Create connection object
                {
                                         client.Connect(); //Connect
                                         client.ChangeDirectory(workdir); //Switch directory
                                                                                                           //var listDirectory = client.ListDirectory(workdir); //Get all files in the directory
 
                                         //foreach (var fi in listDirectory) //traverse the file
                    //{
                    //    if (fi.Name==fileName)
                    //        fi.Delete();
                    //}
                    if (client.Exists(fileName))
                    {
                        client.DeleteFile(fileName);
                    }
                    else
                    {
                                                 MessageBox.Show("The file does not exist");
                    }
 
                                         var listDirectory = client.ListDirectory(workdir); //Get all files in the directory
                                         foreach (var fi in listDirectory) //traverse the file
                    {
                        fileNameList.Add(fi.Name);
                    }
 
                    if (!fileNameList.Contains(fileName))
                        flag = true;
                    else
                        flag = false;
                }
               
            }
            catch (Exception ex)
            {
 
                throw ex;
            }
 
            return flag;
        }