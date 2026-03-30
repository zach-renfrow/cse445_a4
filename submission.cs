using System;
using System.Xml.Schema;
using System.Xml;
using System.Text;
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
            string errors = "";

            try
            {
                XmlSchemaSet sc = new XmlSchemaSet();
                sc.Add(null, xsdUrl);

                XmlReaderSettings settings = new XmlReaderSettings();
                settings.ValidationType = ValidationType.Schema;
                settings.Schemas = sc;
                settings.ValidationFlags |= XmlSchemaValidationFlags.ReportValidationWarnings;

                settings.ValidationEventHandler += delegate (object sender, ValidationEventArgs e)
                {
                    errors += "Validation Error: " + e.Message + "\n";
                };

                XmlReader reader = XmlReader.Create(xmlUrl, settings);

                while (reader.Read()) { }

                reader.Close();
            }
            catch (Exception ex)
            {
                errors += "Validation Error: " + ex.Message;
            }

            if (errors == "")
            {
                return "No errors are found";
            }

            return errors.Trim();
        }

        // Q2.2: Converts an XML file from a URL into a JSON string
        // The returned jsonText needs to be de-serializable by Newtonsoft.Json package
        public static string Xml2Json(string xmlUrl)
        {
            try
            {
                string xmlContent = DownloadContent(xmlUrl);

                XmlDocument doc = new XmlDocument();
                doc.LoadXml(xmlContent);

                XmlNodeList parkNodes = doc.SelectNodes("/NationalParks/NationalPark");
                StringBuilder json = new StringBuilder();

                json.Append("{");
                json.Append("\"NationalParks\":{");
                json.Append("\"NationalPark\":[");

                for (int i = 0; i < parkNodes.Count; i++)
                {
                    XmlNode park = parkNodes[i];

                    if (i > 0)
                    {
                        json.Append(",");
                    }

                    json.Append("{");

                    XmlNode nameNode = park.SelectSingleNode("Name");
                    json.Append("\"Name\":\"");
                    json.Append(EscapeJson(nameNode != null ? nameNode.InnerText : ""));
                    json.Append("\",");

                    XmlNodeList phoneNodes = park.SelectNodes("Phone");
                    json.Append("\"Phone\":[");
                    for (int j = 0; j < phoneNodes.Count; j++)
                    {
                        if (j > 0)
                        {
                            json.Append(",");
                        }

                        json.Append("\"");
                        json.Append(EscapeJson(phoneNodes[j].InnerText));
                        json.Append("\"");
                    }
                    json.Append("],");

                    XmlNode addressNode = park.SelectSingleNode("Address");
                    json.Append("\"Address\":{");

                    AppendJsonField(json, "Number", addressNode.SelectSingleNode("Number").InnerText);
                    json.Append(",");
                    AppendJsonField(json, "Street", addressNode.SelectSingleNode("Street").InnerText);
                    json.Append(",");
                    AppendJsonField(json, "City", addressNode.SelectSingleNode("City").InnerText);
                    json.Append(",");
                    AppendJsonField(json, "State", addressNode.SelectSingleNode("State").InnerText);
                    json.Append(",");
                    AppendJsonField(json, "Zip", addressNode.SelectSingleNode("Zip").InnerText);

                    XmlAttribute nearestAirportAttr = addressNode.Attributes["NearestAirport"];
                    if (nearestAirportAttr != null)
                    {
                        json.Append(",");
                        AppendJsonField(json, "@NearestAirport", nearestAirportAttr.Value);
                    }

                    json.Append("}");

                    XmlAttribute ratingAttr = park.Attributes["Rating"];
                    if (ratingAttr != null)
                    {
                        json.Append(",");
                        AppendJsonField(json, "@Rating", ratingAttr.Value);
                    }

                    json.Append("}");
                }

                json.Append("]");
                json.Append("}");
                json.Append("}");

                return json.ToString();
            }
            catch (Exception ex)
            {
                return "Exception: " + ex.Message;
            }
        }

        private static void AppendJsonField(StringBuilder json, string key, string value)
        {
            json.Append("\"");
            json.Append(EscapeJson(key));
            json.Append("\":\"");
            json.Append(EscapeJson(value));
            json.Append("\"");
        }

        private static string EscapeJson(string value)
        {
            if (value == null)
            {
                return "";
            }

            StringBuilder escaped = new StringBuilder();

            for (int i = 0; i < value.Length; i++)
            {
                char c = value[i];

                switch (c)
                {
                    case '\\':
                        escaped.Append("\\\\");
                        break;
                    case '"':
                        escaped.Append("\\\"");
                        break;
                    case '\b':
                        escaped.Append("\\b");
                        break;
                    case '\f':
                        escaped.Append("\\f");
                        break;
                    case '\n':
                        escaped.Append("\\n");
                        break;
                    case '\r':
                        escaped.Append("\\r");
                        break;
                    case '\t':
                        escaped.Append("\\t");
                        break;
                    default:
                        if (c < 32)
                        {
                            escaped.Append("\\u");
                            escaped.Append(((int)c).ToString("x4"));
                        }
                        else
                        {
                            escaped.Append(c);
                        }
                        break;
                }
            }

            return escaped.ToString();
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
