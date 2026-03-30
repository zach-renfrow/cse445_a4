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
        // Q1.2: URL to the valid NationalParks.xml hosted on GitHub Pages
        public static string xmlURL = "https://zach-renfrow.github.io/cse445_a4/NationalParks.xml";
        // Q1.3: URL to the NationalParksErrors.xml with 5 injected errors
        public static string xmlErrorURL = "https://zach-renfrow.github.io/cse445_a4/NationalParksErrors.xml";
        // Q1.1: URL to the NationalParks.xsd schema definition
        public static string xsdURL = "https://zach-renfrow.github.io/cse445_a4/NationalParks.xsd";

        // Q3: Main method calls Verification on both XML files and converts valid XML to JSON
        public static void Main(string[] args)
        {
            // Q3.1: Validate the valid XML against the XSD - expects "No errors are found"
            string result = Verification(xmlURL, xsdURL);
            Console.WriteLine(result);


            // Q3.2: Validate the error XML against the XSD - expects error messages
            result = Verification(xmlErrorURL, xsdURL);
            Console.WriteLine(result);


            // Q3.3: Convert the valid XML to JSON format
            result = Xml2Json(xmlURL);
            Console.WriteLine(result);
        }

        // Q2.1: Validates an XML file against an XSD schema using XmlSchemaSet and XmlReader
        // Returns "No errors are found" if valid, or the validation error messages if invalid
        public static string Verification(string xmlUrl, string xsdUrl)
        {
            // String to accumulate any validation errors found
            string errors = "";

            try
            {
                // Step 1: Create XmlSchemaSet and add the XSD schema from URL
                XmlSchemaSet sc = new XmlSchemaSet();
                sc.Add(null, xsdUrl);

                // Step 2: Configure XmlReaderSettings for schema validation
                XmlReaderSettings settings = new XmlReaderSettings();
                settings.ValidationType = ValidationType.Schema;
                settings.Schemas = sc;
                settings.ValidationFlags |= XmlSchemaValidationFlags.ReportValidationWarnings;

                // Step 3: Attach event handler to collect validation errors
                settings.ValidationEventHandler += delegate (object sender, ValidationEventArgs e)
                {
                    errors += "Validation Error: " + e.Message + "\n";
                };

                // Step 4: Create XmlReader and parse the XML file against the schema
                XmlReader reader = XmlReader.Create(xmlUrl, settings);

                // Read through entire document - triggers validation events on errors
                while (reader.Read()) { }

                reader.Close();
            }
            catch (Exception ex)
            {
                // Catch any XML parsing exceptions (e.g., malformed XML)
                errors += "Validation Error: " + ex.Message;
            }

            // Return result based on whether errors were found
            if (errors == "")
            {
                return "No errors are found";
            }

            return errors.Trim();
        }

        // Q2.2: Converts an XML file from a URL into a JSON string
        // Uses XmlDocument to load the XML and Newtonsoft.Json to serialize to JSON
        // The returned JSON is deserializable by JsonConvert.DeserializeXmlNode()
        public static string Xml2Json(string xmlUrl)
        {
            try
            {
                // Download the XML content from the remote URL
                string xmlContent = DownloadContent(xmlUrl);

                // Load the XML string into an XmlDocument object
                XmlDocument doc = new XmlDocument();
                doc.LoadXml(xmlContent);

                // Serialize the XmlDocument to a JSON string using Newtonsoft.Json
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
