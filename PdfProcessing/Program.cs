using iTextSharp.text;
using iTextSharp.text.pdf;
using System;
using System.IO;

namespace PdfProcessing
{
    public class ITextEvents : PdfPageEventHelper
    {
        // This is the contentbyte object of the writer
        PdfContentByte cb;

        PdfTemplate footerTemplate;

        // this is the BaseFont we are going to use for the header / footer
        BaseFont bf = null;

        readonly string ttf = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Fonts), "ARIAL.TTF");

        public override void OnOpenDocument(PdfWriter writer, Document document)
        {
            bf = BaseFont.CreateFont(ttf, BaseFont.IDENTITY_H, BaseFont.NOT_EMBEDDED);
            cb = writer.DirectContent;
            footerTemplate = cb.CreateTemplate(200, 200);
        }

        public override void OnEndPage(PdfWriter writer, Document document)
        {
            base.OnEndPage(writer, document);

            string text1 = "Name: Anna Pepper";
            string text2 = "Date: 01.09.2022 20:21";
            string text3 = "Signature: 17a026d05b0e7ddcbb37799750ec06c662f2d4a576bbf706f2c1878fdfdfdsf2232";
 
            {
                cb.BeginText();
                cb.SetFontAndSize(bf, 8);
                cb.SetTextMatrix(document.PageSize.GetLeft(50), document.PageSize.GetBottom(35));
                cb.ShowText(text1);
                cb.EndText();
                float len = bf.GetWidthPoint(text1, 8);
                cb.AddTemplate(footerTemplate, document.PageSize.GetLeft(50) + len, document.PageSize.GetBottom(35));
            }

            {
                cb.BeginText();
                cb.SetFontAndSize(bf, 8);
                cb.SetTextMatrix(document.PageSize.GetLeft(50), document.PageSize.GetBottom(25));
                cb.ShowText(text2);
                cb.EndText();
                float len = bf.GetWidthPoint(text2, 8);
                cb.AddTemplate(footerTemplate, document.PageSize.GetLeft(50) + len, document.PageSize.GetBottom(25));
            }

            {
                cb.BeginText();
                cb.SetFontAndSize(bf, 8);
                cb.SetTextMatrix(document.PageSize.GetLeft(50), document.PageSize.GetBottom(15));
                cb.ShowText(text3);
                cb.EndText();
                float len = bf.GetWidthPoint(text3, 8);
                cb.AddTemplate(footerTemplate, document.PageSize.GetLeft(50) + len, document.PageSize.GetBottom(15));
            }
            
            //Move the pointer and draw line to separate footer section from rest of page
            cb.MoveTo(40, document.PageSize.GetBottom(45));
            cb.LineTo(document.PageSize.Width - 20, document.PageSize.GetBottom(45));
            cb.Stroke();
        }

        public override void OnCloseDocument(PdfWriter writer, Document document)
        {
            base.OnCloseDocument(writer, document);
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            string oldFile = "Input.pdf";
            PdfReader reader = new PdfReader(oldFile);
            
            using (FileStream fileStream = new FileStream("Output.pdf", FileMode.Create, FileAccess.Write))
            {
                var document = new Document(reader.GetPageSizeWithRotation(1));
                var writer = PdfWriter.GetInstance(document, fileStream);
                writer.PageEvent = new ITextEvents();
                document.Open();

                for (var i = 1; i <= reader.NumberOfPages; i++)
                {
                    document.NewPage();
                    PdfImportedPage importedPage = writer.GetImportedPage(reader, i);
                    PdfContentByte contentByte = writer.DirectContent;
                    contentByte.AddTemplate(importedPage, 0, 0);
                }

                document.Close();
                writer.Close();
                reader.Close();
            }
            Console.WriteLine("Footer has been added to pdf!");
            Console.Read();
        }
    }
}
