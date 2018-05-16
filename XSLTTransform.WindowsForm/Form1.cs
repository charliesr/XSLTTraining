using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Xsl;

namespace XSLTTransform.WindowsForm
{
    public partial class Form1 : Form
    {
        private string xsltFilename;
        private string xmlFilename;
        public Form1()
        {
            InitializeComponent();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            xsltFilename = "transformer.xslt";
            xmlFilename = "catalog.xml";

            var xslt = new XslTransform();
            xslt.Load(xsltFilename);
            xslt.Transform(xmlFilename, "final.html");

        }

        private void btSearch_Click(object sender, EventArgs e)
        {
            xsltFilename = "transformer.xslt";
            xmlFilename = "catalog.xml";

            var doc = XDocument.Load(xmlFilename);

            var cds = doc.Element("catalog").Elements();

            var cdsWithS = cds.Where(cd => cd.Element("title").Value.StartsWith("S"));

            var xws = new XmlWriterSettings();

            using (var writer = XmlWriter.Create("catalogFiltered.xml", xws))
            {
                writer.WriteStartElement("catalog");
                
                foreach (var item in cdsWithS)
                {
                    item.WriteTo(writer);
                }

                writer.WriteEndElement();
            }



            var xslt = new XslTransform();

            xslt.Load(xsltFilename);
            xslt.Transform("catalogFiltered.xml", "search.html");
        }
    }
}
