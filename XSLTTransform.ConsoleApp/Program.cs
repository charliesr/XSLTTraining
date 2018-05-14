using System;
using System.Xml.Xsl;

namespace XSLTTransform.ConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            //Create a new XslCompiledTransform and load the compiled transformation.
            XslCompiledTransform xslt = new XslCompiledTransform();
            xslt.Load(typeof(Transform));

            // Execute the transformation and output the results to a file.
            xslt.Transform("books.xml", "discount_books.html");
        }
    }
}
