using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;


namespace EcoConf
{
    class HandleTmpFiles
    {

        /**
         * track all tmp files which might be used
         */
        public static List<string> tmpFiles = new List<string>();

        /**
         * use this tmp directory to store tmp files
         */
        public static string tmpFolder = string.Empty;


        /**
         * create a temporary folder for the temp files
         */
        public static string GetTemporaryDirectory(bool failure = false)
        {
            string folder = string.Empty;
            try
            {
                folder = Path.GetTempFileName();
                File.Delete(folder);
                Directory.CreateDirectory(folder);
            }
            catch (Exception e)
            {
                if (failure)
                {
                    Console.WriteLine("Unable to create TEMP folder: "+e.Message);
                }
                else
                {
                    return GetTemporaryDirectory(true);
                }
            }
            tmpFolder = folder;
            return folder;
        }


        /**
         * Use the tmp file for the input, so that the input can easily be stored if needed
         */
        public static string CreateTmpFile()
        {
            string fileName = string.Empty;

            try
            {
                // Get the full name of the newly created Temporary file. 
                // Note that the GetTempFileName() method actually creates
                // a 0-byte file and returns the name of the created file.
                fileName = Path.GetTempFileName();

                // Craete a FileInfo object to set the file's attributes
                FileInfo fileInfo = new FileInfo(fileName);

                // Set the Attribute property of this file to Temporary. 
                // Although this is not completely necessary, the .NET Framework is able 
                // to optimize the use of Temporary files by keeping them cached in memory.
                fileInfo.Attributes = FileAttributes.Temporary;

                Console.WriteLine("TEMP file created at: " + fileName);
            }
            catch (Exception e)
            {
                Console.WriteLine("Unable to create TEMP file or set its attributes: " + e.Message);
            }

            tmpFiles.Add(fileName);
            return fileName;
        }


        /**
         * delete the tmp files if not needed anymore
         */
        public static void DeleteTmpFile(string tmpFile)
        {
            try
            {
                // Delete the temp file (if it exists)
                if (File.Exists(tmpFile))
                {
                    File.Delete(tmpFile);
                    Console.WriteLine("TEMP file deleted: "+tmpFile);
                    tmpFiles.Remove(tmpFile);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Error deleteing TEMP file: " + e.Message);
            }
        }


        /**
         * delete the tmp folder
         */
         public static void DeleteTmpFolder()
         {
            try
            {
                if(!string.IsNullOrEmpty(tmpFolder))
                    Directory.Delete(tmpFolder, true);
            }
            catch (Exception e)
            {
                Console.WriteLine("Error deleting TEMP folder: "+e.Message);
            }
         }
    }
}
