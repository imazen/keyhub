using System;
using System.IO;
using System.Xml;
using System.Xml.Serialization;

namespace KeyHub.Common.Serializers.Utils
{
    /// <summary>
    /// Helper methods for easy XML Serialization
    /// </summary>
    public static class XmlSerializers
    {
        /// <summary>
        /// Serializes an object to XML
        /// </summary>
        /// <param name="objectToSerialize">The object to serialize</param>
        /// <returns>A MemoryStream containing the XML</returns>
        private static MemoryStream SerializeXmlObject(object objectToSerialize)
        {
            // Create a new XML Serializer
            XmlSerializer serializer = new XmlSerializer(objectToSerialize.GetType());

            // Initialize stream and serialize
            var stream = new MemoryStream();
            var writer = new XmlTextWriterFull(stream);

            try
            {
                serializer.Serialize(writer, objectToSerialize);
            }
            catch
            {
                // Return null if the operations fails
                return null;
            }

            // Return the stream containing the XML
            return stream;
        }

        /// <summary>
        /// Deserializes a string to an object
        /// </summary>
        /// <typeparam name="T">The destination type</typeparam>
        /// <param name="stringToDeserialize">A string containing the XML</param>
        /// <param name="type">Output parameter containing the deserialized object</param>
        private static void DeserializeXmlObject<T>(string stringToDeserialize, ref T type)
        {
            // Create a new XML Serializer
            XmlSerializer serializer = new XmlSerializer(typeof(T));

            // Initialize stream and deserialize
            using (StringReader stream = new StringReader(stringToDeserialize))
            {
                type = (T)serializer.Deserialize(stream);
            }
        }

        /// <summary>
        /// Deserializes a string to an object
        /// </summary>
        /// <typeparam name="T">The destination type</typeparam>
        /// <param name="stringToDeserialize">A string containing the XML</param>
        /// <param name="type">Output parameter containing the deserialized object</param>
        private static T DeserializeXmlObject<T>(string stringToDeserialize, Type type)
        {
            // Create a new XML Serializer
            XmlSerializer serializer = new XmlSerializer(type);

            // Initialize stream and deserialize
            using (StringReader stream = new StringReader(stringToDeserialize))
            {
                return (T)serializer.Deserialize(stream);
            }
        }

        /// <summary>
        /// Serializes an object to XML
        /// </summary>
        /// <param name="obj">The object to be serialized</param>
        /// <returns>The object serialized to a string</returns>
        public static string SerializeClassToXml(object obj)
        {
            if (obj == null)
                throw new ArgumentNullException("obj");

            // Serialize the object into XML
            var memoryStream = SerializeXmlObject(obj);

            if (memoryStream != null)
            {
                // Read the stream
                StreamReader streamReader = new StreamReader(memoryStream, System.Text.Encoding.UTF8);

                // Reset the stream
                streamReader.BaseStream.Seek(0, SeekOrigin.Begin);

                // Return the XML
                return streamReader.ReadToEnd();
            }

            // Return nothing if the Serialize object failed
            return String.Empty;
        }

        /// <summary>
        /// Serializes an object to an XmlDocument
        /// </summary>
        /// <param name="obj">The object to be serialized</param>
        /// <returns>An XmlDocument containing the serialized object</returns>
        public static XmlDocument SerializeClassToXmlDocument(object obj)
        {
            if (obj == null)
                throw new ArgumentNullException("obj");

            // Create document and serializer
            var xmlDocument = new XmlDocument();
            var serializer = new XmlSerializer(obj.GetType());

            using (MemoryStream stream = new MemoryStream())
            {
                // Serialize and save to document
                serializer.Serialize(stream, obj);
                stream.Flush();
                stream.Seek(0, SeekOrigin.Begin);

                xmlDocument.Load(stream);
            }

            // Return the XmlDocument
            return xmlDocument;
        }

        /// <summary>
        /// Deserializes XML to an object
        /// </summary>
        /// <typeparam name="T">The destination type</typeparam>
        /// <param name="stringToDeserialize">A string containing the XML</param>
        /// <returns>A new object of type <typeparamref name="T"/></returns>
        public static T SerializeXmlToClass<T>(string stringToDeserialize) where T : class
        {
            // Create a default value for T
            T returnObj = default(T);

            // Deserialize the XML into T
            DeserializeXmlObject<T>(stringToDeserialize, ref returnObj);

            // Return the new object
            return returnObj;
        }

        /// <summary>
        /// Deserializes XML to an object
        /// </summary>
        /// <typeparam name="T">The destination type</typeparam>
        /// <param name="stringToDeserialize">A string containing the XML</param>
        /// <param name="typeToDeserialize">The destination type</param>
        /// <returns>A new object of type <typeparamref name="T"/></returns>
        public static T SerializeXmlToClass<T>(string stringToDeserialize, Type typeToDeserialize)
        {
            // Deserialize the XML into T and return
            return (T)DeserializeXmlObject<T>(stringToDeserialize, typeToDeserialize);
        }
    }
}