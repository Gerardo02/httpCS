using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.IO;
using Microsoft.VisualBasic;

namespace codecrafters_http_server.src
{
    public class FileHandler
    {

        private readonly string _filePath;
        public FileHandler(string filePath)
        {
            _filePath = filePath;
        }

        public string? lookForFile(string fileName)
        {
            string[] files = Directory.GetFiles(_filePath);
            string[] fileNames;
            string fileFlag;

            foreach(var file in files)
            {
                fileNames = file.Split("/");
                fileFlag = fileNames[fileNames.Length - 1];

                if(fileFlag.Substring(0, fileFlag.Length) == fileName)
                {
                    return file;
                }
            }
            return null;
        }
        public string readFile(string newFilePath)
        {
            string? line;
            string? content;

            if(_filePath.Length == 0) return "No file";

            try
            {
                StreamReader reader = new StreamReader(newFilePath);
                line = reader.ReadLine();
                content = line;
                while(line != null)
                {
                    content += "\n";
                    line = reader.ReadLine();
                    content += line;
                }
                reader.Close();

                if(content == null) return "No file";
                content = content.Substring(0, content.Length - 1);

                return content;

            }catch(Exception e)
            {
                Console.WriteLine(e);
                return "No file";
            }
            
        }
    }
}