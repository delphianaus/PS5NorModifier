using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Management;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace ExtensionsHelper
{
    public static class Extensions
    {
        public static string GetCalculateChecksum(this string str)
        {
            int sum = 0;
            foreach (char c in str)
            {
                sum += (int)c;
            }
            return str + ":" + (sum & 0xFF).ToString("X2");
        }

        public static IEnumerable<int> GetPatternAt(this byte[] source, byte[] pattern)
        {
            for (int i = 0; i < source.Length; i++)
            {
                if (source.Skip(i).Take(pattern.Length).SequenceEqual(pattern))
                {
                    yield return i;
                }
            }
        }

        public static byte[] HexStringToByteArray(this string hexString)
        {
            if (hexString.Length % 2 != 0)
            {
                throw new ArgumentException(String.Format(CultureInfo.InvariantCulture, "The binary key cannot have an odd number of digits: {0}", hexString));
            }

            byte[] data = new byte[hexString.Length / 2];
            for (int index = 0; index < data.Length; index++)
            {
                string byteValue = hexString.Substring(index * 2, 2);
                data[index] = byte.Parse(byteValue, NumberStyles.HexNumber, CultureInfo.InvariantCulture);
            }

            return data;
        }

        public static bool IsDiscEdition(this string str)
        {
            return str == "Disc Edition";
        }
        public static bool IsDigitalEdition(this string str)
        {
            return str == "Digital Edition";
        }
        public static string HexStringToString(this string hexString)
        {
            if (hexString == null || (hexString.Length & 1) == 1)
            {
                throw new ArgumentException();
            }
            var sb = new StringBuilder();
            for (var i = 0; i < hexString.Length; i += 2)
            {
                var hexChar = hexString.Substring(i, 2);
                sb.Append((char)Convert.ToByte(hexChar, 16));
            }
            return sb.ToString();
        }
        public static string GetFriendlyName(this string portName)
        {
            // Declare the friendly name variable for later use
            string friendlyName = portName;
            // We'll wrap this in a try loop simply because this isn't available on all platforms
            try
            {
                // This is basically an SQL query. Let's search for the details of the ports based on the port name
                // Again, this is just for Windows based devices
                using (var searcher = new ManagementObjectSearcher("SELECT * FROM Win32_PnPEntity WHERE Name LIKE '%" + portName + "%'"))
                {
                    // Loop through and output the friendly name
                    foreach (var port in searcher.Get())
                    {
                        friendlyName = port["Name"].ToString();
                    }
                }
            }
            // Catch errors. This would probably only happen on Linux systems
            catch (Exception ex)
            {
                // If there is an error, we'll just declare that we don't know the name of the port
                friendlyName = "Unknown Port Name";
            }
            // Send the friendly name (or unknown port name string) back to the main code for output
            return friendlyName;
        }


        public static string ParseErrors(this string errorCode)
        {
            string results = "";

            try
            {
                // Check if the XML file exists
                if (File.Exists("errorDB.xml"))
                {
                    // Load the XML file
                    XmlDocument xmlDoc = new XmlDocument();
                    xmlDoc.Load("errorDB.xml");

                    // Get the root node
                    XmlNode root = xmlDoc.DocumentElement;

                    // Check if the root node is <errorCodes>
                    if (root.Name == "errorCodes")
                    {
                        // No error was found in the database
                        if (root.ChildNodes.Count == 0)
                        {
                            results = "No result found for error code " + errorCode;
                        }
                        else
                        {
                            // Loop through each errorCode node
                            foreach (XmlNode errorCodeNode in root.ChildNodes)
                            {
                                // Check if the node is <errorCode>
                                if (errorCodeNode.Name == "errorCode")
                                {
                                    // Get ErrorCode and Description
                                    string errorCodeValue = errorCodeNode.SelectSingleNode("ErrorCode").InnerText;
                                    string description = errorCodeNode.SelectSingleNode("Description").InnerText;

                                    // Check if the current error code matches the requested error code
                                    if (errorCodeValue == errorCode)
                                    {
                                        // Output the results
                                        results = "Error code: " + errorCodeValue + Environment.NewLine + "Description: " + description;
                                        break; // Exit the loop after finding the matching error code
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        results = "Error: Invalid XML database file. Please reconfigure the application, redownload the offline database, or uncheck the option to use the offline database.";
                    }
                }
                else
                {
                    results = "Error: Local XML file not found.";
                }
            }
            catch (Exception ex)
            {
                results = "Error: " + ex.Message;
            }

            return results;
        }
    }
}
