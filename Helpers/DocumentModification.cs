using Syncfusion.DocIO;
using Syncfusion.DocIO.DLS;
using Syncfusion.DocToPDFConverter;
using Syncfusion.OfficeChartToImageConverter;

namespace AdminPortalV8.Helpers
{
    public class DocumentModification
    { 
        #region docxStringReplacement
        //Replacing text in docx documents and convert to html
        public void WordReplaceText(string sourceFile, string? destFile, string? textSearch, string? textReplace)
        {
            try
            {
                using (WordDocument document = new WordDocument())
                {
                    //Opnes the input word document
                    Stream docStream = new FileStream(sourceFile, FileMode.Open, FileAccess.Read, FileShare.Read);
                    document.Open(docStream, FormatType.Docx);
                    docStream.Dispose();
                    //Finds all occurrences of a misspelled word and replaces with properly spelled word.

                    document.Replace(textSearch, textReplace, true, true);

                    //Saves the resultant file in the given path.
                    docStream = new FileStream(destFile, FileMode.Create, FileAccess.ReadWrite, FileShare.ReadWrite);
                    //convert to html
                    document.Save(docStream, FormatType.Html);
                    docStream.Dispose();
                }

            }
            catch
            {

            }

        }
        //convert docx to html only without replacing anything
        public void Docx2Html(string sourceFile, string? destFile)
        {
            try
            {
                using (WordDocument document = new WordDocument())
                {
                    //Opnes the input word document
                    Stream docStream = new FileStream(sourceFile, FileMode.Open, FileAccess.Read, FileShare.Read);
                    document.Open(docStream, FormatType.Docx);
                    docStream.Dispose();

                    //Saves the resultant file in the given path.
                    docStream = new FileStream(destFile, FileMode.Create, FileAccess.ReadWrite, FileShare.ReadWrite);
                    //convert to html
                    document.Save(docStream, FormatType.Html);
                    docStream.Dispose();
                }

            }
            catch
            {

            }
        }
        //convert word to pdf
        public void ConvertWord2Pdf(string sourceFile, string? destFile)
        {

            try
            {//Load file
                WordDocument doc = new WordDocument(sourceFile, FormatType.Docx);
                //Initialize for convert char
                //Initializes the ChartToImageConverter for converting charts during Word to pdf conversion
                doc.ChartToImageConverter = new ChartToImageConverter();
                //Creates an instance of the DocToPDFConverter
                DocToPDFConverter converter = new DocToPDFConverter();
                //Converts Word document into PDF document

                Syncfusion.Pdf.PdfDocument pdfDocument = converter.ConvertToPDF(doc);
                //Saves the PDF file 
                pdfDocument.Save(destFile);
                //Closes the instance of document objects
                pdfDocument.Close(true);
                doc.Close();
            }
            catch { }
        }

        #endregion



    }
}
