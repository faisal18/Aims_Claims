using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Aims_Claims
{
    class Program
    {
        static void Main(string[] args)
        {

        }
    }


    public class XMLReader123
    {
        public void DeformXMLtoCSV()
        {
            XmlDocument xdoc = new XmlDocument();
            StringBuilder sb = new StringBuilder();

            try
            {
                string[] xml_files = System.IO.File.ReadAllLines(@"C:\Users\fansari.INTERNAL\Desktop\Rana2.xml");
                int iteration = 1;

                Dictionary<string, string> DICT_DHCC = new Dictionary<string, string>();

                sb.Append("PassportNumber|PassportIssuingCountry|License|FullName|username|password|Email|PhoneNumber|Qualification|FacilityLicense|FacilityName|facilities|Location|ActiveFrom|ActiveTo|IsActive|Source|SpecialtyID1|Gender|Nationality|SpecialtyID2|HCType|DHCCSpecialty1|DHCCSpecialty2\n");

                string[] values = sb.ToString().Split('|');
                foreach (string val in values)
                {
                    DICT_DHCC.Add(val, string.Empty);
                }


                foreach (string file in xml_files)
                {
                    xdoc.LoadXml(file);
                    XmlNode node = xdoc.SelectSingleNode("HCP");
                    XmlNodeList node_list = node.ChildNodes;

                    for (int i = 0; i < node_list.Count; i++)
                    {

                        if (DICT_DHCC.ContainsKey(node_list[i].LocalName))
                        {
                            switch (node_list[i].LocalName.ToLower())
                            {
                                case "facilities":
                                    node_list[i].InnerText = string.Empty;
                                    break;
                                case "password":
                                    string decryptedbydhcc = DecryptbyDHCC(node_list[i].InnerText);
                                    string encryptedbylocal = EncryptByLocal(decryptedbydhcc);
                                    Console.WriteLine("File:" + iteration + " Decrypted:" + decryptedbydhcc + " Encrypted:" + encryptedbylocal);
                                    node_list[i].InnerText = encryptedbylocal;
                                    break;
                                case "activefrom":
                                    node_list[i].InnerText = ConvertDate(node_list[i].InnerText);
                                    break;
                                case "activeto":
                                    node_list[i].InnerText = ConvertDate(node_list[i].InnerText);
                                    break;
                            }
                            DICT_DHCC[node_list[i].LocalName] = node_list[i].InnerText;
                        }
                    }
                    foreach (string key in DICT_DHCC.Keys)
                    {
                        sb.Append(DICT_DHCC[key] + "|");
                    }
                    sb.Append("\n");
                    iteration++;
                }

                using (StreamWriter writer = File.CreateText(@"C:\Users\fansari.INTERNAL\Desktop\dhcc\RANAXML\Rana_Output.csv"))
                {
                    writer.Write(sb.ToString());
                }
                Console.WriteLine("File transformation Completed ");
                Console.Read();
            }
            catch (Exception ex)
            {
                Console.Write(ex.Message);
                Console.Read();
            }


        }
        public string GetPassword(string data)
        {
            try
            {
                data = EncryptByLocal(DecryptbyDHCC(data));
                return data;
            }
            catch (Exception ex)
            {
                Console.WriteLine("EXCEPTION OCCURED! Password:" + data + "\n" + ex.Message);
                return ex.Message;
            }
        }
        public string DecryptbyDHCC(string data)
        {
            try
            {
                EncryptionDecryptionHelper yp = new EncryptionDecryptionHelper();
                data = yp.DecryptMain(data, "hah");
                return data;
            }
            catch (Exception ex)
            {
                Console.WriteLine("EXCEPTION OCCURED! Password:" + data + "\n" + ex.Message);
                return ex.Message;
            }
        }
        public string EncryptByLocal(string data)
        {
            try
            {
                string EncryptionKey = "dhcc_client";
                byte[] clearBytes = Encoding.Unicode.GetBytes(data);
                using (Aes encryptor = Aes.Create())
                {
                    Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(EncryptionKey, new byte[] { 0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76 });
                    encryptor.Key = pdb.GetBytes(32);
                    encryptor.IV = pdb.GetBytes(16);
                    using (MemoryStream ms = new MemoryStream())
                    {
                        using (CryptoStream cs = new CryptoStream(ms, encryptor.CreateEncryptor(), CryptoStreamMode.Write))
                        {
                            cs.Write(clearBytes, 0, clearBytes.Length);
                            cs.Close();
                        }
                        data = Convert.ToBase64String(ms.ToArray());
                    }
                }
                return data;
            }
            catch (Exception ex)
            {
                Console.WriteLine("EXCEPTION OCCURED! Password:" + data + "\n" + ex.Message);
                return ex.Message;
            }
        }

        public void AddBatchIdtoXML()
        {
            try
            {
                int count = 0;
                string[] files = Directory.GetFiles(@"C:\Users\faisal\Desktop\files\Payer Integration\Add Batch to XML\XML\", "*.xml");
                foreach (string file in files)
                {

                    string filename = Path.GetFileNameWithoutExtension(file);
                    string path = Path.GetDirectoryName(file);
                    string newFilename = filename.Replace("Batch_PR_", "PR_");

                    Console.WriteLine("Working on file: " + filename);


                    XmlDocument xdoc = new XmlDocument();
                    xdoc.Load(file);
                    XmlNode BatchNode = xdoc.SelectSingleNode("//BatchId");
                    BatchNode.InnerText = "0695091e-eadf-44a9-91bf-e62a3f0b94b9";
                    xdoc.Save(path + "\\" + newFilename + ".XML");
                    count++;
                }
                Console.WriteLine(count + " files have been processed");
                Console.Read();
            }
            catch (Exception ex)
            {
                Console.Write(ex.Message);
                Console.Read();
            }
        }
    }

    public class BatchtoPR_cls
    {
        public void Run()
        {
            try
            {
                string path = @"C:\tmp\BatchtoPrandConfirm\";
                string[] fileArray = Directory.GetFiles(path, "*.xml");
                string path2 = string.Empty;

                foreach (string file in fileArray)
                {
                    path2 = path + Path.GetFileNameWithoutExtension(file);
                    Directory.CreateDirectory(path2);
                    int i = 1;
                    XmlDocument doc = new XmlDocument();
                    doc.Load(Path.GetFullPath(file));
                    XmlNodeList elemList = doc.GetElementsByTagName("requestsList");

                    foreach (XmlNode abc in elemList)
                    {
                        string file_name = abc.FirstChild.InnerText;
                        //using (var file2 = File.CreateText(string.Format(path + "\\{0}", file_name)))
                        using (var file2 = File.CreateText(path2 + "\\" + file_name))
                        {
                            XmlDocument docNew = new XmlDocument();
                            XmlElement newRoot = docNew.CreateElement("Prior.Request");
                            docNew.AppendChild(newRoot);
                            newRoot.InnerXml = abc.LastChild.InnerXml;
                            String xml = docNew.OuterXml;
                            file2.Write(xml);
                            Console.WriteLine("File Created {0}_{1}.xml", file_name, i++);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
        private string ModifyPR_RootNode()
        {
            System.Xml.XmlDocument xmlDoc = new System.Xml.XmlDocument();
            int i = 1;
            string[] fileArray = System.IO.Directory.GetFiles(@"C:\Users\faisal\Downloads\SOAP Projects\PBMSwitch PayerIntegration\Batch PRs\", "*.xml");
            try
            {
                foreach (string file in fileArray)
                {
                    xmlDoc.Load(file);
                    XmlDocument docNew = new XmlDocument();

                    //newRoot value to be that of the file it needs to be transformed into.
                    XmlElement newRoot = docNew.CreateElement("Prior.Request");

                    docNew.AppendChild(newRoot);
                    newRoot.InnerXml = xmlDoc.DocumentElement.InnerXml;
                    String xml = docNew.OuterXml;
                    using (var file2 = File.CreateText(string.Format(@"C:\Users\faisal\Downloads\SOAP Projects\PBMSwitch PayerIntegration\Batch PRs New\XML_{0}.xml", i++)))
                    {
                        file2.Write(xml);
                    }
                    xmlDoc.Save(file);
                    Console.WriteLine("PR File number {0} created ", i);
                }
                return "Success";
            }
            catch (Exception ex)
            {
                return (ex.Message.ToString());
            }
        }
    }
}
