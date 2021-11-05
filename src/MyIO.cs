using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.Serialization;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace RS3QuestFilter.src
{
    public class UTF8StringWriter : StringWriter
    {
        public override Encoding Encoding
        {
            get { return Encoding.UTF8; }
        }
    }

    public class MyIO
    {
        public static string SerialiseToXML<T>(T? obj) where T : class
        {

            if (obj != null) 
            {
                using (UTF8StringWriter? writer = new())
                {
                    var namespaces = new XmlSerializerNamespaces();
                    namespaces.Add(string.Empty, string.Empty);

                    XmlSerializer serializer = new(obj.GetType());
                    serializer.Serialize(writer, obj, namespaces);
                    return writer.ToString();
                }
            }
            return null;
        }

        public static T? DeserialiseFromXML<T>(string filename) where T : class
        {
            using (XmlReader? reader = XmlReader.Create(filename))
            {
                XmlSerializer? serializer = new(typeof(T));
                T? t;
                try
                {
                    t = serializer.Deserialize(reader) as T;
                }
                catch (InvalidOperationException e)
                {
                    string s = "Parsing error with deserialisation! Reason: " + e.InnerException.Message;
                    Debug.WriteLine(s);
                    
                    throw new InvalidOperationException(s);
                }
                catch (NullReferenceException e)
                {
                    string s = "Cannot parse an empty file! Reason: " + e.Message;
                    Debug.WriteLine(s);
                    throw new NullReferenceException(s);
                }

                return t;
            }
        }
    }
}
