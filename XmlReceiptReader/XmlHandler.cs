using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;

namespace XmlReceiptReader
{
    class XmlHandler
    {
        private string xmldata;

        public static string ReceiptTypeValue = String.Empty;
        public static string ReceiptNumberValue = String.Empty;
        public static string UuidValue = String.Empty;
        public static string AmountValue = "0.00";
        public static string TaxBaseBasicValue = "0.00";
        public static string BasicVatAmountValue = "0.00";
        public static string ReducedVatAmountValue = "0.00";
        public static string TaxBaseReducedValue = "0.00";
        public static string TaxFreeAmountValue = "0.00";
        public static string ParagonNumberValue = String.Empty;
        public static string InvoiceNumberValue = String.Empty;
        public static string IntReceiptNumberValue = String.Empty;
        public static string DateValue = String.Empty;
        public static string OKPValue = String.Empty;

        public static string[,] items;
        public static string[] payment = new string[4];

        public static string BeforeHeaderValue = String.Empty;
        public static string AfterHeaderValue = String.Empty;
        public static string BeforeFooterValue = String.Empty;
        public static string AfterFooterValue = String.Empty;

        public XmlHandler(string xmldata)
        {
            ClearVariables();
            this.xmldata = xmldata;
            SetCulture();
        }

        public XmlHandler()
        {
            ClearVariables();
            SetCulture();
        }

        private void SetCulture()
        {
            CultureInfo nonInvariantCulture = new CultureInfo("en-US");
            nonInvariantCulture.NumberFormat.NumberDecimalSeparator = ".";
            //nonInvariantCulture.NumberFormat.NumberGroupSeparator = " ";
            Thread.CurrentThread.CurrentCulture = nonInvariantCulture;
        }

        public void ClearVariables()
        {
            xmldata = String.Empty;

            ReceiptTypeValue = String.Empty;
            ReceiptNumberValue = String.Empty;
            UuidValue = String.Empty;
            AmountValue = "0.00";
            TaxBaseBasicValue = "0.00";
            BasicVatAmountValue = "0.00";
            ReducedVatAmountValue = "0.00";
            TaxBaseReducedValue = "0.00";
            TaxFreeAmountValue = "0.00";
            ParagonNumberValue = String.Empty;
            InvoiceNumberValue = String.Empty;
            IntReceiptNumberValue = String.Empty;
            DateValue = String.Empty;
            OKPValue = String.Empty;

            items = new string[0, 0];
            payment = new string[4];

            BeforeHeaderValue = String.Empty;
            AfterHeaderValue = String.Empty;
            BeforeFooterValue = String.Empty;
            AfterFooterValue = String.Empty;
        }

