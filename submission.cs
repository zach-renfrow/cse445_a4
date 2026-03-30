using System;
using System.Xml.Schema;
using System.Xml;
using Newtonsoft.Json;
using System.IO;
using System.Net;



/**
 * This template file is created for ASU CSE445 Distributed SW Dev Assignment 4.
 * Please do not modify or delete any existing class/variable/method names. However, you can add more variables and functions.
 * Uploading this file directly will not pass the autograder's compilation check, resulting in a grade of 0.
 * **/


namespace ConsoleApp1
{


    public class Submission
    {
        public static string xmlURL = "https://zach-renfrow.github.io/cse445_a4/NationalParks.xml";
        public static string xmlErrorURL = "https://zach-renfrow.github.io/cse445_a4/NationalParksErrors.xml";
        public static string xsdURL = "https://zach-renfrow.github.io/cse445_a4/NationalParks.xsd";

        public static void Main(string[] args)
        {
            string result = Verification(xmlURL, xsdURL);
            Console.WriteLine(result);

            result = Verification(xmlErrorURL, xsdURL);
            Console.WriteLine(result);

            result = Xml2Json(xmlURL);
            Console.WriteLine(result);
        }

        public static string Verification(string xmlUrl, string xsdUrl)
        {
            try
            {
                string xsdContent = DownloadContent(xsdUrl);

                XmlSchemaSet sc = new XmlSchemaSet();
                sc.Add(null, XmlReader.Create(new StringReader(xsdContent)));

                XmlReaderSettings settings = new XmlReaderSettings();
                settings.ValidationType = ValidationType.Schema;
                settings.Schemas = sc;
                settings.ValidationFlags |= XmlSchemaValidationFlags.ReportValidationWarnings;

                string errors = "";

                settings.ValidationEventHandler += delegate (object sender, ValidationEventArgs e)
                {
                    errors += e.Message + "\n";
                };

                string xmlContent = DownloadContent(xmlUrl);
                XmlReader reader = XmlReader.Create(new StringReader(xmlContent), settings);

                while (reader.Read()) { }

                reader.Close();

                if (errors == "")
                {
                    return "No errors are found";
                }

                return errors.Trim();
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        public static string Xml2Json(string xmlUrl)
        {
            string xmlContent = DownloadContent(xmlUrl);

            XmlDocument doc = new XmlDocument();
            doc.LoadXml(xmlContent);

            XmlElement root = doc.DocumentElement;
            root.RemoveAttribute("xmlns:xsi");
            root.RemoveAttribute("xsi:noNamespaceSchemaLocation");

            string jsonText = JsonConvert.SerializeXmlNode(root, Newtonsoft.Json.Formatting.Indented, false);

            return jsonText;
        }

        private static string DownloadContent(string url)
        {
            using (WebClient client = new WebClient())
            {
                return client.DownloadString(url);
            }
        }
    }

}
