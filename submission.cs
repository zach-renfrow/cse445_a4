using System;
using System.Xml.Schema;
using System.Xml;
using Newtonsoft.Json;



/**
 * This template file is created for ASU CSE445 Distributed SW Dev Assignment 4.
 * Please do not modify or delete any existing class/variable/method names. However, you can add more variables and functions.
 * Uploading this file directly will not pass the autograder's compilation check, resulting in a grade of 0.
 * **/


namespace ConsoleApp1
{


    public class Submission
    {
        // Q1.2: URL to valid NationalParks.xml on GitHub Pages
        public static string xmlURL = "https://zach-renfrow.github.io/cse445_a4/NationalParks.xml";
        // Q1.3: URL to NationalParksErrors.xml with 5 injected errors
        public static string xmlErrorURL = "https://zach-renfrow.github.io/cse445_a4/NationalParksErrors.xml";
        // Q1.1: URL to NationalParks.xsd schema definition
        public static string xsdURL = "https://zach-renfrow.github.io/cse445_a4/NationalParks.xsd";

        // Q3: Main method - calls Verification on both XML files and converts valid XML to JSON
        public static void Main(string[] args)
        {
            // Q3.1: Validate valid XML against XSD - expects "No errors are found"
            string result = Verification(xmlURL, xsdURL);
            Console.WriteLine(result);


            // Q3.2: Validate error XML against XSD - expects error messages
            result = Verification(xmlErrorURL, xsdURL);
            Console.WriteLine(result);


            // Q3.3: Convert valid XML to JSON
            result = Xml2Json(xmlURL);
            Console.WriteLine(result);
        }

        // Q2.1: Validates XML against XSD using XmlSchemaSet, XmlReaderSettings, XmlReader
        // Returns "No errors are found" if valid, or error messages if invalid
        public static string Verification(string xmlUrl, string xsdUrl)
        {
            // String to collect validation errors
            string errors = "";

            try
            {
                // Step 1: Create XmlSchemaSet and add schema from URL
                XmlSchemaSet sc = new XmlSchemaSet();
                sc.Add(null, xsdUrl);

                // Step 2: Configure validation settings
                XmlReaderSettings settings = new XmlReaderSettings();
                settings.ValidationType = ValidationType.Schema;
                settings.Schemas = sc;

                // Step 3: Attach event handler to collect errors
                settings.ValidationEventHandler += delegate (object sender, ValidationEventArgs e)
                {
                    errors += "Validation Error: " + e.Message + "\n";
                };

                // Step 4: Create reader and parse the XML
                XmlReader reader = XmlReader.Create(xmlUrl, settings);

                // Read through entire document - triggers validation on errors
                while (reader.Read()) { }

                reader.Close();
            }
            catch (Exception ex)
            {
                // Catch XML parsing exceptions (e.g., malformed XML)
                errors += "Validation Error: " + ex.Message;
            }

            // Return appropriate result
            if (errors == "")
            {
                return "No errors are found";
            }

            return errors.Trim();
        }

        // Q2.2: Converts XML from URL to JSON using XmlDocument and Newtonsoft.Json
        // Returned JSON is deserializable by JsonConvert.DeserializeXmlNode()
        public static string Xml2Json(string xmlUrl)
        {
            try
            {
                // Download XML content from URL
                string xmlContent = DownloadContent(xmlUrl);

                // Load into XmlDocument
                XmlDocument doc = new XmlDocument();
                doc.LoadXml(xmlContent);

                // Serialize to JSON using Newtonsoft.Json
                string jsonText = JsonConvert.SerializeXmlNode(doc);

                return jsonText;
            }
            catch (Exception ex)
            {
                return "Exception: " + ex.Message;
            }
        }

        // Helper method to download content from URL
        private static string DownloadContent(string url)
        {
            using (System.Net.WebClient client = new System.Net.WebClient())
            {
                return client.DownloadString(url);
            }
        }
    }

}