        public bool ReadXML()
        {
            try
            {
                var rootElement = XElement.Parse(this.xmldata);
                XNamespace ns = rootElement.GetNamespaceOfPrefix("soapenv");

                XName nodeName;
                var soapBody = rootElement;
                ns = XNamespace.Get("http://financnasprava.sk/ekasa/schema/v2");

                if (rootElement.Name.LocalName.Equals("Envelope"))
                {
                    ns = rootElement.GetNamespaceOfPrefix("soapenv");
                    nodeName = ns + "Body";
                    soapBody = rootElement.Element(nodeName);
                    ns = XNamespace.Get("http://financnasprava.sk/ekasa/schema/v1");
                }

                nodeName = ns + "RegisterReceiptRequest";
                rootElement = soapBody.Element(nodeName);

                nodeName = ns + "Header";
                var receiptHeader = rootElement.Element(nodeName);

                UuidValue = receiptHeader.Attribute("Uuid") == null ? String.Empty : receiptHeader.Attribute("Uuid").Value;

                nodeName = ns + "ReceiptData";
                var receiptData = rootElement.Element(nodeName);

                ReceiptTypeValue = receiptData.Attribute("ReceiptType").Value;
                ReceiptNumberValue = receiptData.Attribute("ReceiptNumber") == null ? String.Empty : receiptData.Attribute("ReceiptNumber").Value;
                AmountValue = receiptData.Attribute("Amount").Value;
                BasicVatAmountValue = receiptData.Attribute("BasicVatAmount") == null ? "0.00" : receiptData.Attribute("BasicVatAmount").Value;
                ReducedVatAmountValue = receiptData.Attribute("ReducedVatAmount") == null ? "0.00" : receiptData.Attribute("ReducedVatAmount").Value;
                TaxBaseBasicValue = receiptData.Attribute("TaxBaseBasic") == null ? "0.00" : receiptData.Attribute("TaxBaseBasic").Value;
                TaxBaseReducedValue = receiptData.Attribute("TaxBaseReduced") == null ? "0.00" : receiptData.Attribute("TaxBaseReduced").Value;
                TaxFreeAmountValue = receiptData.Attribute("TaxFreeAmount") == null ? "0.00" : receiptData.Attribute("TaxFreeAmount").Value;
                ParagonNumberValue = receiptData.Attribute("ParagonNumber") == null ? String.Empty : receiptData.Attribute("ParagonNumber").Value;
                InvoiceNumberValue = receiptData.Attribute("InvoiceNumber") == null ? String.Empty : receiptData.Attribute("InvoiceNumber").Value;
                IntReceiptNumberValue = receiptData.Attribute("IntReceiptNumber") == null ? String.Empty : receiptData.Attribute("IntReceiptNumber").Value;
                DateValue = receiptData.Attribute("CreateDate") == null ? String.Empty : receiptData.Attribute("CreateDate").Value;

                nodeName = ns + "Item";
                int index = 0;

                XDocument xmlDoc = XDocument.Parse(xmldata);
                int itemsCount = xmlDoc.Root.Descendants(nodeName).Count();
                items = new string[itemsCount, 8];

                xmlDoc.Root.Descendants(nodeName)
                .ToList()
                .ForEach(element =>
                {
                    items[index, 7] = element.Attribute("Name").Value.Split(',').Last() == element.Attribute("Name").Value ? String.Empty : element.Attribute("Name").Value.Split(',').Last();
                    items[index, 0] = element.Attribute("Name").Value.Replace(',' + items[index, 7], String.Empty);
                    items[index, 1] = element.Attribute("ItemType").Value;
                    double qty = double.Parse(element.Attribute("Quantity").Value, CultureInfo.InvariantCulture);
                    items[index, 2] = qty.ToString("N3");
                    items[index, 3] = element.Attribute("VatRate").Value.Length > 0 ? element.Attribute("VatRate").Value : "0.00";
                    items[index, 3] = double.Parse(items[index, 3]).ToString("N0");
                    items[index, 4] = element.Attribute("UnitPrice") == null ? String.Empty : element.Attribute("UnitPrice").Value;
                    items[index, 5] = element.Attribute("Price").Value;
                    if (element.Attribute("ItemType").Value.Equals("V"))
                    {
                        items[index, 6] = element.Attribute("ReferenceReceiptId") == null ? String.Empty : element.Attribute("ReferenceReceiptId").Value;
                    }
                    else if (element.Attribute("ItemType").Value.Equals("Z"))
                    {
                        items[index, 7] = String.Empty;
                        items[index, 0] = element.Attribute("Name").Value;
                    }
                    index++;
                });

                nodeName = ns + "Payment";
                index = 0;

                payment[0] = "0.00";
                payment[1] = "0.00";
                payment[2] = "0.00";
                payment[3] = "0.00";

                xmlDoc = XDocument.Parse(xmldata);
                xmlDoc.Root.Descendants(nodeName)
                .ToList()
                .ForEach(element =>
                {
                    string payType = element.Attribute("PaymentType").Value;
                    if (payType.Equals("HO"))
                        payment[0] = element.Attribute("Amount").Value;
                    else if (payType.Equals("KA"))
                        payment[1] = element.Attribute("Amount").Value;
                    else if (payType.Equals("ST"))
                        payment[2] = element.Attribute("Amount").Value;
                    else if (payType.Equals("VP"))
                        payment[3] = element.Attribute("Amount").Value;

                    index++;
                });

                nodeName = ns + "ValidationCode";
                var ValidationCode = rootElement.Element(nodeName);
                nodeName = ns + "OKP";
                var OKP = ValidationCode.Element(nodeName);
                OKPValue = OKP == null ? String.Empty : OKP.Value;

                return true;
            }
            catch (Exception e)
            {
                //MessageBox.Show(e.Message + "\n" + e.StackTrace, "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }
    }
}
