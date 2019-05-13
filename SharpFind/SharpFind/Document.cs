using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using DocumentFormat.OpenXml.Wordprocessing;
using iTextSharp.text.pdf;
using iTextSharp.text.pdf.parser;
using System.Collections.Generic;


namespace SharpFind
{
    class Document
    {
        public static bool ParsePdf(string filepath, string keyword)
        {
            bool found = false;
            PdfReader reader;
            try
            {
                reader = new PdfReader(filepath);
            }
            catch
            {
                return found;
            }
            for (int page = 1; page <= reader.NumberOfPages; page++)
            {
                if(PdfTextExtractor.GetTextFromPage(reader, page).Contains(keyword))
                {
                    found = true;
                    break;
                }
            }
            reader.Close();
            return found;
        }
        public static bool ParseDocx(string filepath, string keyword)
        {
            bool found = false;
            WordprocessingDocument wordprocessingDocument;
            try
            {
                wordprocessingDocument = WordprocessingDocument.Open(filepath, false);
            }
            catch
            {
                return found;
            }
            Body body = wordprocessingDocument.MainDocumentPart.Document.Body;
            foreach (var text in body)
            {
                if (text.InnerText.Trim().Contains(keyword))
                {
                    found = true;
                    break;
                }
            }
            wordprocessingDocument.Close();
            return found;
        }
        public static bool ParseXslx(string filepath, string keyword)
        {
            bool found = false;
            SpreadsheetDocument spreadsheetDocument;
            try
            {
                spreadsheetDocument = SpreadsheetDocument.Open(filepath, false);
            }
            catch
            {
                return found;
            }
            Sheets sheets = spreadsheetDocument.WorkbookPart.Workbook.Sheets;
            foreach (Sheet sheet in sheets)
            {
                Worksheet worksheet1 = (spreadsheetDocument.WorkbookPart.GetPartById(sheet.Id.Value) as WorksheetPart).Worksheet;
                IEnumerable<Row> rows1 = worksheet1.GetFirstChild<SheetData>().Descendants<Row>();
                foreach (Row row in rows1)
                {
                    foreach (Cell cell in row.Descendants<Cell>())
                    {
                        if (getValue(spreadsheetDocument, cell).Contains(keyword))
                        {
                            found = true;
                            break;
                        }
                    }
                }
            }
            spreadsheetDocument.Close();
            return found;
        }
        private static string getValue(SpreadsheetDocument spreadsheetDocument, Cell cell)
        {
            string value = string.Empty;
            if (cell.CellValue != null)
            {
                value = cell.CellValue.InnerText;
                if (cell.DataType != null && cell.DataType.Value == CellValues.SharedString)
                {
                    return spreadsheetDocument.WorkbookPart.SharedStringTablePart.SharedStringTable.ChildElements.GetItem(int.Parse(value)).InnerText;
                }
            }
            return value;
        }

    }
}
