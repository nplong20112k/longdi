﻿using System;
using System.IO;

namespace ConfigGenerator
{
    public abstract class ACodeFileProcessor: ICodeFileProcessor
    {
        const string SOURCE_CODE_MAIN_HEADER_FILE = "ConfigItemHeader.h";

        private const string FILE_HEADER =  "///////////////////////////////////////////////////////////\n" +
                                             "//Source code file auto generated by Config Generator tool\n" +
                                             "//Version {0} {1}\n" +
                                             "//Please do not change below this line manually\n" +
                                             "///////////////////////////////////////////////////////////\n\n";

        CODE_FILE_PROCESSOR_TYPE m_CodeProcessorType;
        string[] m_sHeader;
        string[] m_sFooter;
        string m_sFilePath;

        public ACodeFileProcessor(CODE_FILE_PROCESSOR_TYPE FileProcessorType, string[] sHeader, string [] sFooter)
        {
            m_CodeProcessorType = FileProcessorType;
            m_sHeader = sHeader;
            m_sFooter = sFooter;
        }

        public ACodeFileProcessor(CODE_FILE_PROCESSOR_TYPE FileProcessorType)
        {
            m_CodeProcessorType = FileProcessorType;
            m_sHeader = null;
            m_sFooter = null;
        }

        public CODE_FILE_PROCESSOR_TYPE GetProcessorType()
        {
            return m_CodeProcessorType;
        }

        public virtual bool CreateNewFile(string sFilePath)
        {
            bool bRet = false;
            ReInitProperties();

            if (sFilePath == null)
            {
                return bRet;
            }

            string FolderName = Path.GetDirectoryName(sFilePath);
            if (!Directory.Exists(FolderName))
            {
                try
                {
                    Directory.CreateDirectory(FolderName);
                    Program.SetUnixFileFullPermissions(FolderName);
                }
                catch
                {
                    return bRet;
                }
            }

            if (File.Exists(sFilePath))
            {
                try
                {
                    File.Delete(sFilePath);
                }
                catch
                {
                    return bRet;
                }
            }

            try
            {
                string sVerRelease  = CConfigTool.GetInstance().sVersionRelease;
                string sDateRelease = CConfigTool.GetInstance().sDateRelease;
                string sFileHeader  = string.Format(FILE_HEADER,
                                                    sVerRelease,
                                                    sDateRelease);

                File.AppendAllText(sFilePath, sFileHeader);
                //File.Create(sFilePath);   //Nghiem: Seems this method need some delay before writing

                if (m_sHeader != null)
                {
                    for (int i = 0; i < m_sHeader.Length; i++)
                    {
                        File.AppendAllText(sFilePath, m_sHeader[i]);
                    }
                }
            }
            catch
            {
                return bRet;
            }

            m_sFilePath = sFilePath;
            Program.SetUnixFileFullPermissions(m_sFilePath);
            return true;
        }

        public string[] LoadingFile(string sFilePath)
        {
            string[] sContain = null;

            if (sFilePath == null)
            {
                return null;
            }

            if (File.Exists(sFilePath))
            {
                try
                {
                    sContain = File.ReadAllLines(sFilePath);
                }
                catch
                {
                    return null;
                }
            }

            return sContain;
        }

        public bool AddNewItem(string sData)
        {
            bool bRet = false;

            if ((m_sFilePath != null) && (sData != null))
            {
                try
                {
                    File.AppendAllText(m_sFilePath, sData);
                }
                catch (Exception e)
                {
                    //MessageBox.Show(e.ToString());
                    return bRet;
                }
            }

            return true;
        }

        public virtual bool CloseFile()
        {
            bool bRet = true;

            if (m_sFooter != null)
            {
                for (int i = 0; i < m_sFooter.Length; i++)
                {
                    bRet = AddNewItem(m_sFooter[i]);
                    if (bRet == false)
                    {
                        return bRet;
                    }
                }
            }

            return bRet;
        }

        private void ReInitProperties()
        {
            m_sFilePath = null;
        }
    }
}