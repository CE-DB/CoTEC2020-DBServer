using CoTEC_Server.DBModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using iText.Kernel.Colors;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas.Draw;
using iText.Layout;
using iText.Layout.Element;
using iText.Layout.Properties;
using System;

namespace CoTEC_Server.Controllers
{
    /// <summary>
    /// This class controls the generation of New cases and deaths reports, this is just for demostration, in a
    /// production environment this endpoint has to be deleted.
    /// </summary>
    [Route("[controller]")]
    [ApiController]
    public class CasesAndDeathsController : ControllerBase
    {
        private CoTEC_DBContext dbcontext;

        public CasesAndDeathsController(CoTEC_DBContext dbcontext)
        {
            this.dbcontext = dbcontext;
        }

        [HttpGet]
        public IActionResult Get()
        {
            List<CasesAndDeaths> patients = dbcontext.CasesAndDeaths
                 .FromSqlRaw("EXECUTE user_public.deaths_and_cases").ToList();

            MemoryStream m = new MemoryStream();

            PdfWriter writer = new PdfWriter(m);
            PdfDocument pdf = new PdfDocument(writer);
            Document document = new Document(pdf, PageSize.A4, false);


            Paragraph header = new Paragraph("New cases and deaths report")
               .SetTextAlignment(TextAlignment.CENTER)
               .SetFontSize(30);

            document.Add(header);

            LineSeparator ls = new LineSeparator(new SolidLine());
            document.Add(ls);

            Table table = new Table(9, false);
            Cell h1 = new Cell(1, 1)
               .SetBackgroundColor(ColorConstants.LIGHT_GRAY)
               .SetTextAlignment(TextAlignment.CENTER)
               .Add(new Paragraph("Country"));

            Cell h2 = new Cell(1, 1)
               .SetBackgroundColor(ColorConstants.LIGHT_GRAY)
               .SetTextAlignment(TextAlignment.CENTER)
               .Add(new Paragraph("Type"));

            Cell h3 = new Cell(1, 1)
               .SetTextAlignment(TextAlignment.CENTER)
               .SetBackgroundColor(ColorConstants.LIGHT_GRAY)
               .Add(new Paragraph(DateTime.Now.AddDays(-6).ToString("yyyy-MM-dd")));

            Cell h4 = new Cell(1, 1)
               .SetTextAlignment(TextAlignment.CENTER)
               .SetBackgroundColor(ColorConstants.LIGHT_GRAY)
               .Add(new Paragraph(DateTime.Now.AddDays(-5).ToString("yyyy-MM-dd")));

            Cell h5 = new Cell(1, 1)
               .SetTextAlignment(TextAlignment.CENTER)
               .SetBackgroundColor(ColorConstants.LIGHT_GRAY)
               .Add(new Paragraph(DateTime.Now.AddDays(-4).ToString("yyyy-MM-dd")));

            Cell h6 = new Cell(1, 1)
                .SetBackgroundColor(ColorConstants.LIGHT_GRAY)
               .SetTextAlignment(TextAlignment.CENTER)
               .Add(new Paragraph(DateTime.Now.AddDays(-3).ToString("yyyy-MM-dd")));

            Cell h7 = new Cell(1, 1)
                .SetBackgroundColor(ColorConstants.LIGHT_GRAY)
               .SetTextAlignment(TextAlignment.CENTER)
               .Add(new Paragraph(DateTime.Now.AddDays(-2).ToString("yyyy-MM-dd")));

            Cell h8 = new Cell(1, 1)
                .SetBackgroundColor(ColorConstants.LIGHT_GRAY)
               .SetTextAlignment(TextAlignment.CENTER)
               .Add(new Paragraph(DateTime.Now.AddDays(-1).ToString("yyyy-MM-dd")));

            Cell h9 = new Cell(1, 1)
                .SetBackgroundColor(ColorConstants.LIGHT_GRAY)
               .SetTextAlignment(TextAlignment.CENTER)
               .Add(new Paragraph(DateTime.Now.ToString("yyyy-MM-dd")));

            table.AddHeaderCell(h1);
            table.AddHeaderCell(h2);
            table.AddHeaderCell(h3);
            table.AddHeaderCell(h4);
            table.AddHeaderCell(h5);
            table.AddHeaderCell(h6);
            table.AddHeaderCell(h7);
            table.AddHeaderCell(h8);
            table.AddHeaderCell(h9);


            foreach (var p in patients)
            {
                Cell r1 = new Cell(1, 1)
                   .SetTextAlignment(TextAlignment.CENTER)
                   .Add(new Paragraph(p.Country));

                Cell r2 = new Cell(1, 1)
                   .SetTextAlignment(TextAlignment.CENTER)
                   .Add(new Paragraph(p.Type));

                Cell r3 = new Cell(1, 1)
                   .SetTextAlignment(TextAlignment.CENTER)
                   .Add(new Paragraph(p.Fecha1.ToString()));

                Cell r4 = new Cell(1, 1)
                   .SetTextAlignment(TextAlignment.CENTER)
                   .Add(new Paragraph(p.Fecha2.ToString()));

                Cell r5 = new Cell(1, 1)
                   .SetTextAlignment(TextAlignment.CENTER)
                   .Add(new Paragraph(p.Fecha3.ToString()));

                Cell r6 = new Cell(1, 1)
                   .SetTextAlignment(TextAlignment.CENTER)
                   .Add(new Paragraph(p.Fecha4.ToString()));

                Cell r7 = new Cell(1, 1)
                   .SetTextAlignment(TextAlignment.CENTER)
                   .Add(new Paragraph(p.Fecha5.ToString()));

                Cell r8 = new Cell(1, 1)
                   .SetTextAlignment(TextAlignment.CENTER)
                   .Add(new Paragraph(p.Fecha6.ToString()));

                Cell r9 = new Cell(1, 1)
                   .SetTextAlignment(TextAlignment.CENTER)
                   .Add(new Paragraph(p.Fecha7.ToString()));

                table.AddCell(r1);
                table.AddCell(r2);
                table.AddCell(r3);
                table.AddCell(r4);
                table.AddCell(r5);
                table.AddCell(r6);
                table.AddCell(r7);
                table.AddCell(r8);
                table.AddCell(r9);
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
