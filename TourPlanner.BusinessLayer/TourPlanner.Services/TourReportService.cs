using MigraDoc.DocumentObjectModel;
using MigraDoc.DocumentObjectModel.Tables;
using MigraDoc.Rendering;
using PdfSharp.Pdf;
using System.Collections.Generic;
using Tour_planner.TourPlanner.UI.TourPlanner.Models;

namespace Tour_planner.TourPlanner.BusinessLayer.TourPlanner.Services {
    public class TourReportService {
        public void GenerateTourReport(Tour tour, List<TourLog> tourLogs, string outputPath) {
            // Create a new MigraDoc document
            Document document = new Document();
            Section section = document.AddSection();

            // Add a title to the document
            Paragraph title = section.AddParagraph();
            title.Format.Font.Size = 18;
            title.Format.Font.Bold = true;
            title.AddText("Tour Report");

            // Add tour details
            section.AddParagraph($"Tour Name: {tour.TourName}");
            section.AddParagraph($"Description: {tour.Description}");
            section.AddParagraph($"From: {tour.From}");
            section.AddParagraph($"To: {tour.To}");
            section.AddParagraph($"Transport Type: {tour.TransportType}");
            section.AddParagraph($"Distance: {tour.TourDistance} km");
            section.AddParagraph($"Estimated Time: {tour.EstimatedTime}");

            // Add a separator line
            section.AddParagraph(new string('-', 50)); // A simple separator

            // Add a title for the tour logs section
            Paragraph logsTitle = section.AddParagraph();
            logsTitle.Format.Font.Size = 14;
            logsTitle.Format.Font.Bold = true;
            logsTitle.AddText("Tour Logs");

            // Add each tour log
            foreach (var log in tourLogs) {
                section.AddParagraph($"Date/Time: {log.DateTime}");
                section.AddParagraph($"Comment: {log.Comment}");
                section.AddParagraph($"Difficulty: {log.Difficulty}");
                section.AddParagraph($"Total Distance: {log.TotalDistance} km");
                section.AddParagraph($"Total Time: {log.TotalTime}");
                section.AddParagraph($"Rating: {log.Rating}");
                section.AddParagraph(new string('-', 50)); // A simple separator between logs
            }

            // Render the document to a PDF file
            PdfDocumentRenderer renderer = new PdfDocumentRenderer(true) {
                Document = document
            };
            renderer.RenderDocument();
            renderer.PdfDocument.Save(outputPath);
        }

        public void GenerateSummaryReport(List<Tour> tours, string outputPath) {
            // Create a new MigraDoc document
            Document document = new Document();
            Section section = document.AddSection();

            // Add a title to the document
            Paragraph title = section.AddParagraph();
            title.Format.Font.Size = 18;
            title.Format.Font.Bold = true;
            title.AddText("Summary Report");

            // Add a table for summary
            Table table = section.AddTable();
            table.Borders.Width = 0.75;

            // Define columns
            Column column = table.AddColumn("3cm");
            column.Format.Alignment = ParagraphAlignment.Center;

            column = table.AddColumn("3cm");
            column.Format.Alignment = ParagraphAlignment.Center;

            column = table.AddColumn("3cm");
            column.Format.Alignment = ParagraphAlignment.Center;

            column = table.AddColumn("3cm");
            column.Format.Alignment = ParagraphAlignment.Center;

            // Create the header of the table
            Row row = table.AddRow();
            row.HeadingFormat = true;
            row.Format.Alignment = ParagraphAlignment.Center;
            row.Format.Font.Bold = true;
            row.Shading.Color = Colors.LightGray;

            row.Cells[0].AddParagraph("Tour Name");
            row.Cells[1].AddParagraph("Average Distance (km)");
            row.Cells[2].AddParagraph("Average Time");
            row.Cells[3].AddParagraph("Average Rating");

            // Add data rows
            foreach (var tour in tours) {
                var tourLogs = tour.TourLogs;
                if (tourLogs != null && tourLogs.Count > 0) {
                    var averageDistance = tourLogs.Average(log => log.TotalDistance);
                    var averageTime = TimeSpan.FromTicks((long)tourLogs.Average(log => log.TotalTime.Ticks));
                    var averageRating = tourLogs.Average(log => log.Rating);

                    row = table.AddRow();
                    row.Cells[0].AddParagraph(tour.TourName);
                    row.Cells[1].AddParagraph(averageDistance.ToString("F2"));
                    row.Cells[2].AddParagraph(averageTime.ToString());
                    row.Cells[3].AddParagraph(averageRating.ToString("F2"));
                }
            }

            // Render the document to a PDF file
            PdfDocumentRenderer renderer = new PdfDocumentRenderer(true) {
                Document = document
            };
            renderer.RenderDocument();
            renderer.PdfDocument.Save(outputPath);
        }
    }
}
