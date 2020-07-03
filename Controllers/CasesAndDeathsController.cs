using CoTEC_Server.DBModels;
using CoTEC_Server.Logic;
using DinkToPdf;
using DinkToPdf.Contracts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace CoTEC_Server.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class CasesAndDeathsController : ControllerBase
    {
        private IConverter _converter;
        private CoTEC_DBContext dbcontext;

        public CasesAndDeathsController(IConverter converter, CoTEC_DBContext dbcontext)
        {
            _converter = converter;
            this.dbcontext = dbcontext;
        }

        [HttpGet]
        public IActionResult Get()
        {
            List<CasesAndDeaths> patients = dbcontext.CasesAndDeaths
                .FromSqlRaw("EXECUTE user_public.deaths_and_cases").ToList();

            var globalSettings = new GlobalSettings
            {
                ColorMode = ColorMode.Color,
                Orientation = Orientation.Portrait,
                PaperSize = PaperKind.A4,
                Margins = new MarginSettings { Top = 10 },
                DocumentTitle = "New cases and deaths report"
                //Out = @"C:\Users\aleja\Employee_Report.pdf"
            };

            var objectSettings = new ObjectSettings
            {
                PagesCount = true,
                HtmlContent = TemplateGenerator.GetHTMLString(patients),
                WebSettings = { DefaultEncoding = "utf-8", UserStyleSheet = Path.Combine(Directory.GetCurrentDirectory(), "assets", "styles.css") },
                HeaderSettings = { FontName = "Arial", FontSize = 9, Right = "Page [page] of [toPage]", Line = true },
                FooterSettings = { FontName = "Arial", FontSize = 9, Line = true, Center = "CoTEC Systems" }
            };

            var pdf = new HtmlToPdfDocument()
            {
                GlobalSettings = globalSettings,
                Objects = { objectSettings }
            };

            var file = _converter.Convert(pdf);
            return File(file, "application/pdf");
        }
    }
}
