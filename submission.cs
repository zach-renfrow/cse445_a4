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
        // Q1.2: URL to the valid NationalParks.xml hosted on GitHub Pages
        public static string xmlURL = "https://zach-renfrow.github.io/cse445_a4/NationalParks.xml";
        // Q1.3: URL to the NationalParksErrors.xml with 5 injected errors
        public static string xmlErrorURL = "https://zach-renfrow.github.io/cse445_a4/NationalParksErrors.xml";
        // Q1.1: URL to the NationalParks.xsd schema definition
        public static string xsdURL = "https://zach-renfrow.github.io/cse445_a4/NationalParks.xsd";

        // Q3: Main method calls Verification on both XML files and converts valid XML to JSON
        public static void Main(string[] args)
        {
            string result = Verification(xmlURL, xsdURL);
            Console.WriteLine(result);

            result = Verification(xmlErrorURL, xsdURL);
            Console.WriteLine(result);

            result = Xml2Json(xmlURL);
            Console.WriteLine(result);
        }

        // Q2.1: Validates an XML file against an XSD schema using XmlSchemaSet and XmlReader
        // Returns "No errors are found" if valid, or the validation error messages if invalid
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

        // Q2.2: Converts an XML file from a URL into a JSON string
        // Uses XmlDocument to load the XML and Newtonsoft.Json to serialize to JSON
        // The returned JSON is deserializable by JsonConvert.DeserializeXmlNode()
        public static string Xml2Json(string xmlUrl)
        {
            string xmlContent = DownloadContent(xmlUrl);

            XmlDocument doc = new XmlDocument();
            doc.LoadXml(xmlContent);

            XmlElement root = doc.DocumentElement;
            root.RemoveAttribute("xmlns:xsi");
            root.RemoveAttribute("xsi:noNamespaceSchemaLocation");

            string jsonText = JsonConvert.SerializeXmlNode(root, Formatting.Indented, false);

            return jsonText;
        }

        // Helper method to download content from URL
        private static string DownloadContent(string url)
        {
            using (WebClient client = new WebClient())
            {
                return client.DownloadString(url);
            }
        }
    }

}
