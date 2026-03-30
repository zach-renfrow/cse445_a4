using System;
using System.Xml.Schema;
using System.Xml;
using Newtonsoft.Json;
using System.IO;



/**
 * This template file is created for ASU CSE445 Distributed SW Dev Assignment 4.
 * Please do not modify or delete any existing class/variable/method names. However, you can add more variables and functions.
 * Uploading this file directly will not pass the autograder's compilation check, resulting in a grade of 0.
 * **/


namespace ConsoleApp1
{


    public class Submission
    {
        public static string xmlURL = "Your XML URL";
        public static string xmlErrorURL = "Your Error XML URL";
        public static string xsdURL = "Your XSD URL";

        public static void Main(string[] args)
        {
            string result = Verification(xmlURL, xsdURL);
            Console.WriteLine(result);


            result = Verification(xmlErrorURL, xsdURL);
            Console.WriteLine(result);


            result = Xml2Json(xmlURL);
            Console.WriteLine(result);
        }

        // Q2.1
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

        public static string Xml2Json(string xmlUrl)
        {
            try
            {
                XmlDocument sourceDoc = new XmlDocument();
                sourceDoc.LoadXml(DownloadContent(xmlUrl));

                XmlDocument jsonDoc = new XmlDocument();
                XmlElement root = jsonDoc.CreateElement("NationalParks");
                jsonDoc.AppendChild(root);

                XmlNodeList parks = sourceDoc.SelectNodes("/NationalParks/NationalPark");

                if (parks != null)
                {
                    foreach (XmlNode park in parks)
                    {
                        XmlElement parkElement = jsonDoc.CreateElement("NationalPark");

                        if (park.Attributes != null && park.Attributes["Rating"] != null)
                        {
                            XmlAttribute rating = jsonDoc.CreateAttribute("Rating");
                            rating.Value = park.Attributes["Rating"].Value;
                            parkElement.Attributes.Append(rating);
                        }

                        AppendElement(jsonDoc, parkElement, "Name", park.SelectSingleNode("Name"));

                        XmlNodeList phones = park.SelectNodes("Phone");
                        if (phones != null)
                        {
                            foreach (XmlNode phone in phones)
                            {
                                AppendElement(jsonDoc, parkElement, "Phone", phone);
                            }
                        }

                        XmlNode address = park.SelectSingleNode("Address");
                        if (address != null)
                        {
                            XmlElement addressElement = jsonDoc.CreateElement("Address");

                            if (address.Attributes != null && address.Attributes["NearestAirport"] != null)
                            {
                                XmlAttribute airport = jsonDoc.CreateAttribute("NearestAirport");
                                airport.Value = address.Attributes["NearestAirport"].Value;
                                addressElement.Attributes.Append(airport);
                            }

                            AppendElement(jsonDoc, addressElement, "Number", address.SelectSingleNode("Number"));
                            AppendElement(jsonDoc, addressElement, "Street", address.SelectSingleNode("Street"));
                            AppendElement(jsonDoc, addressElement, "City", address.SelectSingleNode("City"));
                            AppendElement(jsonDoc, addressElement, "State", address.SelectSingleNode("State"));
                            AppendElement(jsonDoc, addressElement, "Zip", address.SelectSingleNode("Zip"));

                            parkElement.AppendChild(addressElement);
                        }

                        root.AppendChild(parkElement);
                    }
                }

                string jsonText = JsonConvert.SerializeXmlNode(jsonDoc, Newtonsoft.Json.Formatting.Indented);
                JsonConvert.DeserializeXmlNode(jsonText);
                return jsonText;
            }
            catch (Exception ex)
            {
                return "Exception: " + ex.Message;
            }
        }

        private static void AppendElement(XmlDocument doc, XmlElement parent, string elementName, XmlNode sourceNode)
        {
            if (sourceNode != null)
            {
                XmlElement child = doc.CreateElement(elementName);
                child.InnerText = sourceNode.InnerText;
                parent.AppendChild(child);
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
