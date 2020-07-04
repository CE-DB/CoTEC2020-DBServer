using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using CoTEC_Server.DBModels;
using CoTEC_Server.Logic;
using iText.Kernel.Colors;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas.Draw;
using iText.Layout;
using iText.Layout.Borders;
using iText.Layout.Element;
using iText.Layout.Properties;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace CoTEC_Server.Controllers
{
    /// <summary>
    /// This class controls the generation of patients reports, this is just for demostration, in a
    /// production environment this endpoint has to be deleted.
    /// </summary>
    [Route("[controller]")]
    [ApiController]
    public class PatientsReportController : ControllerBase
    {
        private CoTEC_DBContext dbcontext;

        public PatientsReportController(CoTEC_DBContext dbcontext)
        {
            this.dbcontext = dbcontext;
        }

        [HttpGet]
        public IActionResult Get()
        {
            List<PatientsReport> patients = dbcontext.PatientsReport
                .FromSqlRaw("EXECUTE user_public.patients_by_country").ToList();

            MemoryStream m = new MemoryStream();

            PdfWriter writer = new PdfWriter(m);
            PdfDocument pdf = new PdfDocument(writer);
            Document document = new Document(pdf, PageSize.A4, false);


            Paragraph header = new Paragraph("Patients report")
               .SetTextAlignment(TextAlignment.CENTER)
               .SetFontSize(30);

            document.Add(header);

            LineSeparator ls = new LineSeparator(new SolidLine());
            document.Add(ls);

            Table table = new Table(5, false);
            Cell cell11 = new Cell(1, 1)
               .SetBackgroundColor(ColorConstants.LIGHT_GRAY)
               .SetTextAlignment(TextAlignment.CENTER)
               .Add(new Paragraph("Country"));

            Cell cell12 = new Cell(1, 1)
               .SetBackgroundColor(ColorConstants.LIGHT_GRAY)
               .SetTextAlignment(TextAlignment.CENTER)
               .Add(new Paragraph("Infected"));

            Cell cell21 = new Cell(1, 1)
               .SetTextAlignment(TextAlignment.CENTER)
               .SetBackgroundColor(ColorConstants.LIGHT_GRAY)
               .Add(new Paragraph("Recovered"));

            Cell cell22 = new Cell(1, 1)
               .SetTextAlignment(TextAlignment.CENTER)
               .SetBackgroundColor(ColorConstants.LIGHT_GRAY)
               .Add(new Paragraph("Dead"));

            Cell cell31 = new Cell(1, 1)
                .SetBackgroundColor(ColorConstants.LIGHT_GRAY)
               .SetTextAlignment(TextAlignment.CENTER)
               .Add(new Paragraph("Active"));

            table.AddCell(cell11);
            table.AddCell(cell12);
            table.AddCell(cell21);
            table.AddCell(cell22);
            table.AddCell(cell31);

            foreach (var p in patients)
            {
                Cell r1 = new Cell(1, 1)
                   .SetTextAlignment(TextAlignment.CENTER)
                   .Add(new Paragraph(p.Country));

                Cell r2 = new Cell(1, 1)
                   .SetTextAlignment(TextAlignment.CENTER)
                   .Add(new Paragraph(p.Infected.ToString()));

                Cell r3 = new Cell(1, 1)
                   .SetTextAlignment(TextAlignment.CENTER)
                   .Add(new Paragraph(p.Cured.ToString()));

                Cell r4 = new Cell(1, 1)
                   .SetTextAlignment(TextAlignment.CENTER)
                   .Add(new Paragraph(p.Dead.ToString()));

                Cell r5 = new Cell(1, 1)
                   .SetTextAlignment(TextAlignment.CENTER)
                   .Add(new Paragraph(p.Active.ToString()));

                table.AddCell(r1);
                table.AddCell(r2);
                table.AddCell(r3);
                table.AddCell(r4);
                table.AddCell(r5);
            }

            Paragraph newline = new Paragraph(new Text("\n"));

            table.UseAllAvailableWidth();

            document.Add(newline);
            document.Add(table);


            int n = pdf.GetNumberOfPages();
            for (int i = 1; i <= n; i++)
            {
                document.ShowTextAligned(new Paragraph(string
                   .Format("page" + i + " of " + n)),
                   559, 806, i, TextAlignment.RIGHT,
                   VerticalAlignment.TOP, 0);
            }

            document.Close();
            return File(m.ToArray(), "application/pdf");
        }

        
    }
}
